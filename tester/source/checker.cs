using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    // -5 - ML
    // -4 - TL
    // -3 - CF
    // -2 - PE
    // -1 - WA
    // 0+ - OK, points

    static HashSet<string> ParseFile(TextReader tr)
    {
        var n = int.Parse(tr.ReadLine());

        var res = new HashSet<string>();
        for (var i = 0; i < n; i++)
        {
            var pairs = tr.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Array.Sort(pairs);
            for (var j = 0; j < pairs.Length; j++)
            {
                for (var t = j + 1; t < pairs.Length; t++)
                {
                    // : not using in path
                    res.Add(pairs[j] + ":" + pairs[t]);
                }
            }
        }

        return res;
    }
    static int Main(string[] args)
    {
        var correctPath = args[0];
        var outputPath = args[1];

        if (!File.Exists(correctPath))
        {
            Console.Error.WriteLine("Correct path not exist");
            return -3;
        }
        if (!File.Exists(outputPath))
        {
            return -2;
        }

        try
        {
            var output = ParseFile(new StringReader(File.ReadAllText(outputPath)));
            var correct = ParseFile(new StringReader(File.ReadAllText(correctPath)));

            if (correct.Intersect(output).Count() != output.Count)
            {
                return -1;
            }

            decimal a = output.Count;
            decimal b = correct.Count;

            if (b == 0)
            {
                if (a == 0)
                {
                    return 100;
                }
                else
                {
                    return 0;
                }
            }

            return (int)((a * 100) / b);
        }
        catch
        {
            return -2;
        }
    }
}
