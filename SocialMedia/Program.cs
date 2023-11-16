using AutoMapper;
using Data.Entities;
using Library.Config;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Service.Extensions;
using Service.Mapper;
using Service.Services.Implements;
using Service.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<SocialMediaContext>(
    options => options.UseSqlServer(DbConfig.GetConnectionString()));

AutoMapper.IConfigurationProvider config = new MapperConfiguration(cfg =>
{
    //Service\Mapper\MappingProfile.cs
    cfg.AddProfile<MappingProfile>();
});
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, Mapper>();

// �N�@�ӪA�Ȫ������M��{�i�����p�A�ñN��K�[�� ASP.NET Core �� DI �e�����A�ϱo�b���ε{�Ǥ������K�a�ϥΨ̿�`�J�C
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReplyService, ReplyService>();
builder.Services.AddScoped<ITopicService, TopicService>();

// �K�[ Session �A��
builder.Services.AddDistributedMemoryCache(); // �ϥΤ��s�@�� Session �s�x
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // �]�m Session �O�ɮɶ�
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// �V�A�Ȯe���]IServiceCollection�^���K�[ IHttpContextAccessor �A��
builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<UserLoggedIn>();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.Always; // �o���O�]�mSecure�лx���a��
});

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs\\log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

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

// �t�m���ε{���ϥ� Session �A�Ȫ���k
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseCookiePolicy();

app.Run();
