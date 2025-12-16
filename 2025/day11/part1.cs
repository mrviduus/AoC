var lines = File.ReadAllLines("input.txt");

var graph = new Dictionary<string, List<string>>();

foreach (var raw in lines)
{
    var line = raw.Trim();
    if(line.Length == 0) continue;

    var parts = line.Split(':');
    var from = parts[0].Trim();

    var toList = parts.Length > 1 ? parts[1].Trim() : "";
    var outs = toList.Length == 0
        ? new List<string>()
        : toList.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

    graph[from] = outs;
}

if(!graph.ContainsKey("out")) graph["out"] = new List<string>();

var memo = new Dictionary<string, long>();
var state = new Dictionary<string, int>();

long Dfs(string node)
{
    if(node == "out") return 1;

    if (memo.TryGetValue(node, out var cached)) return cached;

    state.TryGetValue(node, out var st);
    if (st == 1)
        throw new Exception("Cycle detected on a path: number of paths may be infinite.");

    state[node] = 1;

    graph.TryGetValue(node, out var neighbors);
    if (neighbors == null)
    {
        neighbors = new List<string>();
    }

    long total = 0;
    foreach (var nxt in neighbors)
    {
        total += Dfs(nxt);
    }

    state[node] = 2;
    memo[node] = total;
    return total;
}

var answer = Dfs("you");
System.Console.WriteLine(answer);