using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WhatsappMonitor.Shared.Models;
using WhatsappMonitor.API.Context;
using WhatsappMonitor.API.Services;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace WhatsappMonitor.API.Helper
{
    public static class DefaultHelper
    {

        public static List<ChatMessage> SearchChatText(string text, List<ChatMessage> messages)
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

        public static async Task<ChatInfoDate> CheckDates(string from, string until, MyDbContext _context)
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

        public static DateTime? ValidDate(string line, string dateFormat)
        {


            var start = 0;
            var datePosition = line.IndexOf('-', start);
            if (datePosition != -1)
            {
                var temp = line.Substring(start, datePosition - start + 1).Trim();

                if (temp.Length < 6) return null;

                var dateString = temp.Remove(temp.Length - 2);
                var parsedDate = new DateTime();

                if (DateTime.TryParseExact(dateString, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
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
        public static String ValidSender(string line)
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
        public static String CleanMessage(string line)
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
      
    }
}