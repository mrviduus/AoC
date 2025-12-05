var lines = File.ReadAllLines("input.txt");

var sum = 0;
foreach(var line in lines){
    var digits = line.Select(c => c -'0').ToArray();
    var max = digits.Take(digits.Length - 1).Max();
    var index = Array.IndexOf(digits, max);
    var nextHighest = digits.Skip(index + 1).Max();
    sum += max * 10 + nextHighest;
}
Console.WriteLine(sum);  
