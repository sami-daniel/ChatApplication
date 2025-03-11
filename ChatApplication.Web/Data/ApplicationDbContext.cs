using ChatApplication.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApplication.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; } = null!;
    }
}