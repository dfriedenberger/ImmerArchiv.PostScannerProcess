using ImmerArchiv.PostScannerProcess.Shared.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmerArchiv.PostScannerProcess.Impl.Service
{
    class HtmlCreatorService : IHtmlCreatorService
    {
        private readonly IInfrastructureService _infrastructureservice;

        public HtmlCreatorService(IInfrastructureService infrastructureservice)
        {
            _infrastructureservice = infrastructureservice;
        }
        public string CreateFile(string file)
        {
            string ext = Path.GetExtension(file).ToLower();

            string temp = _infrastructureservice.CreateTempFile(ext);

            _infrastructureservice.CopyTo(file, temp);


            if (ext.Equals(".pdf"))
                return temp;

            return HtmlWithSrc(temp);
        }

        private string HtmlWithSrc(string url)
        {
            var page = new StringBuilder();

            page.Append("<html>");
            page.AppendFormat("<body style=\"{0}\">", "margin: 0;");
            page.AppendFormat("<img style=\"{0}\" src=\"{1}\">", "width: 100%;border-style: none;", url);
            page.Append("</body>");
            page.Append("</html>");

            var temp = _infrastructureservice.CreateTempFile(".html");

            _infrastructureservice.WriteAllText(temp, page.ToString());

            return temp;
        }

        private string CopyOf(string file)
        {
            return file;
        }
    }
}
