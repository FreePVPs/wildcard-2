using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

    struct TestResult
    {
        public TestResult(string name, int result)
        {
            Name = name;
            Result = result;
        }
        static string[] msgs = { "unk", "WA", "PE", "CF", "TL", "ML" };

        public string Name;
        public int Result;
        public override string ToString()
        {
            if (Result < 0)
            {
                return string.Format("{0} - {1}", Name, msgs[-Result]);
            }
            else
            {
                return string.Format("{0} - OK, {1} points", Name, Result);
            }
        }
    }
class Program
{
    const string InvokeDir = "invoke0";
    const long TL = 30 * 1000;
    const long ML = 256 * 1024 * 1024;

    static string TestsFile = "tests.txt";
    static string FileName;
    static int Execute(string executableFile, params object[] args)
    {
        var sb = new StringBuilder();

        foreach (var arg in args)
        {
            sb.Append("\"" + arg + "\"" + " ");
        }

        var p = new Process();
        p.StartInfo = new ProcessStartInfo();
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.FileName = executableFile;
        p.StartInfo.Arguments = sb.ToString();
        p.Start();

        p.WaitForExit();

        return p.ExitCode;
    }

    static TestResult Check(string test)
    {
        // tl - 1
        // ml - 2
        var runResult = Execute("invoker.exe", InvokeDir, test, FileName, TL, ML);

        if (runResult != 0)
        {
            return new TestResult(test, -runResult - 2);
        }

        runResult = Execute("checker.exe", test + ".a", Path.Combine(InvokeDir, "output.txt"));

        return new TestResult(test, runResult);
    }
    static void Main(string[] args)
    {
        FileName = args[0];
        if (args.Length > 1)
        {
            TestsFile = args[1];
        }

        if (!File.Exists(FileName))
        {
            Console.WriteLine("{0} not exist", FileName);
            return;
        }
        if (!File.Exists(TestsFile))
        {
            Console.WriteLine("{0} not exist", TestsFile);
            return;
        }

        var tests = File.ReadAllLines(TestsFile);

        long sum = 0;
        foreach (var test in tests)
        {
            var res = Check(test);
            sum += Math.Max(0, res.Result);
            Console.WriteLine(res);
        }
        Console.WriteLine("{0} points", sum);
    }
}