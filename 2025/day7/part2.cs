using System;                                        // Basic system namespace
using System.Collections.Generic;                    // To use Dictionary and other collections
using System.IO;                                     // To read the input file

// Read all lines from "input.txt" (Advent of Code typical setup)
var lines = File.ReadAllLines("input.txt");          // Read file into an array of strings

// Build the grid from the input lines
var grid = new List<string>();                       // List to store processed lines
foreach (var line in lines)                          // Iterate over each input line
{
    grid.Add(line.TrimEnd());                        // Remove trailing spaces and add to the grid
}

int rows = grid.Count;                               // Number of rows in the grid
int cols = grid[0].Length;                           // Number of columns in the grid

// Find the position of 'S' in the grid
int startRow = -1;                                   // Row index of 'S'
int startCol = -1;                                   // Column index of 'S'

for (int r = 0; r < rows; r++)                       // Loop over all rows
{
    int c = grid[r].IndexOf('S');                    // Find 'S' in this row
    if (c != -1)                                     // If 'S' is found
    {
        startRow = r;                                // Save row index
        startCol = c;                                // Save column index
        break;                                       // Stop searching further
    }
}

// We use a dictionary for memoization: from (row, col) to number of timelines
var memo = new Dictionary<(int r, int c), long>();   // Stores the number of timelines for each cell

// Recursive function to count timelines from a given cell (r, c)
long CountTimelines(int r, int c)                    // Define a local function that returns a long
{
    // If the position is outside the grid, we have reached the end of one timeline
    if (r < 0 || r >= rows || c < 0 || c >= cols)    // Check out-of-bounds condition
    {
        return 1L;                                   // One complete timeline ends here
    }

    var key = (r, c);                                // Create a tuple key for this cell

    // If we've already computed this state, just return the cached result
    if (memo.TryGetValue(key, out var cached))       // Try to get a stored value from memo
    {
        return cached;                               // Return the memoized result
    }

    char ch = grid[r][c];                            // Character at the current cell ('.' or '^')

    long result;                                     // Variable to hold the number of timelines from this cell

    if (ch == '^')                                   // If the current cell is a splitter
    {
        // Time splits into two branches: left and right
        long left = CountTimelines(r, c - 1);        // Count timelines for the left branch
        long right = CountTimelines(r, c + 1);       // Count timelines for the right branch
        result = left + right;                       // Total timelines from this cell
    }
    else
    {
        // For empty space '.', the particle continues downward
        result = CountTimelines(r + 1, c);           // Count timelines from the cell below
    }

    memo[key] = result;                              // Store the computed result in the memo dictionary
    return result;                                   // Return the number of timelines from this cell
}

// Starting position of the particle is the cell right below 'S'
int startR = startRow + 1;                           // Row where the particle enters the manifold
int startC = startCol;                               // Column aligned with 'S'

// Compute the total number of different timelines for this quantum particle
long totalTimelines = CountTimelines(startR, startC);// Call the recursive DP function

// Output the final answer for Part Two
Console.WriteLine(totalTimelines);                   // Print the number of timelines

// Time complexity:  O(R * C)
// Space complexity: O(R * C)