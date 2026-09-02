using LF10.Api.Models;

namespace LF10.Api.Services;

public class HarassmentFilterService
{
    public static FilterResponse FilterMessage(string message)
    {
        string filteredMessage = "";

        if (message.Contains("hate"))
        {
            filteredMessage = message.Replace("hate", "love");
        }

        FilterResponse response = new()
        {
            Message = filteredMessage,
            HarassmentType = HarassmentType.Insult.ToString(),
            Timestamp = DateTime.Now
        };

        return response;
    }
}
