using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhatsappMonitor.API.Context;
using WhatsappMonitor.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;

namespace WhatsappMonitor.API.Repository
{
    public class ChatsRepository
    {
        private readonly MyDbContext _context;
        public ChatsRepository(MyDbContext context)
        {
            _context = context;
        }

        private Chat Cleaner(string uncleaned)
        {
            //bug when multiline chats are added, MUST BE FIXED
            try
            {

                String value = uncleaned;
                String valueAfterDate = "";
                String valueAfterName = "";
                var dateString = "";
                var nameString = "";
                var messageString = "";
                var temp = "";
                int start = 0;
                // Extract sentences from the string.
                var datePosition = value.IndexOf('-', start);
                temp = value.Substring(start, datePosition - start + 1).Trim();
                dateString = temp.Remove(temp.Length - 2);

                valueAfterDate = value.Substring(datePosition - start + 1).Trim();
                var namePosition = valueAfterDate.IndexOf(':', start);
                if (namePosition != -1)
                {
                    temp = valueAfterDate.Substring(start, namePosition - start + 1).Trim();
                    nameString = temp.Remove(temp.Length - 1);
                    valueAfterName = valueAfterDate.Substring(namePosition - start + 1).Trim();
                    if (!((valueAfterName.StartsWith("<")) && (valueAfterName.EndsWith(">"))))
                    {
                        messageString = valueAfterName;
                    }
                    else
                    {
                        return null;
                    }

                    var parsedDate = new DateTime();
                    try
                    {
                        parsedDate = DateTime.ParseExact(dateString, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                    }
                    catch (System.Exception)
                    {
                        return null;
                    }

                    return new Chat(nameString, messageString, parsedDate); ;

                }
                return null;
            }
            catch (System.Exception)
            {
                return null;
            }

        }

        //Before adding must check if a equal message with the same date doesn't exist... 
        //Will take some performance, but there is no UUID or similar to keep a single message
        public async Task<bool> MessageAlreadyExist(DateTime messageTime, string message, int entityId)
        {
            var result = await _context.Chats.FirstOrDefaultAsync(c => c.Message == message && c.MessageTime == messageTime && c.EntityId == entityId);
            if (result == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<int> CleanAddChat(string line, int entityId, DateTime systemTime)
        {
            var cleaned = Cleaner(line);
            if (cleaned != null)
            {
                if (!await MessageAlreadyExist(cleaned.MessageTime, cleaned.Message, entityId))
                {
                    var temp = new Chat(cleaned.PersonName, cleaned.MessageTime, systemTime, cleaned.Message, entityId);
                    _context.Chats.Add(temp);
                    await _context.SaveChangesAsync();
                    return 1;
                }
            }
            return 0;
        }

        public async Task<List<Chat>> GetAllChatsEntity(int id)
        {
            return await _context.Chats.Where(c => c.EntityId == id).OrderBy(c => c.EntityId).ToListAsync();
        }

        public async Task<List<Chat>> GetAllChatsPagination(int id, int pagination)
        {
            var chat = await _context.Chats.Where(c => c.EntityId == id).OrderByDescending(c => c.MessageTime).ToListAsync();
            var result = chat.Skip(pagination).Take(50).ToList();
            return result;
        }

        private List<Chat> SearchChatText(string text, List<Chat> messages)
        {
            var listChat = new List<Chat>();
            foreach (var item in messages)
            {
                if (item.Message.Contains(text))
                {
                    listChat.Add(item);
                }
            }
            return listChat;
        }

        public async Task<List<Chat>> SearchEntityChatText(string text, int id)
        {
            var messages = await _context.Chats.Where(c => c.EntityId == id).OrderByDescending(c => c.MessageTime).ToListAsync();
            return SearchChatText(text, messages);
        }

        public async Task<List<ParticipantDTO>> GetChatParticipants(int id)
        {
            var participants = new List<ParticipantDTO>();
            var users = await _context.Chats.Where(c => c.EntityId == id).Select(c => c.PersonName).Distinct().ToListAsync();
            var totalMessages = 0;
            var totalWords = 0;

            foreach (var user in users)
            {
                var firstMessage = await _context.Chats.Where(c => c.EntityId == id && c.PersonName == user).MinAsync(c => c.MessageTime);
                var messages = await _context.Chats.Where(c => c.EntityId == id && c.PersonName == user).Select(c => c.Message).ToListAsync();
                var messageCounter = messages.Count();
                var wordCounter = 0;

                foreach (var item in messages)
                {
                    wordCounter = wordCounter + item.Split(new char[] { '.', '?', '!', ' ', ';', ':', ',' }, StringSplitOptions.RemoveEmptyEntries).Count();
                }

                totalMessages = totalMessages + messageCounter;
                totalWords = totalWords + wordCounter;

                participants.Add(new ParticipantDTO
                {
                    MessageCounter = messageCounter,
                    FirstMessage = firstMessage,
                    PersonName = user,
                    FixedName = user,
                    WordsCounter = wordCounter
                });
            }

            foreach (var item in participants)
            {
                item.MessageCounterPercentage = (item.MessageCounter * 100) / totalMessages;
                item.WordsCounterPercentage = (item.WordsCounter * 100) / totalWords;
            }

            return participants.OrderBy(c => c.PersonName).ToList();
        }

        public async Task UpdateNameChat(int entityId, ParticipantDTO participant)
        {
            var newName = participant.PersonName;
            var oldName = participant.FixedName;

            var toUpdate = await _context.Chats.Where(c => c.PersonName == oldName).ToListAsync();

            foreach (var item in toUpdate)
            {
                item.PersonName = newName;

            }

            await _context.SaveChangesAsync();

        }

        public async Task DeleteNameChat(int entityId, string name)
        {
            var toDelete = await _context.Chats.Where(c => c.EntityId == entityId && c.PersonName == name).ToListAsync();
            _context.Chats.RemoveRange(toDelete);

            await _context.SaveChangesAsync();
        }

        public async Task<List<ChatUploadDTO>> GetChatUploadDate(int id)
        {
            var chatList = new List<ChatUploadDTO>();

            var chatDates = await _context.Chats.Where(c => c.EntityId == id).Select(c => c.SystemTime).Distinct().ToListAsync();

            foreach (var date in chatDates)
            {
                var dateCounter = await _context.Chats.Where(c => c.SystemTime == date && c.EntityId == id).CountAsync();
                chatList.Add(new ChatUploadDTO
                {
                    ChatCount = dateCounter,
                    UploadDate = date
                });
            }

            return chatList;
        }


        public async Task DeleteDateChat(int entityId, ChatUploadDTO dto)
        {
            var date = dto.UploadDate;
            var toDelete = await _context.Chats.Where(c => c.EntityId == entityId && c.SystemTime == date).ToListAsync();
            _context.Chats.RemoveRange(toDelete);

            await _context.SaveChangesAsync();
        }

        private async Task<ChatInfoDate> CheckDates(ChatInfoDate dates)
        {
            if (dates.From != null && dates.Until != null)
            {
                return dates;
            }
            else
            {
                var from = await _context.Chats.MinAsync(c => c.MessageTime);
                var until = await _context.Chats.MaxAsync(c => c.MessageTime);
                var startToFinish = new ChatInfoDate { From = from, Until = until };
                return startToFinish;
            }
        }

        public async Task<TotalFolderInfoDTO> GetFullChatInfo(int entityId, ChatInfoDate dates)
        {
            var checkedDates = await CheckDates(dates);
            var messages = await _context.Chats.Where(e => e.EntityId == entityId).ToListAsync();
            var totalMessages = messages.Count();
            var wordCounter = 0;
            var superWordList = new List<string>();

            foreach (var item in messages)
            {
                var splits = item.Message.Split(new char[] { '.', '?', '!', ' ', ';', ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
                wordCounter = wordCounter + splits.Count();
                foreach (var word in splits)
                {
                    superWordList.Add(word);
                }
            }

            var commonHours = messages.GroupBy(c => c.MessageTime.Hour).Select(c => new Tuple<string, int>(c.Key.ToString(), c.Count())).OrderBy(c => c.Item1).ToList();

            var commonWords = superWordList.GroupBy(c => c).Select(c => new Tuple<string, int>(c.Key.ToString(), c.Count())).OrderByDescending(c => c.Item2).Take(10).ToList();

            var total = new TotalFolderInfoDTO
            {
                TotalMessage = totalMessages.ToString(),
                TotalWords = wordCounter.ToString(),
                CommonWords = commonWords,
                CommonHours = commonHours
            };

            return total;
        }
    }
}