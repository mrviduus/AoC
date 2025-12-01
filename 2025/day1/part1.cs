using System;

//configuration

// the dial has numbers from 0 to 99 
const int DialSize = 100;

// the initial position of the arrow is 50, as defined by puzzle.
int currentPosition = 50;

// This counter will store how many times the dial points at 0 after a rotation.
int zeroHitCount = 0;


string [] lines = File.ReadAllLines("input.txt");


foreach (string rawLine in lines){
    if(string.IsNullOrWhiteSpace(rawLine)){
        continue;
    }
    
    //Trim the line to remove leading/trailling 
    string line = rawLine.Trim();
    
    // The first character is the direction: 'L' (left) or 'R' (right).
    char direction = line[0];
    
    // The rest of the string is the distance part, e.g. "68" in "L68".
    string distancePart = line.Substring(1);
    
    // Convert the distance string into an integer value.
    int distance = int.Parse(distancePart);
    
    // Since the dial is circular with 100 positions, we can reduce
    // the distance modulo DialSize to ignore full circles.
    int effectiveDistance = distance % DialSize;
    
    // Apply movement based on the direction.
    if(direction == 'L'){
        // Moving left means going toward smaller numbers.
        // We subtract the distance from the current position.
        currentPosition = (currentPosition - effectiveDistance) % DialSize;
        
        // In C#, negative values keep the minus sign after modulo,
        // for example: -1 % 100 == -1, so we need to fix that.
        
        if(currentPosition < 0){
            // Add DialSize to wrap back into the [0, 99] range.
            currentPosition += DialSize;
            
        }
        
    }else if(direction == 'R'){
        // Moving right means going toward larger numbers.
        // We add the distance and wrap via modulo.
        currentPosition = (currentPosition + effectiveDistance) % DialSize;
    }else{
        // If the input contains invalid direction characters (not 'L' or 'R'),
        // we simply skip this command.
        continue;
    }
    
    // After applying this rotation, we check if the dial points at 0.
    if(currentPosition == 0){
        // If yes, increment our zero-hit counter.
        zeroHitCount ++;
        
    }
    
    
}
// ------------------------------
// OUTPUT RESULT
// ------------------------------

// Print the total number of times the dial pointed at 0.
// This is the "password" defined by the puzzle.
Console.WriteLine(zeroHitCount);

/*
Big-O analysis:

Let N be the number of rotation commands (lines in input.txt).

- We read all lines once => O(N).
- For each line we do:
  - a few string operations: Trim, Substring, int.Parse (bounded length),
  - constant amount of arithmetic and comparisons.
  So each line is processed in O(1).

Total time complexity: O(N).
Extra space complexity: O(1) beyond the array of input lines.
(The array from File.ReadAllLines itself is O(N) to store the input, which is unavoidable
 if we choose to load all lines at once.)
*/
