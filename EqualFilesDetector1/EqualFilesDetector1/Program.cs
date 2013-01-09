using System;
using System.Diagnostics;

namespace EqualFilesDetector1
{
    class Program
    {
        static void Main(string[] args)
        {
            //const string beginFolderPath = @"c:/windows";
            const string beginFolderPath = @"d:/1/";
            var sw = Stopwatch.StartNew();
            //var equlFilesDetector = new EqualFilesDetecotr(beginFolderPath);
            //var result = equlFilesDetector.Start();
            var parallelEqualsFilesDetector = new ParallelEqualsFilesDetector();
            var result = parallelEqualsFilesDetector.Start(beginFolderPath);
            foreach (var batch in result)
            {
                foreach (var fileName in batch)
                {
                    
                }
                //    Console.WriteLine(fileName);
                //Console.WriteLine("\n\n\n");
            }
            Console.WriteLine(sw.ElapsedMilliseconds.ToString("#.##"));
            Console.Read();
        }
    }
}
