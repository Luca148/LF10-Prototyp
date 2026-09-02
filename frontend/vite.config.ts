import react from "@vitejs/plugin-react";
import { defineConfig } from "vite";

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      // Forwards /api/* calls to the ASP.NET Core backend, avoiding CORS in dev.
      "/api": {
        target: "https://localhost:7255",
        changeOrigin: true,
        secure: false, // backend uses a self-signed dev certificate
      },
    },
  },
});
