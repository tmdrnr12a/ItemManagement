using Microsoft.EntityFrameworkCore;

namespace ItemManagement.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Item> Items { get; set; }

        public DbSet<Item> Item { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
    }
}
