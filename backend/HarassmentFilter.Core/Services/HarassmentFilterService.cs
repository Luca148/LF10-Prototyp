using System.Text.RegularExpressions;
using HarassmentFilter.Core.Interfaces;
using HarassmentFilter.Core.Models;

namespace HarassmentFilter.Core.Services;

public class HarassmentFilterService(HarassmentFilterConfiguration configuration) : IFilterService
{
    // Maps characters to common visual substitutes used in leetspeak or deliberate word obfuscation.
    // For example, 'o' can be written as '0' and 's' as '$' or '5'.
    private static readonly Dictionary<char, char[]> LeetReplacements =
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

        // Process phrases before individual words. Otherwise, replacing a word that is part of a phrase
        // (for example, "kill" in "kill you") could change the text before the full phrase is checked.
        result = ReplaceEntries(result, category => category.Phrases, harassmentTypes);
        result = ReplaceEntries(result, category => category.Words, harassmentTypes);

        // Several entries can belong to the same category, so return each detected category only once.
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

    private string ReplaceEntries(string text, Func<HarassmentCategory, Dictionary<string, string>> selectEntries, List<string> foundTypes)
    {
        foreach (var category in configuration.HarassmentCategories)
        {
            foreach (var entry in selectEntries(category))
            {
                var pattern = BuildObfuscatedPattern(entry.Key);

                // Ignore letter casing, but use culture-independent casing rules so matching behaves
                // consistently regardless of the machine's current culture.
                var regexOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

                if (!Regex.IsMatch(text, pattern, regexOptions))
                {
                    continue;
                }

                foundTypes.Add(category.Type);

                // Replace every occurrence. The named "suffix" group is appended unchanged so that
                // supported inflections such as "fucks" or "fucking" retain their original ending.
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
    /// Builds a regex for an entry while allowing common ways of disguising its spelling:
    /// - separators between letters, such as "F U C K" or "f.u.c.k";
    /// - leetspeak substitutions, such as "b00bs" or "f#ck";
    /// - camel-case boundaries, such as the "F" in "aFuck".
    ///
    /// The generated pattern still checks word boundaries to avoid matching a listed term
    /// as an arbitrary substring of an unrelated word (for example, "ass" inside "class").
    /// </summary>
    private static string BuildObfuscatedPattern(string entry)
    {
        // Treat whitespace-separated parts as separate words. This allows phrase entries to keep
        // their word structure while BuildWordPattern handles obfuscation inside each part.
        var words = Regex.Split(entry.Trim(), @"\s+")
            .Select(BuildWordPattern)
            .Where(wordPattern => !string.IsNullOrEmpty(wordPattern))
            .ToArray();

        // An empty or punctuation-only entry should never match arbitrary input.
        if (words.Length == 0)
        {
            return "(?!)";
        }

        // Require at least one non-letter/non-number character between the words of a phrase.
        // This prevents a phrase such as "bad word" from being treated as the single word "badword".
        var phrasePattern = string.Join(@"[^\p{L}\p{N}]+", words);

        // Keep the existing inflection support. The named group lets the replacement callback
        // preserve the matched ending instead of needing a separate replacement for every form.
        const string suffixPattern = @"(?<suffix>s|ed|ing|d|er|est)?";

        // Both boundary expressions use lookarounds: they inspect adjacent characters without
        // consuming them, so the boundaries are not included in the text that gets replaced.
        // The first alternative is a regular word boundary based on Unicode letters and digits.
        // The second recognizes a lower-case-to-upper-case transition, allowing "aFuck" to match
        // at 'F'. The inline (?-i:...) disables IgnoreCase locally; otherwise [a-z] and [A-Z]
        // would become indistinguishable because the whole regex is matched case-insensitively.
        const string startBoundary =
            @"(?:(?<![\p{L}\p{N}])|(?<=(?-i:[a-z]))(?=(?-i:[A-Z])))";

        // A match may end before punctuation/whitespace, at the end of the input, or at a
        // lower-case-to-upper-case transition. The last case handles the boundary consistently
        // when a term is followed by another camel-case segment.
        const string endBoundary =
            @"(?:(?=$|[^\p{L}\p{N}])|(?<=(?-i:[a-z]))(?=(?-i:[A-Z])))";

        return startBoundary + phrasePattern + suffixPattern + endBoundary;
    }

    private static string BuildWordPattern(string word)
    {
        // Build a pattern for each alphanumeric character and discard punctuation from the JSON
        // entry itself. Punctuation in the user's message is handled separately as an allowed separator.
        var characters = word
            .Where(char.IsLetterOrDigit)
            .Select(BuildCharacterPattern)
            .ToArray();

        // Allow zero or more non-letter/non-number characters between expected characters.
        // Because the repetition is '*', both "fuck" and obfuscated forms such as "f.u.c.k" match.
        return string.Join(@"[^\p{L}\p{N}]*", characters);
    }

    private static string BuildCharacterPattern(char character)
    {
        var normalizedCharacter = char.ToLowerInvariant(character);
        var alternatives = new HashSet<char> { normalizedCharacter };

        // Add the original character plus its configured visual substitutes. A set avoids
        // duplicate alternatives when two mappings happen to produce the same substitute.
        if (LeetReplacements.TryGetValue(normalizedCharacter, out var replacements))
        {
            foreach (var replacement in replacements)
            {
                alternatives.Add(replacement);
            }
        }

        // A character class matches exactly one of its alternatives. Escape each value first so
        // regex metacharacters such as '$', '+', and '|' are interpreted literally, not as syntax.
        var characterClass = string.Concat(alternatives.Select(value => Regex.Escape(value.ToString())));
        return $"[{characterClass}]";
    }
}
