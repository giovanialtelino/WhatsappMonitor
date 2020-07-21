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
            optionsBuilder.UseNpgsql("Server=127.0.0.1;Port=5433;Database=postgres;User Id=postgres;Password=1234;");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<Entity>().ToTable("Group", "postgres");
            modelbuilder.Entity<Entity>(entity =>
            {
                entity.HasKey(e => e.EntityId);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasMany(e => e.Chats);
                entity.Property(e => e.EntityId).ValueGeneratedOnAdd();
                entity.HasMany(e => e.Chats);
            });

            modelbuilder.Entity<Chat>().ToTable("Chat", "postgres");
            modelbuilder.Entity<Chat>(entity =>
            {
                entity.HasKey(e => e.ChatId);
                entity.Property(e => e.ChatId).ValueGeneratedOnAdd();
            });
        }

        public DbSet<Entity> Entities { get; set; }
        public DbSet<Chat> Chats { get; set; }
    }
}