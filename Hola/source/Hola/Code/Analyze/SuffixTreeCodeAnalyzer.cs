using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hola.Structures;

namespace Hola.Code.Analyze
{
    class SuffixTreeCodeAnalyzer : CodeAnalyzer
    {
        const int MultilinePrice = 500;

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

        public override decimal Compare(CodeAnalyzer code)
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

                decimal a = length;
                decimal b = ParsedCodeLength + (CodeLines.Length - 1) * MultilinePrice;

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
