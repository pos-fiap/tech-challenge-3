using EmailSender.Model;
using Microsoft.EntityFrameworkCore;

namespace EmailSender.Context
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Log> Log { get; set; }

       
    }
}
