using ChatApplication.Producer.Data;
using ChatApplication.Producer.Helpers;
using ChatApplication.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System.Text;

namespace ChatApplication.Producer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly MessageSender _messageSender;

        public MessagesController(ApplicationDbContext context, MessageSender messageSender)
        {
            _context = context;
            _messageSender = messageSender;
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages()
        {
            var messages = await _context.Messages.ToListAsync();
            return Ok(messages);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(Message message)
        {
            await _messageSender.SendMessageAsync(message);
            return Created();
        }
    }
}
