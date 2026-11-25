interface ReviewComponentProps {
  pendingReview: {
    suggestion: string;
    harassmentTypes: string[];
  };
  reviewText: string;
  isChecking: boolean;
  setReviewText: (text: string) => void;
  handleUseSuggestion: () => void;
  handleCancelReview: () => void;
  handleConfirmPost: () => void;
}

// "SexualHarassment" -> "Sexual Harassment"
const formatHarassmentType = (type: string) =>
  type.replace(/([a-z])([A-Z])/g, "$1 $2");

export default function ReviewComponent({
  pendingReview,
  reviewText,
  isChecking,
  setReviewText,
  handleUseSuggestion,
  handleCancelReview,
  handleConfirmPost,
}: ReviewComponentProps) {
  return (
    <div>
      {pendingReview && (
        <div className="review-overlay">
          <div className="review-panel">
            <h3 className="review-title">
              This message may contain{" "}
              {pendingReview.harassmentTypes
                .map(formatHarassmentType)
                .join(", ")}
            </h3>
            <p className="review-hint">
              Your original message can't be posted as is. Use the suggested
              version below or rewrite it yourself, then post again.
            </p>

            <textarea
              className="review-textarea"
              value={reviewText}
              onChange={(event) => setReviewText(event.target.value)}
              rows={3}
            />

            <div className="review-choices">
              <button
                type="button"
                className="review-choice-btn"
                onClick={handleUseSuggestion}
                disabled={reviewText === pendingReview.suggestion}
              >
                Reset to suggestion
              </button>
            </div>

            <div className="review-actions">
              <button
                type="button"
                className="review-cancel"
                onClick={handleCancelReview}
              >
                Cancel
              </button>
              <button
                type="button"
                className="review-post"
                disabled={!reviewText.trim() || isChecking}
                onClick={handleConfirmPost}
              >
                {isChecking ? "Checking..." : "Post"}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
