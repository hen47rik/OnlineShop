using Microsoft.Extensions.Options;
using OnlineShop.Configuration;
using OnlineShop.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.Configure<DatabaseConfiguration>(builder.Configuration.GetSection("Database"));

builder.Services.AddSingleton<IDbConnectionFactory>(a => 
    new MariaDbConnectionFactory(
        a.GetRequiredService<IOptions<DatabaseConfiguration>>()
        .Value.MySqlConnectionString));
builder.Services.AddSingleton<DatabaseInitializer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

var databaseInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
await databaseInitializer.InitializeAsync();

app.Run();