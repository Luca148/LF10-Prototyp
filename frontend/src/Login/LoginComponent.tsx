import { useEffect, useState } from "react";
import "./LoginComponent.css";
import hugLogo from "/hug.png";
import { FilterMessageController } from "../Controllers/FilterController";

interface LoginComponentProps {
  onLogin: (username: string) => void;
}

function LoginComponent({ onLogin }: LoginComponentProps) {
  const [username, setUsername] = useState("");
  const [isHarassment, setIsHarassment] = useState(true);
  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    // Whitespace should not change how a username is checked or submitted.
    const trimmed = username.trim();
    let filteredName;

    try {
      filteredName = await FilterMessageController(trimmed);
      // Only let the user join when the filter finds no harassment.
      if (!filteredName.harassmentTypes.includes("None")) {
        setIsHarassment(true);
        console.log("Harassment detected");
      } else {
        setIsHarassment(false);
        onLogin(trimmed);
      }
    } catch (error) {
      console.error(error);
    }
  };

  useEffect(() => {
    // Clear the previous warning as soon as the username is edited.
    setIsHarassment(false);
  }, [username]);

  return (
    <div className="login-page">
      <div className="login-brand">
        <img src={hugLogo} alt="Hug you! logo" className="login-logo" />
        <span className="login-brand-name">HugYou!</span>
      </div>

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
        {isHarassment && <p className="login-error">Username not allowed</p>}
        <button
          type="submit"
          className="login-button"
          disabled={!username.trim() || isHarassment}
        >
          Enter the chatroom
        </button>
      </form>
    </div>
  );
}

export default LoginComponent;
