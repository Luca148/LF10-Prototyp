using System.Text.Json;
using System.Text.RegularExpressions;
using LF10.Api.Models;

namespace LF10.Api.Services;

public class HarassmentFilterService
{
    private HarassmentCategory harassmentCategory = new();

    private FilterResponse emptyResponse = new()
    {
        Message = string.Empty,
        HarassmentTypes = ["None"]
    };


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
        FilterResponse response = new();
        List<string> harassmentTypes = [];

        if (string.IsNullOrWhiteSpace(message))
        {
            emptyResponse.Timestamp = DateTime.Now;

            return emptyResponse;
        }

        var result = message;

        foreach (var harassmentType in harassmentCategory.HarassmentTypes)
        {
            foreach (var phrase in harassmentType.Phrases)
            {
                var oldResult = result;

                result = result.Replace(
                    phrase.Key,
                    phrase.Value,
                    StringComparison.OrdinalIgnoreCase);

                if (result != oldResult)
                {
                    harassmentTypes.Add(harassmentType.Type);
                }
            }
        }

        foreach (var harassmentType in harassmentCategory.HarassmentTypes)
        {
            foreach (var word in harassmentType.Words)
            {
                var pattern = $@"\b{Regex.Escape(word.Key)}\b";

                if (Regex.IsMatch(result, pattern, RegexOptions.IgnoreCase))
                {
                    harassmentTypes.Add(harassmentType.Type);

                    result = Regex.Replace(
                        result,
                        pattern,
                        word.Value,
                        RegexOptions.IgnoreCase);
                }
            }
        }

        harassmentTypes = [.. harassmentTypes.Distinct()];

        if (harassmentTypes.Count == 0) harassmentTypes.Add("None");

        response.Message = result;
        response.HarassmentTypes = harassmentTypes;
        response.Timestamp = DateTime.Now;

        return response;
    }
}
