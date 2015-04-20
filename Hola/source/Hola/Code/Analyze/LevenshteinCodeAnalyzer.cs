using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Hola.Code.Analyze
{
    class LevenshteinCodeAnalyzer : CodeAnalyzer
    {
        const decimal alpha1 = 1.6M;
        const decimal alpha2 = 1.15M;

        static int[,] arr = new int[2000, 2000];

        static bool inComment;

        List<long> hashes = new List<long>();

        private long phash(string s)
        {
            long ans = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ' ' || s[i] == '\t' || s[i] == '\n' || s[i] == '\r')
                    continue;
                if (Language[0] == 'c' || Language[0] == 'j') ///C-like
                {
                    if (i < s.Length - 1 && s[i] == '/' && s[i + 1] == '/')
                        break;
                    if (i < s.Length - 1 && s[i] == '/' && s[i + 1] == '*')
                        inComment = true;
                    if (i < s.Length - 1 && s[i] == '*' && s[i + 1] == '/')
                        inComment = false;
                    if (s[i] == '#')
                        break;
                }
                if (Language == "py")
                {
                    if (s[i] == '#')
                        break;
                }
                if (inComment)
                    continue;
                if (char.IsLetter(s[i]))
                {
                    ans *= 257;
                    ans += '0';
                    continue;
                }
                if (char.IsDigit(s[i]))
                {
                    ans *= 257;
                    ans += '0';
                    continue;
                }
                ans *= 257;
                ans += s[i];
            }
            return ans;
        }
        public override void Analyze(string language, string code)
        {
            base.Analyze(language, code);

            inComment = false;
            var lines = code.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var t = phash(line);
                if (t > 0)
                {
                    hashes.Add(t);
                }
            }
        }
        public override decimal Compare(CodeAnalyzer code)
        {
            if (code is LevenshteinCodeAnalyzer)
            {
                var levenshtein = code as LevenshteinCodeAnalyzer;
                var f1 = levenshtein.hashes;
                var f2 = hashes;

                for (int i = 1; i < Math.Max(f1.Count, f2.Count); i++)
                {
                    arr[0, i] = arr[i, 0] = i;
                }
                for (int i = 1; i <= f1.Count; i++)
                {
                    for (int j = 1; j <= f2.Count; j++)
                    {
                        if (i < f1.Count && j < f2.Count)
                        {
                            arr[i, j] = Math.Min(Math.Min(arr[i, j - 1], arr[i - 1, j]) + 1, (arr[i - 1, j - 1] + ((f1[i] != f2[j]) ? 1 : 0)));
                        }
                        else
                        {
                            arr[i, j] = Math.Min(Math.Min(arr[i, j - 1], arr[i - 1, j]) + 1, (arr[i - 1, j - 1] + 1));
                        }
                    }
                }
                decimal ans = arr[f1.Count, f2.Count];
                if (ans * alpha1 < Math.Min(f1.Count, f2.Count))
                    return 1;
                int c = 0;
                for (int i = 0; i < f1.Count; i++)
                {
                    if (f2.Contains(f1[i]))
                    {
                        c++;
                    }
                }
                if (c * alpha2 > f1.Count)
                    return 1;
                c = 0;
                for (int i = 0; i < f2.Count; i++)
                {
                    if (f1.Contains(f2[i]))
                    {
                        c++;
                    }
                }
                if (c * alpha2 > f2.Count)
                    return 1;
                return 0;
            }
            else
            {
                return base.Compare(code);
            }
        }
    }
}
