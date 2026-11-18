# Deployment

The app runs as three containers, wired together by `docker-compose.yml`:

| Service      | Image base                             | Role                                                                            |
|--------------|----------------------------------------|--------------------------------------------------------------------------------|
| `backend`    | `mcr.microsoft.com/dotnet/aspnet:10.0` | ASP.NET Core API + SignalR hub, listens on `:8080` (internal only)              |
| `frontend`   | `nginx:1.27-alpine`                    | Serves the built React app, reverse-proxies `/api` and `/chathub` to `backend` |
| `cloudflared`| `cloudflare/cloudflared`              | Outbound-only Cloudflare Tunnel connector; publishes the site on the subdomain |

No ports are opened on the host. `cloudflared` dials out to Cloudflare and routes
the subdomain to `http://frontend:80` over the internal Docker network; nginx then
splits traffic between the static app, `/api`, and the `/chathub` WebSocket. So
there is no CORS setup, no firewall/port-forwarding on the router, and the backend
is never reachable from outside.

## One-time Cloudflare setup

1. Cloudflare **Zero Trust** dashboard → **Networks → Tunnels → Create a tunnel** →
   type **Cloudflared** → name it → **Save**.
2. On the "Install and run a connector" screen, copy the token — the long string
   right after `--token` in the shown command.
3. **Public Hostnames** tab → **Add a public hostname**:
   - **Subdomain / Domain**: your subdomain
   - **Type**: `HTTP`
   - **URL**: `frontend:80`
   - Save. (Nothing else — the single route covers `/`, `/api/*` and `/chathub`,
     because nginx inside the `frontend` container does the path routing.)

   Cloudflare creates the DNS `CNAME` for the subdomain automatically.

WebSockets pass through a Cloudflare Tunnel with no extra configuration, so
SignalR works as-is.

## Run it on the server

Prerequisites: Docker Engine + the Compose plugin.

```bash
git clone <repo-url>
cd LF10-Prototyp
cp .env.example .env          # then paste the tunnel token into .env
docker compose up -d --build
```

The app is then live at `https://<your-subdomain>/` (TLS handled by Cloudflare).

Useful commands:

```bash
docker compose logs -f                 # tail logs (all services)
docker compose logs -f cloudflared     # tunnel connection status
docker compose up -d --build           # rebuild + redeploy after a git pull
docker compose down                     # stop (keeps the data volume)
docker compose down -v                  # stop and delete the data volume (wipes users)
```

## Persistent data

Registered users live in `Data/User.json` inside the `backend-data` named volume.
It survives `down` / `up` and redeploys. `Data/Harassment.json` (the filter rules)
is refreshed from the image on every container start, so editing that file and
redeploying is enough to update the rules.

## Notes

- `app.UseHttpsRedirection()` in `Program.cs` does nothing here (no HTTPS port
  inside the container) beyond one startup warning — harmless, since Cloudflare
  terminates TLS. Remove the line if the warning bothers you.
- For direct LAN access to the app without going through Cloudflare, add
  `ports: ["8080:80"]` to the `frontend` service.
