using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmerArchiv.PostScannerProcess.Shared.BO
{
    public class ScanItem
    {
        public string Path { get; set;  }
        public string Name { get; set; }

        public ScanItemType Type { get; set; }

    }
}
