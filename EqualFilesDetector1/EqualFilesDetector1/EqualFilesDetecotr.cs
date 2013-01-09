using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace EqualFilesDetector1
{
    internal class EqualFilesDetecotr
    {
        private readonly string _rootFolder;

        public EqualFilesDetecotr(string beginFolderPath)
        {
            _rootFolder = beginFolderPath;
        }

        public IEnumerable<List<string>> Start()
        {
            var allFiles = SafeWalk.EnumerateFiles(_rootFolder, "*", SearchOption.AllDirectories);
            return HandleAllFiles(allFiles);
        }

        private IEnumerable<List<string>> HandleAllFiles(IEnumerable<string> arg)
        {
            var allFiles = arg;

            var equalSizeDictionary = new Dictionary<long, List<string>>();
            foreach (var fileInfo in allFiles.Select(file => new FileInfo(file)))
            {
                AddFileToDictionary(equalSizeDictionary, fileInfo.Length, fileInfo.FullName);
            }

            var equalSizeFiles = equalSizeDictionary.Where(pair => pair.Value.Count > 1).SelectMany(batch => batch.Value);
            var equalFilesDictionary = new Dictionary<byte[], List<string>>();
            foreach (var file in equalSizeFiles)
            {
                try
                {
                    using (var fileReader = new FileStream(file, FileMode.Open, FileAccess.Read))
                    using (var md5 = new MD5CryptoServiceProvider())
                    {
                        var hash = md5.ComputeHash(fileReader);
                        AddFileToDictionary(equalFilesDictionary, hash, file);
                    }
                }
                catch (IOException exception)
                {
                    continue;
                }
            }

            var equalFiles = equalSizeDictionary.Where(pair => pair.Value.Count > 1).Select(pair => pair.Value);

            return equalFiles;
        }

        private void AddFileToDictionary<TKey> (Dictionary<TKey, List<string>> dictionary, TKey key, string value)
        {   
            List<string> temp;
            if (!dictionary.TryGetValue(key, out temp))
                temp = dictionary[key] = new List<string>();
            temp.Add(value);
        }
    }
}