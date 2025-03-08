using ChatApplication.Shared.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

#nullable disable

namespace ChatApplication.Producer.Helpers
{
    public class MessageSender
    {
        private readonly ConnectionFactory _connectionFactory;

        private IChannel _channel;

        public MessageSender()
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = "192.168.2.100",
                Port = 4000,
                UserName = "root",
                Password = "root",
            };

            Init();
        }

        private async void Init()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: "chatentries", durable: true, exclusive: false, autoDelete: false, arguments: null);

            _channel = channel;
        }

        public async Task SendMessageAsync(Message message)
        {
            ArgumentNullException.ThrowIfNull(message, nameof(message));

            var messageJson = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(messageJson);

            await _channel.BasicPublishAsync(exchange: string.Empty, routingKey: "chatentries", body: body);

            Console.WriteLine($"Sent message {message.MessageContent}");

        }
    }
}
