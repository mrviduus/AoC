var lines = File.ReadAllLines("input.txt");

int width = lines.Max(l => l.Length);

var normalized = lines.Select(l => l.PadRight(width)).ToArray();

long grandTotal = 0;

List<string> currentColumnChars = new List<string>();

for(int col = 0; col < width; col++)
{
    var column = normalized.Select(row => row[col]).ToArray();
        
    bool isEmpty = column.All(c => c == ' ');
    
    if(isEmpty)
    {
        if(currentColumnChars.Count > 0)
        {
            grandTotal += SolveProblemPart2(currentColumnChars);
            
            currentColumnChars.Clear();
        }
    }
    else
    {
        currentColumnChars.Add(new string(column));
    }      
}
        
if(currentColumnChars.Count > 0)
{
    grandTotal += SolveProblemPart2(currentColumnChars);        
}

Console.WriteLine(grandTotal);

static long SolveProblemPart2(List<string> cols)
{
    char operations = cols.Select(col => col[^1]).First(c => c == '+' || c == '*');
    
    var numbers = new List<long>();
    
    foreach(var col in cols)
    {
        string digitsWithSpaces = new string(col.Take(col.Length -1).ToArray());
            
        string trimmed = digitsWithSpaces.Trim();
            
        if(trimmed.Length > 0)
        {
            long value = long.Parse(trimmed);
            
            numbers.Add(value);
        }
            
    }

    long result;

    if(operations == '+')
    {
        result = 0L;
        foreach(var n in numbers)
        {
            result += n;
        }
    }
    else
    {
        result = 1L;
        foreach(var n in numbers)
        {
            result *= n;
        }
    }
    return result;
}
    

