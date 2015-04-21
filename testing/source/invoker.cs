using System;
using System.Diagnostics;
using System.IO;

    class Trash
    {
        public Stream Stream { get; private set; }
        public Trash(Stream s)
        {
            Stream = s;

            async = new AsyncCallback(OnReadComplete);
            buffer = new byte[1024];
        }

        AsyncCallback async;
        byte[] buffer;

        void OnReadComplete(IAsyncResult e)
        {
            try
            {
                var stream = e.AsyncState as Stream;
                var n = stream.EndRead(e);

                Start();
            }
            catch
            {

            }
        }
        public void Start()
        {
            Stream.BeginRead(buffer, 0, buffer.Length, async, Stream);
        }
    }
class Program
{
    static bool error = false;
    static void OnError(string message, params object[] args)
    {
        Console.Error.WriteLine(message, args);
        error = true;
    }

    static int Main(string[] args)
    {
        var invokeDir = args[0];

        var testsDir = args[1];
        var solutionFile = args[2];
        var tl = long.Parse(args[3]);
        var ml = long.Parse(args[4]);


        if (!Directory.Exists(invokeDir))
        {
            OnError("Invoke directory not exist: {0}", invokeDir);
        }
        if (!Directory.Exists(testsDir))
        {
            OnError("Test's directory not exist {0}", testsDir);
        }
        if (!File.Exists(solutionFile))
        {
            OnError("Solution file not exist: {0}", solutionFile);
        }
        if (error) return 1;

        foreach (var file in Directory.GetFiles(invokeDir))
        {
            File.Delete(file);
        }


        foreach (var file in Directory.GetFiles(testsDir))
        {
            File.Copy(file, Path.Combine(invokeDir, Path.GetFileName(file)), true);
        }

        var p = new Process();
        p.StartInfo = new ProcessStartInfo();
        p.StartInfo.WorkingDirectory = invokeDir;
        p.StartInfo.FileName = solutionFile;
        //*
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        //*/
        p.Start();

        new Trash(p.StandardError.BaseStream).Start();
        new Trash(p.StandardOutput.BaseStream).Start();

        do
        {
            if (p.TotalProcessorTime.TotalMilliseconds > tl) return 2;
            if (p.PeakWorkingSet64 > ml) return 3;

        } while (!p.HasExited);


        return 0;
    }
}