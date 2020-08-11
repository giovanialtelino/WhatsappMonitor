using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhatsappMonitor.Shared.Models;
using WhatsappMonitor.API.Context;
using Microsoft.EntityFrameworkCore;
using WhatsappMonitor.API.Services;

namespace WhatsappMonitor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : Controller
    {
        private readonly IChatsService _repo;
        public ChatsController(MyDbContext chat)
        {
            this._repo = new ChatsService(chat);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Chat>>> GetAllChatsEntity(int id)
        {
            return await _repo.GetAllChatsEntity(id);
        }

        [HttpGet("load/{id}/{skip}/{take}")]
        public async Task<ActionResult<Tuple<PaginationDTO, List<Chat>>>> GetChatsPagination(int id, int skip, int take)
        {
            return await _repo.GetAllChatsPagination(id, skip, take);
        }

        [HttpGet("search/{id}/{message}/{skip}/{take}")]
        public async Task<ActionResult<List<Chat>>> SearchChat(int id, string message, int skip, int take)
        {
            return await _repo.SearchEntityChatText(message, id, skip, take);
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

        [HttpGet("chat-info/message-counter/{id}")]
        public async Task<List<MessagesTime>> GetChatInfoMessageCounter(int id, [FromQuery] string from, [FromQuery] string until)
        {
            return await _repo.GetChatInfoMessageCounter(id, from, until);
        }

        [HttpGet("chat-info/word-counter/{id}")]
        public async Task<List<WordsTime>> GetChatInfoWordCounter(int id, [FromQuery] string from, [FromQuery] string until)
        {
            return await _repo.GetChatInfoWordCounter(id, from, until);
        }

        [HttpGet("chat-info/user-counter/{id}")]
        public async Task<List<UsersTime>> GetChatInfoUserCounter(int id, [FromQuery] string from, [FromQuery] string until)
        {
            return await _repo.GetChatInfoUserCounter(id, from, until);
        }

        [HttpGet("chat-users/{id}")]
        public async Task<List<ChatPersonInfoDTO>> GetChatParticipantsInfo(int id, [FromQuery] string from, [FromQuery] string until)
        {
            return await _repo.GetChatParticipantsInfo(id, from, until);
        }
    }
}