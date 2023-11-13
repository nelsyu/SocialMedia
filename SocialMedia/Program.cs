using AutoMapper;
using Data.Entities;
using Library.Config;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Service.Extensions;
using Service.Mapper;
using Service.Services.Implements;
using Service.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<SocialMediaContext>(
    options => options.UseSqlServer(DbConfig.GetConnectionString()));

// 配置 AutoMapper 並將它集成到 ASP.NET Core 的依賴注入中。
AutoMapper.IConfigurationProvider config = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, Mapper>();

// 將一個服務的介面和實現進行關聯，並將其添加到 ASP.NET Core 的 DI 容器中，使得在應用程序中能夠方便地使用依賴注入。
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReplyService, ReplyService>();
builder.Services.AddScoped<ITopicService, TopicService>();

// 添加 Session 服務
builder.Services.AddDistributedMemoryCache(); // 使用內存作為 Session 存儲
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 設置 Session 逾時時間
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 向服務容器（IServiceCollection）中添加 IHttpContextAccessor 服務
builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<UserLoggedIn>();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.HttpOnly = HttpOnlyPolicy.None;
    options.Secure = CookieSecurePolicy.Always; // 確保設置 Secure 標誌
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// 配置應用程式使用 Session 服務的方法
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
