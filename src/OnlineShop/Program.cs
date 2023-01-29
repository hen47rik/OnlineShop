using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using OnlineShop.Configuration;
using OnlineShop.Data;
using OnlineShop.Exceptions;
using OnlineShop.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation(options => options.FileProviders.Add(
        new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory)));

builder.Services.Configure<DatabaseConfiguration>(builder.Configuration.GetSection("Database"));
builder.Services.Configure<StripePaymentConfiguration>(builder.Configuration.GetSection("StripePayment"));

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<PaymentService>();

builder.Services.AddScoped<IDbContext>(provider =>
{
    var fileStorage = provider.GetRequiredService<DatabaseService>();

    var dbToUse = fileStorage.GetActiveDb();

    return dbToUse.CreateDbConnectionFactory(provider);
});

builder.Services.AddSingleton<DatabaseService>();

builder.Services.AddDbContext<MySqlContext>((provider, options) =>
{
    var dbService = provider.GetRequiredService<IOptions<DatabaseConfiguration>>();
    var db = dbService.Value.Databases.FirstOrDefault(x => x.Name == "MariaDb");
    if (db is null)
        throw new Exception("No mariadb configured");
    options.UseMySql(db.ConnectionString, new MariaDbServerVersion("10.10.2"));
});

builder.Services.AddDbContext<SqliteContext>((provider, options) =>
{
    var dbService = provider.GetRequiredService<IOptions<DatabaseConfiguration>>();
    var db = dbService.Value.Databases.FirstOrDefault(x => x.Name == "Sqlite");
    if (db is null)
        throw new Exception("No sqlite configured");
    options.UseSqlite(db.ConnectionString);
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();
builder.Services.AddAuthorization();

var app = builder.Build();

PaymentService.ConfigureStripe(app.Services.GetRequiredService<IOptions<StripePaymentConfiguration>>().Value);

app.UseExceptionHandler(appError =>
{
   appError.Run(async (context ) =>
   {
       var exceptionHandlerPathFeature =
           context.Features.Get<IExceptionHandlerPathFeature>();

       if (exceptionHandlerPathFeature?.Error is not BadRequestException exception)
           return;

       context.Response.StatusCode = StatusCodes.Status400BadRequest;

       await context.Response.WriteAsJsonAsync(exception.Message);
   }); 
});

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

app.Run();