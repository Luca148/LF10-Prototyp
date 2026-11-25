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
| **Database**  | MySQL 8.4 · EF Core              |
| **Real-time** | WebSockets via SignalR Hub       |

## 📁 Project Structure

```
frontend/   React + Vite client
backend/
  LF10.Api/               ASP.NET Core Web API + SignalR hub (connects frontend and backend)
  HarassmentFilter.Core/  Filtering logic, loads its words from LF10.Data
  LF10.Data/              Database: DbContext, tables (Entities/), EF Core migrations
e2e/        Playwright end-to-end tests (frontend + backend + MySQL)
```

## 🚀 Getting Started

### Backend

The backend needs a MySQL server. The connection string is in
`backend/LF10.Api/appsettings.json` (database/user/password `hugyou` on
`localhost:3306`). The quickest way to get a matching server is Docker:

```bash
docker run -d --name hugyou-db -p 3306:3306 \
  -e MYSQL_DATABASE=hugyou -e MYSQL_USER=hugyou -e MYSQL_PASSWORD=hugyou \
  -e MYSQL_ROOT_PASSWORD=root mysql:8.4
```

Then:

```bash
cd backend/LF10.Api
dotnet run
```

Runs on `https://localhost:7255`. On startup it creates the tables and fills in
the filter rules if the database is still empty.

The tests (`dotnet test LF10.Api.slnx`) don't need MySQL.

To change the database schema, edit the entities in `LF10.Data/Entities/` and add a
migration (`dotnet tool restore` once, then from `backend/`):

```bash
dotnet ef migrations add <Name> --project LF10.Data --startup-project LF10.Api --output-dir Migrations
```

### Frontend

```bash
cd frontend
npm install
npm run dev
```

Runs on `http://localhost:5173` — proxies API calls and the SignalR connection to the backend automatically.

### E2E Tests

Playwright tests in `e2e/` drive the real app in a browser: login, posting, the
review panel and two users chatting. They need the MySQL server from above;
Playwright starts the backend (`dotnet run`) and the frontend (`npm run dev`) itself,
or reuses them if they are already running.

```bash
cd e2e
npm install
npx playwright install chromium   # once
npm test                          # or: npm run test:ui
```

They also run on GitHub Actions (`.github/workflows/e2e.yml`) for pushes to
`main`/`Backend` and for pull requests.

## 💬 How It Works

1. Type a message and hit **Send**.
2. It's checked against the harassment filter.
3. ✅ Clean → posted instantly, in real time, to everyone in the room.
4. 🚫 Flagged → a review panel opens with a suggested rewrite — accept it or edit your own wording, then post.

---

Built as a school project. 🎓
