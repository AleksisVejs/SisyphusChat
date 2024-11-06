using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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

        public DbSet<Report> Reports { get; set; }

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
                .OnDelete(DeleteBehavior.NoAction);  // Change from Cascade to NoAction

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.ReqReceiver)
                .WithMany()
                .HasForeignKey(f => f.ReqReceiverId)
                .OnDelete(DeleteBehavior.NoAction);  // Change from Cascade to NoAction

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
            
            
            await context.Database.MigrateAsync();
            
            
            if (await userManager.Users.AnyAsync())
            {
                logger.LogInformation("Database already seeded");
                return;
            }

            logger.LogInformation("Starting database seeding...");

            
            var adminUsers = new[]
            {
                new User
                {
                    UserName = "admin.jones",
                    Email = "admin.jones@example.com",
                    EmailConfirmed = true,
                    IsAdmin = true,
                    TimeCreated = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                },
                new User
                {
                    UserName = "admin.smith",
                    Email = "admin.smith@example.com",
                    EmailConfirmed = true,
                    IsAdmin = true,
                    TimeCreated = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                }
            };

            // Create regular users
            var regularUsers = new User[]
            {
                new User { UserName = "john.doe", Email = "john.doe@example.com" },
                new User { UserName = "sarah.wilson", Email = "sarah.wilson@example.com" },
                new User { UserName = "mike.brown", Email = "mike.brown@example.com" },
                new User { UserName = "emma.davis", Email = "emma.davis@example.com" },
                new User { UserName = "james.miller", Email = "james.miller@example.com" },
                new User { UserName = "olivia.taylor", Email = "olivia.taylor@example.com" },
                new User { UserName = "william.anderson", Email = "william.anderson@example.com" },
                new User { UserName = "sophia.thomas", Email = "sophia.thomas@example.com" },
                new User { UserName = "lucas.jackson", Email = "lucas.jackson@example.com" },
                new User { UserName = "ava.white", Email = "ava.white@example.com" },
                new User { UserName = "henry.martin", Email = "henry.martin@example.com" },
                new User { UserName = "mia.thompson", Email = "mia.thompson@example.com" },
                new User { UserName = "alexander.moore", Email = "alexander.moore@example.com" },
                new User { UserName = "charlotte.lee", Email = "charlotte.lee@example.com" },
                new User { UserName = "daniel.clark", Email = "daniel.clark@example.com" },
                new User { UserName = "amelia.walker", Email = "amelia.walker@example.com" },
                new User { UserName = "joseph.hall", Email = "joseph.hall@example.com" },
                new User { UserName = "victoria.green", Email = "victoria.green@example.com" },
                new User { UserName = "david.baker", Email = "david.baker@example.com" },
                new User { UserName = "grace.adams", Email = "grace.adams@example.com" },
                new User { UserName = "christopher.hill", Email = "christopher.hill@example.com" },
                new User { UserName = "zoe.campbell", Email = "zoe.campbell@example.com" },
                new User { UserName = "andrew.mitchell", Email = "andrew.mitchell@example.com" },
                new User { UserName = "natalie.roberts", Email = "natalie.roberts@example.com" },
                new User { UserName = "ryan.cooper", Email = "ryan.cooper@example.com" },
                new User { UserName = "hannah.morgan", Email = "hannah.morgan@example.com" },
                new User { UserName = "justin.phillips", Email = "justin.phillips@example.com" },
                new User { UserName = "lily.turner", Email = "lily.turner@example.com" },
                new User { UserName = "kevin.parker", Email = "kevin.parker@example.com" },
                new User { UserName = "rachel.evans", Email = "rachel.evans@example.com" }
            }.Select(u => {
                u.EmailConfirmed = true;
                u.TimeCreated = DateTime.UtcNow;
                u.LastUpdated = DateTime.UtcNow;
                return u;
            }).ToArray();

            const string defaultPassword = "Test123!";

            // Create all users
            var allUsers = adminUsers.Concat(regularUsers).ToList();
            foreach (var user in allUsers)
            {
                var result = await userManager.CreateAsync(user, defaultPassword);
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create user {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            var random = new Random();
            foreach (var user in allUsers)
            {
                var otherUsers = allUsers.Where(u => u.Id != user.Id).ToList();
                var friendCount = Math.Min(20, otherUsers.Count);
                var friends = otherUsers.OrderBy(x => random.Next()).Take(friendCount);

                foreach (var friend in friends)
                {
                    var friendship = new Friend
                    {
                        ReqSender = user,
                        ReqReceiver = friend,
                        IsAccepted = true
                    };
                    context.Friends.Add(friendship);
                }
            }
            await context.SaveChangesAsync();

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
