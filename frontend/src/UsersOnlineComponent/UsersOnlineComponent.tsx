import UserComponent from "../UserComponent/UserComponent";

interface UsersOnlineComponentProps {
  channelName: string;
  connectedUsers: string[];
  username: string;
}

export default function UsersOnlineComponent({
  channelName,
  connectedUsers,
  username,
}: UsersOnlineComponentProps) {
  return (
    <aside className="chatroom-sidebar">
      <div className="channel-header">
        <span className="channel-hash">#</span>
        <span className="channel-name">{channelName}</span>
      </div>

      <div className="online-list">
        <span className="online-list-title">Online Now</span>
        {/* The current user is shown separately at the bottom of the sidebar. */}
        {connectedUsers
          .filter((user) => user !== username)
          .map((user) => (
            <UserComponent key={user} username={user} status={"online"} />
          ))}
      </div>

      <div className="current-user">
        <UserComponent username={username} status="online" />
      </div>
    </aside>
  );
}
