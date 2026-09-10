export interface FilterResponse {
  message: string;
  wasModified: boolean;
  harassmentTypes: string[];
  timestamp: string;
}

export const FilterMessageController = async (
  input: string,
): Promise<FilterResponse> => {
  const response = await fetch("/api/Messages/filter", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(input),
  });

  if (!response.ok) {
    throw new Error(`Request failed: ${response.status}`);
  }

  return response.json();
};
