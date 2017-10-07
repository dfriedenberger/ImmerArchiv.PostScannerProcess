using ImmerArchiv.PostScannerProcess.Shared.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmerArchiv.PostScannerProcess.Impl.Service
{
    class InfrastructureService : IInfrastructureService
    {
      

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public bool FolderExists(string path)
        {
            return Directory.Exists(path);
        }

        public IList<string> ListFolder(string path)
        {
            return Directory.GetFiles(path);
        }

        public void MoveTo(string sourceFile, string targetFile)
        {
            if (File.Exists(targetFile))
                throw new IOException("File " + targetFile + " yet exists");
            if (!File.Exists(sourceFile))
                throw new IOException("File " + sourceFile + " not exists");

            File.Move(sourceFile, targetFile);

        }

        public void CopyTo(string sourceFile, string targetFile)
        {
            if (File.Exists(targetFile))
                throw new IOException("File " + targetFile + " yet exists");
            if (!File.Exists(sourceFile))
                throw new IOException("File " + sourceFile + " not exists");

            File.Copy(sourceFile, targetFile);

        }

        public string CreateTempFile(string ext)
        {
            return Path.GetTempPath() + Guid.NewGuid().ToString() + ext;
        }
        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public void WriteAllText(string path, string text)
        {
            File.WriteAllText(path, text);
        }
    }
}
