
// Read all lines from input.txt (Advent of Code style)
var lines = File.ReadAllLines("input.txt");         // Read the entire map as an array of strings

var grid = new List<string>();                      // List to hold trimmed lines
foreach (var line in lines)                         // Loop through each line
{
    grid.Add(line.TrimEnd());                       // Remove trailing spaces/newlines and store the line
}

int rows = grid.Count;                              // Number of rows in the grid
int cols = grid[0].Length;                          // Number of columns in the grid

// Find the coordinates of 'S' (start of the beam)
int startRow = -1;                                  // Row index of 'S'
int startCol = -1;                                  // Column index of 'S'
for (int r = 0; r < rows; r++)                      // Loop over all rows
{
    int c = grid[r].IndexOf('S');                   // Look for 'S' in this row
    if (c != -1)                                    // If 'S' is found
    {
        startRow = r;                               // Save row index of 'S'
        startCol = c;                               // Save column index of 'S'
        break;                                      // Stop searching further
    }
}

// Initialize a queue for BFS over beam positions
var queue = new Queue<(int r, int c)>();            // Queue of tuples (row, column)
queue.Enqueue((startRow + 1, startCol));            // First beam starts right below 'S'

// HashSet to track visited cells (states where a beam has already been)
var visited = new HashSet<(int r, int c)>();        // Stores each cell that already had a beam

int splitCount = 0;                                 // How many splitters were activated

while (queue.Count > 0)                             // Process until there are no more beams
{
    var (r, c) = queue.Dequeue();                   // Take next beam position from the queue

    // If beam is outside the grid, we just ignore it
    if (r < 0 || r >= rows || c < 0 || c >= cols)   // Check if this position is out of bounds
    {
        continue;                                   // Skip this beam
    }

    if (visited.Contains((r, c)))                   // If we have already visited this cell
    {
        // This simulates merging of beams: multiple beams in same cell act as one
        continue;                                   // Do not process again
    }

    visited.Add((r, c));                            // Mark this cell as visited

    char ch = grid[r][c];                           // Character in the current cell ('.' or '^')

    if (ch == '^')                                  // If this is a splitter cell
    {
        splitCount++;                               // Count this splitter activation

        // Original downward beam stops; create two new beams to the left and right
        queue.Enqueue((r, c - 1));                  // New beam appears left of the splitter
        queue.Enqueue((r, c + 1));                  // New beam appears right of the splitter
    }
    else
    {
        // If it's empty space '.', the beam continues downward
        queue.Enqueue((r + 1, c));                  // Beam moves one row down in the same column
    }
}

// Print the total number of times beams were split
Console.WriteLine(splitCount);                      // Output the final answer

// Time complexity:  O(R * C)
// Space complexity: O(R * C)