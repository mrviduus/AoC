// Read all lines from "input.txt" (each line contains one red tile coordinate "x,y").
        string[] lines = File.ReadAllLines("input.txt");

        // Create a list to store all red tiles as (x, y) pairs.
        List<(int x, int y)> points = new List<(int x, int y)>();

        // Loop through each line from the input file.
        foreach (string line in lines)
        {
            // Trim the line to remove any leading/trailing spaces.
            string trimmed = line.Trim();

            // If the line is empty (for example, a blank line at the end), skip it.
            if (string.IsNullOrEmpty(trimmed))
            {
                // Just continue to the next line if this one is empty.
                continue;
            }

            // Split the line by comma to separate x and y coordinates.
            string[] parts = trimmed.Split(',');

            // Parse the first part as x coordinate (integer).
            int x = int.Parse(parts[0]);

            // Parse the second part as y coordinate (integer).
            int y = int.Parse(parts[1]);

            // Add the parsed point into our list of red tiles.
            points.Add((x, y));
        }

        // If there are no points, the maximum rectangle area is 0.
        if (points.Count == 0)
        {
            // Print 0 and exit the program.
            Console.WriteLine(0);
            return;
        }

        // If there is only one red tile, the rectangle degenerates to a 1x1 square.
        // According to the inclusive definition, width = 1 and height = 1, area = 1.
        if (points.Count == 1)
        {
            // Print 1 and exit, because there is only one possible "rectangle".
            Console.WriteLine(1);
            return;
        }

        // Variable to keep track of the maximum area found so far.
        long maxArea = 0;

        // Optional: we can also store which two points gave this maximum area
        // for debugging or curiosity, but it's not required by the problem.
        (int x, int y) bestA = points[0]; // Initialize with some default point.
        (int x, int y) bestB = points[1]; // Initialize with some default point.

        // Outer loop: pick the first corner of the rectangle.
        for (int i = 0; i < points.Count; i++)
        {
            // Get the i-th point as the first corner.
            var p1 = points[i];

            // Inner loop: pick the second corner of the rectangle.
            // We start from i + 1 to avoid duplicate pairs and i == j.
            for (int j = i + 1; j < points.Count; j++)
            {
                // Get the j-th point as the second corner.
                var p2 = points[j];

                // Compute the absolute difference in x coordinates.
                int dx = Math.Abs(p1.x - p2.x);

                // Compute the absolute difference in y coordinates.
                int dy = Math.Abs(p1.y - p2.y);

                // Width of the rectangle is dx + 1 (inclusive counting of tiles).
                long width = dx + 1L;

                // Height of the rectangle is dy + 1 (inclusive counting of tiles).
                long height = dy + 1L;

                // Area is width times height.
                long area = width * height;

                // If this area is larger than any area we've seen so far,
                // update maxArea and remember this pair of points.
                if (area > maxArea)
                {
                    maxArea = area;
                    bestA = p1;
                    bestB = p2;
                }
            }
        }

        // Print only the maximum area, because this is what the puzzle asks.
        Console.WriteLine(maxArea);

        // If you also want to see which corners gave this area, uncomment this:
        // Console.WriteLine($"Best rectangle between ({bestA.x},{bestA.y}) and ({bestB.x},{bestB.y})");

        // Big-O complexity (time and space) for this solution:
        // Time  : O(n^2), because we compare every pair of points.
        // Space : O(n),   because we store all points in a list.