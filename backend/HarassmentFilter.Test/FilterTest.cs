using System.Text.Json;
using HarassmentFilter.Core.Models;
using HarassmentFilter.Core.Services;
namespace HarassmentFilter.Test;

public class FilterTest
{
    [Fact]
    public void HugYouTest()
    {
        string json = File.ReadAllText(
        Path.Combine(
            Directory.GetCurrentDirectory(),
            "Data",
            "Harassment.json"));

        HarassmentFilterConfiguration configuration =
            JsonSerializer.Deserialize<HarassmentFilterConfiguration>(json);

        HarassmentFilterService service = new(configuration);

        Assert.Equal("hug you", service.FilterMessage("Fuck you").Message);
    }
}
