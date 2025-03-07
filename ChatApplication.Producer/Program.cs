using ChatApplication.Producer.Data;
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
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = "";

            try
            {
                connectionString = args[0];
            }
            catch
            {
                throw new ArgumentNullException("Connection string is required. Pass it as an argument!");
            }

            builder.Services.AddDbContext<ApplicationDbContext>(opt =>
            {
#if DEBUG
                opt.EnableSensitiveDataLogging();
#endif
                opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Chat API", Version = "v1" });
            });

            builder.Services.AddSignalR();

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

            app.MapHub<ChatHub>("/chat");

            var factory = new ConnectionFactory
            {
                HostName = "192.168.2.100",
                Port = 4000,
                UserName = "root",
                Password = "root",
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: "chatentries", durable: true, exclusive: false, autoDelete: false, arguments: null);

            Console.WriteLine("ChatApplication is ready for send messages!");

            app.MapGet("/messages", async (ApplicationDbContext db) => await db.Messages.ToListAsync())
            .WithName("GetAllMessages")
            .WithTags("Messages");

            app.MapPost("/messages", async (ApplicationDbContext db, Message message) =>
            {
                var messageJson = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(messageJson);

                await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "chatentries", body: body);
                
                Console.WriteLine($"Sent message {message.MessageContent}");
                
                return Results.Created();
            })
            .WithName("CreateMessage")
            .WithTags("Messages");

            app.Run();
        }
    }
}