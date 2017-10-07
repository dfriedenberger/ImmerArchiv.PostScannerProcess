using Castle.MicroKernel.SubSystems.Configuration;
using ImmerArchiv.PostScannerProcess.Shared.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmerArchiv.PostScannerProcess.Impl.Service
{
    class ConfigurationService : IConfigurationService
    {
        private IInfrastructureService _infrastructureService;

        public ConfigurationService(IInfrastructureService infrastructureService)
        {
            _infrastructureService = infrastructureService;
        }
        public T Read<T>(string path) where T : class, new()
        {
            if (!_infrastructureService.FileExists(path)) return new T();
            return JsonConvert.DeserializeObject<T>(_infrastructureService.ReadAllText(path));
        }

        public void Write<T>(string path, T obj) where T : class, new()
        {
            var text = JsonConvert.SerializeObject(obj, Formatting.Indented);
            _infrastructureService.WriteAllText(path, text);

        }
    }
}
