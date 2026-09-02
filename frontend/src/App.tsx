import { useEffect } from "react";
import LoginComponent from "./Login/LoginComponent";
import "./App.css";

function App() {
  useEffect(() => {
    fetch("/api/WeatherForecast")
      .then((response) => response.json())
      .then((data) => console.log(data));
  }, []);

  return (
    <>
      <LoginComponent onLogin={(username) => console.log(username)} />
    </>
  );
}

export default App;
