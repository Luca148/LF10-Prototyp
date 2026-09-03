using static System.Net.Mime.MediaTypeNames;

namespace LF10.Api.Models;

public class HarassmentCategory
{
    public List<HarassmentTypes> HarassmentTypes { get; set; }
}

public class HarassmentTypes
{
    public string Type { get; set; }

    public Dictionary<string, string> Words { get; set; } = [];
}

public class FilterResponse
    {
        public string Message { get; set; }
        public List<string> HarassmentTypes { get; set; }
        public DateTime Timestamp { get; set; }
    }

public enum HarassmentType
{
    None,
    Bullying,
    Insult,
    Threat,
    Racism,
    Discrimination,
    HateSpeech,
    SexualHarassment,
    SexualContent,
    Violence,
    Profanity,
    Stalking,
    PersonalAttack,
    Spam
}
