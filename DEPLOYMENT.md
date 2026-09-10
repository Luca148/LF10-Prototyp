# Deployment

The app ships as two Docker images, wired together by `docker-compose.yml`:

| Service    | Image base                                | Role                                                          |
|------------|-------------------------------------------|--------------------------------------------------------------|
| `backend`  | `mcr.microsoft.com/dotnet/aspnet:10.0`    | ASP.NET Core API + SignalR hub, listens on `:8080` (internal) |
| `frontend` | `nginx:1.27-alpine`                       | Serves the built React app, reverse-proxies `/api` and `/chathub` to `backend` |

Only the `frontend` container publishes a port (`80`). The browser talks to nginx
only; nginx forwards API and WebSocket traffic to the backend over the internal
Docker network, so there is no CORS setup and the backend is never exposed directly.

## Run it on a server

Prerequisites: Docker Engine + the Compose plugin.

```bash
git clone <repo-url>
cd LF10-Prototyp
docker compose up -d --build
```

The app is then reachable at `http://<server-ip>/`.

Useful commands:

```bash
docker compose logs -f            # tail logs
docker compose up -d --build      # rebuild + redeploy after a git pull
docker compose down               # stop (keeps the data volume)
docker compose down -v            # stop and delete the data volume (wipes users)
```

## Persistent data

Registered users live in `Data/User.json` inside the `backend-data` named volume.
It survives `down` / `up` and redeploys. `Data/Harassment.json` (the filter rules)
is refreshed from the image on every container start, so editing that file and
redeploying is enough to update the rules.

## HTTPS

This setup serves plain HTTP on port 80. For a public deployment put a TLS
terminator in front, e.g.:

- **Caddy** or **Traefik** as an extra compose service (automatic Let's Encrypt), or
- a host nginx with certbot, or
- Cloudflare in front of the server.

The backend keeps `app.UseHttpsRedirection()` in `Program.cs`. With no HTTPS port
configured inside the container it does not redirect (just logs one startup
warning) — harmless behind a TLS-terminating proxy. Remove that line if the
warning bothers you.
