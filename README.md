# Whatsapp Monitor

It was supposed to be a personal CRM, or should I call it a PRM (Personal relationship management) :smile: , my original idea was to offer a open source version were you could deploy your own monitor and also a hosted version, so I could also earn some money back.

After a kinda successful brainstorming of nice ideas about how to display the data and what was supposed to be the main functionalities of the project, I was dissapointed while I built the MVP, Whatsapp has no easy way to generate a backup of any "readable" kind from all the contacts, you can only backup a user or a group each time, and with a limit of around 40 thousand messages so there is also a great chance that you won't have all the messages from groups you are in since years ago. The whatsapp backup which is usually created daily at mid dawn only can be used by the whatsapp app, amazing!

I would argue that no one would like the idea to generate the backup of all the contacts which you sent message in the week, even more in a third-party app, were my target would be power users who use Whatsapp a lot, so yeah, the project is dead, RIP, but I changed the license to MIT, so you can do whatever you want with it.

But still, since I had already some of the back-end ready, I decided to built it a little more, so I could also take a look on Blazor server.

[For the full story, thoughts and ideas, please check my blog.](https://www.giovanialtelino.com/project/whatsapp-monitor/ "blog").

### Deploy

You can easily deploy the system with **docker-compose**, you just need to type **docker-compose up**, after you cloned the repository or downloaded the zip file to your local computer.  **BUT** you must do some changes it the docker-compose.yml file:

**WhatsappDate**: Must be updated to the date format the backup .txt, take a look [here](https://docs.microsoft.com/en-us/dotnet/api/system.datetime.tryparseexact?view=netcore-3.1#System_DateTime_TryParseExact_System_String_System_String___System_IFormatProvider_System_Globalization_DateTimeStyles_System_DateTime__) if you are in doubt of how to write the format. If you are in the USA you simple change dd/MM/yyyy HH:mm to MM/dd/yyyy HH:mm, and don't write quotes in the docker-compose.yml.

**volumes**: Must be updated to a certain path in your file-system, just create any folder and copy the whole absolute path as in the example with my name.

Keep in mind that I did nothing to encrypt the data, it's all stored in plain text in you computer, anyone with access to your computer, may have access to your chats., also **docker-compose down -v** won't delete the folder with the data, you must manually delete it, if you need to.