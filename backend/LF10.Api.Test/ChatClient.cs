using System.Text.Json;
using System.Threading.Channels;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
namespace LF10.Api.Test;

// SignalR client for the ChatHub, connected in memory to the test server.
// It collects everything the hub sends, so tests can wait for it.
public sealed class ChatClient : IAsyncDisposable
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

    private readonly HubConnection connection;
    private readonly Channel<JsonElement> messages = Channel.CreateUnbounded<JsonElement>();
    private readonly Channel<List<string>> userLists = Channel.CreateUnbounded<List<string>>();

    private ChatClient(HubConnection connection)
    {
        this.connection = connection;
        connection.On<JsonElement>("ReceiveMessage", message => { messages.Writer.TryWrite(message); });
        connection.On<List<string>>("UserListUpdated", users => { userLists.Writer.TryWrite(users); });
    }

    public static async Task<ChatClient> Connect(ApiFactory factory)
    {
        HubConnection connection = new HubConnectionBuilder()
            .WithUrl(new Uri(factory.Server.BaseAddress, "chathub"), options =>
            {
                // The test server only offers an in-memory HTTP handler, no real
                // sockets, so the client has to use long polling over it.
                options.HttpMessageHandlerFactory = _ => factory.Server.CreateHandler();
                options.Transports = HttpTransportType.LongPolling;
            })
            .Build();

        ChatClient client = new(connection);
        await connection.StartAsync();
        return client;
    }

    public Task JoinRoom(int chatRoomId, string username) =>
        connection.InvokeAsync("JoinRoom", chatRoomId, username);

    public Task SendMessage(int chatRoomId, string content, string username) =>
        connection.InvokeAsync("SendMessage", chatRoomId, content, username);

    public Task Disconnect() => connection.StopAsync();

    // Kept as raw JSON, so tests see the exact property names the frontend reads.
    public async Task<JsonElement> NextMessage()
    {
        using CancellationTokenSource timeout = new(Timeout);
        try
        {
            return await messages.Reader.ReadAsync(timeout.Token);
        }
        catch (OperationCanceledException)
        {
            Assert.Fail("No message received.");
            throw;
        }
    }

    // The hub sends a new list on every join and leave, so older ones are skipped.
    public async Task WaitForUsers(params string[] expected)
    {
        using CancellationTokenSource timeout = new(Timeout);
        List<string> users = [];
        try
        {
            while (!users.Order().SequenceEqual(expected.Order()))
            {
                users = await userLists.Reader.ReadAsync(timeout.Token);
            }
        }
        catch (OperationCanceledException)
        {
            Assert.Fail($"Expected users [{string.Join(", ", expected)}], last list was [{string.Join(", ", users)}].");
        }
    }

    public ValueTask DisposeAsync() => connection.DisposeAsync();
}
