import { useState } from "react";
import ReportingWindow from "./ReportingWindow";

interface ReportingComponentProps {
  message: {
    id: number;
    username: string;
    text: string;
    timestamp: string;
  };
  reporterUsername: string;
}

export default function ReportingComponent({
  message,
  reporterUsername,
}: ReportingComponentProps) {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <>
      <button
        type="button"
        className="report-trigger"
        onClick={() => setIsOpen(true)}
      >
        Report Message
      </button>
      {isOpen && (
        <ReportingWindow
          message={message}
          reporterUsername={reporterUsername}
          onClose={() => setIsOpen(false)}
        />
      )}
    </>
  );
}
