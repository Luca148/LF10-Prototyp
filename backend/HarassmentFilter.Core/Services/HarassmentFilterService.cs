using System.Text.RegularExpressions;
using HarassmentFilter.Core.Models;

namespace HarassmentFilter.Core.Services;

public class HarassmentFilterService(HarassmentFilterConfiguration configuration) : IHarassmentFilterService
{
    private FilterResult emptyResult = new()
    {
        Message = string.Empty,
        HarassmentTypes = ["None"]
    };

    public FilterResult FilterMessage(string message)
    {
        FilterResult response = new();
        List<string> harassmentTypes = [];

        if (string.IsNullOrWhiteSpace(message))
        {
            emptyResult.Timestamp = DateTime.Now;
            return emptyResult;
        }

        var result = message.ToLower();

        foreach (var harassmentType in configuration.HarassmentCategories)
        {
            foreach (var phrase in harassmentType.Phrases)
            {
                var pattern = $@"\b{Regex.Escape(phrase.Key)}(s|ed|ing|d|er|est)?\b";

                if (Regex.IsMatch(result, pattern, RegexOptions.IgnoreCase))
                {
                    harassmentTypes.Add(harassmentType.Type);

                    result = Regex.Replace(
                        result,
                        pattern,
                        m => phrase.Value + m.Groups[1].Value,
                        RegexOptions.IgnoreCase);
                }
            }
        }

        foreach (var harassmentType in configuration.HarassmentCategories)
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
                        m => word.Value + m.Groups[1].Value,
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