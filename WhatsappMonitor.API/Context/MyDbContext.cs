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
            modelbuilder.Entity<Group>().ToTable("Group", "postgres");
            modelbuilder.Entity<Group>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasMany(e => e.Users);
            });

            modelbuilder.Entity<User>().ToTable("User", "postgres");
            modelbuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasMany(e => e.Groups);
            });
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
    }
}