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
    public class GroupsController : Controller
    {
        private GroupsRepository _repo;
        public GroupsController()
        {
            this._repo = new GroupsRepository(new MyDbContext());
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
    }
}