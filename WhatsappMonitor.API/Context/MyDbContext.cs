using System;
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
            //postgresql://postgres:1234@localhost:5433/postgres
            optionsBuilder.UseNpgsql("Server=127.0.0.1;Port=5432;Database=whatsapp1;User Id=whatsapp;Password=whatsappmonitor;");
            base.OnConfiguring(optionsBuilder);
        }

        //psql "dbname=wmonitor host=127.0.0.1 user=whatsapp password=whatsappmonitor port=5432 sslmode=require"
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