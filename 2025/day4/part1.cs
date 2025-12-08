// Read all lines from input.txt as the grid
            var lines = File.ReadAllLines("input.txt"); // each line is one row of the map

            int rows = lines.Length;                    // total number of rows
            int cols = rows > 0 ? lines[0].Length : 0;  // total number of columns

            // Directions for 8 neighbors: (dr, dc)
            int[][] directions = new int[][]
            {
                new[] {-1, -1}, // up-left
                new[] {-1,  0}, // up
                new[] {-1,  1}, // up-right
                new[] { 0, -1}, // left
                new[] { 0,  1}, // right
                new[] { 1, -1}, // down-left
                new[] { 1,  0}, // down
                new[] { 1,  1}, // down-right
            };

            int accessibleCount = 0;                     // total count of accessible rolls

            // Iterate over each cell in the grid
            for (int r = 0; r < rows; r++)              // loop over rows
            {
                for (int c = 0; c < cols; c++)          // loop over columns
                {
                    char cell = lines[r][c];            // current character at (r, c)

                    if (cell != '@')                    // skip if not a roll of paper
                        continue;

                    int neighborAtCount = 0;            // number of '@' neighbors

                    // Check all 8 neighbors
                    foreach (var dir in directions)     // for each direction
                    {
                        int dr = dir[0];                // row delta
                        int dc = dir[1];                // column delta

                        int nr = r + dr;                // neighbor row
                        int nc = c + dc;                // neighbor column

                        // Boundaries check
                        if (nr < 0 || nr >= rows)       // outside vertically?
                            continue;

                        if (nc < 0 || nc >= cols)       // outside horizontally?
                            continue;

                        if (lines[nr][nc] == '@')       // if neighbor is a roll
                        {
                            neighborAtCount++;          // increase count
                        }
                    }

                    // Accessible if fewer than four neighboring rolls
                    if (neighborAtCount < 4)            // strictly less than 4
                    {
                        accessibleCount++;              // count this roll
                    }
                }
            }

    
Console.WriteLine(accessibleCount);