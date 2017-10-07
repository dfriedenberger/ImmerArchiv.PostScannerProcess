using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using ImmerArchiv.PostScannerProcess.Impl.Service;
using ImmerArchiv.PostScannerProcess.Shared.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmerArchiv.PostScanner.Process.Impl.Locator
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {

            container.Register(Component.For<IScannerService>().ImplementedBy<ScannerService>().LifestyleSingleton());
            container.Register(Component.For<IInfrastructureService>().ImplementedBy<InfrastructureService>().LifestyleSingleton());
            container.Register(Component.For<IConfigurationService>().ImplementedBy<ConfigurationService>().LifestyleSingleton());
            container.Register(Component.For<IHtmlCreatorService>().ImplementedBy<HtmlCreatorService>().LifestyleSingleton());
           
        }
    }
}
