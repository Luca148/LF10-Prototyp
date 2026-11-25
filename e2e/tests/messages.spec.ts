import { expect, test } from "@playwright/test";
import {
  joinChat,
  messageList,
  reviewPanel,
  sendMessage,
  uniqueTag,
} from "./helpers";

// These rely on the seeded filter rules: "idiot" -> "genius" and
// "stupid" -> "wise", both in the Bullying category.

test.beforeEach(async ({ page }) => {
  await joinChat(page, `Alice ${uniqueTag()}`);
});

test("a clean message is posted unchanged", async ({ page }) => {
  const text = `have a nice day ${uniqueTag()}`;
  await sendMessage(page, text);

  await expect(
    messageList(page).getByText(text, { exact: true }),
  ).toBeVisible();
  await expect(page.getByPlaceholder("Send a message...")).toHaveValue("");
  await expect(reviewPanel(page)).toBeHidden();
});

test("a flagged message is held back until the suggestion is posted", async ({
  page,
}) => {
  const tag = uniqueTag();
  await sendMessage(page, `you idiot ${tag}`);

  const review = reviewPanel(page);
  await expect(review.getByRole("heading")).toHaveText(
    "This message may contain Bullying",
  );
  await expect(review.getByRole("textbox")).toHaveValue(`you genius ${tag}`);
  await expect(messageList(page).getByText(tag)).toHaveCount(0);

  await review.getByRole("button", { name: "Post", exact: true }).click();

  await expect(review).toBeHidden();
  await expect(
    messageList(page).getByText(`you genius ${tag}`, { exact: true }),
  ).toBeVisible();
  await expect(messageList(page).getByText(tag)).toHaveCount(1);
});

test("cancelling the review posts nothing", async ({ page }) => {
  const tag = uniqueTag();
  await sendMessage(page, `you idiot ${tag}`);

  const review = reviewPanel(page);
  await review.getByRole("button", { name: "Cancel" }).click();
  await expect(review).toBeHidden();

  // Messages arrive in order, so once this one is shown the cancelled one
  // would be there too if it had been sent.
  const followUp = `all good ${tag}`;
  await sendMessage(page, followUp);
  await expect(
    messageList(page).getByText(followUp, { exact: true }),
  ).toBeVisible();
  await expect(messageList(page).getByText(tag)).toHaveCount(1);
});

test("a rewrite that is still offensive goes back to review", async ({
  page,
}) => {
  const tag = uniqueTag();
  await sendMessage(page, `you idiot ${tag}`);

  const review = reviewPanel(page);
  const textbox = review.getByRole("textbox");
  const post = review.getByRole("button", { name: "Post", exact: true });

  await textbox.fill(`you stupid ${tag}`);
  await post.click();

  await expect(textbox).toHaveValue(`you wise ${tag}`);
  await expect(messageList(page).getByText(tag)).toHaveCount(0);

  await post.click();

  await expect(review).toBeHidden();
  await expect(
    messageList(page).getByText(`you wise ${tag}`, { exact: true }),
  ).toBeVisible();
  await expect(messageList(page).getByText(tag)).toHaveCount(1);
});
