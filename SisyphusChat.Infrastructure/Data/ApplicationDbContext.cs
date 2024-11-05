using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SisyphusChat.Infrastructure.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

        public DbSet<User> Users { get; set; } 
        
        public DbSet<Notification> Notifications { get; set; }


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



            base.OnModelCreating(modelBuilder);
        }
    }
    public static class DbInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
            
            // Ensure migrations are applied
            await context.Database.MigrateAsync();
            
            // Check if there are any users
            if (await userManager.Users.AnyAsync())
            {
                logger.LogInformation("Database already seeded");
                return; // DB has been seeded
            }

            logger.LogInformation("Starting database seeding...");

            // Create admin users
            var adminUsers = new[]
            {
                new User
                {
                    UserName = "admin1@example.com",
                    Email = "admin1@example.com",
                    EmailConfirmed = true,
                    IsAdmin = true,
                    TimeCreated = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                },
                new User
                {
                    UserName = "admin2@example.com",
                    Email = "admin2@example.com",
                    EmailConfirmed = true,
                    IsAdmin = true,
                    TimeCreated = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                }
            };

            // Create regular users
            var regularUsers = new[]
            {
                new User
                {
                    UserName = "user1@example.com",
                    Email = "user1@example.com",
                    EmailConfirmed = true,
                    TimeCreated = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                },
                new User
                {
                    UserName = "user2@example.com",
                    Email = "user2@example.com",
                    EmailConfirmed = true,
                    TimeCreated = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                },
                new User
                {
                    UserName = "user3@example.com",
                    Email = "user3@example.com",
                    EmailConfirmed = true,
                    TimeCreated = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                }
            };

            const string defaultPassword = "Test123!";

            foreach (var user in adminUsers.Concat(regularUsers))
            {
                var result = await userManager.CreateAsync(user, defaultPassword);
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create user {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            // Create system deleted user if it doesn't exist
            var deletedUser = await userManager.FindByNameAsync("DELETED_USER");
            if (deletedUser == null)
            {
                deletedUser = new User
                {
                    UserName = "DELETED_USER",
                    NormalizedUserName = "DELETED_USER",
                    Email = "deleted@system.local",
                    NormalizedEmail = "DELETED@SYSTEM.LOCAL",
                    EmailConfirmed = true,
                    IsDeleted = true
                };
                await userManager.CreateAsync(deletedUser);
            }

            logger.LogInformation("Seeding database completed");
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while seeding the database.", ex);
            }
        }
    }
}
