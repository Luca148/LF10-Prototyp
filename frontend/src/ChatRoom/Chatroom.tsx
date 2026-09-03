import { useEffect, useRef, useState } from "react";
import "./Chatroom.css";
import UserComponent, {
  getAvatarColor,
  type UserStatus,
} from "../UserComponent/UserComponent";

import { FilterMessageController } from "../Controllers/FilterController";

interface ChatMessage {
  id: number;
  username: string;
  text: string;
  timestamp: string;
}

interface OnlineUser {
  username: string;
  status: UserStatus;
}

interface ChatroomProps {
  username: string;
}

// Placeholder data until SignalR is wired up.
const initialMessages: ChatMessage[] = [
  {
    id: 1,
    username: "Maya",
    text: "Hi folks! Welcome to General 👋",
    timestamp: "10:24",
  },
  {
    id: 2,
    username: "Zed",
    text: "Just joined the chat, hello everyone!",
    timestamp: "10:26",
  },
  {
    id: 3,
    username: "Luna",
    text: "Hey Zed, glad you're here 🙌",
    timestamp: "10:28",
  },
];

const onlineUsers: OnlineUser[] = [
  { username: "Lukas", status: "online" },
  { username: "Permata", status: "online" },
  { username: "Jan-Luca", status: "online" },
  { username: "Mariam", status: "offline" },
  { username: "Kevin", status: "online" },
  { username: "Veeti", status: "online" },
];

function Chatroom({ username }: ChatroomProps) {
  const [messages, setMessages] = useState<ChatMessage[]>(initialMessages);
  const [draft, setDraft] = useState("");
  const messagesEndRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  const handleSend = async (event: React.FormEvent) => {
    event.preventDefault();
    const text = draft.trim();
    if (!text) return;

    let filterResult;
    try {
      filterResult = await FilterMessageController(text);
    } catch (error) {
      console.error("Harassment filter check failed", error);
      return;
    }

    const newMessage: ChatMessage = {
      id: Date.now(),
      username,
      text: filterResult.message,
      timestamp: new Date(filterResult.timestamp).toLocaleTimeString([], {
        hour: "2-digit",
        minute: "2-digit",
      }),
    };

    setMessages((previous) => [...previous, newMessage]);
    setDraft("");
  };

  return (
    <div className="chatroom">
      <aside className="chatroom-sidebar">
        <div className="channel-header">
          <span className="channel-hash">#</span>
          <span className="channel-name">general</span>
        </div>

        <div className="online-list">
          <span className="online-list-title">Online Now</span>
          {onlineUsers.map((user) => (
            <UserComponent
              key={user.username}
              username={user.username}
              status={user.status}
            />
          ))}
        </div>

        <div className="current-user">
          <UserComponent username={username} status="online" />
        </div>
      </aside>

      <main className="chatroom-main">
        <header className="chatroom-header">
          <span className="channel-hash">#</span>
          <span className="channel-name">general</span>
        </header>

        <div className="message-list">
          {messages.map((message) => (
            <div className="message" key={message.id}>
              <div
                className="message-avatar"
                style={{ background: getAvatarColor(message.username) }}
              >
                {message.username.charAt(0).toUpperCase()}
              </div>
              <div className="message-body">
                <div className="message-meta">
                  <span className="message-author">{message.username}</span>
                  <span className="message-time">{message.timestamp}</span>
                </div>
                <p className="message-text">{message.text}</p>
              </div>
            </div>
          ))}
          <div ref={messagesEndRef} />
        </div>

        <form className="message-form" onSubmit={handleSend}>
          <input
            type="text"
            className="message-input"
            placeholder="Send a message..."
            value={draft}
            onChange={(event) => setDraft(event.target.value)}
          />
          <button
            type="submit"
            className="message-send"
            disabled={!draft.trim()}
          >
            Send
          </button>
        </form>
      </main>
    </div>
  );
}

export default Chatroom;
