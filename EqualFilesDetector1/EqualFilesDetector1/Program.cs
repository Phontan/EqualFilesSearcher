using System;

namespace EqualFilesDetector1
{
    class Program
    {
        static void Main(string[] args)
        {
            const string beginFolderPath = @"c:/python27";
            var startTime = DateTime.Now.Ticks;
            //var equlFilesDetector = new EqualFilesDetecotr(beginFolderPath);
            //var result = equlFilesDetector.Start();
            var parallelEqualsFilesDetector = new ParallelEqualsFilesDetector();
            var result = parallelEqualsFilesDetector.Start(beginFolderPath);
            foreach (var batch in result)
            {
                foreach (var fileName in batch)
                {
                    Console.WriteLine(fileName);
                }
                Console.WriteLine("\n\n\n");
            }
            Console.WriteLine(DateTime.Now.Ticks - startTime);
        }
    }
}
