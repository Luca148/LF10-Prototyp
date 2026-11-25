# SignalR chat reference

This document describes the current SignalR chat behavior in the HugYou prototype.

## Connection

- Hub URL: `/chathub`
- The current chat screen uses room ID `1`, shown as `general`.
- Register event handlers before starting the connection. After it starts, call `JoinRoom`.

## Client-to-server methods

### `JoinRoom(chatRoomId, username)`

Joins the specified room. The server then sends a `UserListUpdated` event to that room.

| Argument | Type | Meaning |
|---|---|---|
| `chatRoomId` | number | ID of the room to join |
| `username` | string | Name displayed in the participant list |

Example:
```javascript
await connection.invoke("JoinRoom", 1, username);
```

### `SendMessage(chatRoomId, content, username)`

Filters the message and broadcasts it to the room using the `ReceiveMessage` event.

| Argument | Type | Meaning |
|---|---|---|
| `chatRoomId` | number | ID of the room receiving the message |
| `content` | string | Message text |
| `username` | string | Name displayed with the message |

Example:
```javascript
await connection.invoke("SendMessage", 1, "Hello", username);
```

These methods do not return a message payload directly. Clients receive chat and participant updates through the events below.

## Server-to-client events

### `ReceiveMessage`

Sent to clients in the room when a message is broadcast.

| Field | Type | Meaning |
|---|---|---|
| `username` | string | Name displayed with the message |
| `message` | string | Message text after filtering |
| `wasModified` | boolean | Whether the filter changed the message |
| `harassmentTypes` | string array | Filter categories found, or `["None"]` |
| `timestamp` | string | Message time in ISO 8601 format |

Example payload:
```json
{
  "username": "Alex",
  "message": "Hello",
  "wasModified": false,
  "harassmentTypes": ["None"],
  "timestamp": "2026-01-01T12:00:00Z"
}
```

### `UserListUpdated`

Sent to clients in the room when a user joins or disconnects. The payload is an array of usernames.

Example payload:
```json
["Alex", "Sam"]
```

## Example client flow
```javascript
const connection = new HubConnectionBuilder()
  .withUrl("/chathub")
  .build();

connection.on("ReceiveMessage", (message) => {
  console.log(message.username, message.message, message.timestamp);
});

connection.on("UserListUpdated", (usernames) => {
  console.log(usernames);
});

await connection.start();
await connection.invoke("JoinRoom", 1, username);
```

To send a message after joining:
```javascript
await connection.invoke("SendMessage", 1, "Hello", username);
```

## Current limitations

This describes the prototype as it currently works:

- The client supplies the username and room ID. The hub does not yet verify the username against a signed-in identity or check whether the user is allowed to join that room.
- Chat messages are broadcast live but are not saved to MySQL, so the hub does not provide chat history to a newly connected user.
- There is no separate `LeaveRoom` method. When a client disconnects, the server removes that connection from the participant list and sends an updated `UserListUpdated` event.
