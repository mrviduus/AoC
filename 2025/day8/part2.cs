// Read all lines from input file
        var lines = File.ReadAllLines("input.txt");              // each line is "x,y,z"

        // List of 3D points
        var points = new List<(int x, int y, int z)>();          // store (x, y, z) for each junction box

        // Parse each line
        foreach (var line in lines)
        {
            var parts = line.Split(',');                         // split "x,y,z" into three parts

            int x = int.Parse(parts[0]);                         // parse X coordinate
            int y = int.Parse(parts[1]);                         // parse Y coordinate
            int z = int.Parse(parts[2]);                         // parse Z coordinate

            points.Add((x, y, z));                               // add tuple to list
        }

        int n = points.Count;                                    // total number of points

        // Build all edges with squared distances
        var edges = new List<(int i, int j, long dist2)>();      // i, j are indices of points

        for (int i = 0; i < n; i++)                              // outer loop over first index
        {
            for (int j = i + 1; j < n; j++)                      // inner loop over second index
            {
                long dx = (long)points[i].x - points[j].x;       // delta X
                long dy = (long)points[i].y - points[j].y;       // delta Y
                long dz = (long)points[i].z - points[j].z;       // delta Z

                long dist2 = dx * dx + dy * dy + dz * dz;        // squared distance

                edges.Add((i, j, dist2));                        // add edge to list
            }
        }

        // Sort edges by distance ascending
        edges.Sort((a, b) => a.dist2.CompareTo(b.dist2));        // closest edges first

        // ---------- Disjoint Set Union (Union-Find) ----------

        int[] parent = new int[n];                               // parent of each node
        int[] size = new int[n];                                 // size of component for each root

        // Initialize DSU
        for (int i = 0; i < n; i++)
        {
            parent[i] = i;                                       // each node is its own parent
            size[i] = 1;                                         // each component has size 1
        }

        // Find with path compression
        int Find(int x)
        {
            if (parent[x] != x)                                  // if x is not a root
            {
                parent[x] = Find(parent[x]);                     // compress path to root
            }
            return parent[x];                                    // return root of x
        }

        // Union by size; return true if a merge actually happened
        bool Union(int a, int b)
        {
            int rootA = Find(a);                                 // find root of a
            int rootB = Find(b);                                 // find root of b

            if (rootA == rootB) return false;                    // already in the same component

            if (size[rootA] < size[rootB])                       // ensure rootA is larger
            {
                int tmp = rootA;                                 // swap roots if needed
                rootA = rootB;
                rootB = tmp;
            }

            parent[rootB] = rootA;                               // attach smaller tree under bigger
            size[rootA] += size[rootB];                          // update size of merged component

            return true;                                         // merge was successful
        }

        int componentsCount = n;                                 // start with n separate components

        int lastI = -1;                                          // index of first point in last merge
        int lastJ = -1;                                          // index of second point in last merge

        // Process edges until all points become a single component
        for (int e = 0; e < edges.Count; e++)
        {
            var edge = edges[e];                                 // get current edge

            if (Union(edge.i, edge.j))                           // if this union merged two components
            {
                componentsCount--;                               // one merge reduces number of components

                if (componentsCount == 1)                        // all points now in one circuit
                {
                    lastI = edge.i;                              // remember index of first junction
                    lastJ = edge.j;                              // remember index of second junction
                    break;                                       // we can stop, this is the last needed connection
                }
            }
        }

        // Get X coordinates of these two junction boxes
        int x1 = points[lastI].x;                                // X coordinate of first junction
        int x2 = points[lastJ].x;                                // X coordinate of second junction

        // Compute the product of X coordinates
        long result = (long)x1 * x2;                             // multiply as long for safety

        // Print the result
        Console.WriteLine(result); 