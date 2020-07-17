using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhatsappMonitor.Shared.Models;
using WhatsappMonitor.API.Repository;
using WhatsappMonitor.API.Context;
using System.IO;

namespace WhatsappMonitor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private UsersRepository _repo;
        private ChatsRepository _chat;
        public UsersController()
        {
            this._repo = new UsersRepository(new MyDbContext());
            this._chat = new ChatsRepository(new MyDbContext());
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            return await _repo.GetAllUsersAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            return await _repo.GetUserById(id);
        }

        [HttpPost]
        public async Task AddUser([FromBody] User user)
        {
            await _repo.AddUser(user);
        }

        [HttpPut("{id}")]
        public async Task EditUser(int id, [FromBody] User user)
        {
            await _repo.UpdateUser(id, user);
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await _repo.DeleteUser(id);
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
                                counter = counter + (await _chat.CleanAddChatUser(line, id));
                            }
                        }
                    }
                }
            }

            return counter;
        }

    }
}