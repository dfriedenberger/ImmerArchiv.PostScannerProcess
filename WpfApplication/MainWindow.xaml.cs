using ImmerArchiv.PostScannerProcess.Shared.BO;
using ImmerArchiv.PostScannerProcess.Shared.Locator;
using ImmerArchiv.PostScannerProcess.Shared.Service;
using ImmerArchiv.PostScannerProcess.WpfApplication.BO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImmerArchiv.PostScannerProcess.WpfApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private ObservableCollection<UIArchive> _uiArchiveList = new ObservableCollection<UIArchive>();
        private ObservableCollection<UIScanItem> _uiScanItemList = new ObservableCollection<UIScanItem>();

        public MainWindow()
        {
            InitializeComponent();

            var scannerService = Locator.GetScannerService();

            cmbArchives.ItemsSource = _uiArchiveList;


            foreach (var archive in scannerService.GetArchives())
            {
                _uiArchiveList.Add(ToUIArchive(archive));
            }

            cmbArchives.SelectedIndex = -1;  //Keins vorauswaehlen
            btnArchive.IsEnabled = false;

            lbScanItems.ItemsSource = _uiScanItemList;
            txtRootPath.Text = scannerService.ScanPath;



            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;

            worker.DoWork += (object _sender, DoWorkEventArgs _e) =>
            {

               while(true)
               {
                    Thread.Sleep(100);
                    if(scannerService.ScanPathChanged)
                    {
                        scannerService.ScanPathChanged = false;
                        (_sender as BackgroundWorker).ReportProgress(0);
                    }
                }



            };

            worker.ProgressChanged += (object _sender, ProgressChangedEventArgs _e) =>
            {
                ScanFolder();
            };

            //worker.RunWorkerCompleted += (object _sender, RunWorkerCompletedEventArgs _e) =>  {};
            worker.RunWorkerAsync();
        }

        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void mnuAbount_Click(object sender, RoutedEventArgs e)
        {
            AboutBox dialog = new AboutBox();
            dialog.ShowDialog();
        }

        private void txtRootPathChanged(object sender, RoutedEventArgs e)
        {
            var path = txtRootPath.Text;
            var scannerService = Locator.GetScannerService();

            if (!scannerService.IsValidFolder(path)) return;

            scannerService.ScanPath = path;

            ScanFolder();
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            ScanFolder();
        }

        
        private void ScanFolder()
        {

            var scannerService = Locator.GetScannerService();

            _uiScanItemList.Clear();
            foreach(var item in scannerService.ScanFolder())
            {
                _uiScanItemList.Add(ToUIScanItem(item));
            }

         
            var ix = cmbArchives.SelectedIndex;
            btnArchive.IsEnabled = ix >= 0 && (_uiScanItemList.Count > 0);
        }

        private UIScanItem ToUIScanItem(ScanItem item)
        {
            return new UIScanItem()
            {
                FileName = item.Name,
                Icon = item.Type == ScanItemType.Pdf ? UICanvasIcon.FileExtPdf :
                       item.Type == ScanItemType.Image ? UICanvasIcon.FileExtImage :
                       UICanvasIcon.FileExtUnknown,
                Active = item.Type == ScanItemType.Unknown ? false : true,
                AbsolutePath = item.Path
            };
        }

        private void btnSetRootPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                txtRootPath.Text = dialog.SelectedPath;
            }
        }

        private void cmbArchives_Changed(object sender, RoutedEventArgs e)
        {

            var ix = cmbArchives.SelectedIndex;
            btnArchive.IsEnabled = ix >= 0;

        }

        
        private void btnAddArchiv_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {

                //Add to ComboBox and select
                var scannerService = Locator.GetScannerService();
                var archive = scannerService.AddArchiv(dialog.SelectedPath);
                _uiArchiveList.Add(ToUIArchive(archive));
                 
                cmbArchives.SelectedIndex = _uiArchiveList.Count - 1;
            }
        }

        private UIArchive ToUIArchive(Archive archive)
        {
            return new UIArchive()
            {
                Title = archive.Name,
                Icon = archive.Valid ? UICanvasIcon.ArchivOk : UICanvasIcon.ArchivWarning,
                AbsolutePath = archive.Path
            };
        }

        private void lbScanItems_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var ix = lbScanItems.SelectedIndex;

            if (ix < 0) return;

            var htmlCreatorService = Locator.GetHtmlCreatorService();
            var url = htmlCreatorService.CreateFile(_uiScanItemList[ix].AbsolutePath);
            webBrowser.Navigate(new Uri(url));
           
        }
        private void ItemsActiveChanged(object sender, RoutedEventArgs e)
        {
            //CheckBox der ScanItems hat sich geändert
            foreach (var item in lbScanItems.Items)
            {
                var scanitem = item as UIScanItem;
                if (!scanitem.Active) continue;

                //Active
                var ix = cmbArchives.SelectedIndex;
                btnArchive.IsEnabled = ix >= 0;
                return;
            }
            btnArchive.IsEnabled = false;
        }

        private void Archive_Clicked(object sender, RoutedEventArgs e)
        {
            //Archivieren
            var files = new List<string>();
            foreach(var item in lbScanItems.Items)
            {
                var scanitem = item as UIScanItem;

                if (!scanitem.Active) continue;
              
                files.Add(scanitem.AbsolutePath);
            }

            if(files.Count == 0)
            {
                //fehlermeldung
                return;
            }


            var ix = cmbArchives.SelectedIndex;
            if (ix < 0)
            {
                //fehlermeldung
                return;
            }
            
            var archive = cmbArchives.Items[ix] as UIArchive;


            var scannerService = Locator.GetScannerService();

            scannerService.ArchivFiles(files, archive.AbsolutePath);



        }

    }


}
