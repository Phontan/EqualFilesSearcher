using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace EqualFilesDetector1
{
    class ParallelEqualsFilesDetector
    {
        private readonly ConcurrentQueue<string> _allFilesQueue;
        private bool _isFilesDetectorTaskDone;
        private readonly Dictionary<long, List<string>> _equalSizeFiles;

        public ParallelEqualsFilesDetector()
        {
            _allFilesQueue = new ConcurrentQueue<string>();
            _equalSizeFiles = new Dictionary<long, List<string>>();
            _isFilesDetectorTaskDone = false;
        }

        public IEnumerable<List<string>> Start(string rootFolder)
        {
            var fileHandler = new Task(HandleQueue);
            fileHandler.Start();

            foreach (var file in SafeWalk.EnumerateFiles(rootFolder, "*", SearchOption.AllDirectories))
            {
                _allFilesQueue.Enqueue(file);
            }
            _isFilesDetectorTaskDone = true;
            
            fileHandler.Wait();

            return _equalSizeFiles.Values.Where(list => list.Count > 1);
        }

        private void HandleQueue()
        {
            while (true)
            {
                string file;
                if(!_allFilesQueue.TryDequeue(out file))
                {
                    ProcessFile(file);
                }
                else
                {
                    if (_isFilesDetectorTaskDone)
                        break;
                    Thread.Sleep(1);
                }
            }
        }

        private void ProcessFile(string file)
        {
            var fileInfo = new FileInfo(file);
            List<string> containingList;
            if (!_equalSizeFiles.TryGetValue(fileInfo.Length, out containingList))
            {
                containingList = _equalSizeFiles[fileInfo.Length] = new List<string>();
            }
            containingList.Add(file);
        }
    }
}
