using Microsoft.EntityFrameworkCore;

namespace Sportik.Desktop.Infrastructure.Persistence
{
    internal sealed class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }
    }
}