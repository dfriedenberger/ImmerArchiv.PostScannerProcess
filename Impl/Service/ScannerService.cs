using ImmerArchiv.PostScannerProcess.Shared.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImmerArchiv.PostScannerProcess.Shared.BO;
using System.IO;
using System.Configuration;
using log4net;
using System.Security.Cryptography;

namespace ImmerArchiv.PostScannerProcess.Impl.Service
{
    public class ScannerService : IScannerService
    {
        private readonly IConfigurationService _configurationService;
        private readonly IInfrastructureService _infrastructureService;
        private readonly string _configfile;

        private readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly RNGCryptoServiceProvider _random;


        private string _scanPath = null;
        private FileSystemWatcher _watcher = null;
        public string ScanPath
        {
            get
            {
                var configuration = _configurationService.Read<Config>(_configfile);
                _scanPath = configuration.RootPath;
                return configuration.RootPath;
            }

            set
            {
                var configuration = _configurationService.Read<Config>(_configfile);
                configuration.RootPath = value;
                _scanPath = value;

                _watcher = new FileSystemWatcher();
                _watcher.Path = value;
                _watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                       | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                _watcher.Filter = "*.*";

                _watcher.Changed += new FileSystemEventHandler(OnChanged);
                _watcher.Created += new FileSystemEventHandler(OnChanged);
                _watcher.Deleted += new FileSystemEventHandler(OnChanged);
                _watcher.Renamed += new RenamedEventHandler(OnRenamed);

                _watcher.EnableRaisingEvents = true;

                _configurationService.Write<Config>(_configfile, configuration);
            }
        }

        public bool ScanPathChanged { get; set; }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            _log.InfoFormat("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
            ScanPathChanged = true;
        }
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            _log.InfoFormat("File: Event {0} on {1}", e.ChangeType, e.FullPath);
            ScanPathChanged = true;
        }


        public ScannerService(IInfrastructureService infrastructureService, IConfigurationService configurationService)
        {
            _infrastructureService = infrastructureService;
            _configurationService = configurationService;
            _configfile = Path.Combine(ConfigurationManager.AppSettings["ConfigPath"], "postscanner.json");
            _random = new RNGCryptoServiceProvider();
        }

        public Archive AddArchiv(string selectedPath)
        {

            var configuration = _configurationService.Read<Config>(_configfile);
            configuration.Archives.Add(selectedPath);
            _configurationService.Write<Config>(_configfile, configuration);

            return new Archive()
            {
                Path = selectedPath,
                Name = Path.GetFileName(selectedPath),
                Valid = _infrastructureService.FolderExists(selectedPath)
            };

        }

        public IList<Archive> GetArchives()
        {
            var list = new List<Archive>();

            var configuration = _configurationService.Read<Config>(_configfile);

            foreach (var path in configuration.Archives)
            {
                list.Add(new Archive()
                {
                    Path = path,
                    Name = Path.GetFileName(path),
                    Valid = _infrastructureService.FolderExists(path)
                });
            }

            return list;
        }

      

        public bool IsValidFolder(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            return _infrastructureService.FolderExists(path);
        }

        public IList<ScanItem> ScanFolder()
        {
            var list = new List<ScanItem>();
            if (string.IsNullOrWhiteSpace(_scanPath))
                throw new ArgumentException("path");

            _log.InfoFormat("Read folder {0}", _scanPath);


        

            foreach (var file in _infrastructureService.ListFolder(_scanPath))
            {
                list.Add(new ScanItem()
                {
                    Path = file,
                    Name = Path.GetFileName(file),
                    Type = ResolveType(file)
                });
            }

            ScanPathChanged = false;

            return list;
        }

      

        private ScanItemType ResolveType(string file)
        {

            string ext = Path.GetExtension(file).ToLower();


            switch(ext)
            {
                case ".pdf":
                    return ScanItemType.Pdf;
                case ".jpg":
                case ".jpeg":
                case ".png":
                    return ScanItemType.Image;
            }
            _log.ErrorFormat("Could not resolve Extension for {0}", file);
            return ScanItemType.Unknown;

        }

        public void ArchivFiles(List<string> files, string targetPath)
        {

            var date = DateTime.Now;

        
            // Empty salt array
            byte[] salt = new byte[2];

            // Build the random bytes
            _random.GetNonZeroBytes(salt);

            var index = 0;


            foreach (var file in files)
            {
                var ext = Path.GetExtension(file);
                var target = string.Format("{0:yyyyMMdd}_{1}_{2:D3}{3}", date, BitConverter.ToString(salt).Replace("-",""), ++index, ext);

                //Rename and move

                _infrastructureService.MoveTo(file, Path.Combine(targetPath,target));

            }

        }

    }
}
