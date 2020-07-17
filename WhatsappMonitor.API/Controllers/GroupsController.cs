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
    public class GroupsController : Controller
    {
        private GroupsRepository _repo;
        private ChatsRepository _chat;
        public GroupsController()
        {
            this._repo = new GroupsRepository(new MyDbContext());
            this._chat = new ChatsRepository(new MyDbContext());
        }

        [HttpGet]
        public async Task<ActionResult<List<Group>>> GetAllGroups()
        {
            return await _repo.GetAllGroupsAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Group>> GetGroupById(int id)
        {
            return await _repo.GetGroupById(id);
        }

        [HttpPost]
        public async Task AddGroup([FromBody] Group group)
        {
            await _repo.AddGroup(group);
        }

        [HttpPut("{id}")]
        public async Task EditGroup(int id, [FromBody] Group group)
        {
            await _repo.UpdateGroup(id, group);
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await _repo.DeleteGroup(id);
        }

        [HttpPost("file/{id}")]
        public async Task<int> OnPostUploadAsync(int id)
        {
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
                                counter = counter + (await _chat.CleanAddChatGroup(line, id));
                            }
                        }
                    }
                }
            }

            return counter;
        }
    }
}