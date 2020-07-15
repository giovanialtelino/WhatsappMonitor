using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhatsappMonitor.Shared.Models;
using WhatsappMonitor.API.Repository;
using WhatsappMonitor.API.Context;

namespace WhatsappMonitor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private UsersRepository _repo;
        public UsersController()
        {
            this._repo = new UsersRepository(new MyDbContext());
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
    }
}