import { expect, test } from "@playwright/test";
import { joinChat, onlineList, sendMessage, uniqueTag } from "./helpers";

test("two users see each other and each other's messages", async ({
  browser,
}) => {
  const alice = `Alice ${uniqueTag()}`;
  const bob = `Bob ${uniqueTag()}`;

  // Separate contexts, so each user has their own page and hub connection.
  const aliceContext = await browser.newContext();
  const bobContext = await browser.newContext();
  const alicePage = await aliceContext.newPage();
  const bobPage = await bobContext.newPage();

  await joinChat(alicePage, alice);
  await joinChat(bobPage, bob);

  await expect(onlineList(alicePage)).toContainText(bob);
  await expect(onlineList(bobPage)).toContainText(alice);

  const text = `hello ${uniqueTag()}`;
  await sendMessage(alicePage, text);

  const received = bobPage.locator(".message").filter({ hasText: text });
  await expect(received.locator(".message-author")).toHaveText(alice);

  // Leaving the chat removes the user from the other one's list.
  await aliceContext.close();
  await expect(onlineList(bobPage)).not.toContainText(alice);

  await bobContext.close();
});
