using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EqualFilesDetector1
{
    public static class SafeWalk
    {
        public static IEnumerable<string> EnumerateFiles(string path)
        {
            double progress = 0;
            return RecursiveEnumerateFiles(path, 100, ref progress);
        }

        private static IEnumerable<string> RecursiveEnumerateFiles(string path, double folderProgress, ref double progress)
        {
            try
            {
                var dirFiles = Directory.GetFiles(path);
                var dirDirectories = Directory.GetDirectories(path);
                var dirsAndFiles = dirFiles.Length + dirDirectories.Length;

                if (dirsAndFiles == 0)
                {
                    progress += folderProgress;
                    return Enumerable.Empty<string>();
                }

                var currentFolderProgress = folderProgress / dirsAndFiles;

                var innerFiles =new List<string>();
                foreach (var directory in dirDirectories)
                {
                    innerFiles.AddRange(RecursiveEnumerateFiles(directory, currentFolderProgress, ref progress));
                }
                
                progress += dirFiles.Length*currentFolderProgress;
                Console.WriteLine(progress);

                return dirFiles.Union(innerFiles);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Enumerable.Empty<string>();
            }
        }
    }
}