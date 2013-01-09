using System;
using System.Diagnostics;

namespace EqualFilesDetector1
{
    class Program
    {
        static void Main(string[] args)
        {
            const string root = @"c:/windows";

            Console.WriteLine("Looking for equal files in directory '{0}'", root);

            var sw = Stopwatch.StartNew();
            //var equlFilesDetector = new EqualFilesDetecotr(beginFolderPath);
            //var result = equlFilesDetector.DetectEqualFiles();
            var parallelEqualsFilesDetector = new ParallelEqualsFilesDetector();
            var result = parallelEqualsFilesDetector.DetectEqualFiles(root);
            Console.WriteLine(sw.ElapsedMilliseconds.ToString("#.##"));
            Console.Read();

        }
    }
}
