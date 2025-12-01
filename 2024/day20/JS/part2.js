const fs = require("fs");
const path = require("path");
const INPUT_FILE = path.join(__dirname, "../input.txt");

fs.readFile(INPUT_FILE, "utf-8", (err, input) => {
  if (err) {
    console.log("Error reading file", err);
    process.exit(1);
  }

  // Split the whole map text into lines (each line = one row of the maze)
  const gridLines = input.trim().split("\n");
  // Height and width of the grid
  const H = gridLines.length;
  const W = gridLines[0].length;

  // Convert map into a 2D array of chars for convenient indexing
  const grid = gridLines.map(line => line.split(""));

  // Locate S (start) and E (end)
  let start = null; // [x, y]
  let end = null;   // [x, y]
  for (let y = 0; y < H; y++) {
    for (let x = 0; x < W; x++) {
      if (grid[y][x] === "S") start = [x, y];
      if (grid[y][x] === "E") end = [x, y];
    }
  }

  // Standard BFS over passable cells (not '#')
  function bfs(sourceX, sourceY) {
    // Distance matrix; -1 means "unreachable"
    const dist = Array.from({ length: H }, () => Array(W).fill(-1));
    // Queue for BFS
    const queue = [];

    // Start cell takes 0 steps to reach
    dist[sourceY][sourceX] = 0;
    queue.push([sourceX, sourceY]);

    // Movement directions (4-neighborhood)
    const dirs = [
      [1, 0],
      [-1, 0],
      [0, 1],
      [0, -1],
    ];

    // Perform the BFS
    while (queue.length > 0) {
      const [x, y] = queue.shift();

      for (const [dx, dy] of dirs) {
        const nx = x + dx;
        const ny = y + dy;

        // Must be in bounds
        if (nx < 0 || nx >= W || ny < 0 || ny >= H) continue;
        // Can't walk through walls
        if (grid[ny][nx] === "#") continue;
        // Skip already visited
        if (dist[ny][nx] !== -1) continue;

        // One more step from current
        dist[ny][nx] = dist[y][x] + 1;
        queue.push([nx, ny]);
      }
    }

    // Return the distance grid
    return dist;
  }

  // Distances from S and to E
  const distFromStart = bfs(start[0], start[1]);
  const distFromEnd = bfs(end[0], end[1]);

  // Shortest honest path length from S to E (no cheats)
  const baseTime = distFromStart[end[1]][end[0]];

  // Build the "track": cells that lie on at least one shortest path
  // A cell (x,y) is on-track if distS + distE == baseTime (and both distances are not -1)
  const onTrack = Array.from({ length: H }, () => Array(W).fill(false));
  const trackCells = [];
  for (let y = 0; y < H; y++) {
    for (let x = 0; x < W; x++) {
      if (grid[y][x] === "#") continue;                   // wall can't be on-track
      if (distFromStart[y][x] === -1) continue;           // not reachable from S
      if (distFromEnd[y][x] === -1) continue;             // can't reach E from here
      if (distFromStart[y][x] + distFromEnd[y][x] === baseTime) {
        onTrack[y][x] = true;                              // mark as part of a shortest path
        trackCells.push([x, y]);                           // keep a list of track cells
      }
    }
  }

  // For ordering along the track we use distance from start (strictly increasing along any shortest path)
  // This lets us compute the "normal path distance" between two track cells as delta of distFromStart.
  function orderAt(x, y) {
    return distFromStart[y][x];
  }

  // Precompute all offsets within Manhattan distance <= 20 (the cheat budget)
  const BUDGET = 20; // Part Two rule
  const offsets = [];
  for (let dy = -BUDGET; dy <= BUDGET; dy++) {
    const maxDx = BUDGET - Math.abs(dy); // |dx| + |dy| <= BUDGET
    for (let dx = -maxDx; dx <= maxDx; dx++) {
      if (dx === 0 && dy === 0) continue; // skip zero move
      offsets.push([dx, dy]);
    }
  }

  // Count cheats that save at least 100 picoseconds
  const MIN_SAVE = 100;
  let countCheatsAtLeast100 = 0;

  // For each track cell A, try cheating to any track cell B within Manhattan <= 20.
  // We only count moves that go "forward" along the path (orderB > orderA) to avoid double counting.
  for (const [ax, ay] of trackCells) {
    const orderA = orderAt(ax, ay);

    for (const [dx, dy] of offsets) {
      const bx = ax + dx;
      const by = ay + dy;

      // Must be in bounds
      if (bx < 0 || bx >= W || by < 0 || by >= H) continue;
      // End of cheat must land back on the normal track
      if (!onTrack[by][bx]) continue;

      const orderB = orderAt(bx, by);
      // Only jump forward along the path (prevents double counting and negative "normal" distance)
      if (orderB <= orderA) continue;

      // Manhattan time spent during the cheat
      const md = Math.abs(dx) + Math.abs(dy);

      // "Normal" distance along the track between A and B
      const normalDist = orderB - orderA;

      // Saved time = how much shorter the cheat is vs walking the track
      const saving = normalDist - md;

      // Count if it meets threshold
      if (saving >= MIN_SAVE) {
        countCheatsAtLeast100++;
      }
    }
  }

  console.log(countCheatsAtLeast100);

  /*
    Big-O:
    - Two BFS: O(H*W)
    - Build track set: O(H*W)
    - For each track cell T (T ≤ H*W) we scan a diamond of radius R=20:
      number of offsets ~ O(R^2) which is constant here (<= ~841).
    - Total time: O(H*W + T*R^2) ≈ O(H*W) for fixed R
    - Space: O(H*W) for distance grids and track masks
  */
});
