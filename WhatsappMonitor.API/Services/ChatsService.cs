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

namespace WhatsappMonitor.API.Services
{
    public interface IChatsMessageService
    {
        Task<Tuple<PaginationDTO, List<ChatMessage>>> GetAllChatsPagination(int id, int pagination, int take);
        Task<List<ChatMessage>> GetChatsAfter(int id, string last);
        Task<List<ChatMessage>> GetChatsBefore(int id, string first);
        Task<int> SearchEntityChatTextByDate(int id, string date);
        Task<List<ChatMessage>> SearchEntityChatText(string text, int id);
        Task<List<ParticipantDTO>> GetChatParticipants(int id);
        Task<List<ParticipantDTO>> UpdateParticipantsChat(int FolderId, List<ParticipantDTO> participants);
        Task<List<ChatUploadDTO>> GetChatUploadDate(int id);
        Task DeleteDateChat(int FolderId, ChatUploadDTO dto);
        Task<TotalFolderInfoDTO> GetFullChatInfo(int FolderId, string from, string until);
        Task ProcessEntityFiles();
    }

    public class ChatsMessageService : IChatsMessageService
    {
        private readonly MyDbContext _context;
        public ChatsMessageService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChatMessage>> GetChatsAfter(int id, string last)
        {
            var parsedDate = DateTime.Parse(last);
            var chatMessage = await _context.Chats.Where(c => c.FolderId == id && c.MessageTime > parsedDate).OrderByDescending(c => c.MessageTime).ToListAsync();

            return chatMessage.Take(200).ToList();
        }
        public async Task<List<ChatMessage>> GetChatsBefore(int id, string first)
        {
            var parsedDate = DateTime.Parse(first);
            var chatMessage = await _context.Chats.Where(c => c.FolderId == id && c.MessageTime < parsedDate).OrderByDescending(c => c.MessageTime).ToListAsync();

            return chatMessage.Take(200).ToList();
        }

        public async Task<Tuple<PaginationDTO, List<ChatMessage>>> GetAllChatsPagination(int id, int skip, int take)
        {
            var cleanTake = 200;
            var cleanSkip = 0;
            //if (skip >= 0) cleanSkip = skip;
            //if (take >= 0 && take <= 100) cleanTake = take;

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

        public async Task<int> SearchEntityChatTextByDate(int id, string date)
        {
            var parsedDate = DateTime.Parse(date);
            var ChatMessage = await _context.Chats.Where(c => c.FolderId == id && c.MessageTime >= parsedDate).OrderByDescending(c => c.MessageTime).CountAsync();

            return ChatMessage - 1;
        }
        private List<ChatMessage> SearchChatText(string text, List<ChatMessage> messages)
        {
            var listChat = new List<ChatMessage>();
            foreach (var item in messages)
            {
                var lowText = text.ToLower();
                if (item.Message.ToLower().Contains(lowText))
                {
                    listChat.Add(item);
                }
            }
            return listChat;
        }
        public async Task<List<ChatMessage>> SearchEntityChatText(string text, int id)
        {
            var messages = await _context.Chats.Where(c => c.FolderId == id).OrderByDescending(c => c.MessageTime).ToListAsync();
            var findText = SearchChatText(text, messages);
            return findText;
        }

        public async Task<List<ParticipantDTO>> GetChatParticipants(int id)
        {
            var participantsDTO = new List<ParticipantDTO>();
            var totalMessages = 0;
            var totalWords = 0;

            //bring everything already grouped by user
            var participantsStep1 = await _context.Chats.Where(c => c.FolderId == id).ToListAsync();
            var participants = participantsStep1.GroupBy(c => c.PersonName);

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
            //check for differentes and deletions, deletions have priority over updates
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

        private async Task<ChatInfoDate> CheckDates(string from, string until)
        {
            if (!String.IsNullOrWhiteSpace(from) && !String.IsNullOrWhiteSpace(until))
            {
                return new ChatInfoDate { From = DateTime.Parse(from), Until = DateTime.Parse(until) };
            }
            else
            {
                var fromMin = await _context.Chats.MinAsync(c => c.MessageTime);
                var untilMax = await _context.Chats.MaxAsync(c => c.MessageTime);
                var startToFinish = new ChatInfoDate { From = fromMin, Until = untilMax };
                return startToFinish;
            }
        }

        public async Task<TotalFolderInfoDTO> GetFullChatInfo(int FolderId, string from, string until)
        {
            var checkedDates = await CheckDates(from, until);
            var messages = await _context.Chats.Where(e => e.FolderId == FolderId && e.MessageTime > checkedDates.From && e.MessageTime < checkedDates.Until).ToListAsync();
            var totalMessages = messages.Count();
            var wordCounter = 0;
            var superWordList = new List<string>();

            foreach (var item in messages)
            {
                var splits = item.Message.Split(new char[] { '.', '?', '!', ' ', ';', ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
                wordCounter = wordCounter + splits.Count();
                foreach (var word in splits)
                {
                    if (word.Length > 5)
                    {
                        superWordList.Add(word);
                    }
                }
            }

            var commonHours = messages.GroupBy(c => c.MessageTime.Hour).Select(c => new Tuple<string, int>(c.Key.ToString(), c.Count())).ToList();

            var totalHours = commonHours.Sum(c => c.Item2);
            var commonHoursPercentage = new List<Tuple<string, double>>();

            foreach (var item in commonHours)
            {
                double percentage = (item.Item2 * 100) / totalMessages;
                commonHoursPercentage.Add(new Tuple<string, double>(item.Item1, percentage));
            }

            var commonWords = superWordList.GroupBy(c => c).Select(c => new Tuple<string, double>(c.Key.ToString(), c.Count())).OrderByDescending(c => c.Item2).Take(10).ToList();

            var total = new TotalFolderInfoDTO
            {
                TotalMessage = totalMessages,
                TotalWords = wordCounter,
                CommonWords = commonWords,
                CommonHours = commonHoursPercentage.OrderByDescending(c => c.Item2).Take(10).ToList()
            };
            return total;
        }

        private DateTime? ValidDate(string line)
        {
            var start = 0;
            var datePosition = line.IndexOf('-', start);
            if (datePosition != -1)
            {
                var temp = line.Substring(start, datePosition - start + 1).Trim();

                if (temp.Length < 6) return null;

                var dateString = temp.Remove(temp.Length - 2);
                var parsedDate = new DateTime();
                if (DateTime.TryParseExact(dateString, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                {
                    return parsedDate;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        private String ValidSender(string line)
        {
            var start = 0;
            var datePosition = line.IndexOf('-', start);
            var valueAfterDate = line.Substring(datePosition - start + 1).Trim();
            var namePosition = valueAfterDate.IndexOf(':', start);
            if (namePosition != -1)
            {
                var temp = valueAfterDate.Substring(start, namePosition - start + 1).Trim();
                return temp.Remove(temp.Length - 1);
            }
            else
            {
                return "";
            }
        }
        private String CleanMessage(string line)
        {
            var start = 0;
            var datePosition = line.IndexOf('-', start);
            var valueAfterDate = line.Substring(datePosition - start + 1).Trim();
            var namePosition = valueAfterDate.IndexOf(':', start);
            var valueAfterName = valueAfterDate.Substring(namePosition - start + 1).Trim();

            if (!((valueAfterName.StartsWith("<")) && (valueAfterName.EndsWith(">"))))
            {
                return valueAfterName;
            }
            else
            {
                return "";
            }
        }
        private static SemaphoreSlim semaphore;
        private async Task ProcessTxt(Upload file)
        {

            var systemTime = DateTime.Now;
            var chatList = new List<ChatMessage>();
            var toString = Encoding.UTF8.GetString(file.FileContent);
            var entityChat = await _context.Chats.Where(c => c.FolderId == file.FolderId).Select(c => new Tuple<string, DateTime>(c.Message, c.MessageTime)).ToListAsync();
            var hashSet = new HashSet<Tuple<string, DateTime>>(entityChat);

            string[] lines = toString.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );

            //not a fan of this approach
            var linesCounter = lines.Count() - 1;

            var messageDate = ValidDate(lines[0]);
            var messageSender = ValidSender(lines[0]);
            var messageText = CleanMessage(lines[0]);

            for (int i = 0; i < linesCounter; i++)
            {
                var date = ValidDate(lines[i]);
                var sender = ValidSender(lines[i]);
                var message = CleanMessage(lines[i]);

                if (date != null)
                {
                    if (String.IsNullOrWhiteSpace(message) == false)
                    {
                        if (String.IsNullOrWhiteSpace(messageText) == false && String.IsNullOrWhiteSpace(messageSender) == false)
                        {

                            if (!(hashSet.Contains(new Tuple<string, DateTime>(messageText, messageDate.Value))))
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
