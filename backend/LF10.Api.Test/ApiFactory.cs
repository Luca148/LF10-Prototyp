using HarassmentFilter.Core.Interfaces;
using HarassmentFilter.Core.Models;
using HarassmentFilter.Core.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
namespace LF10.Api.Test;

// Runs the API without MySQL: no migrations, and fixed filter rules
// instead of the ones from the database.
public class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("Database:MigrateOnStartup", "false");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IFilterService>();
            services.AddSingleton<IFilterService>(new HarassmentFilterService(new HarassmentFilterConfiguration
            {
                HarassmentCategories =
                [
                    new() { Type = "Bullying", Words = new() { ["idiot"] = "genius" } }
                ]
            }));
        });
    }
}
