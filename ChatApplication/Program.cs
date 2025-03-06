using ChatApplication.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(opt =>
            {
#if DEBUG
                opt.EnableSensitiveDataLogging();
#endif

                opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}
