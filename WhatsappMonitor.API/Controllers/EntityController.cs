using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using WhatsappMonitor.Shared.Models;
using WhatsappMonitor.API.Context;
using Microsoft.EntityFrameworkCore;
using WhatsappMonitor.API.Repository;
using System.IO;

namespace WhatsappMonitor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntitiesController : Controller
    {
        private EntitiesRepository _repo;
        private ChatsRepository _chat;
        public EntitiesController()
        {
            this._repo = new EntitiesRepository(new MyDbContext());
            this._chat = new ChatsRepository(new MyDbContext());
        }

        [HttpGet]
        public async Task<ActionResult<List<Entity>>> GetAllEntites()
        {
            return await _repo.GetAllEntitiesAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Entity>> GetEntityById(int id)
        {
            return await _repo.GetEntityById(id);
        }

        [HttpPost]
        public async Task AddEntity([FromBody] Entity entity)
        {
            await _repo.AddEntity(entity);
        }

        [HttpPut("{id}")]
        public async Task EditEntity(int id, [FromBody] Entity entity)
        {
            await _repo.UpdateEntity(id, entity);
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await _repo.DeleteEntity(id);
        }

        [HttpPost("file/{id}")]
        public async Task<int> OnPostUploadAsync(int id)
        {
            var systemTime = DateTime.Now;
            var counter = 0;
            if (HttpContext.Request.Form.Files.Any())
            {
                foreach (var file in HttpContext.Request.Form.Files)
                {
                    var path = Path.Combine(Path.GetTempFileName());
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        String line;

                        using (var sr = new StreamReader(path))
                        {
                            while ((line = sr.ReadLine()) != null)
                            {
                                counter = counter + (await _chat.CleanAddChat(line, id, systemTime));
                            }
                        }
                    }
                }
            }
            return counter;
        }
    }
}