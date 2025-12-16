var lines = File.ReadAllLines("input.txt");

var graph = new Dictionary<string, List<string>>();

foreach (var raw in lines)
{
    var line = raw.Trim();
    if(line.Length == 0) continue;

    var parts = line.Split(':');
    var from = parts[0].Trim();

    var right = parts.Length > 1 ? parts[1].Trim() : "";
    var outs = right.Length == 0
        ? new List<string>()
        : right.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();
    
    graph[from] = outs;
}

if(!graph.ContainsKey("out")) graph["out"] = new List<string>();

const int DAC_BIT = 1;
const int FFT_BIT = 2;
const int BOTH = DAC_BIT | FFT_BIT;

var memo = new Dictionary<(string node, int mask), long>();
var state = new Dictionary<(string node, int mask), int>();

int AddFlag(string node, int mask)
{
    if(node == "dac") mask |= DAC_BIT;
    if(node == "fft") mask |= FFT_BIT;
    return mask;
}

long Dfs(string node, int mask)
{
    mask = AddFlag(node, mask);
    if(node == "out") return mask == BOTH ? 1 : 0;

    var key = (node, mask);
    
    if(memo.TryGetValue(key, out var cached)) return cached;

    state.TryGetValue(key, out var st);
    
    if(st == 1)
        throw new Exception("Cycle detected on a path: number of paths may be infinite.");

    state[key] = 1;
    
    graph.TryGetValue(node, out var neighbors);
    if (neighbors == null)
    {
        neighbors = new List<string>();
    }

    long total = 0;
    foreach (var nxt in neighbors)
    {
        total += Dfs(nxt, mask);
    }
    state[key] = 2;
    memo[key] = total;
    return total;
}

var answer = Dfs("svr", 0);
System.Console.WriteLine( answer);