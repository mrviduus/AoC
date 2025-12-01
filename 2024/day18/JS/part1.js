const fs = require("fs");
const path = require("path");

const INPUT_FILE = path.join(__dirname, "../input.txt");

// Read input file with coordinates of corrupted cells
fs.readFile(INPUT_FILE, "utf-8", (err, input) => {
  if (err) {
    console.log("Error reading file:", err);
    process.exit(1);
  }

  // Parse input lines into an array of coordinate pairs
  const inputLines = input.trim().split("\n");

  // Initialize an empty grid (size: 71x71) with zeros (safe cells)
  const map = [];
  const size = 70;
  for (let i = 0; i <= size; ++i) {
    map.push(new Array(size + 1).fill(0));
  }

  // Mark corrupted cells on the grid with 1
  for (let line of inputLines.slice(0, 1024)) { // use first 1024 coordinates
    let [x, y] = line.split(',').map(Number);
    map[y][x] = 1;
  }

  // Breadth-First Search (BFS) algorithm implementation
  let queue = [[0, 0, 0]]; // queue contains [x, y, steps from start]
  let visited = {};        // stores visited cells to prevent revisiting

  while (queue.length) {
    let [x, y, steps] = queue.shift(); // take the next cell from the queue

    // Check if we reached the target (70,70)
    if (x === size && y === size) {
      console.log("Minimum steps to reach the exit:", steps);
      return;
    }

    // Check all four possible directions: down, up, right, left
    for (let [dx, dy] of [[0, 1], [0, -1], [1, 0], [-1, 0]]) {
      let newX = x + dx;
      let newY = y + dy;

      // Ensure the visited object has an entry for the current row
      if (!visited[newY]) visited[newY] = {};
      
      // Skip cell if already visited
      if (visited[newY][newX]) continue;

      visited[newY][newX] = true; // mark cell as visited

      // Check if the new cell is within bounds and safe
      if (
        map[newY] !== undefined &&        // check if row exists
        map[newY][newX] !== undefined &&  // check if column exists
        map[newY][newX] !== 1             // ensure cell is not corrupted
      ) {
        queue.push([newX, newY, steps + 1]); // enqueue cell with incremented step count
      }
    }
  }

  // If queue is empty and exit not found, there is no path
  console.log("No path found!");
});