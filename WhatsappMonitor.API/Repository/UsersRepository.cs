using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhatsappMonitor.API.Context;
using WhatsappMonitor.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace WhatsappMonitor.API.Repository
{
    public class UsersRepository
    {
        private readonly MyDbContext _context;
        public UsersRepository(MyDbContext context)
        {
            _context = context;
        }
        private async Task<bool> UsernameAlreadyExist(string name)
        {
            var user = await _context.Users.FirstOrDefaultAsync(c => c.Name == name);
            if (user == null)
            {
                return false;
            }
            return true;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = await _context.Users.OrderBy(c => c.Name).ToListAsync();
            return users;
        }

        public async Task<User> GetUserById(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == id);
            return user;
        }

        public async Task UpdateUser(int id, User user)
        {
            var userExist = await UsernameAlreadyExist(user.Name);

            if (userExist == false)
            {
                var update = await _context.Users.FirstOrDefaultAsync(c => c.Id == id);
                _context.Entry(update).CurrentValues.SetValues(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddUser(User user)
        {
            var userExist = await UsernameAlreadyExist(user.Name);

            if (userExist == false)
            {
                var newUser = new User
                {
                    Name = user.Name,
                    CreationDate = DateTime.Now
                };

                var add = _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteUser(int id)
        {
            var delete = await _context.Users.FirstOrDefaultAsync(c => c.Id == id);
            _context.Users.Remove(delete);
            await _context.SaveChangesAsync();
        }
    }
}