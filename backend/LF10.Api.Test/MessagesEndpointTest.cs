using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using HarassmentFilter.Core.Models;
namespace LF10.Api.Test;

public class MessagesEndpointTest(ApiFactory factory)
    : IClassFixture<ApiFactory>
{
    private readonly HttpClient client = factory.CreateClient();

    [Fact]
    public async Task FilterEndpointReturnsFilteredMessage()
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/messages/filter", "you idiot");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        FilterResult result = await response.Content.ReadFromJsonAsync<FilterResult>();
        Assert.Equal("you genius", result.Message);
        Assert.Contains("Bullying", result.HarassmentTypes);
    }

    [Fact]
    public async Task FilterEndpointLeavesCleanMessageUnchanged()
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/messages/filter", "have a nice day");

        FilterResult result = await response.Content.ReadFromJsonAsync<FilterResult>();
        Assert.Equal("have a nice day", result.Message);
        Assert.Equal("None", Assert.Single(result.HarassmentTypes));
    }

    [Fact]
    public async Task ResponseUsesCamelCasePropertyNames()
    {
        // The frontend reads these exact names; a casing change would silently break it.
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/messages/filter", "you idiot");

        using JsonDocument json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        JsonElement root = json.RootElement;

        Assert.True(root.TryGetProperty("message", out _));
        Assert.True(root.TryGetProperty("harassmentTypes", out _));
        Assert.True(root.TryGetProperty("timestamp", out _));
    }

    [Fact]
    public async Task MissingBodyReturnsBadRequest()
    {
        HttpResponseMessage response = await client.PostAsync(
            "/api/messages/filter",
            new StringContent("", System.Text.Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
