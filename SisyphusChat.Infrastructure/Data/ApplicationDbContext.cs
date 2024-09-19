using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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

        public DbSet<Attachment> Attachments { get; set; }

        public DbSet<Chat> Chats { get; set; }

        public DbSet<ChatUser> ChatUsers { get; set; }
        
        public DbSet<Friend> Friends { get; set; }
        
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            modelBuilder.Entity<ChatUser>().HasKey(ur => new { ur.UserId, ur.ChatId });
            modelBuilder.Entity<Friend>().HasKey(fr => new { fr.ReqSenderID, fr.ReqReceiverID });

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.ReqSender)
                .WithMany()
                .HasForeignKey(f => f.ReqSenderID)
                .OnDelete(DeleteBehavior.Restrict);  // Disable cascade delete

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.ReqReceiver)
                .WithMany()
                .HasForeignKey(f => f.ReqReceiverID)
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete for receiver

            modelBuilder.Entity<ChatUser>()
                .HasOne(cu => cu.Chat)
                .WithMany(c => c.ChatUsers)
                .HasForeignKey(cu => cu.ChatId)
                .OnDelete(DeleteBehavior.Restrict); // Disable cascade delete

            modelBuilder.Entity<ChatUser>()
                .HasOne(cu => cu.User)
                .WithMany(u => u.ChatUsers)
                .HasForeignKey(cu => cu.UserId)
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


            base.OnModelCreating(modelBuilder);
        }
    }
}
