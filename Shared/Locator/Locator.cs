using Castle.Windsor;
using Castle.Windsor.Installer;
using ImmerArchiv.PostScannerProcess.Shared.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmerArchiv.PostScannerProcess.Shared.Locator
{
    public class Locator
    {
        
        private static IWindsorContainer _container;
        private static IWindsorContainer Container
        {
            get
            {
                if (_container == null)
                {
                    _container = new WindsorContainer();
                    _container.Install(Configuration.FromAppConfig());
                }
                return _container;
            }
        }

      
        public static IScannerService GetScannerService()
        {
            return Container.Resolve<IScannerService>();
        }

        public static IHtmlCreatorService GetHtmlCreatorService()
        {
            return Container.Resolve<IHtmlCreatorService>();
        }
    }
}
