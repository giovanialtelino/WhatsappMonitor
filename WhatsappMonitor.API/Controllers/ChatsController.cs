using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhatsappMonitor.Shared.Models;
using WhatsappMonitor.API.Context;
using Microsoft.EntityFrameworkCore;
using WhatsappMonitor.API.Repository;

namespace WhatsappMonitor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : Controller
    {
        private ChatsRepository _repo;
        public ChatsController()
        {
            this._repo = new ChatsRepository(new MyDbContext());
        }

        [HttpGet("group/{id}")]
        public async Task<ActionResult<List<Chat>>> GetAllChatsGroup(int id)
        {
            return await _repo.GetAllChatsGroup(id);
        }

        [HttpGet("user/{id}")]
        public async Task<ActionResult<List<Chat>>> GetAllChatsUser(int id)
        {
            return await _repo.GetAllChatsUsers(id);
        }

        [HttpGet("group-search/{id}/{message}")]
        public async Task<ActionResult<List<Chat>>> SearchChatGroup(string message, int id)
        {
            return await _repo.SearchGroupChatText(message, id);
        }

        [HttpGet("user-search/{id}/{message}")]
        public async Task<ActionResult<List<Chat>>> SearchChatUser(string message, int id)
        {
            return await _repo.SearchUserChatText(message, id);
        }

        [HttpGet("group-members/{id}")]
        public async Task<ActionResult<List<ParticipantDTO>>> GetChatGroupParticipant(int id)
        {
            return await _repo.GetChatGroupParticipant(id);
        }

        [HttpGet("user-members/{id}")]
        public async Task<ActionResult<List<ParticipantDTO>>> GetChatUserParticipant(int id)
        {
            return await _repo.GetChatUserParticipant(id);
        }


    }
}