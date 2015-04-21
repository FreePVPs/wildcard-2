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
        static void Main(string[] args)
        {
            InitIO();

            var n = int.Parse(Console.ReadLine());

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

            for (var i = 0; i < n; i++)
            {
                for (var j = i + 1; j < n; j++)
                {
                    decimal compare = comparer.Compare(files[i], files[j]);


                    Console.Error.WriteLine("{0} | {1} -> {2:0.00}%", files[i], files[j], compare * 100);
                    if (compare > 0.32M)
                    {
                        graph.AddEdge(files[i], files[j]);
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
