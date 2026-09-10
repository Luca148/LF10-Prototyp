import { useEffect, useRef, useState } from "react";
import "./Chatroom.css";
import UserComponent, {
  getAvatarColor,
  type UserStatus,
} from "../UserComponent/UserComponent";

import { FilterMessageController } from "../Controllers/FilterController";
import { HubConnectionBuilder, HubConnectionState } from "@microsoft/signalr";

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

interface PendingReview {
  original: string;
  suggestion: string;
  harassmentTypes: string[];
}

const onlineUsers: OnlineUser[] = [
  { username: "Lukas", status: "online" },
  { username: "Permata", status: "online" },
  { username: "Jan-Luca", status: "online" },
  { username: "Mariam", status: "offline" },
  { username: "Kevin", status: "online" },
  { username: "Veeti", status: "online" },
];

// "SexualHarassment" -> "Sexual Harassment"
const formatHarassmentType = (type: string) =>
  type.replace(/([a-z])([A-Z])/g, "$1 $2");

// Built once when this module first loads, so React's Strict Mode double-render
// never creates or races multiple connections.
const connection = new HubConnectionBuilder().withUrl("/chathub").build();

function Chatroom({ username }: ChatroomProps) {
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [draft, setDraft] = useState("");
  const [isChecking, setIsChecking] = useState(false);
  const [pendingReview, setPendingReview] = useState<PendingReview | null>(
    null,
  );
  const [reviewText, setReviewText] = useState("");
  const messagesEndRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handleReceiveMessage = (result: {
      message: string;
      timestamp: string;
      username: string;
    }) => {
      const newMessage: ChatMessage = {
        id: Date.now(),
        username: result.username,
        text: result.message,
        timestamp: new Date(result.timestamp).toLocaleTimeString([], {
          hour: "2-digit",
          minute: "2-digit",
        }),
      };
      setMessages((prev) => [...prev, newMessage]);
    };

    connection.on("ReceiveMessage", handleReceiveMessage);

    if (connection.state === HubConnectionState.Disconnected) {
      connection
        .start()
        .then(() => connection.invoke("JoinRoom", 1))
        .catch((error) => console.error(error));
    }

    return () => {
      connection.off("ReceiveMessage", handleReceiveMessage);
    };
  }, []);

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  const postMessage = (text: string) => {
    connection.invoke("SendMessage", 1, text, username).catch((error) => {
      console.error(error);
    });
  };

  const handleSend = async (event: React.FormEvent) => {
    event.preventDefault();
    const text = draft.trim();
    if (!text || isChecking) return;

    setIsChecking(true);
    let filterResult;
    try {
      filterResult = await FilterMessageController(text);
    } catch (error) {
      console.error("Harassment filter check failed", error);
      setIsChecking(false);
      return;
    }
    setIsChecking(false);

    const flaggedTypes = (filterResult.harassmentTypes ?? []).filter(
      (type) => type.toLowerCase() !== "none",
    );

    if (flaggedTypes.length === 0) {
      postMessage(filterResult.message);
      setDraft("");
      return;
    }

    // Hold the message back until the user reviews the flagged content.
    setPendingReview({
      original: text,
      suggestion: filterResult.message,
      harassmentTypes: flaggedTypes,
    });
    setReviewText(filterResult.message);
  };

  const handleUseSuggestion = () => {
    if (!pendingReview) return;
    setReviewText(pendingReview.suggestion);
  };

  const handleCancelReview = () => {
    setPendingReview(null);
    setReviewText("");
  };

  const handleConfirmPost = async () => {
    const finalText = reviewText.trim();
    if (!finalText || isChecking) return;

    // Re-check the edited text so flagged wording can't slip through unchanged.
    setIsChecking(true);
    let filterResult;
    try {
      filterResult = await FilterMessageController(finalText);
    } catch (error) {
      console.error("Harassment filter check failed", error);
      setIsChecking(false);
      return;
    }
    setIsChecking(false);

    const flaggedTypes = (filterResult.harassmentTypes ?? []).filter(
      (type) => type.toLowerCase() !== "none",
    );

    if (flaggedTypes.length > 0) {
      setPendingReview({
        original: finalText,
        suggestion: filterResult.message,
        harassmentTypes: flaggedTypes,
      });
      setReviewText(filterResult.message);
      return;
    }

    postMessage(filterResult.message);
    setDraft("");
    setPendingReview(null);
    setReviewText("");
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
            disabled={isChecking || !!pendingReview}
          />
          <button
            type="submit"
            className="message-send"
            disabled={!draft.trim() || isChecking || !!pendingReview}
          >
            {isChecking ? "Checking..." : "Send"}
          </button>
        </form>

        {pendingReview && (
          <div className="review-overlay">
            <div className="review-panel">
              <h3 className="review-title">
                This message may contain{" "}
                {pendingReview.harassmentTypes
                  .map(formatHarassmentType)
                  .join(", ")}
              </h3>
              <p className="review-hint">
                Your original message can't be posted as is. Use the suggested
                version below or rewrite it yourself, then post again.
              </p>

              <textarea
                className="review-textarea"
                value={reviewText}
                onChange={(event) => setReviewText(event.target.value)}
                rows={3}
              />

              <div className="review-choices">
                <button
                  type="button"
                  className="review-choice-btn"
                  onClick={handleUseSuggestion}
                  disabled={reviewText === pendingReview.suggestion}
                >
                  Reset to suggestion
                </button>
              </div>

              <div className="review-actions">
                <button
                  type="button"
                  className="review-cancel"
                  onClick={handleCancelReview}
                >
                  Cancel
                </button>
                <button
                  type="button"
                  className="review-post"
                  disabled={!reviewText.trim() || isChecking}
                  onClick={handleConfirmPost}
                >
                  {isChecking ? "Checking..." : "Post"}
                </button>
              </div>
            </div>
          </div>
        )}
      </main>
    </div>
  );
}

export default Chatroom;
