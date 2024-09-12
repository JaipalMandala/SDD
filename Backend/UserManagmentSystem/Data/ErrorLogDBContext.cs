using Microsoft.EntityFrameworkCore;
using UserManagmentSystem.Models;

namespace UserManagmentSystem.Data
{
    public class ErrorLogDBContext : DbContext
    {
        public ErrorLogDBContext(DbContextOptions<ErrorLogDBContext> options) : base(options) { }

        public DbSet<ExceptionsLog> exceptionsLogs { get; set; }

    }
}
