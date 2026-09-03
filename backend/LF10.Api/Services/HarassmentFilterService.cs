using LF10.Api.Models;
using System.Text.Json;

namespace LF10.Api.Services;

public class HarassmentFilterService
{
    private HarassmentCategory harassmentCategory = new();

    public HarassmentFilterService(IHostEnvironment environment)
    {
        var path = Path.Combine(
            environment.ContentRootPath,
            "Data",
            "harassment.json");

        var json = File.ReadAllText(path);

        harassmentCategory = JsonSerializer.Deserialize<HarassmentCategory>(json);
    }

    public FilterResponse FilterMessage(string message)
    {
        List<string> harassmentTypes = [];

        foreach (string word in message.Split(' '))
        {
            foreach(HarassmentTypes harassmentType in harassmentCategory.HarassmentTypes)
            {
                if (harassmentType.Words.TryGetValue(word, out var replacement))
                {
                    message = message.Replace(word, replacement);
                    harassmentTypes.Add(harassmentType.Type);
                }
            }
        }

        harassmentTypes = [.. harassmentTypes.Distinct()];

        FilterResponse response = new()
        {
            Message = message,
            HarassmentTypes = harassmentTypes,
            Timestamp = DateTime.Now
        };

        return response;
    }
}
