var lines = File.ReadAllLines("input.txt");

long sum = 0;

foreach(var line in lines){
    
    var digits = line.Select(c => c - '0').ToArray();
    // We must turn on exactly 12 batteries in each bank.
    int k = 12;
    
    int n = digits.Length;
    
    // How many digits we are allowed to remove
    // so that we end up with exactly k digits.
    int toDrop = n - k;
    
    // This list will act as a stack to store the chosen digits.
    var stack = new List<int>();
    
    foreach(var d in digits){
        
        while(stack.Count > 0 && toDrop > 0 && stack[^1] < d){
            
            // While:
            // 1) there is at least one digit in the stack,
            // 2) we still can drop digits (toDrop > 0),
            // 3) the last chosen digit is smaller than the current one,
            // we remove the last digit to make room for a larger digit.
            stack.RemoveAt(stack.Count - 1);
            toDrop--;
        }
        stack.Add(d);
        
    }
    if(stack.Count > k){
        // If we still have more than k digits (because no more drops left),
        // we just cut the extra digits from the end.
        stack = stack.Take(k).ToList();
    }
    
    long value = 0;
    
    foreach(var d in stack){
        value = value * 10 + d;
    }
    
    sum += value;
    
}
Console.WriteLine(sum);