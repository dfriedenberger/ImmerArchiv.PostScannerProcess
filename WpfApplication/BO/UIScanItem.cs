using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmerArchiv.PostScannerProcess.WpfApplication.BO
{
    public class UIScanItem
    {
        public bool Active { get; set; }
        public UICanvasIcon Icon { get; set; }
        public string FileName { get; set; }

        public string AbsolutePath { get; set; }

    }
}
