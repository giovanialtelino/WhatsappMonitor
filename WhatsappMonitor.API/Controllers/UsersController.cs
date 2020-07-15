using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhatsappMonitor.Shared.Models;

namespace WhatsappMonitor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private static readonly List<User> users = RandomUsers(5);

        private static List<User> RandomUsers(int number)
        {
            return Enumerable.Range(1, number).Select(i => new User
            {
                Id = i,
                Name = $"Name {i}",
                CreationDate = DateTime.Now
            }).ToList();
        }

        private int newId()
        {
            return users.Count + 1;
        }

        [HttpGet]
        public ActionResult<List<User>> GetAllUsers()
        {
            return users;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<User> GetUserById(int id)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();
            return user;
        }

        [HttpPost]
        public void AddUser([FromBody] User user)
        {
            var newUser = new User
            {
                Name = user.Name,
                Id = newId(),
                CreationDate = DateTime.Now
            };
            users.Add(newUser);
        }

        [HttpPut("{id}")]
        public void EditUser(int id, [FromBody] User user)
        {
            int i = users.FindIndex(p => p.Id == id);
            if (i != -1) users[i] = user;
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            int i = users.FindIndex(p => p.Id == id);
            if (i != -1) users.RemoveAt(i);
        }


    }
}