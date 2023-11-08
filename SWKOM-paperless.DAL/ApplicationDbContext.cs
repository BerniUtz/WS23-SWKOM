using Microsoft.EntityFrameworkCore;

namespace SWKOM_paperless.DAL
{
    public class ApplicationDbContext<T> : DbContext where T : class
    {
        public DbSet<T>? Elements { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext<T>> options) : base(options)
        {
        }
    }
}