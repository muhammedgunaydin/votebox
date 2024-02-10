using Microsoft.EntityFrameworkCore;
using Npgsql;

string connString = "Host=localhost;Username=postgres;Password=test;Database=votebox";

using (var conn = new NpgsqlConnection(connString))
{
    try
    {
        conn.Open();
        Console.WriteLine("Succesfully connected to Psql");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connString));

builder.Services.AddSession();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}");

app.Run();
