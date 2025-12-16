using System; // Provide core types
using System.Collections.Generic; // Provide List/Dictionary/HashSet
using System.IO; // Provide File IO
using System.Linq; // Provide LINQ
using System.Text; // Provide StringBuilder
using System.Text.RegularExpressions; // Provide Regex

var input = File.ReadAllText("input.txt").Replace("\r", ""); // Read input file and normalize line endings
var lines = input.Split('\n'); // Split input into lines

var (shapeArr, regions) = ParseInput(lines); // Parse shapes + regions

var shapeCount = shapeArr.Count; // Store number of shape ids
var orientationsByShape = new List<List<List<(int x, int y)>>>(shapeCount); // shapeId -> orientations -> cells
for (var sid = 0; sid < shapeCount; sid++) // Loop over all shape ids
{
    if (shapeArr[sid] == null) // If shape is missing
    {
        orientationsByShape.Add(null!); // Keep index alignment
        continue; // Skip
    }

    orientationsByShape.Add(GenerateOrientations(shapeArr[sid]!)); // Generate unique rotations + flips
}

var answer = 0; // Count solvable regions

foreach (var region in regions) // Process each region
{
    var pre = BuildPlacementsForRegion(orientationsByShape, region.W, region.H); // Precompute placement masks per shape
    if (CanFitRegion(region, pre.PlacementsByShape, pre.AreaByShape, pre.BlocksCount)) // Solve using MRV + memo
        answer++; // Increment if solvable
}

Console.WriteLine(answer); // Print final answer

// ---------------- Parsing ----------------

static (List<string[]?> Shapes, List<Region> Regions) ParseInput(string[] lines) // Parse shapes and regions from input
{
    var shapes = new Dictionary<int, List<string>>(); // Temporary store of shapes by id
    var regions = new List<Region>(); // List of regions
    var i = 0; // Line pointer

    while (i < lines.Length && string.IsNullOrWhiteSpace(lines[i])) i++; // Skip leading empty lines

    while (i < lines.Length) // Parse shapes until region section begins
    {
        var line = lines[i].Trim(); // Read current line
        if (line.Length == 0) { i++; continue; } // Skip empty lines

        if (Regex.IsMatch(line, @"^\d+x\d+\s*:")) break; // Stop when we hit "WxH:"

        var m = Regex.Match(line, @"^(\d+)\s*:\s*$"); // Match "id:"
        if (!m.Success) throw new Exception("Bad shape header: " + line); // Validate format

        var id = int.Parse(m.Groups[1].Value); // Parse shape id
        i++; // Move to grid lines

        var grid = new List<string>(); // Collect shape rows
        while (i < lines.Length && !string.IsNullOrWhiteSpace(lines[i])) // Read until blank line
        {
            grid.Add(lines[i].Trim()); // Add row
            i++; // Advance
        }

        shapes[id] = grid; // Save shape
        while (i < lines.Length && string.IsNullOrWhiteSpace(lines[i])) i++; // Skip blanks
    }

    while (i < lines.Length) // Parse regions
    {
        var line = lines[i].Trim(); // Read region line
        i++; // Advance
        if (line.Length == 0) continue; // Skip empty

        var parts = line.Split(':'); // Split "WxH" and counts
        var wh = parts[0].Trim().Split('x'); // Split by 'x'
        var w = int.Parse(wh[0]); // Parse width
        var h = int.Parse(wh[1]); // Parse height

        var counts = parts[1].Trim() // Take counts substring
            .Split(' ', StringSplitOptions.RemoveEmptyEntries) // Split on spaces
            .Select(int.Parse) // Parse ints
            .ToArray(); // Materialize

        regions.Add(new Region(w, h, counts)); // Add region
    }

    var maxId = shapes.Count == 0 ? -1 : shapes.Keys.Max(); // Determine max shape id
    var shapeArr = new List<string[]?>(); // Build array-like list indexed by id
    for (var id = 0; id <= maxId; id++) shapeArr.Add(null); // Fill with nulls
    foreach (var kv in shapes) shapeArr[kv.Key] = kv.Value.ToArray(); // Assign grids

    return (shapeArr, regions); // Return parsed data
}

// ---------------- Orientations (rotate + flip) ----------------

static List<(int x, int y)> ShapeToCells(string[] grid) // Convert grid to list of '#' cells
{
    var cells = new List<(int x, int y)>(); // Create output list
    for (var y = 0; y < grid.Length; y++) // Loop rows
        for (var x = 0; x < grid[y].Length; x++) // Loop cols
            if (grid[y][x] == '#') cells.Add((x, y)); // Record filled cell
    return cells; // Return cells list
}

static List<(int x, int y)> RotateCells(List<(int x, int y)> cells) // Rotate 90 degrees
    => cells.Select(p => (p.y, -p.x)).ToList(); // Map (x,y)->(y,-x)

static List<(int x, int y)> FlipCells(List<(int x, int y)> cells) // Horizontal flip
    => cells.Select(p => (-p.x, p.y)).ToList(); // Map (x,y)->(-x,y)

static List<(int x, int y)> NormalizeCells(List<(int x, int y)> cells) // Normalize to minX=minY=0 and sort
{
    var minX = int.MaxValue; // Track min x
    var minY = int.MaxValue; // Track min y

    foreach (var (x, y) in cells) // Scan all cells
    {
        if (x < minX) minX = x; // Update minX
        if (y < minY) minY = y; // Update minY
    }

    return cells // Return normalized list
        .Select(p => (p.x - minX, p.y - minY)) // Shift to top-left
        .OrderBy(p => p.Item2) // Sort by y
        .ThenBy(p => p.Item1) // Then by x
        .ToList(); // Materialize
}

static string CellsKey(List<(int x, int y)> cells) // Serialize cells to dedup key
{
    var sb = new StringBuilder(); // Create builder
    for (var i = 0; i < cells.Count; i++) // Loop cells
    {
        if (i > 0) sb.Append(';'); // Separator
        sb.Append(cells[i].x).Append(',').Append(cells[i].y); // Append "x,y"
    }
    return sb.ToString(); // Return key
}

static List<List<(int x, int y)>> GenerateOrientations(string[] grid) // Generate unique orientations
{
    var baseCells = NormalizeCells(ShapeToCells(grid)); // Base normalized cells
    var seen = new HashSet<string>(); // Track unique keys
    var result = new List<List<(int x, int y)>>(); // Store orientations

    var cur = baseCells; // Start rotation state
    for (var r = 0; r < 4; r++) // Try 4 rotations
    {
        var a = NormalizeCells(cur); // Normalize rotation
        var ka = CellsKey(a); // Key
        if (seen.Add(ka)) result.Add(a); // Add if new

        var f = NormalizeCells(FlipCells(cur)); // Normalize flipped
        var kf = CellsKey(f); // Key
        if (seen.Add(kf)) result.Add(f); // Add if new

        cur = RotateCells(cur); // Rotate for next loop
    }

    return result; // Return all unique orientations
}

// ---------------- Bitset (ulong[]) helpers ----------------

static void SetBit(ulong[] blocks, int idx) // Set one bit in bitset
{
    var b = idx / 64; // Block index
    var bit = idx % 64; // Bit index
    blocks[b] |= 1UL << bit; // Set bit
}

static bool Intersects(ulong[] board, ulong[] mask) // Check overlap between board and mask
{
    for (var i = 0; i < board.Length; i++) // Loop blocks
        if ((board[i] & mask[i]) != 0UL) return true; // Overlap found
    return false; // No overlap
}

static void OrInto(ulong[] board, ulong[] mask) // board |= mask
{
    for (var i = 0; i < board.Length; i++) // Loop blocks
        board[i] |= mask[i]; // OR block
}

static void XorInto(ulong[] board, ulong[] mask) // board ^= mask (undo)
{
    for (var i = 0; i < board.Length; i++) // Loop blocks
        board[i] ^= mask[i]; // XOR block
}

// ---------------- Precompute placements for one board size ----------------

static Precomp BuildPlacementsForRegion(List<List<List<(int x, int y)>>> orientationsByShape, int w, int h) // Precompute placements
{
    var n = w * h; // Total cells
    var blocks = (n + 63) / 64; // Number of ulong blocks

    var placementsByShape = new List<ulong[]>[orientationsByShape.Count]; // Allocate placements
    var areaByShape = new int[orientationsByShape.Count]; // Allocate areas

    for (var shapeId = 0; shapeId < orientationsByShape.Count; shapeId++) // Loop shapes
    {
        var orients = orientationsByShape[shapeId]; // Get orientations
        if (orients == null) // If missing
        {
            placementsByShape[shapeId] = new List<ulong[]>(); // Empty
            areaByShape[shapeId] = 0; // Zero area
            continue; // Skip
        }

        placementsByShape[shapeId] = new List<ulong[]>(); // Init list
        areaByShape[shapeId] = orients[0].Count; // Store area (# cells)

        foreach (var cells in orients) // For each orientation
        {
            var maxX = 0; // Max x extent
            var maxY = 0; // Max y extent

            foreach (var (cx, cy) in cells) // Scan cells
            {
                if (cx > maxX) maxX = cx; // Update maxX
                if (cy > maxY) maxY = cy; // Update maxY
            }

            for (var oy = 0; oy + maxY < h; oy++) // Slide over y
            for (var ox = 0; ox + maxX < w; ox++) // Slide over x
            {
                var mask = new ulong[blocks]; // Create empty mask
                foreach (var (cx, cy) in cells) // For each filled cell
                {
                    var x = ox + cx; // Absolute x
                    var y = oy + cy; // Absolute y
                    var idx = y * w + x; // Linear index
                    SetBit(mask, idx); // Set bit
                }
                placementsByShape[shapeId].Add(mask); // Store placement
            }
        }
    }

    return new Precomp // Return precomp object
    {
        PlacementsByShape = placementsByShape, // Save placements
        AreaByShape = areaByShape, // Save areas
        BlocksCount = blocks // Save block count
    };
}

// ---------------- Solver: MRV by shape-type + memoization ----------------

static bool CanFitRegion(Region region, List<ulong[]>[] placementsByShape, int[] areaByShape, int blocksCount) // Solve region feasibility
{
    var w = region.W; // Width
    var h = region.H; // Height
    var n = w * h; // Total cells

    var remaining = new int[region.Counts.Length]; // Remaining counts per shape
    Array.Copy(region.Counts, remaining, region.Counts.Length); // Copy counts

    long remainingArea = 0; // Track remaining occupied cells needed
    for (var s = 0; s < remaining.Length; s++) remainingArea += (long)remaining[s] * areaByShape[s]; // Sum areas

    if (remainingArea > n) return false; // Prune by raw area

    var board = new ulong[blocksCount]; // Current occupied mask

    // ---- Zobrist hashing for memo ----
    var rng = new Random(1234567); // Deterministic seed for reproducibility
    var zobBoard = new ulong[blocksCount, 64]; // Random values for each block-bit
    for (var bi = 0; bi < blocksCount; bi++) // Loop blocks
        for (var b = 0; b < 64; b++) // Loop bits
            zobBoard[bi, b] = ((ulong)rng.Next() << 32) ^ (uint)rng.Next(); // Create random 64-bit-ish value

    var zobCount = new ulong[remaining.Length, 128]; // Random values for (shape, countValue) (counts in input are < 128 in your file)
    for (var s = 0; s < remaining.Length; s++) // Loop shapes
        for (var c = 0; c < 128; c++) // Loop possible counts
            zobCount[s, c] = ((ulong)rng.Next() << 32) ^ (uint)rng.Next(); // Random value

    ulong HashState() // Compute hash from board + remaining counts
    {
        ulong hsh = 0UL; // Start hash at 0
        for (var bi = 0; bi < board.Length; bi++) // For each block
        {
            var x = board[bi]; // Copy block bits
            while (x != 0UL) // While there are set bits
            {
                var lsb = x & (~x + 1UL); // Extract lowest set bit
                var bit = BitIndex(lsb); // Get bit index 0..63
                hsh ^= zobBoard[bi, bit]; // XOR zobrist value
                x ^= lsb; // Remove that bit
            }
        }

        for (var s = 0; s < remaining.Length; s++) // For each shape count
        {
            var c = remaining[s]; // Read count
            if (c >= 0 && c < 128) hsh ^= zobCount[s, c]; // Mix count into hash
            else hsh ^= (ulong)c * 11400714819323198485UL; // Fallback mix if count out of range
        }

        return hsh; // Return hash
    }

    // Note: BitIndex for a single-bit ulong (0..63)
    static int BitIndex(ulong singleBit) // Compute index of a single set bit
    {
        var idx = 0; // Index counter
        while ((singleBit >>= 1) != 0UL) idx++; // Shift until zero
        return idx; // Return index
    }

    var memo = new HashSet<ulong>(); // Store failed states by hash

    // ---- MRV helper: count how many placements are currently valid for a shape ----
    int CountValidPlacements(int shapeId) // Count placements that do not overlap current board
    {
        var list = placementsByShape[shapeId]; // Get placement list
        var cnt = 0; // Counter
        for (var i = 0; i < list.Count; i++) // Loop placements
            if (!Intersects(board, list[i])) cnt++; // Count if fits
        return cnt; // Return count
    }

    bool Dfs(long remArea) // Depth-first search over remaining piece counts
    {
        if (remArea == 0) return true; // If nothing left to place, success

        var stateHash = HashState(); // Hash current state
        if (memo.Contains(stateHash)) return false; // If already known dead-end, prune
        // We add to memo only when we are sure this state fails (at the end)

        // Choose next shape by MRV: among remaining>0 pick minimal valid placements
        var bestShape = -1; // Store selected shape
        var bestOptions = int.MaxValue; // Store minimal branching
        for (var s = 0; s < remaining.Length; s++) // Loop shapes
        {
            if (remaining[s] <= 0) continue; // Skip shapes with no remaining pieces
            var options = CountValidPlacements(s); // Count valid placements right now
            if (options == 0) { memo.Add(stateHash); return false; } // If a needed shape cannot be placed, fail fast
            if (options < bestOptions) { bestOptions = options; bestShape = s; } // Keep the most constrained shape
        }

        // Try to place one piece of bestShape
        var placements = placementsByShape[bestShape]; // Get all placements for this shape
        remaining[bestShape]--; // Consume one piece of this type

        for (var i = 0; i < placements.Count; i++) // Loop all placements
        {
            var mask = placements[i]; // Take placement mask
            if (Intersects(board, mask)) continue; // Skip if overlaps
            OrInto(board, mask); // Place piece
            if (Dfs(remArea - areaByShape[bestShape])) return true; // Recurse
            XorInto(board, mask); // Undo piece placement
        }

        remaining[bestShape]++; // Restore count

        memo.Add(stateHash); // Mark this state as failed
        return false; // No placement worked
    }

    return Dfs(remainingArea); // Start DFS
}

// Big-O: NP-complete in worst-case. Heuristics used: MRV by shape-type + bitset overlap + memoization.
// Worst-case still exponential, but for large boards and 6 shape types it is usually drastically faster.

readonly record struct Region(int W, int H, int[] Counts); // Region definition

sealed class Precomp // Hold precomputed placements for one region size
{
    public List<ulong[]>[] PlacementsByShape = null!; // shapeId -> list of placement masks
    public int[] AreaByShape = null!; // shapeId -> number of '#' cells
    public int BlocksCount; // Block count per mask
}