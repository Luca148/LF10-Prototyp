namespace HarassmentFilter.Core.Models;

public class HarassmentCategory
{
    public string Type { get; set; }
    public Dictionary<string, string> Words { get; set; } = [];
    public Dictionary<string, string> Phrases { get; set; } = [];
}
