using Microsoft.EntityFrameworkCore;

namespace bankaccount.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
	    public DbSet<User> User { get; set; }
        public DbSet<Transaction> Transaction { get; set; }

    }
}
