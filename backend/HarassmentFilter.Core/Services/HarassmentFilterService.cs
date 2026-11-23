using System.Text.RegularExpressions;
using HarassmentFilter.Core.Interfaces;
using HarassmentFilter.Core.Models;

namespace HarassmentFilter.Core.Services;

public class HarassmentFilterService(HarassmentFilterConfiguration configuration) : IFilterService
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

        var result = message;

        // Phrases first, otherwise a word inside a phrase (e.g. "kill" in "kill you")
        // gets replaced first and the phrase can no longer match.
        result = ReplaceEntries(result, category => category.Phrases, harassmentTypes);
        result = ReplaceEntries(result, category => category.Words, harassmentTypes);

        harassmentTypes = [.. harassmentTypes.Distinct()];

        if (harassmentTypes.Count == 0) harassmentTypes.Add("None");

        response.Message = result;
        response.HarassmentTypes = harassmentTypes;
        response.Timestamp = DateTime.Now;

        return response;
    }

    private string ReplaceEntries(
        string text,
        Func<HarassmentCategory, Dictionary<string, string>> selectEntries,
        List<string> foundTypes)
    {
        foreach (var category in configuration.HarassmentCategories)
        {
            foreach (var entry in selectEntries(category))
            {
                var pattern = $@"\b{Regex.Escape(entry.Key)}(s|ed|ing|d|er|est)?\b";

                if (Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase))
                {
                    foundTypes.Add(category.Type);

                    text = Regex.Replace(
                        text,
                        pattern,
                        m => entry.Value + m.Groups[1].Value,
                        RegexOptions.IgnoreCase);
                }
            }
        }

        return text;
    }
}