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
#if !DEBUG
            Console.SetIn(new StreamReader("input.txt"));
            Console.SetOut(new StreamWriter("output.txt"));
#endif
        }
        static void Main(string[] args)
        {
            InitIO();

            var n = int.Parse(Console.ReadLine());

            var graph = new Graph<SuffixTreeCodeAnalyzer>();
            var sources = new List<SuffixTreeCodeAnalyzer>();
            var files = new Dictionary<SuffixTreeCodeAnalyzer, string>();

            for (var i = 0; i < n; i++)
            {
                var file = Console.ReadLine();

                var language = Path.GetExtension(file);
                var code = File.ReadAllText(file);

                var codeAnalyzer = new SuffixTreeCodeAnalyzer(language, code);

                graph.AddVertex(codeAnalyzer);
                sources.Add(codeAnalyzer);
                files.Add(codeAnalyzer, file);
            }

            for (var i = 0; i < n; i++)
            {
                for (var j = i + 1; j < n; j++)
                {
                    decimal compare = sources[i].Compare(sources[j]);


                    if (compare > 0.32M)
                    {
                        Console.Error.WriteLine("{0} | {1} -> {2:0.00}%", files[sources[i]], files[sources[j]], compare * 100);
                        graph.AddEdge(sources[i], sources[j]);
                    }
                }
            }

            var res = from c in graph.GetConnectedComponents()
                      where c.Count >= 2
                      select c;

            Console.WriteLine(res.Count());
            Console.Error.WriteLine(res.Count());
            foreach(var g in res)
            {
                foreach(var code in g.Verticies)
                {
                    Console.Write(files[code] + " ");
                }
                Console.WriteLine();
            }
            Console.Out.Dispose();
        }
    }
}
