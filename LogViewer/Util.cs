using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogViewer
{
    public class Util
    {
        public static LogEntry parseLine(string line, int lineNumber)
        {
            List<string> elements = line.Split(' ').ToList();
            string logmsg = "";

            for(int i=3; i<elements.Count; i++)
            {
                logmsg += elements[i];
            }

            LogEntry le = new LogEntry()
            {
                Index = lineNumber,
                Time = elements[0],
                Message = logmsg,
            };
            return le;
        }

        public static async Task<List<string>> ReadAllLinesAsync(string path, Encoding encoding)
        {
            const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
            const int DefaultBufferSize = 4096;

            var lines = new List<string>();

            // Open the FileStream with the same FileMode, FileAccess
            // and FileShare as a call to File.OpenText would've done.
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions))
            using (var reader = new StreamReader(stream, encoding))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lines.Add(line);
                }
            }

            //return lines.ToArray();
            return lines;
        }
    }
}
