import "./UserComponent.css";

export type UserStatus = "online" | "offline";

interface UserComponentProps {
  username: string;
  status: UserStatus;
}

const AVATAR_COLORS = [
  "#6366f1", // indigo
  "#06b6d4", // cyan
  "#22c55e", // green
  "#f97316", // orange
  "#ec4899", // pink
];

// Same username always maps to the same color, so it doesn't shuffle on re-render.
export function getAvatarColor(username: string) {
  const hash = username
    .trim()
    .toLowerCase()
    .split("")
    .reduce((sum, char) => sum + char.charCodeAt(0), 0);

  return AVATAR_COLORS[hash % AVATAR_COLORS.length];
}

function UserComponent({ username, status }: UserComponentProps) {
  const initial = username.trim().charAt(0).toUpperCase();
  const avatarColor = getAvatarColor(username);

  return (
    <div className="user">
      <div className="user-avatar" style={{ background: avatarColor }}>
        {initial}
        <span className={`status-dot ${status}`} />
      </div>
      <div className="user-info">
        <span className="user-name">{username}</span>
        <span className={`user-status ${status}`}>{status}</span>
      </div>
    </div>
  );
}

export default UserComponent;
