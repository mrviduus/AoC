            // Read all lines from "input.txt" into an array of strings
            // Each line in the file represents one row of the grid
            string[] lines = File.ReadAllLines("input.txt"); 

            // Determine how many rows we have in the grid
            int rows = lines.Length;                          

            // Determine how many columns we have
            // If there are no rows, columns count is 0, otherwise it's the length of the first line
            int cols = rows > 0 ? lines[0].Length : 0;        

            // Create a 2D jagged char array to hold a mutable grid
            // We want char[][] so we can change cells to '.' when we remove rolls
            char[][] grid = new char[rows][];                 

            // Copy each string line into a char array so we can modify characters
            for (int r = 0; r < rows; r++)                    
            {
                grid[r] = lines[r].ToCharArray();             // Convert string row to char array
            }

            // Define the 8 directions around each cell as pairs of (dr, dc)
            // dr = delta row, dc = delta column
            int[][] directions = new int[][]
            {
                new[] { -1, -1 },  // up-left
                new[] { -1,  0 },  // up
                new[] { -1,  1 },  // up-right
                new[] {  0, -1 },  // left
                new[] {  0,  1 },  // right
                new[] {  1, -1 },  // down-left
                new[] {  1,  0 },  // down
                new[] {  1,  1 },  // down-right
            };

            // This variable will store the total number of removed paper rolls
            int totalRemoved = 0;                             

            // We now repeat the process:
            // 1) find all accessible rolls
            // 2) remove them
            // 3) stop when none are accessible
            while (true)                                      // Loop until we break explicitly
            {
                // Create a list to store all coordinates of rolls that should be removed this round
                List<(int r, int c)> toRemove = new List<(int r, int c)>(); 

                // Scan the entire grid to find all accessible rolls '@'
                for (int r = 0; r < rows; r++)                // Iterate over each row
                {
                    for (int c = 0; c < cols; c++)            // Iterate over each column
                    {
                        // If this cell is not a roll of paper, we skip it
                        if (grid[r][c] != '@')                
                            continue;                         // Move to next cell

                        // Count how many neighboring cells contain '@'
                        int neighborAtCount = 0;              // Number of neighboring rolls

                        // Check all 8 neighbors using our directions array
                        foreach (int[] dir in directions)     
                        {
                            int dr = dir[0];                  // Row offset for this direction
                            int dc = dir[1];                  // Column offset for this direction

                            int nr = r + dr;                  // Neighbor row index
                            int nc = c + dc;                  // Neighbor column index

                            // Check if neighbor is outside the vertical bounds of the grid
                            if (nr < 0 || nr >= rows)         
                                continue;                     // Skip this neighbor

                            // Check if neighbor is outside the horizontal bounds of the grid
                            if (nc < 0 || nc >= cols)         
                                continue;                     // Skip this neighbor

                            // If the neighbor cell contains a roll '@', increment the count
                            if (grid[nr][nc] == '@')          
                            {
                                neighborAtCount++;            // We found one neighboring roll
                            }
                        }

                        // According to the problem, a roll is accessible if
                        // there are fewer than 4 neighboring rolls of paper
                        if (neighborAtCount < 4)              
                        {
                            // We mark this cell for removal in this iteration
                            toRemove.Add((r, c));             
                        }
                    }
                }

                // If in this iteration we found no rolls to remove, we are done
                if (toRemove.Count == 0)                      
                {
                    // No more accessible rolls, so we break out of the loop
                    break;                                   
                }

                // Otherwise, we remove all marked rolls at once
                foreach ((int r, int c) in toRemove)          
                {
                    // Turn the roll '@' into empty space '.'
                    grid[r][c] = '.';                        

                    // Increase the total number of removed rolls
                    totalRemoved++;                           
                }
            }

            // After all iterations, we print the total number of removed rolls
            Console.WriteLine(totalRemoved); 