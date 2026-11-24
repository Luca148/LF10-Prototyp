using HarassmentFilter.Core.Interfaces;
using LF10.Data;
using Microsoft.Extensions.DependencyInjection;

namespace HarassmentFilter.Core.Services;

public static class HarassmentFilterSetup
{
    // The rules are read from the database once, so changes made there
    // only take effect after a restart of the backend.
    public static IServiceCollection AddHarassmentFilter(this IServiceCollection services) =>
        services.AddSingleton<IFilterService>(provider =>
        {
            using var scope = provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return new HarassmentFilterService(db.LoadFilterConfiguration());
        });
}
