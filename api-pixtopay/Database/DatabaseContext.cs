using api_pixtopay.Model;
using Microsoft.EntityFrameworkCore;

namespace api_pixtopay.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) 
        { }

        public DbSet<Blog> blog { get; set; }
        public DbSet<User> user { get; set; }

    }
}
