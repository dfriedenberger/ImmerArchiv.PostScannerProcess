using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmerArchiv.PostScannerProcess.Shared.Service
{
    public interface IInfrastructureService
    {
        bool FolderExists(string path);
        IList<string> ListFolder(string path);
        bool FileExists(string path);
        string ReadAllText(string path);
        void WriteAllText(string path, string text);
        void MoveTo(string sourceFile, string targetFile);
        string CreateTempFile(string ext);
        void CopyTo(string file, string temp);
    }
}
