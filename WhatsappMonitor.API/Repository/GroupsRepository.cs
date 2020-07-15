using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhatsappMonitor.API.Context;
using WhatsappMonitor.Shared.Models;
using System;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.IO;

namespace WhatsappMonitor.API.Repository
{
    public class GroupsRepository
    {
        private readonly MyDbContext _context;
        public Groups(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = await _context.Users.OrderBy(c => c.Name).ToListAsync();
            return users;
        }

    }

}