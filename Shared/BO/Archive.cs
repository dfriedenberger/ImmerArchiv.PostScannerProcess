using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmerArchiv.PostScannerProcess.Shared.BO
{
    public class Archive
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public bool Valid { get; set; }
    }
}
