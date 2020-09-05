using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WhatsappMonitor.Shared.Models;
using WhatsappMonitor.API.Context;
using WhatsappMonitor.API.Services;

namespace WhatsappMonitor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : Controller
    {
        private readonly IChatsMessageService _repo;
        public ChatsController(MyDbContext chat)
        {
            this._repo = new ChatsMessageService(chat);
        }

        [HttpGet("load/{id}/{skip}/{take}")]
        public async Task<ActionResult<Tuple<PaginationDTO, List<ChatMessage>>>> GetChatsPagination(int id, int skip, int take)
        {
            return await _repo.GetAllChatsPagination(id, skip, take);
        }

        [HttpGet("load-after/{id}")]
        public async Task<ActionResult<List<ChatMessage>>> GetChatsAfter(int id, [FromQuery] string date)
        {
            return await _repo.GetChatsAfter(id, date);
        }

        [HttpGet("load-before/{id}")]
        public async Task<ActionResult<List<ChatMessage>>> GetChatsBefore(int id, [FromQuery] string date)
        {
            return await _repo.GetChatsBefore(id, date);
        }   

         [HttpGet("first-message/{id}")]
        public async Task<ActionResult<List<ChatMessage>>> GetFirstMessage(int id)
        {
            return await _repo.GetFirstMessage(id);
        }

        [HttpGet("last-message/{id}")]
        public async Task<ActionResult<List<ChatMessage>>> GetLastMessage(int id)
        {
            return await _repo.GetLastMessage(id);
        }       

        [HttpGet("search/{id}/{message}")]
        public async Task<ActionResult<List<ChatMessage>>> SearchChat(int id, string message)
        {
            return await _repo.SearchEntityChatText(message, id);
        }

        [HttpGet("jump-date/{id}")]
        public async Task<ActionResult<List<ChatMessage>>> SearchChatDate(int id, [FromQuery] string startDate)
        {
            return await _repo.SearchEntityChatTextByDate(id, startDate);
        }

        [HttpGet("uploads/{id}")]
        public async Task<ActionResult<List<ChatUploadDTO>>> GetChatUploadDates(int id)
        {
            return await _repo.GetChatUploadDate(id);
        }

        [HttpPut("update-participants/{id}")]
        public async Task<ActionResult<List<ParticipantDTO>>> UpdateParticipantsChat(int id, [FromBody] List<ParticipantDTO> participant)
        {
            return await _repo.UpdateParticipantsChat(id, participant);
        }

        [HttpPut("delete-date/{id}")]
        public async Task DeleteDateChat(int id, [FromBody] ChatUploadDTO dto)
        {
            await _repo.DeleteDateChat(id, dto);
        }

        [HttpGet("chat-participants/{id}")]
        public async Task<List<ParticipantDTO>> GetChatParticipantsInfo(int id)
        {
            return await _repo.GetChatParticipants(id);
        }
    }
}