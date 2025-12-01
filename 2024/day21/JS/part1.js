const fs = require("fs");                         // Import Node's file system module
const path = require("path");                     // Import Node's path module to resolve file paths

const INPUT_FILE = path.join(__dirname, "../input.txt"); // Build absolute path to the input file

fs.readFile(INPUT_FILE, "utf-8", (err, input) => { // Asynchronously read the input file as UTF-8 text
  if (err) {                                      // If there was an error while reading the file
    console.error("Error reading file:", err);    //   Log the error to the console
    process.exit(1);                              //   Exit the process with a non-zero exit code
  }

  const lines = input                             // Take the raw input text
    .trim()                                       // Remove trailing whitespace/newlines
    .split(/\r?\n/)                               // Split into lines (handle both Windows and Unix newlines)
    .filter((line) => line.length > 0);           // Drop any empty lines just in case

  // ---------------------------
  // Define keypad layouts
  // ---------------------------

  // Numeric keypad coordinates (x = column, y = row, top-left is (0,0))
  // Layout:
  //   (0,0)=7  (1,0)=8  (2,0)=9
  //   (0,1)=4  (1,1)=5  (2,1)=6
  //   (0,2)=1  (1,2)=2  (2,2)=3
  //   (0,3)=gap (no key), (1,3)=0, (2,3)=A
  const numericLayout = {                         // Map each numeric key to its grid coordinates
    "7": { x: 0, y: 0 },                          // '7' at top-left
    "8": { x: 1, y: 0 },                          // '8' at top-middle
    "9": { x: 2, y: 0 },                          // '9' at top-right
    "4": { x: 0, y: 1 },                          // '4' at second row, left
    "5": { x: 1, y: 1 },                          // '5' at second row, middle
    "6": { x: 2, y: 1 },                          // '6' at second row, right
    "1": { x: 0, y: 2 },                          // '1' at third row, left
    "2": { x: 1, y: 2 },                          // '2' at third row, middle
    "3": { x: 2, y: 2 },                          // '3' at third row, right
    "0": { x: 1, y: 3 },                          // '0' at bottom row, middle
    "A": { x: 2, y: 3 },                          // 'A' at bottom row, right
  };

  const numericWidth = 3;                         // Numeric keypad has 3 columns
  const numericHeight = 4;                        // Numeric keypad has 4 rows
  const numericGap = { x: 0, y: 3 };              // The only invalid cell on the numeric keypad

  // Directional keypad coordinates (x = column, y = row, top-left is (0,0))
  // Layout:
  //   (0,0)=gap   (1,0)=^   (2,0)=A
  //   (0,1)=<     (1,1)=v   (2,1)=>
  const directionalLayout = {                     // Map each directional key to its grid coordinates
    "^": { x: 1, y: 0 },                          // '^' is top-middle
    "A": { x: 2, y: 0 },                          // 'A' is top-right
    "<": { x: 0, y: 1 },                          // '<' is bottom-left
    "v": { x: 1, y: 1 },                          // 'v' is bottom-middle
    ">": { x: 2, y: 1 },                          // '>' is bottom-right
  };

  const dirWidth = 3;                             // Directional keypad has 3 columns
  const dirHeight = 2;                            // Directional keypad has 2 rows
  const dirGap = { x: 0, y: 0 };                  // The only invalid cell on the directional keypad

  // ---------------------------
  // Precompute shortest move paths for both keypads
  // ---------------------------

  /**
   * Build a map from each key to every other key for a keypad,
   * containing all shortest movement strings (only ^ v < >, no 'A').
   *
   * @param {Object} layout - Map from key (char) to {x,y}
   * @param {number} width - Number of columns on keypad grid
   * @param {number} height - Number of rows on keypad grid
   * @param {Object} gap - Coordinates of the forbidden cell {x,y}
   * @returns {Object} paths[fromKey][toKey] = array of movement strings
   */
  function computeShortestMovePaths(layout, width, height, gap) {
    const keys = Object.keys(layout);             // List of all valid keys on this keypad

    // Build reverse map from coordinates to key for quick lookup
    const posToKey = Array.from({ length: height }, () => // Create array for rows
      Array.from({ length: width }, () => null)   // For each row, create array for columns initialized to null
    );

    for (const key of keys) {                     // For each key in the layout
      const pos = layout[key];                    //   Get its coordinates
      posToKey[pos.y][pos.x] = key;               //   Store the key at its (x,y) position
    }

    // Directions we can move: char and corresponding delta (dx, dy)
    const moves = [                               // Define all 4 possible move directions
      { ch: "^", dx: 0, dy: -1 },                 // Move up (decrease y)
      { ch: "v", dx: 0, dy: 1 },                  // Move down (increase y)
      { ch: "<", dx: -1, dy: 0 },                 // Move left (decrease x)
      { ch: ">", dx: 1, dy: 0 },                  // Move right (increase x)
    ];

    const paths = {};                             // This will hold all shortest paths from each key

    for (const startKey of keys) {                // For each starting key on the keypad
      paths[startKey] = {};                       //   Prepare an object for all destination keys
      for (const k of keys) {                     //   Initialize arrays for all possible destination keys
        paths[startKey][k] = [];                  //   Start with empty list of paths
      }

      // BFS over positions to find all shortest paths from startKey to every other key
      const dist = Array.from({ length: height }, () => // Distance grid: best distance found to (x,y)
        Array.from({ length: width }, () => Infinity) // Initialize all distances as Infinity
      );

      const startPos = layout[startKey];          // Get starting coordinates for this key

      // Queue holds objects: { x, y, path } where path is string of moves to reach (x,y)
      const queue = [];                           // Initialize BFS queue
      queue.push({ x: startPos.x, y: startPos.y, path: "" }); // Start BFS at the start key with empty path
      dist[startPos.y][startPos.x] = 0;           // Distance to start position is 0

      while (queue.length > 0) {                  // Process the queue until empty
        const { x, y, path } = queue.shift();     // Pop the first element in FIFO manner
        const d = path.length;                    // Current distance is the path length so far

        const keyHere = posToKey[y][x];           // Find which key (if any) is at this position

        if (keyHere !== null) {                   // If this cell corresponds to a valid key
          const existingPaths = paths[startKey][keyHere]; // Get paths already recorded to this key
          if (existingPaths.length === 0) {       // If no paths recorded yet
            existingPaths.push(path);             //   This is the first shortest path
          } else if (existingPaths[0].length === d) { // If we have paths of same minimal length
            existingPaths.push(path);             //   Add this alternative path as well
          }
          // If this path is longer than already-known shortest, we ignore it automatically
        }

        for (const move of moves) {               // Try each possible movement direction
          const nx = x + move.dx;                 //   Compute new x coordinate
          const ny = y + move.dy;                 //   Compute new y coordinate

          if (nx < 0 || nx >= width) continue;    //   Skip if new x is out of bounds
          if (ny < 0 || ny >= height) continue;   //   Skip if new y is out of bounds
          if (nx === gap.x && ny === gap.y) continue; // Skip the forbidden gap cell

          const nd = d + 1;                       //   New distance is current path length + 1
          if (nd > dist[ny][nx]) {                //   If we already have a shorter path to this cell
            continue;                             //     Don't explore this longer path
          }

          const newPath = path + move.ch;         //   Append this move to the path

          if (nd < dist[ny][nx]) {                //   If this is the new best distance to this cell
            dist[ny][nx] = nd;                    //     Update the distance
            queue.push({ x: nx, y: ny, path: newPath }); //     Enqueue the new state
          } else if (nd === dist[ny][nx]) {       //   If this path is also shortest (same length)
            queue.push({ x: nx, y: ny, path: newPath }); //     Enqueue to record alternate path
          }
        }
      }

      // Also capture the "stay on the same key" path (zero moves)
      // BFS already handles this because we started at that key with empty path.
    }

    return paths;                                 // Return all shortest movement paths for this keypad
  }

  // Precompute shortest movement paths on numeric keypad
  const numericMovePaths = computeShortestMovePaths(
    numericLayout,                                // Numeric keypad layout
    numericWidth,                                 // Numeric keypad width
    numericHeight,                                // Numeric keypad height
    numericGap                                    // Numeric keypad forbidden cell
  );

  // Precompute shortest movement paths on directional keypad
  const directionalMovePaths = computeShortestMovePaths(
    directionalLayout,                            // Directional keypad layout
    dirWidth,                                     // Directional keypad width
    dirHeight,                                    // Directional keypad height
    dirGap                                        // Directional keypad forbidden cell
  );

  // ---------------------------
  // DP / memoization for costs
  // ---------------------------

  const NUM_ROBOTS = 2;                           // Number of directional robots between you and numeric keypad

  const memoNum = new Map();                      // Memo table for numeric moves: (depth,from,to) -> cost
  const memoDir = new Map();                      // Memo table for directional moves: (depth,from,to) -> cost

  /**
   * Build a unique key string for memoization.
   *
   * @param {number} depth - Remaining robot depth
   * @param {string} fromKey - Starting key name
   * @param {string} toKey - Destination key name
   * @param {string} kind - Either "num" or "dir" to distinguish keypad type
   * @returns {string} composed key
   */
  function memoKey(depth, fromKey, toKey, kind) {
    return `${kind}|${depth}|${fromKey}|${toKey}`; // Concatenate fields with separators
  }

  /**
   * Compute the cost (number of your button presses) to move on a
   * DIRECTIONAL keypad from 'fromKey' to 'toKey' and press it once.
   *
   * @param {number} depth - Remaining robot depths below this keypad
   * @param {string} fromKey - Current key on this directional keypad
   * @param {string} toKey - Target key to be pressed on this directional keypad
   * @returns {number} minimal cost in terms of your presses
   */
  function costForMoveDirectional(depth, fromKey, toKey) {
    const key = memoKey(depth, fromKey, toKey, "dir"); // Build memoization key

    if (memoDir.has(key)) {                     // If we already computed this state
      return memoDir.get(key);                  //   Return cached value
    }

    const paths = directionalMovePaths[fromKey][toKey]; // All shortest movement strings between keys

    let best = Infinity;                        // Track minimal cost among all candidate paths

    for (const path of paths) {                 // For each equally-short movement string
      const sequence = path + "A";              //   We must move then press 'A' to press the key

      let cost;                                 //   Cost to produce this sequence at lower depth

      if (depth === 0) {                        //   Base case: this directional keypad is the one YOU press
        cost = sequence.length;                 //   Cost is just the number of button presses
      } else {                                  //   Otherwise, it is controlled by another directional keypad
        cost = costForDirectionalSequence(depth - 1, sequence); //   Recurse to lower depth
      }

      if (cost < best) {                        //   If this path is cheaper than what we have so far
        best = cost;                            //     Update best cost
      }
    }

    memoDir.set(key, best);                     // Store computed result in memo table
    return best;                                // Return minimal cost
  }

  /**
   * Compute the cost to type a whole SEQUENCE on a directional keypad,
   * when there are 'depth' more directional robots below it.
   *
   * @param {number} depth - Remaining robot depths below this keypad
   * @param {string} sequence - String of keys (^ v < > A) to press on this directional keypad
   * @returns {number} minimal cost in terms of your presses
   */
  function costForDirectionalSequence(depth, sequence) {
    let total = 0;                               // Total cost to type the whole sequence
    let currentKey = "A";                        // Pointer starts at 'A' on any keypad

    for (const ch of sequence) {                 // For each character we need to press
      total += costForMoveDirectional(depth, currentKey, ch); //   Add cost to move and press that key
      currentKey = ch;                           //   Pointer is now on that key
    }

    return total;                                // Return accumulated cost
  }

  /**
   * Compute the cost to move on a NUMERIC keypad from 'fromKey' to 'toKey' and press it once.
   * This numeric keypad is always controlled by a directional keypad underneath it.
   *
   * @param {number} depth - Remaining directional robots between NUMERIC keypad and YOU
   * @param {string} fromKey - Current key on numeric keypad
   * @param {string} toKey - Target key to be pressed on numeric keypad
   * @returns {number} minimal cost in terms of your presses
   */
  function costForMoveNumeric(depth, fromKey, toKey) {
    const key = memoKey(depth, fromKey, toKey, "num"); // Build memoization key

    if (memoNum.has(key)) {                     // If we computed this before
      return memoNum.get(key);                  //   Return cached result
    }

    const paths = numericMovePaths[fromKey][toKey]; // All shortest movement strings between numeric keys

    let best = Infinity;                        // Track minimal cost among all candidate paths

    for (const path of paths) {                 // For each equally-short movement path
      const sequence = path + "A";              //   Convert to sequence on underlying directional keypad

      // The numeric keypad is always controlled by a directional keypad one level down
      const cost = costForDirectionalSequence(depth - 1, sequence); //   Compute cost on lower depth

      if (cost < best) {                        //   If this candidate is cheaper
        best = cost;                            //     Remember it as best
      }
    }

    memoNum.set(key, best);                     // Cache computed result
    return best;                                // Return minimal cost
  }

  // ---------------------------
  // Main: compute total complexity
  // ---------------------------

  let totalComplexity = 0;                      // Running sum of all code complexities

  for (const code of lines) {                   // For each code line from the input
    let totalPresses = 0;                       //   Track how many presses YOU will do
    let currentKey = "A";                       //   Numeric keypad pointer starts at 'A'

    for (const ch of code) {                    //   For each character in the code (digits and final 'A')
      totalPresses += costForMoveNumeric(NUM_ROBOTS, currentKey, ch); //     Cost to produce this press
      currentKey = ch;                          //     Numeric pointer now sits on that key
    }

    const numericPart = parseInt(code.slice(0, -1), 10); //   Extract numeric part (all but last 'A'), ignore leading zeros

    const complexity = totalPresses * numericPart; //   Complexity = presses * numeric value

    totalComplexity += complexity;              //   Add to global sum
  }

  console.log(totalComplexity);                 // Finally, print the sum of complexities
});

// ---------------------------
// Big-O Complexity
// ---------------------------
// Let N be the number of codes and L be the average length of a code.
//
// Precomputation:
// - Keypad has constant size (numeric: 10 keys, directional: 5 keys).
// - BFS from each key explores at most a small constant number of states.
//   So precomputation is O(1) time and O(1) memory.
//
// Main computation:
// - For each pair (from,to) on keypads, we compute cost once and memoize.
//   Number of keys is constant, depth is small (2 for part 1).
//   So total distinct DP states is O(1).
// - For each character of each code, we just look up memoized costs.
//
// Therefore overall time is O(N * L) and space is O(1) beyond input storage.
