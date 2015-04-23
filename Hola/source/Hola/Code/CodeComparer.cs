using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hola.Code.Analyze;

namespace Hola.Code
{
    class CodeAnalyzerBuilder
    {
        public virtual CodeAnalyzer[] Make()
        {
            return new[]
            {
                new CodeAnalyzer(),
                new SuffixTreeCodeAnalyzer(),
                /new LevenshteinCodeAnalyzer()
            };
        }
    }
    class CodeComparer
    {
        public CodeAnalyzerBuilder CodeAnalyzerBuilder { get; private set; }
        public CodeComparer(CodeAnalyzerBuilder codeAnalyzerBuilder)
        {
            CodeAnalyzerBuilder = codeAnalyzerBuilder;
        }

        Dictionary<string, CodeAnalyzer[]> files = new Dictionary<string, CodeAnalyzer[]>();
        public void Register(string fileName, string language, string code)
        {
            var analyzers = CodeAnalyzerBuilder.Make();
            foreach(var analyzer in analyzers)
            {
                analyzer.Analyze(language, code);
                analyzer.FileName = fileName;
            }

            files.Add(fileName, analyzers);
        }

        public decimal Compare(string file1, string file2)
        {
            decimal res = 0;

            var analyzers1 = files[file1];
            var analyzers2 = files[file2];

            for(var i = 0; i < analyzers1.Length; i++)
            {
                res = Math.Max(res, analyzers1[i].Compare(analyzers2[i]));
            }

            return res;
        }
    }
}
