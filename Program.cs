using System.IO;

static void RunFile(string path)
{
    byte[] bytes = File.ReadAllBytes(path);
    
}

static void RunPrompt()
{
    for (;;)
    {
       Console.Write("> ");
       var line = Console.ReadLine();
       if (line.Length == 0)
           break;
       Console.WriteLine(line);
    }
}

if (args.Length > 1)
{
    Console.WriteLine("Usage: cslox [script]");
    return;
}
else if (args.Length == 1)
{
    Console.WriteLine(args[0]);
}
else
{
    RunPrompt();
}