using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace IdentityService
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static void Connection(this DbContextOptionsBuilder builder, string connectionString)
        {
            builder.UseSqlite(connectionString, options => options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
        }

        public static void Connection(this DbContextOptionsBuilder builder)
        {
            builder.Connection("Data Source=IdentityService.db;");
        }
    }
}
