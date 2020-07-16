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
        public async Task<ActionResult<List<Chat>>> GetAllChatsGroup(int id)
        {
            return await _repo.GetAllChatsGroup(id);
        }



    }
}