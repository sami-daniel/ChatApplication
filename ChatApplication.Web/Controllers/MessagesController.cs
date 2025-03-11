using ChatApplication.Web.Data;
using ChatApplication.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApplication.Web.Controllers
{
    [Route("messages")]
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetMessages()
        {
            return Json(await _context.Messages.ToListAsync());
        }

        [Route("{id}")]
        public async Task<IActionResult> GetMessage(int id)
        {
            return Json(await _context.Messages.FindAsync(id));
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateMessage([FromBody] Message message)
        {
            if (message == null)
            {
                return BadRequest("Invalid message data.");
            }

            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
            return Json(message);
        }
    }
}
