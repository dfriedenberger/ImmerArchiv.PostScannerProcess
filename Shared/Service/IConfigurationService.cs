using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImmerArchiv.PostScannerProcess.Shared.BO;

namespace ImmerArchiv.PostScannerProcess.Shared.Service
{
    public interface IConfigurationService
    {
        T Read<T>(string path) where T : class, new();
        void Write<T>(string path, T obj) where T : class, new();
    }
}
