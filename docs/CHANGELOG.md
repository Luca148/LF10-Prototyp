# Changelog

This changelog summarizes the current project snapshot. No version number or release date is assigned here.

## Current project snapshot

### Added

- A real-time chat room using SignalR, including live message delivery and an online-user list. The hub reference is in [`docs/SIGNALR.md`](https://github.com/Luca148/LF10-Prototyp/blob/main/docs/SIGNALR.md).
- A message-filtering endpoint: `POST /api/messages/filter`.
- A message-review flow: flagged messages are held for review, users can accept or edit the suggested wording, and edited text is checked again before posting.
- A reporting dialog where a user can choose a reason for reporting a message. The dialog is currently interface-only; reports are not sent to or stored by the backend.
- MySQL storage and seed data for filter categories, words, and phrases.
- Backend unit tests, API and SignalR integration tests, and Playwright browser tests.

### Improved

- The filter now handles common obfuscation patterns, including separators between letters, leetspeak substitutions, and camel-case boundaries. See the [filter service](https://github.com/Luca148/LF10-Prototyp/blob/main/backend/HarassmentFilter.Core/Services/HarassmentFilterService.cs).
- Word and phrase replacement use a shared method in the filter service.
- The message-review interface, online-user list, and report dialog are separated into their own frontend components.

### Test status

At the latest checked `main` commit, the backend test workflow and browser end-to-end workflow both passed:

- [Backend test run](https://github.com/Luca148/LF10-Prototyp/actions/runs/45608538346)
- [Browser test run](https://github.com/Luca148/LF10-Prototyp/actions/runs/45608538029)

### Known limitations and follow-up work

- Chat messages are broadcast live but are not saved to MySQL. The hub also still needs to verify user identity and room access; currently the client supplies the username and room ID. See [`ChatHub.cs`](https://github.com/Luca148/LF10-Prototyp/blob/main/backend/LF10.Api/Hubs/ChatHub.cs).
- Reports are not submitted to or saved by the backend.
- The backend generates an OpenAPI JSON document in Development, but a browsable Swagger UI page is not included yet.
- Add a dedicated unit test for an obfuscated, spaced word such as `fu ck`, alongside harmless-text cases.
- The frontend has an automatic code-check command, but GitHub Actions does not run it yet. This check looks for likely coding mistakes; it is a development check, not an app feature.
- Check the token-like value in the public `.env.example`. If it is real, revoke or rotate it and replace it with a harmless placeholder. Do not copy the value into release notes or screenshots.
