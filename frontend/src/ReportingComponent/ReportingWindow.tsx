import { useEffect, useRef, useState } from "react";
import "./ReportingWindow.css";

interface ReportingWindowProps {
  message: {
    username: string;
    text: string;
    timestamp: string;
  };
  reporterUsername: string;
  onClose: () => void;
}

// Values that are considered harassment types for reporting purposes.
const harassmentTypes = [
  "Bullying",
  "Insult",
  "Threat",
  "Racism",
  "Discrimination",
  "HateSpeech",
  "SexualHarassment",
  "SexualContent",
  "Violence",
  "Profanity",
  "Stalking",
  "PersonalAttack",
  "Spam",
];

export default function ReportingWindow({
  message,
  reporterUsername,
  onClose,
}: ReportingWindowProps) {
  const dialogRef = useRef<HTMLDialogElement>(null);
  const [reason, setReason] = useState("");

  useEffect(() => {
    const dialog = dialogRef.current;
    // Use the browser's modal behavior and close it when this window is removed.
    dialog?.showModal();
    return () => dialog?.close();
  }, []);

  return (
    <dialog
      ref={dialogRef}
      className="report-dialog"
      aria-labelledby="report-title"
      onCancel={(event) => {
        event.preventDefault();
        onClose();
      }}
    >
      <h3 id="report-title" className="report-title">
        Report message
      </h3>
      <p className="report-hint">You are reporting this message:</p>
      <div className="report-message">
        <div className="report-message-meta">
          <strong>{message.username}</strong>
          <span>{message.timestamp}</span>
        </div>
        <p>{message.text}</p>
      </div>
      <form
        onSubmit={(event) => {
          event.preventDefault();
          console.log(`message "${message.text}" was reported`);
          onClose();
        }}
      >
        <label className="report-label" htmlFor="report-reason">
          Based on
        </label>
        <select
          id="report-reason"
          className="report-select"
          value={reason}
          onChange={(event) => setReason(event.target.value)}
          required
        >
          <option value="" disabled>
            Select a reason
          </option>
          {harassmentTypes.map((type) => (
            <option key={type} value={type}>
              {type.replace(/([a-z])([A-Z])/g, "$1 $2")}
            </option>
          ))}
        </select>
        <p className="report-reporter">
          Reporting as <strong>{reporterUsername}</strong>
        </p>
        <p className="report-unavailable">
          Reports are only logged in the browser console and are not sent.
        </p>
        <div className="report-actions">
          <button type="button" className="review-cancel" onClick={onClose}>
            Cancel
          </button>
          <button type="submit" className="review-post" disabled={!reason}>
            Submit report
          </button>
        </div>
      </form>
    </dialog>
  );
}
