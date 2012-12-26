using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EqualFilesDetector
{
    internal class FileExplorer
    {
        public static IEnumerable<KeyValuePair<string, List<string>>> GetEqualFiles(string path)
        {
            var files = new Task<IDictionary<string, List<string>>>(HandleDirectory, path);
            files.Start();
            files.Wait();

            return files.Result.Where(pair => pair.Value.Count>1);
        }

        private static IDictionary<string, List<string>> HandleDirectory(object arg)
        {
            var path = (string) arg;

            var innerDirectories = Directory.EnumerateDirectories(path);
            var handlingDirectories = new List<Task>();
            foreach (var directoryPath in innerDirectories)
            {
                var newTask = new Task<IDictionary<string, List<string>>>(HandleDirectory, directoryPath);
                handlingDirectories.Add(newTask);
                newTask.Start();
            }

            var result = new Dictionary<string, List<string>>();
            var innerFiles = Directory.EnumerateFiles(path);
            foreach (var innerFile in innerFiles)
            {
                using (var fileReader = new FileStream(innerFile, FileMode.Open, FileAccess.Read))
                {
                    using (var md5 = new MD5CryptoServiceProvider())
                    {
                        var hash = md5.ComputeHash(fileReader);

                        AddFile(result, Encoding.UTF8.GetString(hash), innerFile);
                    }
                }
            }

            Task.WaitAll(handlingDirectories.ToArray());
            foreach (var handlingDirectory in handlingDirectories)
            {
                var taskResult = ((Task<IDictionary<string, List<string>>>) handlingDirectory).Result;

                foreach (var pair in taskResult)
                {
                    foreach (var res in pair.Value)
                    {
                        AddFile(result, pair.Key, res);
                    }
                }
            }

            return result;

        }

        private static void AddFile(Dictionary<string, List<string>> result, string hash, string filePath)
        {
            if (result.ContainsKey(hash))
                result[hash].Add(filePath);
            else
                result.Add(hash, new List<string>{filePath});
        }
    }
}