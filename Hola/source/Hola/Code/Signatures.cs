using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hola.Structures;
using System.Reflection;

namespace Hola.Code
{
    class Signatures
    {
        private const string SignaturesDB = "IgnoredSignatures";

        private Bor<char> Bor = new Bor<char>();
        public int Count { get; private set; }

        public Signatures()
        {
            var db = Type.GetType(SignaturesDB, false);
            if (db != null)
            {
                var field = db.GetField("Signatures");

                var signatures = (string[])field.GetValue(null);
                if(signatures != null)
                {
                    foreach (var signature in signatures)
                    {
                        Register(Encoding.ASCII.GetString(Convert.FromBase64String(signature)));
                    }
                }
            }

            Console.Error.WriteLine("Registered {0} signatures", Count);
        }

        public void Register(IEnumerable<string> signatures)
        {
            foreach(var signature in signatures)
            {
                Register(signature);
            }
        }
        public void Register(string signature)
        {
            Bor.Push(signature.ToCharArray(), 0);
            Count++;
        }

        public string FormatCode(string code)
        {
            var withoutWhiteSpace = new List<char>(code.Length);
            foreach(var ch in code)
            {
                if (!char.IsWhiteSpace(ch))
                {
                    withoutWhiteSpace.Add(ch);
                }
            }

            var withoutWhiteSpaceArray = withoutWhiteSpace.ToArray();
            var ignored = new bool[withoutWhiteSpace.Count];

            var deep = 0;
            for(var i = 0; i < withoutWhiteSpace.Count; i++)
            {
                deep = Math.Max(deep, Bor.GetDeep(withoutWhiteSpaceArray, i));
                if(deep > 0)
                {
                    ignored[i] = true;
                    deep--;
                }
            }

            var res = new StringBuilder();
            var p = 0;
            foreach (var ch in code)
            {
                if (char.IsWhiteSpace(ch))
                {
                    res.Append(ch);
                }
                else
                {
                    if (!ignored[p])
                    {
                        res.Append(ch);
                    }
                    p++;
                }
            }

            return res.ToString();
        }
    }
}
