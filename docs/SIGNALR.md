# SignalR chat reference

## Connection

- Hub URL: `/chathub`
- The current demo joins chat room `1` (shown as `general` in the UI).
- Clients should register event handlers, start the connection, then join a room.

## Client-to-server methods

### `JoinRoom(chatRoomId, username)`

Joins the specified room. The server then broadcasts `UserListUpdated` to that room.

| Argument | Type | Meaning |
|---|---|---|
| `chatRoomId` | number | Room to join |
| `username` | string | Name shown in the participant list |

### `SendMessage(chatRoomId, content, username)`

Filters the message and sends a `ReceiveMessage` event to the room.

| Argument | Type | Meaning |
|---|---|---|
| `chatRoomId` | number | Room to receive the message |
| `content` | string | Message text |
| `username` | string | Name shown with the message |

These methods do not return a message result directly. Clients receive updates through the server-to-client events below.

## Server-to-client events

### `ReceiveMessage`

Sent to clients in the room when a message is broadcast.
```json
{
  "username": "Alex",
  "message": "Hello",
  "wasModified": false,
  "harassmentTypes": ["None"],
  "timestamp": "2026-03-19T14:30:00Z"
}
