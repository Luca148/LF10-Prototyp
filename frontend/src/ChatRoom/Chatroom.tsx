import { useEffect, useRef, useState } from "react";
import "./Chatroom.css";
import { getAvatarColor } from "../UserComponent/UserComponent";

import { FilterMessageController } from "../Controllers/FilterController";
import { HubConnectionBuilder, HubConnectionState } from "@microsoft/signalr";
import ReviewComponent from "../ReviewComponent/ReviewComponent.tsx";
import UsersOnlineComponent from "../UsersOnlineComponent/UsersOnlineComponent.tsx";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faEllipsis } from "@fortawesome/free-solid-svg-icons";
interface ChatMessage {
  id: number;
  username: string;
  text: string;
  timestamp: string;
}

interface ChatroomProps {
  username: string;
}

interface PendingReview {
  original: string;
  suggestion: string;
  harassmentTypes: string[];
}

// Built once when this module first loads, so React's Strict Mode double-render
// never creates or races multiple connections.
const connection = new HubConnectionBuilder().withUrl("/chathub").build();

function Chatroom({ username }: ChatroomProps) {
  const channelName = "general";
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [draft, setDraft] = useState("");
  const [isChecking, setIsChecking] = useState(false);
  const [pendingReview, setPendingReview] = useState<PendingReview | null>(
    null,
  );
  const [reviewText, setReviewText] = useState("");
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const [connectedUsers, setConnectedUsers] = useState<string[]>([]);
  const messageInputRef = useRef<HTMLInputElement>(null);
  const [selectedMessageId, setSelectedMessageId] = useState<number | null>(
    null,
  );
  useEffect(() => {
    if (!isChecking && !pendingReview) {
      messageInputRef.current?.focus();
    }
  }, [isChecking, pendingReview]);

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
    connection.on("UserListUpdated", (users: string[]) => {
      setConnectedUsers(users);
    });

    if (connection.state === HubConnectionState.Disconnected) {
      connection
        .start()
        .then(() => connection.invoke("JoinRoom", 1, username))
        .catch((error) => console.error(error));
    }

    return () => {
      connection.off("ReceiveMessage", handleReceiveMessage);
      connection.off("UserListUpdated");
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
      <UsersOnlineComponent
        channelName={channelName}
        connectedUsers={connectedUsers}
        username={username}
      />
      <main className="chatroom-main">
        <header className="chatroom-header">
          <span className="channel-hash">#</span>
          <span className="channel-name">{channelName}</span>
        </header>

        <div className="message-list">
          {messages.map((message) => (
            <div className="message" key={message.id}>
              <div
                className="message-avatar"
                style={{
                  background: getAvatarColor(message.username),
                }}
                onClick={() => {
                  console.log(
                    message.username,
                    message.timestamp,
                    message.text,
                  );
                }}
              >
                {message.username.charAt(0).toUpperCase()}
              </div>
              <div className="message-body">
                <div className="message-meta">
                  <span className="message-author">{message.username}</span>
                  <span className="message-time">{message.timestamp}</span>
                  <button
                    type="button"
                    aria-label="Message options"
                    style={{
                      backgroundColor: "transparent",
                      border: "none",
                      cursor: "pointer",
                    }}
                    onClick={() =>
                      setSelectedMessageId((current) =>
                        current === message.id ? null : message.id,
                      )
                    }
                  >
                    <FontAwesomeIcon icon={faEllipsis} />
                  </button>

                  {selectedMessageId == message.id && (
                    <div
                      style={{
                        fontSize: "12px",
                        padding: "0px 10px 0px 10px",
                        borderRadius: "100px",
                        backgroundColor: "#334155",
                        cursor: "pointer",
                      }}
                      onClick={() => {
                        console.log(message.id, message.text, message.username);
                      }}
                    >
                      <span>Report Message</span>
                    </div>
                  )}
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
            ref={messageInputRef}
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
          <ReviewComponent
            pendingReview={pendingReview}
            reviewText={reviewText}
            isChecking={isChecking}
            setReviewText={setReviewText}
            handleUseSuggestion={handleUseSuggestion}
            handleCancelReview={handleCancelReview}
            handleConfirmPost={handleConfirmPost}
          />
        )}
      </main>
    </div>
  );
}

export default Chatroom;
