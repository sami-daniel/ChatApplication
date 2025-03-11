using ChatApplication.Web.Data;
using ChatApplication.Web.Hubs;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ChatApplication.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var filename = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "chat.db");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite($"Filename={filename}"));

            builder.Services.AddControllersWithViews();

            builder.Services.AddSignalR();

            var app = builder.Build();

            app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
            
            app.UseRouting();
            
            app.UseStaticFiles();

            app.MapHub<Chat>("/hubs/chat");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
