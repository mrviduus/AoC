var lines = File.ReadAllLines("input.txt");

var points = new List<(int x, int y, int z)>(); // list of tuples for points

foreach(var line in lines)
{
    var parts = line.Split(',');
    
    int x = int.Parse(parts[0]);
    int y = int.Parse(parts[1]);
    int z = int.Parse(parts[2]);
    
    points.Add((x, y, z));
}
        
int n = points.Count; // number of points

var edges = new List<(int i, int j, long dist2)>();

for(int i = 0; i < n; i++)
{
    for(int j = i + 1; j < n; j++)
    {
        long dx = (long)points[i].x - points[j].x;
        long dy = (long)points[i].y - points[j].y;
        long dz = (long)points[i].z - points[j].z;
                
        // Squared distance (use long to avoid overflow)
        long dist2 = dx * dx + dy * dy + dz * dz;
                
        edges.Add((i, j, dist2));
    }
}
        
edges.Sort((a, b) => a.dist2.CompareTo(b.dist2));

int [] parent = new int[n]; // parent array
int [] size = new int[n];

for(int i = 0; i < n; i++)
{
    parent[i] = i;
    size[i] = 1;
}
        
int Find(int x)
{
    if(parent[x] != x)
    {
        parent[x] = Find(parent[x]);
    }
    return parent[x];
}

void Union(int a, int b)
{
    int rootA = Find(a);
    int rootB = Find(b);
    
    if(rootA == rootB) return;
    
    if(size[rootA] < size[rootB])
    {
        int tmp = rootA;
        rootA = rootB;
        rootB = tmp;
    }
    
    parent[rootB] = rootA;
    size[rootA] += size[rootB];
    
}

const int K = 1000;

int connections = 0;

for(int e = 0; e < edges.Count && connections < K; e++)
{
    var edge = edges[e];
    Union(edge.i, edge.j);
    connections++;
}

var componentSize = new Dictionary<int, int>();

for (int i = 0; i < n; i++)
{
    int root = Find(i);                 // find the root of node i

    if (!componentSize.ContainsKey(root))
    {
        componentSize[root] = 0;       // initialize size for this root
    }

    componentSize[root]++;             // increment size for this root
}

var sizes = componentSize.Values.ToList();

sizes.Sort((a, b) => b.CompareTo(a));

int a1 = sizes.Count > 0 ? sizes[0] : 0;
int b1 = sizes.Count > 1 ? sizes[1] : 0;
int c1 = sizes.Count > 2 ? sizes[2] : 0;

long result = (long)a1 * b1 * c1;

Console.WriteLine(result);