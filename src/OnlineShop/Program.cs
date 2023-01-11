using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OnlineShop.Configuration;
using OnlineShop.Data;
using OnlineShop.Services;
using static System.Text.Encoding;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation(options => options.FileProviders.Add(
        new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory)));

builder.Services.Configure<DatabaseConfiguration>(builder.Configuration.GetSection("Database"));

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProductService>();

builder.Services.AddScoped<IDbConnectionFactory>(provider =>
{
    var fileStorage = provider.GetRequiredService<DatabaseService>();

    var dbToUse = fileStorage.GetActiveDb();

    return dbToUse.CreateDbConnectionFactory();
});

builder.Services.AddSingleton<DatabaseInitializer>();

DatabaseService? databaseService = null;
builder.Services.AddSingleton<DatabaseService>(provider =>
{
    var dbConfiguration = provider.GetRequiredService<IOptions<DatabaseConfiguration>>();
    databaseService = new DatabaseService(dbConfiguration);
    return databaseService;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKeyResolver = (_, _, _, _) =>
            {
                return new[]
                {
                    new SymmetricSecurityKey(UTF8.GetBytes(databaseService?.GetActiveDb().Secret!))
                };
            }
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["dasToken"];
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var databaseInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
await databaseInitializer.InitializeAsync();

app.Run();