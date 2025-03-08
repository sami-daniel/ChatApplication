using ChatApplication.Producer.Data;
using ChatApplication.Producer.Helpers;
using ChatApplication.Producer.SignalR;
using ChatApplication.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ChatApplication.Producer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            var nullEnvException = (string nullEnvVarName) => {
                return new ArgumentNullException(nullEnvVarName, $"Environment variable {nullEnvVarName} is not set!");
            };

            var mySqlPort = Environment.GetEnvironmentVariable("MYSQL_PORT") ?? throw nullEnvException("MYSQL_PORT");
            var mySqlHost = Environment.GetEnvironmentVariable("MYSQL_HOST") ?? throw nullEnvException("MYSQL_HOST");
            var databaseName = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? throw nullEnvException("DATABASE_NAME");
            var mySqlRootPassword = Environment.GetEnvironmentVariable("MYSQL_ROOT_PASSWORD") ?? throw nullEnvException("MYSQL_ROOT_PASSWORD");

            var connectionString = $"server={mySqlHost};port={mySqlPort};database={databaseName};user=root;password={mySqlRootPassword};";

            builder.Services.AddCors(policy =>
            {
                var frontendHost = Environment.GetEnvironmentVariable("FRONTEND_HOST") ?? throw nullEnvException("FRONTEND_HOST");

                policy.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(frontendHost)
                           .WithMethods("GET", "POST")
                           .AllowAnyHeader();
                });
            });

            builder.Services.AddDbContext<ApplicationDbContext>(opt =>
            {
#if DEBUG
                Console.WriteLine("Sensitive database commands activated!");
                opt.EnableSensitiveDataLogging();
#else
                Console.WriteLine("Sensitive database commands deactivated!");
#endif
                opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Chat API", Version = "v1" });
            });

            builder.Services.AddSignalR();

            builder.Services.AddSingleton<MessageSender>();

            var app = builder.Build();
            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chat API V1");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseCors();

            app.UseRouting();

            app.UseStaticFiles();

            app.MapHub<ChatHub>("/chat");

            Console.WriteLine("ChatApplication is ready for send messages!");

            app.Run();
        }
    }
}