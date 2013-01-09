using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EqualFilesDetector1
{
    class ParallelEqualsFilesDetector
    {
        private readonly ConcurrentQueue<string> _allFilesQueue;
        private readonly Dictionary<long, List<string>> _equalSizeFiles;
        private readonly Dictionary<string, List<string>> _equalFiles;
        private readonly ConcurrentQueue<string> _hashEqualFiles; 

        public ParallelEqualsFilesDetector()
        {
            _allFilesQueue = new ConcurrentQueue<string>();
            _equalSizeFiles = new Dictionary<long, List<string>>();
            _equalFiles = new Dictionary<string, List<string>>();
            _hashEqualFiles = new ConcurrentQueue<string>();
        }

        public IEnumerable<List<string>> Start(string rootFolder)
        {
            bool isFilesDetectorTaskDone = false;
            bool isFilesHandlerTaskDone = false;
            var fileHandler = Task.Factory.StartNew(() =>
            {
                while (! (isFilesDetectorTaskDone && _allFilesQueue.IsEmpty))
                {
                    string file;
                    if (_allFilesQueue.TryDequeue(out file))
                        ProcessFile(file);
                    else
                        Thread.Sleep(1);
                }
            });

            var hashHandler = Task.Factory.StartNew(() =>
            {
                while (!(_hashEqualFiles.IsEmpty && isFilesHandlerTaskDone))
                {
                    string file;
                    if (_hashEqualFiles.TryDequeue(out file))
                        AddFileToDictionary(_equalFiles, GetHash(file).Result, file);
                    else
                        Thread.Sleep(1);
                }
            });

            foreach (var file in SafeWalk.EnumerateFiles(rootFolder, "*", SearchOption.AllDirectories))
                _allFilesQueue.Enqueue(file);

            isFilesDetectorTaskDone = true;
            fileHandler.Wait();
            isFilesHandlerTaskDone = true;
            hashHandler.Wait();

            return _equalFiles.Values.Where(list => list.Count > 1);
        }
        
        private void ProcessFile(object arg)
        {
            var processedFile = (string) arg;
            try
            {
                var fileInfo = new FileInfo(processedFile);
                AddFileToDictionary(_equalSizeFiles, fileInfo.Length, processedFile);
                var currentList = _equalSizeFiles[fileInfo.Length];
                if (currentList.Count == 2)
                {
                    foreach (var fileInList in currentList)
                        _hashEqualFiles.Enqueue(fileInList);
                    //    AddFileToDictionary(_equalFiles, GetHash(fileInList).Result, fileInList);
                }
                else if (currentList.Count > 2)
                {
                    _hashEqualFiles.Enqueue(processedFile);
                    //AddFileToDictionary(_equalFiles, GetHash(processedFile).Result, processedFile);
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private async Task<string> GetHash(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                const int batchSize = 4096;
                var md5 = new MD5CryptoServiceProvider();
                var block = new byte[batchSize];
                while (true)
                {
                    var bytesRead = await fileStream.ReadAsync(block, 0, block.Length);
                    if (bytesRead < batchSize)
                    {
                        md5.TransformFinalBlock(block, 0, bytesRead);
                        return Encoding.UTF8.GetString(md5.Hash);
                    }
                    md5.TransformBlock(block, 0, bytesRead, null, 0);
                }
            }
        }

        private void AddFileToDictionary<TKey>(Dictionary<TKey, List<string>> dictionary, TKey key, string value)
        {
            List<string> temp;
            if (!dictionary.TryGetValue(key, out temp))
                temp = dictionary[key] = new List<string>();
            temp.Add(value);
        }
    }
}
