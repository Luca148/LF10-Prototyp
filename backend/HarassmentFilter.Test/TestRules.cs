using HarassmentFilter.Core.Models;
namespace HarassmentFilter.Test;

// The real rules live in the database. These are the entries the tests need,
// copied from there, in the same category order.
public static class TestRules
{
    public static HarassmentFilterConfiguration Create() => new()
    {
        HarassmentCategories =
        [
            new()
            {
                Type = "Bullying",
                Words = new() { ["loser"] = "winner", ["idiot"] = "genius", ["crybaby"] = "tough", ["scrub"] = "mvp", ["hate"] = "love" },
                Phrases = new() { ["hate you"] = "love you" }
            },
            new()
            {
                Type = "Insult",
                Words = new() { ["scumbag"] = "legend", ["scum"] = "star" },
                Phrases = new() { ["waste of space"] = "valuable person", ["shut up"] = "please be quiet" }
            },
            new()
            {
                Type = "Threat",
                Phrases = new() { ["kill you"] = "support you", ["watch your back"] = "have your back" }
            },
            new() { Type = "Racism", Words = new() { ["beaner"] = "person" } },
            new() { Type = "Discrimination", Words = new() { ["boomer"] = "experienced" } },
            new() { Type = "HateSpeech", Words = new() { ["nazi"] = "peace lover" } },
            new() { Type = "SexualHarassment", Phrases = new() { ["send nudes"] = "send memes" } },
            new() { Type = "SexualContent", Words = new() { ["fuck"] = "hug", ["ass"] = "friend" } },
            new() { Type = "Violence", Words = new() { ["kill"] = "help", ["punch"] = "pat" } },
            new() { Type = "Profanity", Words = new() { ["damn"] = "darn", ["hell"] = "heck" } },
            new() { Type = "Stalking", Phrases = new() { ["i know where you live"] = "i know you're great" } },
            new() { Type = "PersonalAttack", Phrases = new() { ["nobody likes you"] = "people like you" } }
        ]
    };
}
