using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Attachment> Attachments { get; set; }

        public DbSet<Chat> Chats { get; set; }

        public DbSet<ChatUser> ChatUsers { get; set; }
        
        public DbSet<Friend> Friends { get; set; }
        
        public DbSet<Message> Messages { get; set; }

        public DbSet<Report> Reports { get; set; }

        public DbSet<User> Users { get; set; }  

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            modelBuilder.Entity<ChatUser>().HasKey(ur => new { ur.UserId, ur.ChatId });
            modelBuilder.Entity<Friend>().HasKey(fr => new { fr.ReqSenderId, fr.ReqReceiverId });

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.ReqSender)
                .WithMany()
                .HasForeignKey(f => f.ReqSenderId)
                .OnDelete(DeleteBehavior.Restrict);  // Disable cascade delete

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.ReqReceiver)
                .WithMany()
                .HasForeignKey(f => f.ReqReceiverId)
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete for receiver

            modelBuilder.Entity<ChatUser>()
                .HasOne(cu => cu.Chat)
                .WithMany(c => c.ChatUsers)
                .HasForeignKey(cu => cu.ChatId)
                .OnDelete(DeleteBehavior.Restrict); // Disable cascade delete

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages) 
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Restrict); // Disable cascade delete

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.Messages) 
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict); // Disable cascade delete

            modelBuilder.Entity<Report>()
                .HasOne(r => r.Chat)
                .WithMany()
                .HasForeignKey(r => r.ChatId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Report>()
                .HasOne(r => r.Message)
                .WithMany()
                .HasForeignKey(r => r.MessageId)
                .OnDelete(DeleteBehavior.Restrict);


            base.OnModelCreating(modelBuilder);
        }
    }
}
