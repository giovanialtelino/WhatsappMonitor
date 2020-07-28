using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Threading.Tasks.Sources;
using WhatsappMonitor.API.Context;
using WhatsappMonitor.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Text;
using System.IO;

namespace WhatsappMonitor.API.Repository
{
    public class ChatsRepository
    {
        private readonly MyDbContext _context;
        public ChatsRepository(MyDbContext context)
        {
            _context = context;
        }

        //Before adding must check if a equal message with the same date doesn't exist... 
        //Will take some performance, but there is no UUID or similar to keep a single message
        private async Task<bool> MessageAlreadyExist(DateTime messageTime, string message, int entityId)
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

        public async Task<List<Chat>> GetAllChatsEntity(int id)
        {
            return await _context.Chats.Where(c => c.EntityId == id).OrderBy(c => c.EntityId).ToListAsync();
        }

        public async Task<List<Chat>> GetAllChatsPagination(int id, int pagination)
        {
            var cleanPagination = 0;
            if (pagination >= 0) cleanPagination = pagination;

            cleanPagination = cleanPagination - 50;

            var chat = await _context.Chats.Where(c => c.EntityId == id).OrderByDescending(c => c.MessageTime).ToListAsync();
            var result = chat.Skip(cleanPagination).Take(50).ToList();
            return result;
        }

        public async Task<int> SearchEntityChatTextByDate(int id, string date)
        {
            var parsedDate = DateTime.Parse(date);
            var chat = await _context.Chats.Where(c => c .EntityId == id && c.MessageTime >= parsedDate).OrderByDescending(c => c.MessageTime).CountAsync();

            return chat;
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

            return participants.OrderByDescending(c => c.MessageCounterPercentage).ToList();
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

       public async Task<List<Upload>> GetUploadAwaiting(int id)
       {
           var uploadList =await _context.Uploads.Where(e => e.EntityId == id).ToListAsync();

           foreach (var item in uploadList)
           {
               item.FileContent = null;
           }

           return uploadList;
       }

        public async Task DeleteDateChat(int entityId, ChatUploadDTO dto)
        {
            var date = dto.UploadDate;
            var toDelete = await _context.Chats.Where(c => c.EntityId == entityId && c.SystemTime == date).ToListAsync();
            _context.Chats.RemoveRange(toDelete);

            await _context.SaveChangesAsync();
        }

        private async Task<ChatInfoDate> CheckDates(string from, string until)
        {
            if (String.IsNullOrWhiteSpace(from) && String.IsNullOrWhiteSpace(until))
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

        public async Task<TotalFolderInfoDTO> GetFullChatInfo(int entityId, string from, string until)
        {
            var checkedDates = await CheckDates(from, until);
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

        public async Task<List<ChatPersonInfoDTO>> GetChatParticipantsInfo(int entityId, string from, string until)
        {
            var personList = new List<ChatPersonInfoDTO>();

            var checkedDates = await CheckDates(from, until);
            var Messages = await _context.Chats.Where(e => e.EntityId == entityId).ToListAsync();

            var personMessages = Messages.GroupBy(c => c.PersonName);


            foreach (var person in personMessages)
            {
                var messageCounter = person.Select(c => c.Message).Count();
                var wordCounter = 0;
                var personWordList = new List<String>();

                foreach (var message in person.Select(c => c.Message))
                {
                    var splits = message.Split(new char[] { '.', '?', '!', ' ', ';', ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    wordCounter = wordCounter + splits.Count();
                    foreach (var word in splits)
                    {
                        personWordList.Add(word);
                    }
                }

                var commonWords = personWordList.GroupBy(c => c).Select(c => new Tuple<string, int>(c.Key.ToString(), c.Count())).OrderByDescending(c => c.Item2).Take(10).ToList();

                var commonHours = person.GroupBy(c => c.MessageTime.Hour).Select(c => new Tuple<string, int>(c.Key.ToString(), c.Count())).ToList();
                var totalHours = commonHours.Sum(c => c.Item2);

                var commonHoursPercentage = new List<Tuple<string, double>>();

                foreach (var item in commonHours)
                {
                    double percentage = (item.Item2 * 100) / messageCounter;
                    commonHoursPercentage.Add(new Tuple<string, double>(item.Item1, percentage));
                }

                personList.Add(new ChatPersonInfoDTO
                {
                    PersonName = person.Select(c => c.PersonName).FirstOrDefault(),
                    MessageCounter = messageCounter,
                    WordsCounter = wordCounter,
                    CommonWords = commonWords,
                    Hours = commonHours
                });
            }

            var totalMessages = personList.Sum(c => c.MessageCounter);
            var totalWords = personList.Sum(c => c.WordsCounter);

            foreach (var item in personList)
            {
                var message = (item.MessageCounter * 100) / totalMessages;
                var words = (item.WordsCounter * 100) / totalWords;
                item.MessagePercentage = message;
                item.WordsPercentage = words;
            }
            return personList;
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

        public async Task ProcessEntityFiles()
        {
            semaphore = new SemaphoreSlim(1, 1);

            await semaphore.WaitAsync();

            try
            {
                var fileList = await _context.Uploads.AsNoTracking().ToListAsync();
                var systemTime = DateTime.Now;

                foreach (var file in fileList)
                {
                    var chatList = new List<Chat>();
                    var toString = Encoding.UTF8.GetString(file.FileContent);

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
                                    if (!await MessageAlreadyExist(messageDate.Value, messageText, file.EntityId))
                                    {
                                        var newChat = new Chat(messageSender, messageDate.Value, systemTime, messageText, file.EntityId);
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
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
