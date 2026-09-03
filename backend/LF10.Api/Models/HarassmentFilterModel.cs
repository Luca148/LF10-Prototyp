namespace LF10.Api.Models;

public class FilterResponse
{
    public string Message { get; set; }
    public string HarassmentType { get; set; }
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
