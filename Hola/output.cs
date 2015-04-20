using Hola.Code.Analyze;
using Hola.Code;
using Hola.Structures;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System;

namespace Hola
{
    class Program
    {
        static void Main(string[] args)
        {
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
                    double compare = sources[i].Compare(sources[j]);
                    
#if DEBUG
                    Console.Error.WriteLine("{0} | {1} -> {2:0.00}%", files[sources[i]], files[sources[j]], compare * 100);
#endif

                    if (compare > 0.70)
                    {
                        graph.AddEdge(sources[i], sources[j]);
                    }
                }
            }

            var res = from c in graph.GetConnectedComponents()
                      where c.Count >= 2
                      select c;

            Console.WriteLine(res.Count());
            foreach(var g in res)
            {
                foreach(var code in g.Verticies)
                {
                    Console.Write(files[code] + " ");
                }
                Console.WriteLine();
            }
        }
    }
}

namespace Hola.Code
{
    class CodeFormatProvider
    {
        public virtual string TextBegin { get; set; } // "
        public virtual string TextEnd { get; set; } // "
        public virtual string IgnoredTextBegin { get; set; } // @"
        public virtual string IgnoredTextEnd { get; set; } // "
        public virtual string Begin { get; set; } // {
        public virtual string End { get; set; } // }
        public virtual string[] CommentBegin { get; set; } // { //, /// }
        public virtual string[] MultilineCommentBegin { get; set; } // { /* }
        public virtual string[] MultilineCommentEnd { get; set; } // { */ }
        public virtual string[] IDELine { get; set; } // #
        public virtual string[] Cycles { get; set; } // { for, while, foreach }
        public virtual string[] IgnoredWords { get; set; } // { try/catch }
        public virtual string[] IgnoredCodeLines { get; set; } // { using, import }
        public virtual bool UseEndOfLine { get; set; } // true
        public virtual string EndOfLine { get; set; } // ;
    }
}

namespace Hola.Code
{
    static class CodeParser
    {
        public static bool PrefixIs(this string line, string prefix)
        {
            return line.PrefixIs(prefix, 0);
        }
        public static bool PrefixIs(this string line, string prefix, int index)
        {
            if (index + prefix.Length > line.Length) return false;

            for(var i = 0; i < prefix.Length; i++)
            {
                if (line[index + i] != prefix[i]) return false;
            }
            return true;
        }

        public static bool IsWord(this string str)
        {
            foreach(var ch in str)
            {
                if (!char.IsLetterOrDigit(ch) && ch != '_') return false;
            }
            return true;
        }

        private static SortedSet<char> IgnoredChars = new SortedSet<char>()
        {
            '{', '}', '(', ')', ';', ':'
        };
        private static SortedSet<string> IgnoredPrefixes = new SortedSet<string>()
        {
            "using", "#", "import"
        };
        private static string CodeLineHash(this string codeLine)
        {
            var res = new StringBuilder();
            var lastIsWord = false;

            foreach(var ch in codeLine)
            {
                var isWord = char.IsLetterOrDigit(ch) || ch == '_';

                if (isWord && lastIsWord)
                {
                    continue;
                }
                lastIsWord = isWord;

                if(!char.IsWhiteSpace(ch) && !IgnoredChars.Contains(ch))
                {
                    res.Append(isWord ? '0' : ch);
                }
            }

            var chars = res.ToString().ToCharArray();
            Array.Sort(chars);

            return new string(chars);
        }
        private static bool IsIgnored(string codeLine)
        {
            codeLine = codeLine.Trim();
            if (string.IsNullOrWhiteSpace(codeLine)) return true;

            foreach(var ignoredPrefix in IgnoredPrefixes)
            {
                if (ignoredPrefix.Length <= codeLine.Length)
                {
                    if (codeLine.IndexOf(ignoredPrefix) == 0) return true;
                }
            }
            return false;
        }
        public static string[] ParseCode(this string code, string language)
        {
            var codeLines = new List<string>();

            var lines = code.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();

            // Remove comments
            bool commented = false;
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                for (var j = 0; j < line.Length; j++)
                {
                    if (line.PrefixIs("/*"))
                    {
                        commented = true;
                        j++;
                        continue;
                    }
                    if (line.PrefixIs("*/"))
                    {
                        commented = false;
                        j++;
                        continue;
                    }
                    if (line.PrefixIs("//")) break;

                    if (!commented) sb.Append(line[j]);
                }
                lines[i] = sb.ToString();
                sb.Clear();
            }

            // Splitting by ;
            foreach (var line in lines)
            {
                int l = 0;
                int r = 0;
                while ((r = line.IndexOf(';', l)) != -1)
                {
                    var codeLine = line.Substring(l, r - l + 1);
                    l = r + 1;
                    if (!IsIgnored(codeLine))
                        codeLines.Add(codeLine);
                }
                r = line.Length - 1;

                if ((r - l + 1) > 1)
                {
                    var codeLine = line.Substring(l, r - l + 1);
                    if (!IsIgnored(codeLine))
                        codeLines.Add(codeLine);
                }
            }
            
            for(var i = 0; i < codeLines.Count; i++)
            {
                codeLines[i] = codeLines[i].CodeLineHash();
            }

            return codeLines.ToArray();
        }
    }
}

namespace Hola.Code.Analyze
{
    class CodeAnalyzer
    {
        public CodeAnalyzer()
        {
            Code = string.Empty;
            Language = string.Empty;
        }
        public CodeAnalyzer(string language, string code)
        {
            Analyze(language, code);
        }

        public virtual string Language { get; set; }
        public virtual string Code { get; set; }
        public virtual void Analyze(string language, string code)
        {
            Code = code;
            Language = language;
        }
        public virtual double Compare(CodeAnalyzer code)
        {
            if (code.Code == Code) return 1;
            else return 0;
        }
    }
}

namespace Hola.Code.Analyze
{
    class SuffixTreeCodeAnalyzer : CodeAnalyzer
    {
        const int MultilinePrice = 2;

        public SuffixTreeCodeAnalyzer() : base()
        {

        }
        public SuffixTreeCodeAnalyzer(string language, string code) : base(language, code)
        {

        }

        string[] CodeLines;
        SuffixTree<string> SuffixTree;
        int ParsedCodeLength = 0;

        public override void Analyze(string language, string code)
        {
            base.Analyze(language, code);

            CodeLines = code.ParseCode(language);
            SuffixTree = new SuffixTree<string>();

            for (var i = 0; i < CodeLines.Length; i++)
            {
                SuffixTree.Push(CodeLines, i);
                ParsedCodeLength += CodeLines[i].Length;
            }
        }

        public override double Compare(CodeAnalyzer code)
        {
            if (code is SuffixTreeCodeAnalyzer)
            {
                var suffixCode = code as SuffixTreeCodeAnalyzer;
                if (suffixCode.CodeLines.Length > CodeLines.Length) return code.Compare(this);

                // TODO : Remove?
                var deeps = new List<int>();
                var lengs = new List<int>();

                long length = 0;

                for (var i = 0; i < suffixCode.CodeLines.Length;)
                {
                    int deep = SuffixTree.GetDeep(suffixCode.CodeLines, i);
                    if (deep == 0)
                    {
                        i++;
                        continue;
                    }
                    else
                    {
                        var leng = MultilinePrice * (deep - 1);
                        for (var j = 0; j < deep; j++)
                        {
                            leng += suffixCode.CodeLines[j + i].Length;
                        }

                        deeps.Add(deep);
                        lengs.Add(leng);

                        length += leng;

                        i += deep;
                    }
                }

                double a = length;
                double b = ParsedCodeLength + (CodeLines.Length - 1) * MultilinePrice;

                if (b == 0)
                {
                    return a == 0 ? 1 : 0;
                }

                return a / b;
            }
            else
            {
                return base.Compare(code);
            }
        }
    }
}

namespace Hola.Structures
{
    class Graph<T>
    {
        public Graph()
        {

        }
        public Graph(IEnumerable<T> verticies)
        {
            foreach(var vertex in verticies)
            {
                AddVertex(vertex);
            }
        }

        Dictionary<T, HashSet<T>> VerticiesDict = new Dictionary<T, HashSet<T>>();
        public int Count
        {
            get
            {
                return VerticiesDict.Count;
            }
        }
        public IEnumerable<T> Verticies
        {
            get
            {
                foreach(var pair in VerticiesDict)
                {
                    yield return pair.Key;
                }
            }
        }

        public bool Contains(T vertex)
        {
            return VerticiesDict.ContainsKey(vertex);
        }
        public void AddVertex(T vertex)
        {
            if (Contains(vertex)) return;

            VerticiesDict.Add(vertex, new HashSet<T>());
        }
        public void AddEdge(T vertex1, T vertex2)
        {
            if (!Contains(vertex1)) throw new Exception("vertex1 not exist");
            if (!Contains(vertex2)) throw new Exception("vertex2 not exist");

            VerticiesDict[vertex1].Add(vertex2);
            VerticiesDict[vertex2].Add(vertex1);
        }
        public void RemoveEdge(T vertex1, T vertex2)
        {
            if (!Contains(vertex1)) throw new Exception("vertex1 not exist");
            if (!Contains(vertex2)) throw new Exception("vertex2 not exist");

            VerticiesDict[vertex1].Remove(vertex2);
            VerticiesDict[vertex2].Remove(vertex1);
        }

        public Graph<T>[] GetConnectedComponents()
        {
            var used = new HashSet<T>();

            var r = new List<Graph<T>>();
            var q = new Queue<T>();
            foreach (var vertex in Verticies)
            {
                if (!used.Contains(vertex))
                {
                    var g = new Graph<T>();

                    used.Add(vertex);
                    g.AddVertex(vertex);

                    q.Enqueue(vertex);
                    while (q.Count > 0)
                    {
                        var v = q.Dequeue();

                        foreach(var v2 in VerticiesDict[v])
                        {
                            if (!used.Contains(v2))
                            {
                                used.Add(v2);

                                g.AddVertex(v2);
                                g.AddEdge(v, v2);

                                q.Enqueue(v2);
                            }
                        }
                    }

                    r.Add(g);
                }

            }
            return r.ToArray();
        }
    }
}

namespace Hola.Structures
{
    class SuffixTree<T>
    {
        Dictionary<T, SuffixTree<T>> Nodes = new Dictionary<T, SuffixTree<T>>();

        public void Push(T[] array, int index)
        {
            if (index >= array.Length) return;

            SuffixTree<T> node;
            if (!Nodes.TryGetValue(array[index], out node))
            {
                node = new SuffixTree<T>();
                Nodes[array[index]] = node;
            }

            node.Push(array, index + 1);
        }
        public int GetDeep(T[] array, int index)
        {
            if (index >= array.Length) return 0;

            SuffixTree<T> node;
            if (!Nodes.TryGetValue(array[index], out node))
            {
                return 0;
            }

            return node.GetDeep(array, index + 1) + 1;
        }
    }
}

