using LF10.Api.Models;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

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
                // Pattern: base word + optional suffix (s, d, ed, ing, etc.)
                var pattern = $@"\b{Regex.Escape(phrase.Key)}(s|ed|ing|d|er|est)?\b";

                if (Regex.IsMatch(result, pattern, RegexOptions.IgnoreCase))
                {
                    harassmentTypes.Add(harassmentType.Type);

                    result = Regex.Replace(
                        result,
                        pattern,
                        m => phrase.Value + m.Groups[1].Value,  // replacement + captured suffix
                        RegexOptions.IgnoreCase);
                }
            }
        }

        foreach (var harassmentType in harassmentCategory.HarassmentTypes)
        {
            foreach (var word in harassmentType.Words)
            {
                var pattern = $@"\b{Regex.Escape(word.Key)}(s|ed|ing|d|er|est)?\b";

                if (Regex.IsMatch(result, pattern, RegexOptions.IgnoreCase))
                {
                    harassmentTypes.Add(harassmentType.Type);

                    result = Regex.Replace(
                        result,
                        pattern,
                        m => word.Value + m.Groups[1].Value,  // replacement + captured suffix
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
