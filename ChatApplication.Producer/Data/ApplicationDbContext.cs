using ChatApplication.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApplication.Producer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; } = null!;
    }
}
