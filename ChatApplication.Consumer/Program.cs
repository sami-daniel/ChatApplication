using ChatApplication.Consumer.Data;
using ChatApplication.Shared.Models;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING") ?? throw new ArgumentException("MYSQL_CONNECTION_STRING is required");

var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
    .Options;

var dbContext = new ApplicationDbContext(dbContextOptions);

dbContext.Database.Migrate();

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

Console.WriteLine("ChatApplication is ready for consume messages!");

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var messageJson = Encoding.UTF8.GetString(body);
    var chatEntryJson = JsonSerializer.Deserialize<Message>(messageJson);
    await dbContext.Messages.AddAsync(chatEntryJson!);
    await dbContext.SaveChangesAsync();
    Console.WriteLine($"Chat entry received: {chatEntryJson!.MessageContent}");
};

await channel.BasicConsumeAsync(queue: "chatentries", autoAck: true, consumer: consumer);

// TODO: Search for better solution to keep the process alive
while (true) 
{
	await Task.Delay(1000);	
}
