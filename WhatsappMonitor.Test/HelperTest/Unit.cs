using System;
using Xunit;
using WhatsappMonitor.API.Helper;
using System.Collections.Generic;
using WhatsappMonitor.Shared.Models;

namespace WhatsappMonitor.Test.Helper
{
    public class UnitTest1
    {
        [Fact]
        public void SearchChatTextTest()
        {
            var msgList = new List<ChatMessage>();
            msgList.Add(new ChatMessage("tester", DateTime.Now, DateTime.Now, "ShOuLD IgNoRe Case", 0));

            var search = "should";

            var helper = DefaultHelper.SearchChatText(search, msgList);

            Assert.Equal(helper.Count, 1);
        }

        [Fact]
        public void ExpectValidDateTest()
        {
            var dateFormat = "dd/MM/yyyy HH:mm";
            var line = "31/03/2019 03:23 - Giovani: To aqui cara";

            var parsedDate = DefaultHelper.ValidDate(line, dateFormat);

            Assert.NotNull(parsedDate);
        }

        [Fact]
        public void ExpectValidSenderTest()
        {
            var line = "31/03/2019 03:23 - Giovani: To aqui cara";

            var sender = DefaultHelper.ValidSender(line);

            Assert.Equal(sender, "Giovani");
        }

        [Fact]
        public void ExpectCleanMessageTest ()
        {
        //Given
        var line = "31/03/2019 03:23 - Giovani: To aqui cara";

        var message = DefaultHelper.CleanMessage(line);

        Assert.Equal(message, "To aqui cara");
        }

    }
}
