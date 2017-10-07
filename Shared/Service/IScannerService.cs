using ImmerArchiv.PostScannerProcess.Shared.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmerArchiv.PostScannerProcess.Shared.Service
{
    public interface IScannerService
    {

        bool IsValidFolder(string path);

        string ScanPath { get; set; }
        bool ScanPathChanged { get; set;  }

        IList<ScanItem> ScanFolder();


        IList<Archive> GetArchives();

        Archive AddArchiv(string selectedPath);
        void ArchivFiles(List<string> files, string absolutePath);
    }
}
