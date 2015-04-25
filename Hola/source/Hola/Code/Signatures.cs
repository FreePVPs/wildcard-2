using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hola.Structures;
using System.Reflection;
using System.IO.Compression;
using System.IO;

namespace Hola.Code
{
    class Signatures
    {
        private const string SignaturesDB = "IgnoredSignatures";

        private Bor<char> Bor = new Bor<char>();
        public int Count { get; private set; }


        private static string Decompress(string compressedText)
        {
            byte[] gzBuffer = Convert.FromBase64String(compressedText);
            { } using (MemoryStream ms = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

                byte[] buffer = new byte[msgLength];

                ms.Position = 0;
                { } using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }

                return Encoding.ASCII.GetString(buffer);
            }
        }
        public Signatures()
        {
            var db = Type.GetType(SignaturesDB, false);
            if (db != null)
            {
                var field = db.GetField("Signatures");

                var archive = (string)field.GetValue(null);
                if(archive != null)
                {
                    var decompressed = Decompress(archive);
                    var tr = new StringReader(decompressed);

                    var n = int.Parse(tr.ReadLine());
                    if (n != 0)
                    {
                        var signatures = tr.ReadLine().Split(' ');
                        for (var i = 0; i < n; i++)
                        {
                            var signature = signatures[i];
                            Register(Encoding.ASCII.GetString(Convert.FromBase64String(signature)));
                        }
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
