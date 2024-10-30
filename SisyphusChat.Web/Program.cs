using AutoMapper;
using SisyphusChat.Core;
using SisyphusChat.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Infrastructure.Data;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Infrastructure.Interfaces;
using SisyphusChat.Infrastructure.Repositories;
using SisyphusChat.Web.Hubs;
using Azure.Communication.Email;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");
builder.Services.AddScoped<SignInManager<User>>();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString).EnableSensitiveDataLogging());

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddSingleton(new EmailClient(builder.Configuration["AzureCommunicationServices:ConnectionString"]));
builder.Services.AddTransient<IEmailService, EmailService>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); 
    options.SlidingExpiration = true; 
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout"; 
});

builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IFriendRepository, FriendRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IFriendRepository, FriendRepository>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IFriendService, FriendService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReportService, ReportService>();

// Map the AutoMapper profile
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new AutoMapperProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Add security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
   await next();
});

// Block access to hidden files
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;
    if (path.StartsWith("/.") || path.StartsWith("/.BitKeeper"))
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("Not Found");
    }
    else
    {
        await next();
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.MapRazorPages();
app.MapHub<ChatHub>("/chatHub");
app.UseAuthentication();
app.UseAuthorization();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // General error handler for production
    app.UseStatusCodePagesWithReExecute("/Home/Error/{0}"); // Custom error handler
}
app.UseStatusCodePagesWithRedirects("/Home/Error?statusCode={0}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();