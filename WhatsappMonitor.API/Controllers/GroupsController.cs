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
        public ActionResult<List<Group>> GetAllGroups()
        {

        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Group> GetGroupById(int id)
        {
            var group = groups.FirstOrDefault(u => u.Id == id);
            if (group == null) return NotFound();
            return group;
        }

        [HttpPost]
        public void AddGroup([FromBody] Group group)
        {
            var newGroup = new Group
            {
                Name = group.Name,
                Id = newId(),
                CreationDate = DateTime.Now
            };
            groups.Add(newGroup);
        }

        [HttpPut("{id}")]
        public void EditGroup(int id, [FromBody] Group user)
        {
            int i = groups.FindIndex(p => p.Id == id);
            if (i != -1) groups[i] = user;
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            int i = groups.FindIndex(p => p.Id == id);
            if (i != -1) groups.RemoveAt(i);
        }
    }
}