using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LF10.Data;

public static class DatabaseSetup
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString) =>
        services.AddDbContext<AppDbContext>(options => options.UseMySQL(connectionString));

    // Creates the tables (and the initial filter rules) on a fresh database
    // and applies new migrations after an update.
    public static void MigrateDatabase(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
    }
}
