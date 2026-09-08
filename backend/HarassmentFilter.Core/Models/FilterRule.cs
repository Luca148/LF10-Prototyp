namespace HarassmentFilter.Core.Models;

public class FilterRule(
    string replacement,
    string type)
{
    public string Replacement { get; } = replacement;

    public HashSet<string> Types { get; } =
        [
            type
        ];
}