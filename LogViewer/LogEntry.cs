using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogViewer
{
    public class LogEntry : PropertyChangedBase
    {
        public int Index { get; set; }
        public string Time { get; set; }
        public string Message { get; set; }

    }
}
