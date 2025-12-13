using System.Numerics;

var input = File.ReadAllLines("input.txt").Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

var buttons = new List<List<int[]>>();
var joltages = new List<int[]>();

foreach (var line in input)
{
    var linePart = line.Split(' ');
    var lineButtons = new List<int[]>();

    foreach (var part in linePart)
    {
        if (part.StartsWith('('))
        {
            string buttonPart = part.Substring(1, part.Length - 2);
            lineButtons.Add(buttonPart.Split(',').Select(int.Parse).ToArray());
        }
        else if (part.StartsWith('{'))
        {
            string joltagePart = part.Substring(1, part.Length - 2);
            joltages.Add(joltagePart.Split(',').Select(int.Parse).ToArray());
        }
    }
    buttons.Add(lineButtons);
}

long totalPresses = 0;

for (int m = 0; m < joltages.Count; m++)
{
    int[] target = joltages[m];
    var machineButtons = buttons[m];
    int rows = target.Length;
    int cols = machineButtons.Count;

    // Build augmented matrix
    var origA = new BigInteger[rows, cols + 1];
    for (int r = 0; r < rows; r++)
    {
        for (int c = 0; c < cols; c++) origA[r, c] = 0;
        origA[r, cols] = target[r];
    }
    for (int b = 0; b < cols; b++)
        foreach (int r in machineButtons[b])
            origA[r, b] = 1;

    // Copy for RREF
    var A = (BigInteger[,])origA.Clone();

    // Gaussian elimination to RREF
    int pivotRow = 0;
    var pivotCols = new List<int>();

    for (int col = 0; col < cols && pivotRow < rows; col++)
    {
        int pr = -1;
        for (int r = pivotRow; r < rows; r++)
            if (A[r, col] != 0) { pr = r; break; }
        if (pr < 0) continue;

        for (int c = 0; c <= cols; c++)
            (A[pivotRow, c], A[pr, c]) = (A[pr, c], A[pivotRow, c]);

        pivotCols.Add(col);
        BigInteger pv = A[pivotRow, col];

        for (int r = 0; r < rows; r++)
        {
            if (r == pivotRow) continue;
            BigInteger factor = A[r, col];
            if (factor == 0) continue;
            for (int c = 0; c <= cols; c++)
                A[r, c] = A[r, c] * pv - factor * A[pivotRow, c];
        }
        pivotRow++;
    }

    // Free variables are columns not in pivotCols
    var freeVars = Enumerable.Range(0, cols).Except(pivotCols).ToList();

    // Search over free variables
    long? best = null;
    int maxFree = target.Max() + 50;

    void Search(int idx, long[] freeVals)
    {
        if (idx == freeVars.Count)
        {
            // Compute pivot variables
            var sol = new long[cols];
            for (int i = 0; i < freeVars.Count; i++)
                sol[freeVars[i]] = freeVals[i];

            bool valid = true;
            for (int i = pivotCols.Count - 1; i >= 0 && valid; i--)
            {
                int r = i, pc = pivotCols[i];
                BigInteger rhs = A[r, cols];
                for (int c = pc + 1; c < cols; c++)
                    rhs -= A[r, c] * sol[c];
                BigInteger coef = A[r, pc];
                if (coef == 0 || rhs % coef != 0) { valid = false; break; }
                BigInteger val = rhs / coef;
                if (val < 0) { valid = false; break; }
                sol[pc] = (long)val;
            }

            if (valid)
            {
                long sum = sol.Sum();
                if (best == null || sum < best) best = sum;
            }
            return;
        }

        for (long v = 0; v <= maxFree; v++)
        {
            freeVals[idx] = v;
            Search(idx + 1, freeVals);
        }
    }

    if (freeVars.Count == 0)
    {
        // No free vars - direct solution
        var sol = new long[cols];
        bool valid = true;
        for (int i = 0; i < pivotCols.Count && valid; i++)
        {
            int r = i, pc = pivotCols[i];
            BigInteger rhs = A[r, cols], coef = A[r, pc];
            if (coef == 0 || rhs % coef != 0) valid = false;
            else
            {
                BigInteger val = rhs / coef;
                if (val < 0) valid = false;
                else sol[pc] = (long)val;
            }
        }
        if (!valid) throw new Exception($"No solution for machine {m}");
        best = sol.Sum();
    }
    else if (freeVars.Count <= 3)
    {
        Search(0, new long[freeVars.Count]);
    }
    else
    {
        throw new Exception($"Too many free vars ({freeVars.Count}) for machine {m}");
    }

    if (best == null) throw new Exception($"No valid solution for machine {m}");
    totalPresses += best.Value;
}

Console.WriteLine($"Day 10 Part 2: {totalPresses}");
