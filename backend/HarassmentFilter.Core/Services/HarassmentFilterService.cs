using System.Text.RegularExpressions;
using HarassmentFilter.Core.Interfaces;
using HarassmentFilter.Core.Models;

namespace HarassmentFilter.Core.Services;

public class HarassmentFilterService(HarassmentFilterConfiguration configuration) : IFilterService
{
    private static readonly IReadOnlyDictionary<char, char[]> LeetReplacements =
        new Dictionary<char, char[]>
        {
            ['a'] = ['4', '@'],
            ['b'] = ['8'],
            ['e'] = ['3'],
            ['g'] = ['6', '9'],
            ['i'] = ['1', '!', '|'],
            ['l'] = ['1', '|'],
            ['o'] = ['0'],
            ['s'] = ['5', '$'],
            ['t'] = ['7', '+'],
            ['u'] = ['#']
        };

    private readonly FilterResult emptyResult = new()
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

        if (harassmentTypes.Count == 0)
        {
            harassmentTypes.Add("None");
        }

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
                var pattern = BuildObfuscatedPattern(entry.Key);
                var regexOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

                if (!Regex.IsMatch(text, pattern, regexOptions))
                {
                    continue;
                }

                foundTypes.Add(category.Type);

                text = Regex.Replace(
                    text,
                    pattern,
                    match => entry.Value + match.Groups["suffix"].Value,
                    regexOptions);
            }
        }

        return text;
    }

    /// <summary>
    /// Builds a pattern that recognizes common obfuscation techniques:
    /// - separators between letters, e.g. "F U C K" or "f.u.c.k"
    /// - common leetspeak substitutions, e.g. "b00bs" or "f#ck"
    /// - camel-case boundaries, e.g. "aFuck"
    /// </summary>
    private static string BuildObfuscatedPattern(string entry)
    {
        var words = Regex.Split(entry.Trim(), @"\s+")
            .Select(BuildWordPattern)
            .Where(wordPattern => !string.IsNullOrEmpty(wordPattern))
            .ToArray();

        if (words.Length == 0)
        {
            return "(?!)";
        }

        // A phrase must still have a separator between its individual words.
        var phrasePattern = string.Join(@"[^\p{L}\p{N}]+", words);

        // Keep the original inflection handling (s, ed, ing, d, er, est).
        const string suffixPattern = @"(?<suffix>s|ed|ing|d|er|est)?";

        // The second alternative allows a match after a lowercase-to-uppercase
        // transition, so "aFuck" is detected without matching "class" as "ass".
        const string startBoundary =
            @"(?:(?<![\p{L}\p{N}])|(?<=(?-i:[a-z]))(?=(?-i:[A-Z])))";
        const string endBoundary =
            @"(?:(?=$|[^\p{L}\p{N}])|(?<=(?-i:[a-z]))(?=(?-i:[A-Z])))";

        return startBoundary + phrasePattern + suffixPattern + endBoundary;
    }

    private static string BuildWordPattern(string word)
    {
        var characters = word
            .Where(char.IsLetterOrDigit)
            .Select(BuildCharacterPattern)
            .ToArray();

        // Non-letter/non-number characters may appear between letters of a word.
        // This covers spaces, dots, underscores, hyphens and similar separators.
        return string.Join(@"[^\p{L}\p{N}]*", characters);
    }

    private static string BuildCharacterPattern(char character)
    {
        var normalizedCharacter = char.ToLowerInvariant(character);
        var alternatives = new HashSet<char> { normalizedCharacter };

        if (LeetReplacements.TryGetValue(normalizedCharacter, out var replacements))
        {
            foreach (var replacement in replacements)
            {
                alternatives.Add(replacement);
            }
        }

        var characterClass = string.Concat(alternatives.Select(value => Regex.Escape(value.ToString())));
        return $"[{characterClass}]";
    }
}
