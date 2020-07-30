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
            optionsBuilder.UseNpgsql("Server=127.0.0.1;Port=5432;Database=wmonitor;User Id=whatsapp;Password=whatsappmonitor;");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<Entity>().ToTable("Group");
            modelbuilder.Entity<Entity>(entity =>
            {
                entity.HasKey(e => e.EntityId);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasMany(e => e.Chats);
                entity.Property(e => e.EntityId).ValueGeneratedOnAdd();
                entity.HasMany(e => e.Chats);
                entity.HasMany(e => e.Uploads);
            });

            modelbuilder.Entity<Chat>().ToTable("Chat");
            modelbuilder.Entity<Chat>(entity =>
            {
                entity.HasKey(e => e.ChatId);
                entity.Property(e => e.ChatId).ValueGeneratedOnAdd();
            });

            modelbuilder.Entity<Upload>().ToTable("Upload");
            modelbuilder.Entity<Upload>(entity =>
            {
                entity.HasKey(e => e.UploadId);
                entity.HasIndex(e => e.FileName);
                entity.Property(e => e.UploadId).ValueGeneratedOnAdd();
            });
        }

        public DbSet<Entity> Entities { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Upload> Uploads { get; set; }
        public DbSet<User> Users {get;set;}
    }
}