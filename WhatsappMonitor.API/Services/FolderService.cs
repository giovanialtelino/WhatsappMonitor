using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhatsappMonitor.API.Context;
using WhatsappMonitor.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace WhatsappMonitor.API.Services
{
    public class FoldersService
    {
        private readonly MyDbContext _context;
        public FoldersService(MyDbContext context)
        {
            _context = context;
        }

        private async Task<bool> EntitynameAlreadyExist(string name)
        {
            var Folder = await _context.Entities.FirstOrDefaultAsync(c => c.Name == name);
            if (Folder == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<List<Folder>> GetAllEntitiesAsync()
        {
            var entities = await _context.Entities.OrderBy(c => c.Name).ToListAsync();
            return entities;
        }

        public async Task<Folder> GetEntityById(int id)
        {
            var entities = await _context.Entities.FirstOrDefaultAsync(c => c.FolderId == id);
            return entities;
        }

        public async Task UpdateEntity(int id, Folder Folder)
        {
            var entityExist = await EntitynameAlreadyExist(Folder.Name);

            if (entityExist == false)
            {
                var update = await _context.Entities.FirstOrDefaultAsync(c => c.FolderId == id);
                _context.Entry(update).CurrentValues.SetValues(Folder);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddEntity(Folder Folder)
        {
            var entityExist = await EntitynameAlreadyExist(Folder.Name);

            if (entityExist == false)
            {
                var newEntity = new Folder(Folder.Name);
                var add = _context.Entities.Add(newEntity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteEntity(int id)
        {
            var delete = await _context.Entities.FirstOrDefaultAsync(c => c.FolderId == id);
            _context.Entities.Remove(delete);
            await _context.SaveChangesAsync();
        }

        public async Task UploadFile(int FolderId, DateTime uploadTime, string fileName, byte[] fileContent)
        {
            var newUpload = new Upload
            {
                FileName = fileName,
                CreationDate = uploadTime,
                FileContent = fileContent,
                FolderId = FolderId
            };

            _context.Uploads.Add(newUpload);
            await _context.SaveChangesAsync();
        }
    }
}