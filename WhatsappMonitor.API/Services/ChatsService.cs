using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WhatsappMonitor.API.Context;
using WhatsappMonitor.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using WhatsappMonitor.API.Helper;

namespace WhatsappMonitor.API.Services
{
    public class ChatsMessageService
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _conf;
        public ChatsMessageService(MyDbContext context, IConfiguration conf)
        {
            _context = context;
            _conf = conf;
        }

        public async Task<List<ChatMessage>> SearchEntityChatTextByDate(int id, string date)
        {
            //jump the chat to a specific date, if it exists, in a 70 day gap
            var moved = 0;
            var initialDate = DateTime.Parse(date);
            var parsedDate = initialDate;
            var chatMessage = await _context.Chats
            .Where(c => c.FolderId == id)
            .OrderByDescending(c => c.MessageTime)
            .ToListAsync();

            int? index = null;
            while (index == null)
            {
                var dateIndex = chatMessage.FindLastIndex(c => c.MessageTime == parsedDate);
                if (dateIndex != -1)
                {
                    index = dateIndex;
                }
                else
                {
                    //number or runs of the loop
                    moved++;
                    if (moved < 35)
                    {
                        parsedDate = parsedDate.AddDays(1);
                    }
                    else if (moved == 35)
                    {
                        parsedDate = initialDate;
                        parsedDate = parsedDate.AddDays(-1);
                    }
                    else if (moved > 35 && moved < 70)
                    {
                        parsedDate = parsedDate.AddDays(-1);
                    }
                    else if (moved == 70)
                    {
                        return await GetLastMessage(id);
                    }
                }
            }

            return chatMessage.GetRange(index.Value - 200, 400);
        }

        public async Task<List<ChatMessage>> GetChatsAfter(int id, string last)
        {
            var parsedDate = DateTime.Parse(last);
            var chatMessage = await _context.Chats.Where(c => c.FolderId == id && c.MessageTime > parsedDate).OrderByDescending(c => c.MessageTime).ToListAsync();

            return chatMessage.TakeLast(200).ToList();
        }

        public async Task<List<ChatMessage>> GetChatsBefore(int id, string first)
        {
            var parsedDate = DateTime.Parse(first);
            var chatMessage = await _context.Chats.Where(c => c.FolderId == id && c.MessageTime < parsedDate).OrderByDescending(c => c.MessageTime).ToListAsync();

            return chatMessage.Take(200).ToList();
        }

        public async Task<List<ChatMessage>> GetFirstMessage(int id)
        {
            var chatMessage = await _context.Chats.Where(c => c.FolderId == id).OrderByDescending(c => c.MessageTime).ToListAsync();
            return chatMessage.TakeLast(250).ToList();
        }

        public async Task<List<ChatMessage>> GetLastMessage(int id)
        {
            var chatMessage = await _context.Chats.Where(c => c.FolderId == id).OrderByDescending(c => c.MessageTime).ToListAsync();
            return chatMessage.Take(250).ToList();
        }

        public async Task<Tuple<PaginationDTO, List<ChatMessage>>> GetAllChatsPagination(int id, int skip, int take)
        {
            var cleanTake = 200;
            var cleanSkip = 0;
           
            var ChatMessage = await _context.Chats.Where(c => c.FolderId == id).OrderByDescending(c => c.MessageTime).ToListAsync();

            var result = ChatMessage.Skip(cleanSkip).Take(cleanTake).ToList();

            var allowNext = false; //allow older messages
            var allowBack = false; //allow newer messages

            if (cleanSkip > 0) allowBack = true;
            if ((cleanSkip + cleanTake) < ChatMessage.Count()) allowNext = true;

            var pagesCounter = ChatMessage.Count() / cleanTake; //totalmessages divided by current take
            var currentPage = (cleanSkip + cleanTake) / cleanTake;

            var paginationDto = new PaginationDTO(cleanSkip, cleanTake, allowNext, allowBack, pagesCounter, currentPage);

            return new Tuple<PaginationDTO, List<ChatMessage>>(paginationDto, result);
        }

        public async Task<List<ChatMessage>> SearchEntityChatText(string text, int id)
        {
            var messages = await _context.Chats.Where(c => c.FolderId == id).OrderByDescending(c => c.MessageTime).ToListAsync();
            var findText = DefaultHelper.SearchChatText(text, messages);
            return findText;
        }

        public async Task<List<ParticipantDTO>> GetChatParticipants(int id)
        {
            var participantsDTO = new List<ParticipantDTO>();
            var totalMessages = 0;
            var totalWords = 0;

            //bring everything already grouped by user
            var participantsStep1 = await _context.Chats
            .Where(c => c.FolderId == id)
            .ToListAsync();

            var participants = participantsStep1
            .GroupBy(c => c.PersonName);

            foreach (var participant in participants)
            {
                var tFirstMessage = participant.Min(c => c.MessageTime);
                var tLastMessage = participant.Max(c => c.MessageTime);
                var tMessages = participant.Select(c => c.Message);
                var tMessageCount = tMessages.Count();
                var tWordCount = 0;

                foreach (var message in tMessages)
                {
                    tWordCount = tWordCount + message.Split(new char[] { '.', '?', '!', ' ', ';', ':', ',' }, StringSplitOptions.RemoveEmptyEntries).Count();
                }

                totalMessages = totalMessages + tMessageCount;
                totalWords = totalWords + tWordCount;

                var participantDto = new ParticipantDTO(participant.Key, tFirstMessage, tLastMessage, tMessageCount, tWordCount);
                participantsDTO.Add(participantDto);
            }

            foreach (var participant in participantsDTO)
            {
                decimal tMessageCountPercentage = Math.Round((decimal)(participant.MessageCounter * 100) / totalMessages);
                decimal tWordCountPercentage = Math.Round((decimal)(participant.WordsCounter * 100) / totalWords);
                participant.AddMessageAndWordPercentage(tMessageCountPercentage, tWordCountPercentage);
            }

            return participantsDTO.OrderBy(c => c.PersonName).ToList();
        }
        private async Task UpdateNameChat(int FolderId, ParticipantDTO participant)
        {
            var newName = participant.NewName;
            var oldName = participant.PersonName;

            var toUpdate = await _context.Chats.Where(c => c.PersonName == oldName && c.FolderId == FolderId).ToListAsync();

            foreach (var item in toUpdate)
            {
                item.PersonName = newName;
            }
            await _context.SaveChangesAsync();
        }

        private async Task DeleteNameChat(int FolderId, string name)
        {
            var toDelete = await _context.Chats.Where(c => c.FolderId == FolderId && c.PersonName == name).ToListAsync();
            _context.Chats.RemoveRange(toDelete);

            await _context.SaveChangesAsync();
        }

        public async Task<List<ParticipantDTO>> UpdateParticipantsChat(int FolderId, List<ParticipantDTO> participants)
        {
            //check for differences and deletions, deletions have priority over updates
            foreach (var item in participants)
            {
                if (item.ToDelete == false && item.PersonName != item.NewName && !String.IsNullOrWhiteSpace(item.NewName))
                {
                    await UpdateNameChat(FolderId, item);
                }
                else if (item.ToDelete == true)
                {
                    await DeleteNameChat(FolderId, item.PersonName);
                }
            }

            var updatedInfo = await GetChatParticipants(FolderId);

            return updatedInfo;
        }

        public async Task<List<ChatUploadDTO>> GetChatUploadDate(int id)
        {
            var chatList = new List<ChatUploadDTO>();

            var chatDates = await _context.Chats.Where(c => c.FolderId == id).Select(c => c.SystemTime).Distinct().ToListAsync();

            foreach (var date in chatDates)
            {
                var dateCounter = await _context.Chats.Where(c => c.SystemTime == date && c.FolderId == id).CountAsync();
                chatList.Add(new ChatUploadDTO
                {
                    ChatCount = dateCounter,
                    UploadDate = date
                });
            }

            return chatList;
        }
        public async Task DeleteDateChat(int FolderId, ChatUploadDTO dto)
        {
            var date = dto.UploadDate;
            var toDelete = await _context.Chats.Where(c => c.FolderId == FolderId && c.SystemTime == date).ToListAsync();
            _context.Chats.RemoveRange(toDelete);

            await _context.SaveChangesAsync();
        }

       
        private static SemaphoreSlim semaphore;
        private async Task ProcessTxt(Upload file)
        {
            string WhatsappDate = _conf["WhatsappDate"];
            var systemTime = DateTime.Now;
            var chatList = new List<ChatMessage>();
            var toString = Encoding.UTF8.GetString(file.FileContent);
            var entityChat = await _context.Chats.Where(c => c.FolderId == file.FolderId).Select(c => new Tuple<string, string, DateTime>(c.Message, c.PersonName, c.MessageTime)).ToListAsync();
            var hashSet = new HashSet<Tuple<string, string, DateTime>>(entityChat);

            string[] lines = toString.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );

            //not a fan of this approach
            var linesCounter = lines.Count() - 1;

            var messageDate = DefaultHelper.ValidDate(lines[0], WhatsappDate);
            var messageSender = DefaultHelper.ValidSender(lines[0]);
            var messageText = DefaultHelper.CleanMessage(lines[0]);

            for (int i = 0; i < linesCounter; i++)
            {
                var date = DefaultHelper.ValidDate(lines[i], WhatsappDate);
                var sender = DefaultHelper.ValidSender(lines[i]);
                var message = DefaultHelper.CleanMessage(lines[i]);

                if (date != null)
                {
                    if (String.IsNullOrWhiteSpace(message) == false)
                    {
                        if (String.IsNullOrWhiteSpace(messageText) == false && String.IsNullOrWhiteSpace(messageSender) == false)
                        {

                            if (!(hashSet.Contains(new Tuple<string, string, DateTime>(messageText, messageSender, messageDate.Value))))
                            {

                                var newChat = new ChatMessage(messageSender, messageDate.Value, systemTime, messageText, file.FolderId);
                                chatList.Add(newChat);

                                if (chatList.Count > 9999)
                                {
                                    _context.Chats.AddRange(chatList);
                                    await _context.SaveChangesAsync();
                                    chatList.Clear();
                                }
                            }
                        }

                        messageDate = date;
                        messageSender = sender;
                        messageText = message;
                    }
                }
                else
                {
                    //Keep adding the message text until a new line is avaliable.
                    messageText = String.Concat(messageText, " \n ", message);
                }
            }
            _context.Chats.AddRange(chatList);
            await _context.SaveChangesAsync();

            _context.Uploads.Remove(file);
            await _context.SaveChangesAsync();
        }
        private async Task ProcessJson(Upload file)
        {
            var systemTime = DateTime.Now;
            var toString = Encoding.UTF8.GetString(file.FileContent);
            var chatList = new List<ChatMessage>();
            var jsonChatList = JsonSerializer.Deserialize<List<RootWhatsapp>>(toString);
            var entityChat = await _context.Chats.Where(c => c.FolderId == file.FolderId).Select(c => new Tuple<string, DateTime>(c.Message, c.MessageTime)).ToListAsync();
            var hashSet = new HashSet<Tuple<string, DateTime>>(entityChat);

            //!await MessageAlreadyExist(tempDate, ChatMessage.MsgContent, file.FolderId)
            foreach (var ChatMessage in jsonChatList)
            {
                var tempDate = DateTime.Parse(ChatMessage.Date);
                if (!(hashSet.Contains(new Tuple<string, DateTime>(ChatMessage.MsgContent, tempDate))))
                {
                    var newChat = new ChatMessage(ChatMessage.From, tempDate, systemTime, ChatMessage.MsgContent, file.FolderId);
                    chatList.Add(newChat);

                    if (chatList.Count > 5000)
                    {
                        _context.Chats.AddRange(chatList);
                        await _context.SaveChangesAsync();
                        chatList.Clear();
                    }
                }
            }

            _context.Chats.AddRange(chatList);
            await _context.SaveChangesAsync();

            _context.Uploads.Remove(file);
            await _context.SaveChangesAsync();
        }
        public async Task ProcessEntityFiles()
        {
            semaphore = new SemaphoreSlim(1, 1);

            await semaphore.WaitAsync();

            try
            {
                var fileList = await _context.Uploads.ToListAsync();

                foreach (var file in fileList)
                {
                    await _context.SaveChangesAsync();

                    if (file.FileName.EndsWith("txt"))
                    {
                        await ProcessTxt(file);
                    }
                    else if (file.FileName.EndsWith("json"))
                    {
                        await ProcessJson(file);
                    }
                    else
                    {
                        _context.Uploads.Remove(file);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
