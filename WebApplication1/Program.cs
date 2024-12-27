using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDistributedMemoryCache(); // 使用內存緩存來存儲會話數據
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 設置會話超時時間
    options.Cookie.HttpOnly = true; // 設置HttpOnly屬性
    options.Cookie.IsEssential = true; // 設置會話Cookie為必需
});

builder.Services.AddControllersWithViews();

//取得組態中資料庫連綫設定
string connectionString = builder.Configuration.GetConnectionString("CmsContext");

//注冊EF Core的CmsContext
builder.Services.AddDbContext<CmsContext>(options => options.UseSqlServer(connectionString));

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

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession(); // Use session before MVC

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
