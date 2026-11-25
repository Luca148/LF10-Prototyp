using System.Text.Json;
namespace LF10.Api.Test;

// The hub keeps its users in a static list shared by all tests,
// so every test uses its own room.
public class ChatHubTest(ApiFactory factory)
    : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task JoinRoomSendsUserListToEveryoneInRoom()
    {
        await using ChatClient alice = await ChatClient.Connect(factory);
        await using ChatClient bob = await ChatClient.Connect(factory);

        await alice.JoinRoom(101, "Alice");
        await alice.WaitForUsers("Alice");

        await bob.JoinRoom(101, "Bob");
        await alice.WaitForUsers("Alice", "Bob");
        await bob.WaitForUsers("Alice", "Bob");
    }

    [Fact]
    public async Task SendMessageBroadcastsFilteredMessage()
    {
        await using ChatClient alice = await ChatClient.Connect(factory);
        await using ChatClient bob = await ChatClient.Connect(factory);
        await alice.JoinRoom(102, "Alice");
        await bob.JoinRoom(102, "Bob");

        await alice.SendMessage(102, "you idiot", "Alice");

        // The frontend reads these exact names; a casing change would silently break it.
        JsonElement received = await bob.NextMessage();
        Assert.Equal("Alice", received.GetProperty("username").GetString());
        Assert.Equal("you genius", received.GetProperty("message").GetString());
        Assert.Contains("Bullying", received.GetProperty("harassmentTypes").EnumerateArray().Select(type => type.GetString()));
        Assert.True(received.TryGetProperty("timestamp", out _));

        // The sender gets it too, that's how it shows up in their own chat.
        JsonElement echoed = await alice.NextMessage();
        Assert.Equal("you genius", echoed.GetProperty("message").GetString());
    }

    [Fact]
    public async Task MessagesStayInTheirRoom()
    {
        await using ChatClient alice = await ChatClient.Connect(factory);
        await using ChatClient carol = await ChatClient.Connect(factory);
        await alice.JoinRoom(103, "Alice");
        await carol.JoinRoom(104, "Carol");

        await alice.SendMessage(103, "hello room 103", "Alice");
        await carol.SendMessage(104, "hello room 104", "Carol");

        // Messages arrive in order, so Alice's would come first if it had leaked.
        JsonElement received = await carol.NextMessage();
        Assert.Equal("hello room 104", received.GetProperty("message").GetString());
    }

    [Fact]
    public async Task DisconnectRemovesUserFromList()
    {
        await using ChatClient alice = await ChatClient.Connect(factory);
        await using ChatClient bob = await ChatClient.Connect(factory);
        await alice.JoinRoom(105, "Alice");
        await bob.JoinRoom(105, "Bob");
        await alice.WaitForUsers("Alice", "Bob");

        await bob.Disconnect();

        await alice.WaitForUsers("Alice");
    }
}
