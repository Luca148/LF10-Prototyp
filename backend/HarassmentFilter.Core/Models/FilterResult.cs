namespace HarassmentFilter.Core.Models;

public class FilterResult
{
    public string Message { get; set; } = "";

    public bool WasModified { get; set; }

    public List<string> HarassmentTypes { get; set; } = [];
    public DateTime Timestamp { get; set; }
}
