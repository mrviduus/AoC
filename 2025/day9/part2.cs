var lines = File.ReadAllLines("input.txt");
var redWorld = new List<(int x, int y)>();

foreach (var line in lines)
{
    var trimmed = line.Trim();
    if (string.IsNullOrEmpty(trimmed)) continue;
    var parts = trimmed.Split(',');
    redWorld.Add((int.Parse(parts[0]), int.Parse(parts[1])));
}

if (redWorld.Count == 0) { Console.WriteLine(0); return; }
if (redWorld.Count == 1) { Console.WriteLine(1); return; }

// Coordinate compression
var xSet = new SortedSet<int>();
var ySet = new SortedSet<int>();

foreach (var p in redWorld)
{
    xSet.Add(p.x);
    ySet.Add(p.y);
}

// Add boundary for flood fill
int minX = xSet.Min(), maxX = xSet.Max();
int minY = ySet.Min(), maxY = ySet.Max();
xSet.Add(minX - 1);
xSet.Add(maxX + 1);
ySet.Add(minY - 1);
ySet.Add(maxY + 1);

var xList = xSet.ToList();
var yList = ySet.ToList();

var xIdx = new Dictionary<int, int>();
var yIdx = new Dictionary<int, int>();
for (int i = 0; i < xList.Count; i++) xIdx[xList[i]] = 2 * i;
for (int i = 0; i < yList.Count; i++) yIdx[yList[i]] = 2 * i;

int W = 2 * xList.Count - 1;
int H = 2 * yList.Count - 1;

var grid = new char[H, W];
for (int y = 0; y < H; y++)
    for (int x = 0; x < W; x++)
        grid[y, x] = '.';

// Place red tiles
foreach (var p in redWorld)
    grid[yIdx[p.y], xIdx[p.x]] = '#';

// Draw green edges
int n = redWorld.Count;
for (int i = 0; i < n; i++)
{
    var p1 = redWorld[i];
    var p2 = redWorld[(i + 1) % n];
    int gx1 = xIdx[p1.x], gy1 = yIdx[p1.y];
    int gx2 = xIdx[p2.x], gy2 = yIdx[p2.y];

    if (p1.x == p2.x)
    {
        int x = gx1;
        int y1 = Math.Min(gy1, gy2), y2 = Math.Max(gy1, gy2);
        for (int y = y1; y <= y2; y++)
            if (grid[y, x] == '.') grid[y, x] = 'X';
    }
    else
    {
        int y = gy1;
        int x1 = Math.Min(gx1, gx2), x2 = Math.Max(gx1, gx2);
        for (int x = x1; x <= x2; x++)
            if (grid[y, x] == '.') grid[y, x] = 'X';
    }
}

// Flood fill from outside
var q = new Queue<(int y, int x)>();
grid[0, 0] = 'O';
q.Enqueue((0, 0));

int[] dy = { -1, 1, 0, 0 };
int[] dx = { 0, 0, -1, 1 };

while (q.Count > 0)
{
    var (cy, cx) = q.Dequeue();
    for (int d = 0; d < 4; d++)
    {
        int ny = cy + dy[d], nx = cx + dx[d];
        if (ny >= 0 && ny < H && nx >= 0 && nx < W && grid[ny, nx] == '.')
        {
            grid[ny, nx] = 'O';
            q.Enqueue((ny, nx));
        }
    }
}

// Inside cells remain '.', mark as X
for (int y = 0; y < H; y++)
    for (int x = 0; x < W; x++)
        if (grid[y, x] == '.') grid[y, x] = 'X';

// Prefix sum for 'O' (bad) cells
var pref = new int[H + 1, W + 1];
for (int y = 0; y < H; y++)
    for (int x = 0; x < W; x++)
        pref[y + 1, x + 1] = (grid[y, x] == 'O' ? 1 : 0) + pref[y, x + 1] + pref[y + 1, x] - pref[y, x];

int GetBad(int y1, int x1, int y2, int x2)
{
    return pref[y2 + 1, x2 + 1] - pref[y1, x2 + 1] - pref[y2 + 1, x1] + pref[y1, x1];
}

long maxArea = 0;

for (int i = 0; i < redWorld.Count; i++)
{
    var (x1, y1) = redWorld[i];
    int gx1 = xIdx[x1], gy1 = yIdx[y1];

    for (int j = i + 1; j < redWorld.Count; j++)
    {
        var (x2, y2) = redWorld[j];
        int gx2 = xIdx[x2], gy2 = yIdx[y2];

        int left = Math.Min(gx1, gx2), right = Math.Max(gx1, gx2);
        int top = Math.Min(gy1, gy2), bottom = Math.Max(gy1, gy2);

        if (GetBad(top, left, bottom, right) == 0)
        {
            long area = (long)(Math.Abs(x2 - x1) + 1) * (Math.Abs(y2 - y1) + 1);
            if (area > maxArea) maxArea = area;
        }
    }
}

Console.WriteLine(maxArea);
