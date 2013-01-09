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
        private ConcurrentQueue<string> _allFilesQueue;
        private ConcurrentQueue<string> _equalHashesQueue;
        private Dictionary<long, List<string>> _equalSizeFiles;
        private Dictionary<string, List<string>> _equalHashFiles;

        private volatile bool _isFilesHandlerTaskDone;
        private volatile bool _isFilesDetectorTaskDone;


        public List<string>[] DetectEqualFiles(string rootFolder)
        {
            _isFilesDetectorTaskDone = false;
            _isFilesHandlerTaskDone = false;

            _allFilesQueue = new ConcurrentQueue<string>();
            _equalHashesQueue = new ConcurrentQueue<string>();
            _equalSizeFiles = new Dictionary<long, List<string>>();
            _equalHashFiles = new Dictionary<string, List<string>>();

            var fileHandler = Task.Factory.StartNew(HandleFiles);
            var hashHandler = Task.Factory.StartNew(HandleHashes);

            var filesCounter = 0;
            foreach (var file in SafeWalk.EnumerateFiles(rootFolder))
            {
                filesCounter++;
                _allFilesQueue.Enqueue(file);
            }
            Console.WriteLine("{0} files detected. Processing...", filesCounter);
            _isFilesDetectorTaskDone = true;
            fileHandler.Wait();

            Console.WriteLine("Step 1 finished. Calculating hashes...");
            //var filesLeft = _equalHashesQueue.Count;
            //Task.Factory.StartNew(() =>
            //                          {
            //                              var phasesDone = 0;
            //                              while (true)
            //                              {
            //                                  var equalHashesQueueCount = _equalHashesQueue.Count;
            //                                  var currentPhasesDone = 100 * (filesLeft - equalHashesQueueCount) / filesLeft;
            //                                  for (var i = 0; i < currentPhasesDone - phasesDone; i++)
            //                                      Console.Write('.');
            //                                  phasesDone = currentPhasesDone;
            //                                  Thread.Sleep(1000);
            //                              }
            //                          });

            _isFilesHandlerTaskDone = true;
            hashHandler.Wait();

            return _equalHashFiles.Values.Where(list => list.Count > 1).ToArray();
        }

        private void HandleFiles()
        {
            while (!(_isFilesDetectorTaskDone && _allFilesQueue.IsEmpty))
            {
                string file;
                if (_allFilesQueue.TryDequeue(out file))
                    ProcessFile(file);
                else
                    Thread.Sleep(1);
            }
        }

        private void ProcessFile(string processedFile)
        {
            var fileInfo = new FileInfo(processedFile);
            _equalSizeFiles.AddFile(fileInfo.Length, processedFile);
            var currentList = _equalSizeFiles[fileInfo.Length];
            if (currentList.Count == 2)
            {
                foreach (var fileInList in currentList)
                    _equalHashesQueue.Enqueue(fileInList);
            }
            else if (currentList.Count > 2)
            {
                _equalHashesQueue.Enqueue(processedFile);
            }
        }

        private void HandleHashes()
        {
            while (!(_isFilesHandlerTaskDone && _equalHashesQueue.IsEmpty))
            {
                string file;
                if (_equalHashesQueue.TryDequeue(out file))
                {
                    string hash;
                    try
                    {
                        hash = GetHash(file).Result;
                    }
                    catch (AggregateException ex)
                    {
                        if (ex.InnerException is UnauthorizedAccessException || ex.InnerException is IOException)
                            continue;
                        throw;
                    }
                    _equalHashFiles.AddFile(hash, file);
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

        private static async Task<string> GetHash(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                const int batchSize = 16384;
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
    }

    public static class Helper
    {
        public static void AddFile<TKey>(this Dictionary<TKey, List<string>> dictionary, TKey key, string value)
        {
            List<string> temp;
            if (!dictionary.TryGetValue(key, out temp))
                temp = dictionary[key] = new List<string>();
            temp.Add(value);
        }
    }
}
