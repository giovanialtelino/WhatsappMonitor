using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WhatsappMonitor.Shared.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace WhatsappMonitor.API.Context
{
    public class MyDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbString = Environment.GetEnvironmentVariable("ConnectionString");

            optionsBuilder.UseNpgsql(dbString);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<Folder>().ToTable("Folder");
            modelbuilder.Entity<Folder>(entity =>
            {
                entity.HasKey(e => e.FolderId);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasMany(e => e.FolderMessages);
                entity.Property(e => e.FolderId).ValueGeneratedOnAdd();
                entity.HasMany(e => e.Uploads);
            });

            modelbuilder.Entity<ChatMessage>().ToTable("Chat");
            modelbuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(e => e.ChatMessageId);
                entity.Property(e => e.ChatMessageId).ValueGeneratedOnAdd();
            });

            modelbuilder.Entity<Upload>().ToTable("Upload");
            modelbuilder.Entity<Upload>(entity =>
            {
                entity.HasKey(e => e.UploadId);
                entity.HasIndex(e => e.FileName);
                entity.Property(e => e.UploadId).ValueGeneratedOnAdd();
            });
        }

        public DbSet<Folder> Entities { get; set; }
        public DbSet<ChatMessage> Chats { get; set; }
        public DbSet<Upload> Uploads { get; set; }
    }
}