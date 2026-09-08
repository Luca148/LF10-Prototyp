import { useState } from "react";
import "./LoginComponent.css";

interface LoginComponentProps {
  onLogin: (username: string) => void;
}

function LoginComponent({ onLogin }: LoginComponentProps) {
  const [username, setUsername] = useState("");

  const handleSubmit = (event: React.FormEvent) => {
    event.preventDefault();
    const trimmed = username.trim();
    if (trimmed) {
      onLogin(trimmed);
    }
  };

  return (
    <div className="login-page">
      <form className="login-card" onSubmit={handleSubmit}>
        <h1 className="login-title">Welcome</h1>
        <p className="login-subtitle">Enter a username to join the chatroom</p>

        <input
          type="text"
          className="login-input"
          placeholder="Username"
          value={username}
          onChange={(event) => setUsername(event.target.value)}
          autoFocus
        />

        <button
          type="submit"
          className="login-button"
          disabled={!username.trim()}
        >
          Enter the chatroom
        </button>
      </form>
    </div>
  );
}

export default LoginComponent;
