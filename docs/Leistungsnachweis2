# HugYou — Project Documentation

## Project overview

HugYou is a real-time chat prototype that filters potentially harmful messages and allows users to review modified text before it is sent. The application uses a React and TypeScript frontend, an ASP.NET Core backend, SignalR for live communication, and MySQL with Entity Framework Core for filter data.

Repository snapshot: public `main` branch at commit [`10c7d44`](https://github.com/Luca148/LF10-Prototyp/commit/10c7d4449b3944627bf9fc2095605fc3aca97648).

## MVP features

### Message reporting

The frontend provides a report dialog for a chat message. It displays the message being reported and lets the user choose a reason, such as bullying, insult, threat, racism, or spam. At present, submitting the dialog only writes a message to the browser console. The report is not sent to the backend or stored, so report submission is not yet a complete end-to-end feature. Source: [`ReportingWindow.tsx`](https://github.com/Luca148/LF10-Prototyp/blob/main/frontend/src/ReportingComponent/ReportingWindow.tsx).

### MySQL filter data

MySQL, accessed through Entity Framework Core, stores filter categories, words, and phrases. These records provide the data used by the filter. The current database does not store chat messages or reports. Source: [`AppDbContext.cs`](https://github.com/Luca148/LF10-Prototyp/blob/main/backend/LF10.Data/AppDbContext.cs).

### Message filter method

The filtering service checks configured phrases before individual words, replaces matches, and returns the filtered text, detected categories, and a timestamp. Its pattern-building logic handles common obfuscations such as separators between letters, leetspeak substitutions, and camel-case boundaries. Source: [`HarassmentFilterService.cs`](https://github.com/Luca148/LF10-Prototyp/blob/main/backend/HarassmentFilter.Core/Services/HarassmentFilterService.cs).

The filter tests cover categories, phrases, suffixes, empty input, clean messages, and examples that should remain unchanged: [`FilterTest.cs`](https://github.com/Luca148/LF10-Prototyp/blob/main/backend/HarassmentFilter.Test/FilterTest.cs). There is not yet a dedicated unit test for an obfuscated spelling such as `f.u.c.k`; this remains a test gap.

### Real-time chat

SignalR connects clients to chat rooms. `JoinRoom` adds a connection to a room and updates the online-user list. `SendMessage` filters the text before broadcasting it to the room. When a client disconnects, the online list is updated. Room ID 1 is displayed as “general.” Source: [`ChatHub.cs`](https://github.com/Luca148/LF10-Prototyp/blob/main/backend/LF10.Api/Hubs/ChatHub.cs).

The hub route is `/chathub`; the documented methods are `JoinRoom` and `SendMessage`, with events including `ReceiveMessage` and `UserListUpdated`. See the [SignalR documentation](https://github.com/Luca148/LF10-Prototyp/blob/main/docs/SIGNALR.md).

## API documentation

The backend exposes `POST /api/messages/filter` for filtering a message through HTTP. OpenAPI generation and Swagger UI are configured in [`Program.cs`](https://github.com/Luca148/LF10-Prototyp/blob/main/backend/LF10.Api/Program.cs). In the Development environment, the OpenAPI document is available at `/openapi/v1.json` and the Swagger UI at `/swagger`. The live SignalR interface is documented separately in [`docs/SIGNALR.md`](https://github.com/Luca148/LF10-Prototyp/blob/main/docs/SIGNALR.md).

## Refactoring and code organization

The repository separates several responsibilities into distinct methods or components:

- Word and phrase replacement use shared logic in the filter service.
- Message review is separated into a [`ReviewComponent`](https://github.com/Luca148/LF10-Prototyp/tree/main/frontend/src/ReviewComponent).
- Online-user display is separated into a [`UsersOnlineComponent`](https://github.com/Luca148/LF10-Prototyp/tree/main/frontend/src/UsersOnlineComponent).
- Reporting is separated into a dialog component and stylesheet in [`ReportingComponent`](https://github.com/Luca148/LF10-Prototyp/tree/main/frontend/src/ReportingComponent).

Possible future refactoring includes moving SignalR connection handling out of the main chat view and separating filter-pattern construction from the replacement step. These are proposals, not completed changes.

## Quality assurance

The project includes several levels of automated testing:

- **Unit tests** check filter behavior and configuration.
- **API integration tests** check filtered and clean responses, JSON property names, and a missing request body: [`MessagesEndpointTest.cs`](https://github.com/Luca148/LF10-Prototyp/blob/main/backend/LF10.Api.Test/MessagesEndpointTest.cs).
- **SignalR integration tests** check filtered message delivery, room isolation, user-list updates, and disconnect behavior: [`ChatHubTest.cs`](https://github.com/Luca148/LF10-Prototyp/blob/main/backend/LF10.Api.Test/ChatHubTest.cs).
- **End-to-end tests** use Playwright to cover login, message review, edited messages, and interaction between two users: [E2E tests](https://github.com/Luca148/LF10-Prototyp/tree/main/e2e/tests).

The latest checked [backend workflow](https://github.com/Luca148/LF10-Prototyp/actions/runs/45619954092) and [end-to-end workflow](https://github.com/Luca148/LF10-Prototyp/actions/runs/45619954063) passed. Both runs reported a duplicate `using` directive warning in `ChatHub.cs`. The frontend also has an `npm run lint` command, but it is not currently run by CI.

## Current limitations

Chat messages are not persisted. The client supplies the username and room ID; the hub still has TODOs to use trusted identity claims and verify room access. Reports are not saved or sent to the backend. Swagger UI is configured for Development, so this configuration alone does not establish that it is available in every deployment environment.

## Project screenshots

- Chat interface and message-review flow: (/images/message review.png)
- Message-report dialog: [Insert screenshot]
- Swagger UI at `/swagger` in Development: [Insert screenshot]
- Successful backend and end-to-end Actions runs: [Insert screenshot]

Screenshots to be supplied by the project team; personal information should be hidden.
