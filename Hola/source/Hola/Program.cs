using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hola.Structures;
using Hola.Code;
using Hola.Code.Analyze;
using System.IO;
using System.Diagnostics;

namespace Hola
{
    class Program
    {
        static void InitIO()
        {
//#if !DEBUG
            Console.SetIn(new StreamReader("input.txt"));
            Console.SetOut(new StreamWriter("output.txt"));
//#endif
        }
        static decimal Percent(decimal value, decimal max)
        {
            if (max == 0)
            {
                return 1;
            }
            return value / max * 100;
        }
        static void OnProgress(long value, long max)
        {
            Console.Error.WriteLine("{0}/{1} ({2}%)", value, max, Percent(value, max));
        }
        static void Main(string[] args)
        {
            InitIO();

            var n = int.Parse(Console.ReadLine());
            Console.Error.WriteLine("Input files: {0}", n);

            var graph = new Graph<string>();
            var comparer = new CodeComparer(new CodeAnalyzerBuilder());
            var files = new string[n];

            for (var i = 0; i < n; i++)
            {
                files[i] = Console.ReadLine();

                var language = Path.GetExtension(files[i]);
                var code = File.ReadAllText(files[i]);

                var codeAnalyzer = new SuffixTreeCodeAnalyzer(language, code);

                graph.AddVertex(files[i]);
                comparer.Register(files[i], language, code);
            }

            var maximum = (long)(n) * (long)(n - 1) / 2;
            var progress = (long)0;

            OnProgress(progress, maximum);
            for (var i = 0; i < n; i++)
            {
                for (var j = i + 1; j < n; j++)
                {
                    if(++progress % 1000 == 0)
                    {
                        OnProgress(progress, maximum);
                    }

                    if (files[i].Contains('-') && files[j].Contains('-'))
                    {
                        var id1 = files[i].Substring(0, files[i].IndexOf('-'));
                        var id2 = files[j].Substring(0, files[j].IndexOf('-'));

                        if (id1 == id2) continue;
                    }

                    decimal compare = comparer.Compare(files[i], files[j]);


                    if (compare > 0.33M)
                    {
                        Console.Error.WriteLine("{0} | {1} -> {2:0.00}%", files[i], files[j], compare * 100);
                        graph.AddEdge(files[i], files[j]);
                    }
                }
            }

            OnProgress(progress, maximum);

            var res = from c in graph.GetConnectedComponents()
                      where c.Count >= 2
                      select c;

            Console.WriteLine(res.Count());
            Console.Error.WriteLine(res.Count());
            foreach(var g in res)
            {
                foreach(var file in g.Verticies)
                {
                    Console.Write(file + " ");
                    Console.Error.Write(file + " ");
                }
                Console.WriteLine();
                Console.Error.WriteLine();
            }
            Console.Out.Dispose();
        }
    }
}
