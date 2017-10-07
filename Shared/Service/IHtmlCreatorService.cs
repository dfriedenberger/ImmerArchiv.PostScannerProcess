using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmerArchiv.PostScannerProcess.Shared.Service
{
    public interface IHtmlCreatorService
    {
        string CreateFile(string file);
    }
}
