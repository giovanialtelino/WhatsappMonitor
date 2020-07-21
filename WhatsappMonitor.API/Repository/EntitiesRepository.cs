using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhatsappMonitor.API.Context;
using WhatsappMonitor.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace WhatsappMonitor.API.Repository
{
    public class EntitiesRepository
    {
        private readonly MyDbContext _context;
        public EntitiesRepository(MyDbContext context)
        {
            _context = context;
        }

        private async Task<bool> EntitynameAlreadyExist(string name)
        {
            var entity = await _context.Entities.FirstOrDefaultAsync(c => c.Name == name);
            if (entity == null)
            {
                return false;
            }
            return true;
        }

        public async Task<List<Entity>> GetAllEntitiesAsync()
        {
            var entities = await _context.Entities.OrderBy(c => c.Name).ToListAsync();
            return entities;
        }

        public async Task<Entity> GetEntityById(int id)
        {
            var entities = await _context.Entities.FirstOrDefaultAsync(c => c.EntityId == id);
            return entities;
        }

        public async Task UpdateEntity(int id, Entity entity)
        {
            var entityExist = await EntitynameAlreadyExist(entity.Name);

            if (entityExist == false)
            {
                var update = await _context.Entities.FirstOrDefaultAsync(c => c.EntityId == id);
                _context.Entry(update).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddEntity(Entity entity)
        {
            var entityExist = await EntitynameAlreadyExist(entity.Name);

            if (entityExist == false)
            {
                var newEntity = new Entity(entity.Name);               
                var add = _context.Entities.Add(newEntity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteEntity(int id)
        {
            var delete = await _context.Entities.FirstOrDefaultAsync(c => c.EntityId == id);
            _context.Entities.Remove(delete);
            await _context.SaveChangesAsync();
        }
    }
}