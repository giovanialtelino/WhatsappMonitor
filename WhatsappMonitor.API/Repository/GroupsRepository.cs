using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhatsappMonitor.API.Context;
using WhatsappMonitor.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace WhatsappMonitor.API.Repository
{
    public class GroupsRepository
    {
        private readonly MyDbContext _context;
        public GroupsRepository(MyDbContext context)
        {
            _context = context;
        }

        private async Task<int> NextGroupId()
        {
            return await _context.Groups.CountAsync();
        }

        private async Task<bool> GroupnameAlreadyExist(string name)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(c => c.Name == name);
            if (group == null)
            {
                return false;
            }
            return true;
        }

        public async Task<List<Group>> GetAllGroupsAsync()
        {
            var groups = await _context.Groups.OrderBy(c => c.Name).ToListAsync();
            return groups;
        }

        public async Task<Group> GetGroupById(int id)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(c => c.Id == id);
            return group;
        }

        public async Task UpdateGroup(int id, Group group)
        {
            var groupExist = await GroupnameAlreadyExist(group.Name);

            if (groupExist == false)
            {
                var update = await _context.Groups.FirstOrDefaultAsync(c => c.Id == id);
                _context.Entry(update).CurrentValues.SetValues(group);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddGroup(Group group)
        {
            var groupExist = await GroupnameAlreadyExist(group.Name);

            if (groupExist == false)
            {
                var id = await NextGroupId();
                var newGroup = new Group
                {
                    Name = group.Name,
                    CreationDate = DateTime.Now,
                    Id = (id + 1)
                };

                var add = _context.Groups.Add(newGroup);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteGroup(int id)
        {
            var delete = await _context.Groups.FirstOrDefaultAsync(c => c.Id == id);
            _context.Groups.Remove(delete);
            await _context.SaveChangesAsync();
        }
    }
}