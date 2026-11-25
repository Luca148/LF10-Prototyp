import { defineConfig, devices } from "@playwright/test";

// Runs against the dev setup: the backend via `dotnet run` and the frontend via
// Vite, which proxies /api and /chathub to the backend. MySQL has to be running
// already (see README). Locally, servers that are already running get reused.
export default defineConfig({
  testDir: "./tests",
  // All tests share chat room 1, so they run one after another.
  workers: 1,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 1 : 0,
  reporter: process.env.CI ? [["list"], ["html", { open: "never" }]] : "list",
  use: {
    baseURL: "http://localhost:5173",
    trace: "retain-on-failure",
  },
  projects: [{ name: "chromium", use: { ...devices["Desktop Chrome"] } }],
  webServer: [
    {
      command: "dotnet run --launch-profile https",
      cwd: "../backend/LF10.Api",
      // Only mapped in Development, which the https launch profile sets.
      url: "https://localhost:7255/openapi/v1.json",
      ignoreHTTPSErrors: true,
      reuseExistingServer: !process.env.CI,
      // Includes building the backend and migrating the database.
      timeout: 180_000,
    },
    {
      command: "npm run dev -- --strictPort",
      cwd: "../frontend",
      url: "http://localhost:5173",
      reuseExistingServer: !process.env.CI,
    },
  ],
});
