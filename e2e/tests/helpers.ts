import type { Page } from "@playwright/test";

// Six random lowercase letters, so names and messages from other tests or
// earlier runs never match. No digits: the filter reads them as leetspeak.
export function uniqueTag() {
  return Array.from({ length: 6 }, () =>
    String.fromCharCode(97 + Math.floor(Math.random() * 26)),
  ).join("");
}

// Logs in and waits until the hub has answered JoinRoom. The page shows no sign
// of the connection, and a message sent before that would be lost.
export async function joinChat(page: Page, username: string) {
  await page.goto("/");

  const joined = page
    .waitForEvent("websocket", (socket) => socket.url().includes("/chathub"))
    .then((socket) =>
      socket.waitForEvent("framereceived", (frame) =>
        String(frame.payload).includes('"UserListUpdated"'),
      ),
    );

  await page.getByPlaceholder("Username").fill(username);
  await page.getByRole("button", { name: "Enter the chatroom" }).click();
  await joined;
}

export async function sendMessage(page: Page, text: string) {
  await page.getByPlaceholder("Send a message...").fill(text);
  await page.getByRole("button", { name: "Send", exact: true }).click();
}

export const messageList = (page: Page) => page.locator(".message-list");

export const onlineList = (page: Page) => page.locator(".online-list");

export const reviewPanel = (page: Page) => page.locator(".review-panel");
