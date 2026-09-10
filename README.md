# HugYou! 🤗

A real-time chat app with a built-in harassment filter — messages are checked before they're sent, and users get a chance to soften flagged wording before it ever reaches the room.

## ✨ Features

- **Live chat** powered by SignalR — messages appear instantly for everyone in the room
- **Harassment filter** — every message is scanned before sending
- **Review & rewrite UI** — flagged messages can't be posted as-is; pick the suggested rewrite or edit it yourself
- **Simple username login** — no accounts, just pick a name and join

## 🛠️ Tech Stack

|               |                                  |
| ------------- | -------------------------------- |
| **Frontend**  | React 19 · TypeScript · Vite     |
| **Backend**   | ASP.NET Core (.NET 10) · SignalR |
| **Real-time** | WebSockets via SignalR Hub       |

## 📁 Project Structure

```
frontend/   React + Vite client
backend/
  LF10.Api/               ASP.NET Core Web API + SignalR hub
  HarassmentFilter.Core/  Shared filtering logic & word lists
```

## 🚀 Getting Started

### Backend

```bash
cd backend/LF10.Api
dotnet run
```

Runs on `https://localhost:7255`.

### Frontend

```bash
cd frontend
npm install
npm run dev
```

Runs on `http://localhost:5173` — proxies API calls and the SignalR connection to the backend automatically.

## 💬 How It Works

1. Type a message and hit **Send**.
2. It's checked against the harassment filter.
3. ✅ Clean → posted instantly, in real time, to everyone in the room.
4. 🚫 Flagged → a review panel opens with a suggested rewrite — accept it or edit your own wording, then post.

---

Built as a school project. 🎓
