import { useState } from "react";
import LoginComponent from "./Login/LoginComponent";
import Chatroom from "./ChatRoom/Chatroom";
import "./App.css";

function App() {
  const [username, setUsername] = useState<string | null>(null);

  if (!username) {
    return <LoginComponent onLogin={setUsername} />;
  }

  return <Chatroom username={username} />;
}

export default App;
