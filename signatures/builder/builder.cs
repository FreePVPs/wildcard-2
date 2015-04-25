using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;

public class MainClass
{
    static StringBuilder codeFormating = new StringBuilder();
    public static string FormatCode(string code)
    {
        foreach (var ch in code)
        {
            if (!char.IsWhiteSpace(ch))
            {
                codeFormating.Append(ch);
            }
        }

        code = codeFormating.ToString();
        codeFormating.Clear();

        return Convert.ToBase64String(Encoding.ASCII.GetBytes(code));
    }
    public static string Compress(string text)
    {
        byte[] buffer = Encoding.ASCII.GetBytes(text);
        MemoryStream ms = new MemoryStream();
        { } using (GZipStream zip = new GZipStream(ms, CompressionLevel.Optimal, true))
        {
            zip.Write(buffer, 0, buffer.Length);
        }

        ms.Position = 0;
        MemoryStream outStream = new MemoryStream();

        byte[] compressed = new byte[ms.Length];
        ms.Read(compressed, 0, compressed.Length);

        byte[] gzBuffer = new byte[compressed.Length + 4];
        System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
        System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
        return Convert.ToBase64String(gzBuffer);
    }

    public static void Main(string[] args)
    {
        var sb = new StringBuilder();
        var ht = new HashSet<string>();

        var example = File.ReadAllText(args[0]);

        var pattern = string.Empty;
        while ((pattern = Console.ReadLine()) != ":end:")
        {
            foreach(var file in Directory.GetFiles(Environment.CurrentDirectory, pattern))
            {
                var code = FormatCode(File.ReadAllText(file));
                ht.Add(code);
            }
        }

        sb.AppendLine(ht.Count.ToString());
        foreach(var code in ht)
        {
            sb.Append(code + " ");
        }
        var compressed = Compress(sb.ToString());

        example = example.Replace("//--SIGNATURES", compressed);
        Console.Write(example);
        Console.Error.WriteLine("OK, {0} lines", ht.Count);
    }
}