using System.Text.Json;
using HarassmentFilter.Core.Models;
using HarassmentFilter.Core.Services;
namespace HarassmentFilter.Test;

public class FilterTest
{
    private readonly HarassmentFilterService service;

    public FilterTest()
    {
        string json = get_json_harrasment();

        HarassmentFilterConfiguration configuration =
            JsonSerializer.Deserialize<HarassmentFilterConfiguration>(json);

        service = new(configuration);
    }

    [Fact]
    public void DetectSexualContent()
    {
        FilterResult result = service.FilterMessage("Fuck you");

        Assert.Equal("hug you", result.Message);
        Assert.Contains("SexualContent", result.HarassmentTypes);
    }

    [Fact]
    public void DetectBullying()
    {
        FilterResult result = service.FilterMessage("you are a crybaby");

        Assert.Equal("you are a tough", result.Message);
        Assert.Contains("Bullying", result.HarassmentTypes);
    }

    [Fact]
    public void DetectInsult()
    {
        FilterResult result = service.FilterMessage("you are a scumbag");

        Assert.Equal("you are a legend", result.Message);
        Assert.Contains("Insult", result.HarassmentTypes);
    }

    [Fact]
    public void DetectRacism()
    {
        FilterResult result = service.FilterMessage("you beaner");

        Assert.Equal("you person", result.Message);
        Assert.Contains("Racism", result.HarassmentTypes);
    }

    [Fact]
    public void DetectDiscrimination()
    {
        FilterResult result = service.FilterMessage("ok boomer");

        Assert.Equal("ok experienced", result.Message);
        Assert.Contains("Discrimination", result.HarassmentTypes);
    }

    [Fact]
    public void DetectHateSpeech()
    {
        FilterResult result = service.FilterMessage("you nazi");

        Assert.Equal("you peace lover", result.Message);
        Assert.Contains("HateSpeech", result.HarassmentTypes);
    }

    [Fact]
    public void DetectViolence()
    {
        FilterResult result = service.FilterMessage("i will punch him");

        Assert.Equal("i will pat him", result.Message);
        Assert.Contains("Violence", result.HarassmentTypes);
    }

    [Fact]
    public void DetectProfanity()
    {
        FilterResult result = service.FilterMessage("oh damn");

        Assert.Equal("oh darn", result.Message);
        Assert.Contains("Profanity", result.HarassmentTypes);
    }

    [Fact]
    public void DetectBullyingPhrases()
    {
        FilterResult result = service.FilterMessage("i hate you");

        Assert.Equal("i love you", result.Message);
        Assert.Contains("Bullying", result.HarassmentTypes);
    }

    [Fact]
    public void DetectInsultPhrases()
    {
        FilterResult result = service.FilterMessage("you are a waste of space");

        Assert.Equal("you are a valuable person", result.Message);
        Assert.Contains("Insult", result.HarassmentTypes);
    }

    [Fact]
    public void DetectThreatPhrases()
    {
        FilterResult result = service.FilterMessage("watch your back");

        Assert.Equal("have your back", result.Message);
        Assert.Contains("Threat", result.HarassmentTypes);
    }

    [Fact]
    public void DetectSexualHarassmentPhrases()
    {
        FilterResult result = service.FilterMessage("send nudes");

        Assert.Equal("send memes", result.Message);
        Assert.Contains("SexualHarassment", result.HarassmentTypes);
    }

    [Fact]
    public void DetectStalkingPhrases()
    {
        FilterResult result = service.FilterMessage("i know where you live");

        Assert.Equal("i know you're great", result.Message);
        Assert.Contains("Stalking", result.HarassmentTypes);
    }

    [Fact]
    public void DetectPersonalAttackPhrases()
    {
        FilterResult result = service.FilterMessage("nobody likes you");

        Assert.Equal("people like you", result.Message);
        Assert.Contains("PersonalAttack", result.HarassmentTypes);
    }

    [Fact]
    public void PhrasesHavePriorityOverWords()
    {
        FilterResult result = service.FilterMessage("i will kill you");

        Assert.Equal("i will support you", result.Message);
        Assert.Contains("Threat", result.HarassmentTypes);
        Assert.DoesNotContain("Violence", result.HarassmentTypes);
    }

    [Fact]
    public void DetectMultipleCategories()
    {
        FilterResult result = service.FilterMessage("shut up you idiot");

        Assert.Equal("please be quiet you genius", result.Message);
        Assert.Contains("Insult", result.HarassmentTypes);
        Assert.Contains("Bullying", result.HarassmentTypes);
    }

    [Fact]
    public void SameCategoryIsOnlyListedOnce()
    {
        FilterResult result = service.FilterMessage("idiot loser");

        Assert.Equal("genius winner", result.Message);
        Assert.Equal("Bullying", Assert.Single(result.HarassmentTypes));
    }

    [Fact]
    public void IsCaseInsensitive()
    {
        FilterResult result = service.FilterMessage("You Are An IDIOT");

        Assert.Equal("you are an genius", result.Message);
        Assert.Contains("Bullying", result.HarassmentTypes);
    }

    [Fact]
    public void KeepsWordSuffix()
    {
        FilterResult result = service.FilterMessage("you losers");

        Assert.Equal("you winners", result.Message);
    }

    [Theory]
    [InlineData("this is a classic")]   
    [InlineData("hello there")]         
    [InlineData("the scrubbing brush")] 
    public void DoesNotFilterPartsOfWords(string message)
    {
        FilterResult result = service.FilterMessage(message);

        Assert.Equal(message, result.Message);
        Assert.Equal("None", Assert.Single(result.HarassmentTypes));
    }

    [Fact]
    public void CleanMessageStaysUnchanged()
    {
        FilterResult result = service.FilterMessage("have a nice day");

        Assert.Equal("have a nice day", result.Message);
        Assert.Equal("None", Assert.Single(result.HarassmentTypes));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void EmptyMessageReturnsEmptyResult(string message)
    {
        FilterResult result = service.FilterMessage(message);

        Assert.Equal(string.Empty, result.Message);
        Assert.Equal("None", Assert.Single(result.HarassmentTypes));
    }

    [Fact]
    public void SetsTimestamp()
    {
        DateTime before = DateTime.Now;

        FilterResult result = service.FilterMessage("hello");

        Assert.InRange(result.Timestamp, before, DateTime.Now);
    }

    public string get_json_harrasment()
    {
        return File.ReadAllText(
        Path.Combine(
            Directory.GetCurrentDirectory(),
            "Data",
            "Harassment.json"));
    }
}
