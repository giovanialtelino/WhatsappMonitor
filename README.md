# Whatsapp Monitor

It was supposed to be a personal CRM, or should I call it a PRM (Personal relationship management), my original idea was to offer an open-source version in which you could deploy your own monitor and also a hosted version, so I could also earn some money back.  

After a kinda successful brainstorming of nice ideas about how to display the data and what was supposed to be the main functionalities of the project, I was disappointed while I built the MVP, Whatsapp has no easy way to generate a backup of any "readable" kind from all the contacts, you can only backup a user or a group each time, and with a limit of around 40 thousand messages so there is also a great chance that you won't have all the messages from groups you are in since years ago. The Whatsapp backup which is usually created daily at mid dawn only can be used by the WhatsApp app, amazing!  

I would argue that no one would like the idea to generate the backup of all the contacts which you sent a message in the week, even more in a third-party app, where my target would be power users who use Whatsapp a lot, so yeah, the project is dead, RIP, but the project license is MIT, so you can do whatever you want with it.
But still, since I had already some of the back-end ready, I decided to build it a little more, so I could also take a look at the Blazor server.

[For the full story, and software ideas, please check my blog](https://www.giovanialtelino.com/project/whatsapp-monitor/ "blog").

## Wiki
Please check the [project wiki](https://github.com/giovanialtelino/WhatsappMonitor/wiki), all the information and examples about the deployment and functionalities are there.

### Observation

Keep in mind that I did nothing to encrypt the data, it's all stored in plain text in you computer, anyone with access to your computer, may have access to your chats., also **docker-compose down -v** won't delete the folder with the data, you must manually delete it, if you need to.


