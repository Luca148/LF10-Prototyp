using HarassmentFilter.Core.Models;
using HarassmentFilter.Core.Services;
using LF10.Data;
using LF10.Data.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
namespace HarassmentFilter.Test;

// Uses an in-memory SQLite database instead of MySQL; the loader only
// does plain queries, so the provider doesn't matter here.
public class FilterConfigurationLoaderTest : IDisposable
{
    private readonly SqliteConnection connection = new("DataSource=:memory:");
    private readonly AppDbContext db;

    public FilterConfigurationLoaderTest()
    {
        connection.Open();
        db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options);
        db.Database.EnsureCreated();
    }

    public void Dispose()
    {
        db.Dispose();
        connection.Dispose();
    }

    [Fact]
    public void LoadsCategoriesWithTheirWordsAndPhrases()
    {
        FilterCategoryEntity bullying = new() { Name = "Bullying" };
        FilterCategoryEntity threat = new() { Name = "Threat" };
        db.FilterWords.Add(new() { Category = bullying, Language = "en", CurseWord = "idiot", FilterWord = "genius" });
        db.FilterPhrases.AddRange(
            new() { Category = bullying, Language = "en", CursePhrase = "hate you", FilterPhrase = "love you" },
            new() { Category = threat, Language = "en", CursePhrase = "kill you", FilterPhrase = "support you" });
        db.SaveChanges();

        HarassmentFilterConfiguration configuration = db.LoadFilterConfiguration();

        Assert.Collection(configuration.HarassmentCategories,
            category =>
            {
                Assert.Equal("Bullying", category.Type);
                Assert.Equal(new Dictionary<string, string> { ["idiot"] = "genius" }, category.Words);
                Assert.Equal(new Dictionary<string, string> { ["hate you"] = "love you" }, category.Phrases);
            },
            category =>
            {
                Assert.Equal("Threat", category.Type);
                Assert.Empty(category.Words);
                Assert.Equal(new Dictionary<string, string> { ["kill you"] = "support you" }, category.Phrases);
            });
    }

    [Fact]
    public void UsesWordsOfAllLanguages()
    {
        FilterCategoryEntity insult = new() { Name = "Insult" };
        db.FilterWords.AddRange(
            new() { Category = insult, Language = "en", CurseWord = "jerk", FilterWord = "buddy" },
            new() { Category = insult, Language = "de", CurseWord = "depp", FilterWord = "schatz" });
        db.SaveChanges();

        HarassmentCategory category = Assert.Single(db.LoadFilterConfiguration().HarassmentCategories);

        Assert.Equal(new Dictionary<string, string> { ["jerk"] = "buddy", ["depp"] = "schatz" }, category.Words);
    }

    [Fact]
    public void EmptyDatabaseGivesNoCategories()
    {
        Assert.Empty(db.LoadFilterConfiguration().HarassmentCategories);
    }
}
