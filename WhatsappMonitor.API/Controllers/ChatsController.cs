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

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Chat>>> GetAllChatsEntity(int id)
        {
            return await _repo.GetAllChatsEntity(id);
        }

        [HttpGet("load/{id}/{pagination}")]
        public async Task<ActionResult<List<Chat>>> GetChatsPagination(int id, int pagination)
        {
            return await _repo.GetAllChatsPagination(id, pagination);
        }

        [HttpGet("search/{id}/{message}")]
        public async Task<ActionResult<List<Chat>>> SearchChat(string message, int id)
        {
            return await _repo.SearchEntityChatText(message, id);
        }

        [HttpGet("search-date/{id}")]
        public async Task<ActionResult<int>> SearchChat(int id, [FromQuery] string startDate)
        {
            return await _repo.SearchEntityChatTextByDate(id, startDate);
        }

        [HttpGet("members/{id}")]
        public async Task<ActionResult<List<ParticipantDTO>>> GetChatParticipant(int id)
        {
            return await _repo.GetChatParticipants(id);
        }

        [HttpGet("uploads/{id}")]
        public async Task<ActionResult<List<ChatUploadDTO>>> GetChatUploadDates(int id)
        {
            return await _repo.GetChatUploadDate(id);
        }

        [HttpGet("awaiting-process/{id}")]
        public async Task<ActionResult<List<Upload>>> GetUploadAwaiting(int id)
        {
            return await _repo.GetUploadAwaiting(id);
        }

        [HttpPut("update-name/{id}")]
        public async Task UpdateNameChat(int id, [FromBody] ParticipantDTO participant)
        {
            await _repo.UpdateNameChat(id, participant);
        }

        [HttpDelete("delete-name/{id}/{name}")]
        public async Task DeleteNameChat(int id, string name)
        {
            await _repo.DeleteNameChat(id, name);
        }

        [HttpPut("delete-date/{id}")]
        public async Task DeleteDateChat(int id, [FromBody] ChatUploadDTO dto)
        {
            await _repo.DeleteDateChat(id, dto);
        }

        [HttpGet("chat-info/{id}")]
        public async Task<TotalFolderInfoDTO> GetChatInfo(int id, [FromQuery] string from, [FromQuery] string until)
        {
            return await _repo.GetFullChatInfo(id, from, until);
        }

        [HttpGet("chat-users/{id}")]
        public async Task<List<ChatPersonInfoDTO>> GetChatParticipantsInfo(int id, [FromQuery] string from, [FromQuery] string until)
        {
            return await _repo.GetChatParticipantsInfo(id, from, until);
        }
    }
}