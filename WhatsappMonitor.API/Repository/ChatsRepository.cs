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

                    return new Chat
                    {
                        PersonName = nameString,
                        Message = messageString,
                        MessageTime = parsedDate
                    };

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
        public async Task<bool> GroupMessageAlreadyExist(DateTime messageTime, string message, int groupId)
        {
            var result = await _context.Chats.FirstOrDefaultAsync(c => c.Message == message && c.MessageTime == messageTime && c.GroupId == groupId);
            if (result == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<int> CleanAddChatGroup(string line, int groupId)
        {
            var cleaned = Cleaner(line);
            if (cleaned != null)
            {

                if (!await GroupMessageAlreadyExist(cleaned.MessageTime, cleaned.Message, groupId))
                {
                    var temp = new Chat
                    {
                        PersonName = cleaned.PersonName,
                        MessageTime = cleaned.MessageTime,
                        Message = cleaned.Message,
                        GroupId = groupId
                    };

                    _context.Chats.Add(temp);
                    await _context.SaveChangesAsync();
                    return 1;
                }
            }
            return 0;
        }

        //Before adding must check if a equal message with the same date doesn't exist... 
        //Will take some performance, but there is no UUID or similar to keep a single message
        public async Task<bool> UserMessageAlreadyExist(DateTime messageTime, string message, int userId)
        {
            var result = await _context.Chats.FirstOrDefaultAsync(c => c.Message == message && c.MessageTime == messageTime && c.UserId == userId);
            if (result == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<int> CleanAddChatUser(string line, int userId)
        {
            var cleaned = Cleaner(line);
            if (cleaned != null)
            {

                if (!await UserMessageAlreadyExist(cleaned.MessageTime, cleaned.Message, userId))
                {
                    var temp = new Chat
                    {
                        PersonName = cleaned.PersonName,
                        MessageTime = cleaned.MessageTime,
                        Message = cleaned.Message,
                        UserId = userId
                    };

                    _context.Chats.Add(temp);
                    await _context.SaveChangesAsync();
                    return 1;
                }
            }
            return 0;
        }

        public async Task<List<Chat>> GetAllChatsGroup(int id)
        {
            return await _context.Chats.Where(c => c.GroupId == id).OrderBy(c => c.Id).ToListAsync();
        }

        public async Task<List<Chat>> GetAllChatsUsers(int id)
        {
            return await _context.Chats.Where(c => c.UserId == id).OrderBy(c => c.Id).ToListAsync();
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

        public async Task<List<Chat>> SearchGroupChatText(string text, int id)
        {

            var messages = await _context.Chats.Where(c => c.GroupId == id).ToListAsync();
            return SearchChatText(text, messages);
        }

        public async Task<List<Chat>> SearchUserChatText(string text, int id)
        {
            var messages = await _context.Chats.Where(c => c.UserId == id).ToListAsync();
            return SearchChatText(text, messages);
        }

        public async Task<List<ParticipantDTO>> GetChatGroupParticipant(int id)
        {
            var participants = new List<ParticipantDTO>();
            var users = await _context.Chats.Where(c => c.GroupId == id).Select(c => c.PersonName).Distinct().ToListAsync();
            var totalMessages = 0;
            var totalWords = 0;

            foreach (var user in users)
            {
                var firstMessage = await _context.Chats.Where(c => c.GroupId == id && c.PersonName == user).MinAsync(c => c.MessageTime);
                var messages = await _context.Chats.Where(c => c.GroupId == id && c.PersonName == user).Select(c => c.Message).ToListAsync();
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
                    WordsCounter = wordCounter
                });
            }

            foreach (var item in participants)
            {
                item.MessageCounterPercentage = (item.MessageCounter * 100) / totalMessages;
                item.WordsCounterPercentage = (item.WordsCounter * 100) / totalWords;
            }

            return participants;
        }

        public async Task<List<ParticipantDTO>> GetChatUserParticipant(int id)
        {
            var participants = new List<ParticipantDTO>();
            var users = await _context.Chats.Where(c => c.UserId == id).Select(c => c.PersonName).Distinct().ToListAsync();
            var totalMessages = 0;
            var totalWords = 0;

            foreach (var user in users)
            {
                var firstMessage = await _context.Chats.Where(c => c.UserId == id && c.PersonName == user).MinAsync(c => c.MessageTime);
                var messages = await _context.Chats.Where(c => c.UserId == id && c.PersonName == user).Select(c => c.Message).ToListAsync();
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
                    WordsCounter = wordCounter
                });
            }

            foreach (var item in participants)
            {
                item.MessageCounterPercentage = (item.MessageCounter * 100) / totalMessages;
                item.WordsCounterPercentage = (item.WordsCounter * 100) / totalWords;
            }

            return participants;
        }
    }
}
