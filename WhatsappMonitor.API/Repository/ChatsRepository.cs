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

        public async Task CleanAddChatGroup(string line, int groupId)
        {
            var cleaned = Cleaner(line);

            if (cleaned != null)
            {
                var temp = new Chat
                {
                    PersonName = cleaned.PersonName,
                    MessageTime = cleaned.MessageTime,
                    Message = cleaned.Message,
                    GroupId = groupId
                };
                Console.WriteLine(temp);
                _context.Chats.Add(temp);
                await _context.SaveChangesAsync();
            }
        }

        public async Task CleanAddChatUser(string line, int userId)
        {
            var cleaned = Cleaner(line);
            if (cleaned != null)
            {
                _context.Chats.Add(cleaned);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Chat>> GetAllChatsGroup(int id)
        {
            return await _context.Chats.Where(c => c.GroupId == id).ToListAsync();
        }
    }
}
