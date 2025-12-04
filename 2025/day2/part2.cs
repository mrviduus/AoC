using System;     // For basic console functionality
using System.IO;  // For reading the input file

public class Program
{
    public static void Main()
    {
        // Read the entire content of "input.txt" into a single string
        // The file should be in the same directory where the program runs
        string? fileContent = File.ReadAllText("input.txt");

        // If the file is empty or only whitespace, there is nothing to process
        if (string.IsNullOrWhiteSpace(fileContent))
        {
            // Print 0 as a result and exit
            Console.WriteLine(0);
            return;
        }

        // Split the content by commas to get individual ranges
        // Example token: "12077-25471"
        string[] ranges = fileContent.Split(',', StringSplitOptions.RemoveEmptyEntries);

        // This variable will store the sum of all invalid IDs
        long totalSum = 0;

        // Optional: if you want, you can also count how many invalid IDs we found
        long invalidCount = 0;

        // Iterate over each range string
        foreach (string rawRange in ranges)
        {
            // Remove any leading/trailing whitespace from the token
            string range = rawRange.Trim();

            // Skip empty tokens, just in case
            if (range.Length == 0)
                continue;

            // Split by '-' to separate start and end of the range
            string[] parts = range.Split('-', StringSplitOptions.RemoveEmptyEntries);

            // We expect exactly two parts: start and end
            if (parts.Length != 2)
                continue; // If format is wrong, skip this range

            // Parse the start of the range as a long
            long start = long.Parse(parts[0]);
            // Parse the end of the range as a long
            long end = long.Parse(parts[1]);

            // Loop through every ID in the inclusive range [start, end]
            for (long id = start; id <= end; id++)
            {
                // Check if this ID is invalid according to the new "repeated pattern" rule
                if (IsInvalidId(id))
                {
                    // Add this invalid ID to the total sum
                    totalSum += id;
                    // Increment the counter of invalid IDs
                    invalidCount++;
                }
            }
        }

        // Print the sum of all invalid IDs
        Console.WriteLine(totalSum);

        // If you also want to see how many invalid IDs were found, uncomment the next line:
        // Console.WriteLine($"Invalid count: {invalidCount}");
    }

    // This method checks whether a given ID is invalid under the new rules:
    // An ID is invalid if it consists of some digit sequence repeated at least twice.
    // Examples: 12341234 (1234 repeated), 123123123 (123 repeated),
    //           1212121212 (12 repeated), 1111111 (1 repeated 7 times)
    private static bool IsInvalidId(long id)
    {
        // Convert the number to a string so we can work with its digits
        string s = id.ToString();

        // Get the total length of the digit string
        int length = s.Length;

        // If the number has only 1 digit, it cannot be formed by repeating a smaller block
        if (length <= 1)
            return false;

        // We will try all possible pattern lengths from 1 up to half of the string length
        for (int patternLen = 1; patternLen <= length / 2; patternLen++)
        {
            // If the total length is not divisible by this pattern length,
            // then this pattern cannot tile the whole string without remainder
            if (length % patternLen != 0)
                continue;

            // Calculate how many times the pattern would need to repeat to form the full string
            int repeatCount = length / patternLen;

            // We only care about patterns that repeat at least twice
            if (repeatCount < 2)
                continue;

            // Take the first 'patternLen' characters as the candidate pattern
            string pattern = s.Substring(0, patternLen);

            // Assume initially that the whole string is formed by this pattern
            bool allMatch = true;

            // Now we verify that every block of length 'patternLen' equals 'pattern'
            for (int blockIndex = 1; blockIndex < repeatCount && allMatch; blockIndex++)
            {
                // Compute the starting index of this block in the string
                int startIndex = blockIndex * patternLen;

                // Compare each character in this block with the corresponding character in 'pattern'
                for (int j = 0; j < patternLen; j++)
                {
                    // If any character does not match, this pattern fails
                    if (s[startIndex + j] != pattern[j])
                    {
                        allMatch = false;
                        break;
                    }
                }
            }

            // If all blocks matched the pattern, then the ID is invalid
            if (allMatch)
                return true;
        }

        // If no repeating pattern was found, the ID is valid
        return false;
    }
}

// ==========================================
// BIG-O ANALYSIS
// Let N be the total number of IDs checked across all ranges.
// Let D be the maximum number of digits in any ID.
// For each ID, we try pattern lengths up to D/2, and for each pattern
// we may scan the whole string again in blocks.
// This gives us O(D^2) work per ID in the worst case.
// Since D is very small in practice (usually <= 10 digits),
// we can treat it as a constant.
// Therefore:
//   Time Complexity:  O(N * D^2)  ≈ O(N)
//   Space Complexity: O(1)        (only a few local variables and small strings).
// ==========================================