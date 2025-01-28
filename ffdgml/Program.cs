using Dgml;
using System.Diagnostics;

if (args.Length < 3 && !Debugger.IsAttached)
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  ffdgml inputfile outputfile successparagraph");
    Console.WriteLine("e.g.");
    Console.WriteLine("  ffdgml 40-Dead-of-Night.txt 40-Dead-of-Night.dgml 400");
    return;
}

var inFile = string.Empty;
var outFile = string.Empty;
var successParagraph = string.Empty;
if (Debugger.IsAttached)
{
    inFile = @"..\..\..\..\resources\txt\40-Dead-of-Night.txt";
    outFile = @"..\..\..\..\resources\dgml\40-Dead-of-Night.dgml";
    successParagraph = "400";
}
else
{
    inFile = args[0];
    outFile = args[1];
    successParagraph = args[2];
}

if (!File.Exists(inFile))
{
    Console.WriteLine("inputfile does not exist");
    return;
}

if (!Int32.TryParse(successParagraph, out var successParagraphEntry))
{
    Console.WriteLine("successparagraph could not be parsed");
    return;
}

var lines = File.ReadAllLines(inFile);

var paragraphs = new List<Paragraph>();
var currentParagraph = new Paragraph();
var line = 0;
var newParagraph = true;
while (line < lines.Length)
{
    if (newParagraph)
    {
        newParagraph = false;
        var paragraph = Convert.ToInt32(lines[line]);
        currentParagraph = new Paragraph
        {
            Id = paragraph
        };
        paragraphs.Add(currentParagraph);
        Console.WriteLine($"Processing paragraph {paragraph}");
    }
    else if (string.IsNullOrWhiteSpace(lines[line]))
    {
        Console.WriteLine($"  Added {currentParagraph.Links.Count} links");
        newParagraph = true;
    }
    else
    {
        var paragraph = Convert.ToInt32(lines[line]);
        currentParagraph.Links.Add(paragraph);
    }
    line++;
}
Console.WriteLine($"  Added {currentParagraph.Links.Count} links");

Console.WriteLine("-----");
Console.WriteLine($"Processed {paragraphs.Count} paragraphs ");
Console.WriteLine($"Instant deaths: {paragraphs.Count(w => w.Links.Count == 0) - 1}");

var builder = new DgmlBuilder();


foreach (var paragraph in paragraphs)
{
    var id = paragraph.Id;
    if (!builder.Nodes.Any(a => a.Id == Convert.ToString(id)))
    {
        var start = paragraph.Id == 1;
        var colour = start ? "00ff00" : string.Empty;

        var end = paragraph.Links.Count == 0;
        colour = end ? "ff0000" : colour;

        var success = paragraph.Id == successParagraphEntry;
        colour = success ? "00ff00" : colour;

        builder.Nodes.Add(new Node(Convert.ToString(id), $"Paragraph{id}", colour));
    }

    foreach (var link in paragraph.Links)
    {
        var l = new Link(Convert.ToString(id), Convert.ToString(link));
        builder.Links.Add(l);
    }
}

builder.Save(outFile);

class Paragraph
{
    public int Id { get; set; }
    public List<int> Links { get; set; } = [];
}