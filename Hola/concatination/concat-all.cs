using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        var c = 0;
        var code = new StringBuilder();
        var usings = new List<string>();

        var pattern = string.Empty;
        while ((pattern = Console.ReadLine()) != ":end:")
        {
            Console.Error.WriteLine("> " + Path.Combine(Environment.CurrentDirectory, pattern));
            foreach(var file in Directory.GetFiles(Environment.CurrentDirectory, pattern))
            {
                var lines = File.ReadAllLines(file);
                foreach(var line in lines)
                {
                    if (line.Trim().IndexOf("using") == 0)
                    {
                        usings.Add(line.Trim());
                    }
                    else
                    {
                        code.AppendLine(line);
                    }
                }

                Console.Error.WriteLine(file);
                c++;
            }
        }
        Console.Error.WriteLine("Files: {0}", c);

        usings.Sort();
        var last = string.Empty;
        for(var i = 0; i < usings.Count; i++)
        {
            if (usings[i] != last)
            {
                Console.WriteLine(usings[i]);
            }
            last = usings[i];
        }

        Console.WriteLine(code);
    }
}