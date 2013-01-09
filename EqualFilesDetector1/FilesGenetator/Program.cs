using System.IO;
using System.Text;

namespace FilesGenetator
{
    class Program
    {
        static void Main(string[] args)
        {
            const string folder = @"d:/1/";

            FilesGenerator.Generate(folder, 5, 10000);
        }
    }

    internal class FilesGenerator
    {
        public static void Generate(string folder, int equalFilesCount, int differentFilesCount)
        {
            for (int i = 0; i < differentFilesCount; i++)
            {
                var stringData = string.Format("{0}...............", i);
                var data = Encoding.UTF8.GetBytes(stringData);
                for (int j = 0; j < equalFilesCount; j++)
                    using (var sw = new FileStream(string.Format("{0}.{1}.{2}", folder, i,j), FileMode.Create))
                        sw.Write(data, 0, data.Length);
            }
        }
    }
}
