var rawInput = File.ReadAllText("input.txt");

var parts = rawInput.Trim().Split(new string[] {"\r\n\r\n", "\n\n"},
                StringSplitOptions.RemoveEmptyEntries);

var rangeSections = parts[0];
var idsSections = parts[1];

var ranges = new List<(long start, long end)>();

foreach(var line in rangeSections.Split(new [] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries))
{
    var trimmed = line.Trim(); //remove spaces
    if(trimmed.Length == 0)
    {
       continue; 
    }
    
    var partsLine = trimmed.Split('-');// split "3-5" into ["3", "5"]
    long start = long.Parse(partsLine[0]);
    long end = long.Parse(partsLine[1]);
        
    ranges.Add((start, end));
}

var ids = new List<long>();
foreach(var line in idsSections.Split(new [] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries))
{
    var trimmed = line.Trim();
    if(trimmed.Length == 0)
    {
        continue;
    }
    long id = long.Parse(trimmed);
    ids.Add(id);
}
        
int freshCount = 0;
foreach(var id in ids)
{
    bool isFresh = false;
    foreach(var (start, end) in ranges)
    {
        if(id >= start && id <= end) //check if id is inside [start, end]
        {
            isFresh = true; //mark as fresh
            break;
        }
    }
        
    if(isFresh)
    {
        freshCount++;
                
    }
}
        
Console.WriteLine(freshCount);