using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmerArchiv.PostScannerProcess.Shared.BO
{
    public class Config
    {
        public Config()
        {
            RootPath = "";
            Archives = new List<string>();
        }

        public string RootPath { get; set; }
        public IList<string> Archives { get; set; } 
    }
}
