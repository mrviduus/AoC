using System;

//configuration

// the dial has numbers from 0 to 99 
const int DialSize = 100;

// the initial position of the arrow is 50, as defined by puzzle.
int currentPosition = 50;

// This counter will store how many times the dial points at 0 after a rotation.
int zeroHitCount = 0;


string [] lines = File.ReadAllLines("input.txt");


// Loop through every command line from the input file.
foreach(string stringRaw in lines){
    
    // If the line is null/empty/whitespace, we skip it.
    if (string.IsNullOrWhiteSpace(stringRaw))
    {
        continue; // Move on to the next command.
    }
    
    // Trim whitespace just in case (e.g., "  L68  ").
    string line = stringRaw.Trim();
    
    // The first character of the line is the direction: 'L' or 'R'.
    char direction = line[0];
    
    // The rest of the string after the first character is the distance.
    // Example: "L68" -> "68".
    string distancePart = line.Substring(1);
    
    // Parse the distance as a potentially large number.
    // We use 'long' in case the puzzle gives very large values.
    
    long distance = long.Parse(distancePart);
    
    // ------------------------------
    // COUNT ZERO HITS DURING THIS ROTATION (PART TWO LOGIC)
    // ------------------------------

    // Compute how many times this rotation causes the dial to point at 0
    // considering each single click step.
    
    long hitsThisRotation = CountZeroHits(currentPosition, direction, distance, DialSize);
    
    // Add that to our global counter.
    zeroHitCount += (int)hitsThisRotation;
    
    // ------------------------------
    // UPDATE FINAL POSITION ON THE DIAL
    // ------------------------------

    // For the final position, we only care about distance modulo DialSize,
    // because moving a full circle (100 steps) returns us to the same position.
    
    int effectiveDistance  = (int)(distance % DialSize);
    
    if(direction == 'L'){
        // Move left: subtract the distance.
        int newPosition = (currentPosition - effectiveDistance) % DialSize;
        
        
        // In C#, modulo with negative numbers keeps the sign (e.g. -1 % 100 == -1),
        // so if we get a negative value, we wrap it back into [0, 99].
        if (newPosition < 0)
        {
            newPosition += DialSize;
        }
        
        // Update current position after applying this rotation.
        currentPosition = newPosition;
        
    }else if(direction == 'R'){
        // Move right: add the distance and wrap using modulo DialSize.
        int newPosition = (currentPosition + effectiveDistance) % DialSize;
        // This will never be negative, so no extra fix is needed here.
        currentPosition = newPosition;
    }else{
        // If direction is not 'L' or 'R', we ignore this line.
        // Advent of Code inputs are usually clean, so this is just a safeguard.
        continue;
    }
    
    
    
}
    
// ------------------------------
// OUTPUT THE RESULT
// ------------------------------

// Print the total number of times the dial pointed at 0
// during all clicks of all rotations.
Console.WriteLine(zeroHitCount);

/*
Big-O analysis:

Let N be the number of rotation commands (lines in input.txt).

- File.ReadAllLines reads all lines once: O(N).
- For each line, we:
  - trim the string,
  - parse a character and a number,
  - call CountZeroHits (which does O(1) work),
  - update the current position in O(1).
So total time is O(N).

Extra space complexity (algorithm-wise) is O(1):
- we store just a few integers and longs.
The input array from ReadAllLines is O(N), which is unavoidable if we load all lines at once.
*/

// ------------------------------
// HELPER FUNCTION
// ------------------------------

// This helper computes how many times a rotation of "distance" steps
// causes the dial to point at 0, assuming the dial moves 1 step per click.
// We do this in O(1) time using modular arithmetic, without simulating each step.
static long CountZeroHits(int startPosition, char direction, long distance, int dialSize)
{
    // If there is no movement or negative distance, we treat it as 0 hits.
    if (distance <= 0)
    {
        return 0;
    }

    // First, we normalize the start position to [0, dialSize - 1],
    // just in case, although in this puzzle it should already be valid.
    int start = ((startPosition % dialSize) + dialSize) % dialSize;

    long firstStepToZero;

    if (direction == 'R')
    {
        // Moving right: positions are (start + k) % dialSize.
        // We want (start + k) % dialSize == 0.
        // That means k ≡ -start (mod dialSize) => k ≡ dialSize - start (mod dialSize).
        firstStepToZero = (dialSize - start) % dialSize;

        // If start == 0, then (dialSize - start) % dialSize == 0,
        // but we cannot hit 0 at step 0; the first time will be after one full circle.
        if (firstStepToZero == 0)
        {
            firstStepToZero = dialSize;
        }
    }
    else if (direction == 'L')
    {
        // Moving left: positions are (start - k) % dialSize.
        // We want (start - k) % dialSize == 0.
        // That means start - k ≡ 0 (mod dialSize) => k ≡ start (mod dialSize).
        firstStepToZero = start % dialSize;

        // Again, if start == 0, then firstStepToZero == 0,
        // so the first time we hit 0 is after one full circle.
        if (firstStepToZero == 0)
        {
            firstStepToZero = dialSize;
        }
    }
    else
    {
        // For any invalid direction, we say there are no zero hits.
        return 0;
    }

    // If the total distance is shorter than the first step where we would reach 0,
    // then we never reach 0 in this rotation.
    if (distance < firstStepToZero)
    {
        return 0;
    }

    // Otherwise, we hit 0 the first time at step firstStepToZero,
    // and then every dialSize steps after that.
    // Number of hits = 1 (for the first time) + how many full dialSize-steps fit
    // into the remaining distance.
    long remainingDistanceAfterFirstHit = distance - firstStepToZero;

    long extraHits = remainingDistanceAfterFirstHit / dialSize;

    long totalHits = 1 + extraHits;

    return totalHits;
}