﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Chat> Chats { get; set; }

        public DbSet<ChatUser> ChatUsers { get; set; }

        public DbSet<ChatOwner> ChatOwners { get; set; }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            modelBuilder.Entity<ChatUser>().HasKey(ur => new { ur.UserId, ur.ChatId });
            modelBuilder.Entity<ChatOwner>().HasKey(ur => new { ur.OwnerId, ur.ChatId });

            base.OnModelCreating(modelBuilder);
        }
    }
}
