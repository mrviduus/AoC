using System;                // Basic system functions
using System.IO;             // For reading files

public class Program
{
    public static void Main()
    {
        // Read the entire file content into one string.
        // The file "input.txt" must be located next to your executable.
        string? fileContent = File.ReadAllText("input.txt");

        // If the file is empty or whitespace, nothing to process → return 0
        if (string.IsNullOrWhiteSpace(fileContent))
        {
            Console.WriteLine(0);
            return;
        }

        // Split by commas to get each range (e.g. "12077-25471")
        string[] ranges = fileContent.Split(',', StringSplitOptions.RemoveEmptyEntries);

        // Sum of all invalid IDs
        long totalSum = 0;

        // Process each range one by one
        foreach (string rawRange in ranges)
        {
            // Trim any surrounding whitespace
            string range = rawRange.Trim();

            // Skip empty tokens (just in case)
            if (range.Length == 0)
                continue;

            // Split "start-end" into two parts
            string[] parts = range.Split('-', StringSplitOptions.RemoveEmptyEntries);

            // Must have exactly 2 numbers: start and end
            if (parts.Length != 2)
                continue;

            // Parse start and end of the interval
            long start = long.Parse(parts[0]);
            long end = long.Parse(parts[1]);

            // Iterate through each ID in the interval
            for (long id = start; id <= end; id++)
            {
                // Check whether this id consists of two repeated halves
                if (IsInvalidId(id))
                {
                    totalSum += id; // Add to the final sum
                }
            }
        }

        // Output the result
        Console.WriteLine(totalSum);
    }

    // Determines whether a given number is "invalid":
    // invalid means it is exactly two copies of the same digit sequence.
    private static bool IsInvalidId(long id)
    {
        // Convert number to string for easier inspection
        string s = id.ToString();

        // If length is odd → cannot be evenly split → automatically valid
        if (s.Length % 2 != 0)
            return false;

        // Calculate half point of the string
        int half = s.Length / 2;

        // Compare first half vs second half
        for (int i = 0; i < half; i++)
        {
            if (s[i] != s[i + half])
                return false; // Not identical → not invalid
        }

        // All characters matched: number is "XX" pattern
        return true;
    }
}

// ==========================================
// BIG-O ANALYSIS
// Let N be the total number of IDs across all ranges.
// Let D be the max digit length of any ID.
// For each ID, checking the halves costs O(D).
// Since D is tiny (≤ 10 digits), the total is effectively:
//
// Time:  O(N)
// Space: O(1) — no additional data structures.
// ==========================================