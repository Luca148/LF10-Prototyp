import { expect, test } from "@playwright/test";
import { uniqueTag } from "./helpers";

test("logging in opens the chatroom", async ({ page }) => {
  const username = `Alice ${uniqueTag()}`;
  await page.goto("/");

  const enter = page.getByRole("button", { name: "Enter the chatroom" });
  await expect(enter).toBeDisabled();

  await page.getByPlaceholder("Username").fill(`  ${username}  `);
  await enter.click();

  await expect(page.locator(".current-user")).toContainText(username);
  await expect(page.getByPlaceholder("Send a message...")).toBeVisible();
});
