using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

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

        var p = 1;
        foreach(var code in ht)
        {
            if (p != ht.Count)
            {
                sb.AppendLine(string.Format("\"{0}\",", code));
            }
            else
            {
                sb.AppendLine(string.Format("\"{0}\"", code));
            }
            p++;
        }

        example = example.Replace("//--SIGNATURES", sb.ToString());
        Console.Write(example);
        Console.Error.WriteLine("OK, {0} lines", ht.Count);
    }
}