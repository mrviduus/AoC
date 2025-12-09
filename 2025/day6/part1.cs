using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Read all lines from file
var lines = File.ReadAllLines("input.txt");

// Determine width of worksheet
int width = lines.Max(l => l.Length);

// Normalize lines to same width by right-padding with spaces
var normalized = lines.Select(l => l.PadRight(width)).ToArray();

long grandTotal = 0;

// Buffer for building one problem (vertical block)
List<string> currentColumnChars = new List<string>();

// Iterate each column left-to-right
for (int col = 0; col < width; col++)
{
    // Extract all characters from this column (top → bottom)
    var column = normalized.Select(row => row[col]).ToArray();

    // Check if this column is fully empty (spaces only)
    bool isEmpty = column.All(c => c == ' ');

    if (isEmpty)
    {
        // If empty column AND we have active collected block → finalize problem
        if (currentColumnChars.Count > 0)
        {
            grandTotal += SolveProblem(currentColumnChars);
            currentColumnChars.Clear();
        }
    }
    else
    {
        // Build vertical column text as a string
        currentColumnChars.Add(new string(column));
    }
}

// If last block wasn't closed by empty column → close manually
if (currentColumnChars.Count > 0)
{
    grandTotal += SolveProblem(currentColumnChars);
}

// Output final result
Console.WriteLine(grandTotal);

// Local static function used above
static long SolveProblem(List<string> cols)
{
    // cols = list of vertical strings, each string holds characters from top to bottom in that column

    // Find operation: bottom row among all columns contains + or *
    char operation = cols
        .Select(col => col.Last())   // last char in each column
        .First(c => c == '+' || c == '*');

    // Extract numeric rows (everything except bottom row)
    int height = cols[0].Length;
    var numbers = new List<long>();

    // For each row above the last bottom row:
    for (int row = 0; row < height - 1; row++)
    {
        // Build the horizontal number slice by taking char[row] from each column
        string rowSlice = new string(cols.Select(col => col[row]).ToArray());

        // Clean spaces
        string trimmed = rowSlice.Trim();

        if (trimmed.Length > 0)
        {
            numbers.Add(long.Parse(trimmed));
        }
    }

    // Compute result
    long result = (operation == '+')
        ? numbers.Aggregate(0L, (a, b) => a + b)
        : numbers.Aggregate(1L, (a, b) => a * b);

    return result;
}

/*
Big O:
Let H = number of rows (height), W = number of columns (width).
- Reading input:          O(H)
- Normalizing lines:      O(H * W)
- Scanning all columns:   O(H * W)
- Solving problems:       O(H * W) total (каждая ячейка участвует пару раз)
Итого: O(H * W) по времени, O(H * W) по памяти (из-за нормализованной матрицы).
*/