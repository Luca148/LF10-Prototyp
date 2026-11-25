using HarassmentFilter.Core.Models;
using LF10.Data;
using Microsoft.EntityFrameworkCore;

namespace HarassmentFilter.Core.Services;

public static class FilterConfigurationLoader
{
    public static HarassmentFilterConfiguration LoadFilterConfiguration(this AppDbContext db)
    {
        // Rows of all languages are used. The filter applies categories and
        // entries in list order, so read them in a fixed order to get the same
        // results on every start.
        var categories = db.FilterCategories.AsNoTracking().OrderBy(c => c.Id).ToList();
        var words = db.FilterWords.AsNoTracking().OrderBy(w => w.Id).ToList();
        var phrases = db.FilterPhrases.AsNoTracking().OrderBy(p => p.Id).ToList();

        return new HarassmentFilterConfiguration
        {
            // Baut pro Kategorie die Wörter- und Phrasen-Map für den Runtime-Filter.
            HarassmentCategories = [.. categories.Select(c => new HarassmentCategory
            {
                Type = c.Name,
                Words = words
                    .Where(w => w.CategoryId == c.Id)
                    .ToDictionary(w => w.CurseWord, w => w.FilterWord),
                Phrases = phrases
                    .Where(p => p.CategoryId == c.Id)
                    .ToDictionary(p => p.CursePhrase, p => p.FilterPhrase)
            })]
        };
    }
}
