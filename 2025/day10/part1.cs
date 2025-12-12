using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {

        var lines = File.ReadAllLines("input.txt")
                        .Where(l => !string.IsNullOrWhiteSpace(l))
                        .ToArray();

        long totalPresses = 0;

        foreach (var line in lines)
        {
            var (targetMask, numLights, buttonMasks) = ParseMachine(line);
            int presses = MinPressesBfs(targetMask, numLights, buttonMasks);
            if (presses < 0)
            {
                // Теоретически такого быть не должно (по условию),
                // но на всякий случай.
                throw new InvalidOperationException(
                    $"Target state is unreachable for line: {line}");
            }

            totalPresses += presses;
        }

        Console.WriteLine(totalPresses);
    }
    
    static (int targetMask, int numLights, int[] buttonMasks) ParseMachine(string line)
    {

        var patternMatch = Regex.Match(line, @"\[(.*?)\]");
        if (!patternMatch.Success)
            throw new FormatException("Cannot find light pattern in line: " + line);

        string pattern = patternMatch.Groups[1].Value; 
        int numLights = pattern.Length;

        if (numLights == 0)
            throw new FormatException("Empty light pattern in line: " + line);
        
        int targetMask = 0;
        for (int i = 0; i < numLights; i++)
        {
            char c = pattern[i];
            if (c == '#')
            {
                targetMask |= (1 << i);
            }
            else if (c == '.')
            {
            }
            else
            {
                throw new FormatException($"Unexpected char '{c}' in pattern: " + line);
            }
        }
        
        var buttonMatches = Regex.Matches(line, @"\((.*?)\)");
        var buttonMasks = new List<int>();

        foreach (Match m in buttonMatches)
        {
            string inside = m.Groups[1].Value.Trim(); 
            if (string.IsNullOrEmpty(inside))
            {
                continue;
            }

            int mask = 0;
            
            string[] parts = inside.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (var p in parts)
            {
                if (!int.TryParse(p.Trim(), out int idx))
                    throw new FormatException($"Cannot parse lamp index '{p}' in line: {line}");

                if (idx < 0 || idx >= numLights)
                    throw new FormatException(
                        $"Lamp index {idx} out of range [0, {numLights - 1}] in line: {line}");
                
                mask |= (1 << idx);
            }

            buttonMasks.Add(mask);
        }
        

        if (buttonMasks.Count == 0)
            throw new FormatException("No buttons found in line: " + line);

        return (targetMask, numLights, buttonMasks.ToArray());
    }


    static int MinPressesBfs(int targetMask, int numLights, int[] buttonMasks)
    {
        if (targetMask == 0)
            return 0;
        
        int totalStates = 1 << numLights; 


        var dist = new int[totalStates];
        for (int i = 0; i < totalStates; i++)
            dist[i] = -1;

        var queue = new Queue<int>();


        dist[0] = 0;
        queue.Enqueue(0);

        while (queue.Count > 0)
        {
            int state = queue.Dequeue();
            int d = dist[state];
            
            if (state == targetMask)
                return d;
            foreach (int buttonMask in buttonMasks)
            {
                int next = state ^ buttonMask;

                if (dist[next] == -1)
                {
                    dist[next] = d + 1;
                    queue.Enqueue(next);
                }
            }
        }
        
        return -1;
    }
}