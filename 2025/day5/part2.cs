// Read entire input file as one big string
        var rawInput = File.ReadAllText("input.txt"); // read puzzle input

        // Trim spaces/newlines from both ends and split into two parts by empty line
        var parts = rawInput
            .Trim()                                       // remove trailing spaces/newlines
            .Split(new string[] { "\r\n\r\n", "\n\n" },   // split by double newline (Windows/Unix)
                   StringSplitOptions.RemoveEmptyEntries);

        // First part contains the fresh ID ranges
        var rangesSection = parts[0]; // we ignore the second section in part 2

        // Prepare a list to store all ranges as tuples (start, end)
        var ranges = new List<(long start, long end)>(); // use long to be extra safe with big numbers

        // Split ranges section into individual lines
        var lines = rangesSection.Split(new[] { "\r\n", "\n" },
                                        StringSplitOptions.RemoveEmptyEntries); // split by line breaks

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim(); // remove extra spaces around the line
            if (line.Length == 0)      // skip empty lines if any
                continue;

            var partsLine = line.Split('-'); // split "3-5" into ["3", "5"]

            // Parse start and end as long integers
            long start = long.Parse(partsLine[0]); // convert first part to long
            long end = long.Parse(partsLine[1]);   // convert second part to long

            ranges.Add((start, end));             // add parsed range to the list
        }

        // If there are no ranges, then nothing can be fresh
        if (ranges.Count == 0)
        {
            Console.WriteLine(0); // print 0 as answer
            return;              // exit program
        }

        // Sort ranges by start, and then by end if start is the same
        ranges.Sort((a, b) =>
        {
            int cmp = a.start.CompareTo(b.start); // compare by start first
            if (cmp != 0)
                return cmp;                       // if starts differ, use that result
            return a.end.CompareTo(b.end);        // otherwise compare by end
        });

        // Initialize current merged interval using the first sorted range
        long curStart = ranges[0].start; // start of the current merged interval
        long curEnd = ranges[0].end;     // end of the current merged interval

        // This will accumulate the total count of unique fresh ingredient IDs
        long totalFreshCount = 0; // answer variable

        // Go through all remaining ranges starting from index 1
        for (int i = 1; i < ranges.Count; i++)
        {
            var (start, end) = ranges[i]; // deconstruct tuple into start and end

            if (start <= curEnd)
            {
                // Current range overlaps with the merged interval [curStart, curEnd]
                // We extend the merged interval to include this range
                if (end > curEnd)
                {
                    curEnd = end; // update curEnd to the maximum end
                }
            }
            else
            {
                // No overlap: we finalize the previous merged interval
                // Add its inclusive length to the total count
                totalFreshCount += (curEnd - curStart + 1); // count all integers in [curStart, curEnd]

                // Start a new merged interval from the current range
                curStart = start; // new current start
                curEnd = end;     // new current end
            }
        }

        // After processing all ranges, finalize the last merged interval
        totalFreshCount += (curEnd - curStart + 1); // add the last interval length

        // Print the total number of fresh ingredient IDs
        Console.WriteLine(totalFreshCount); // 