using DocumentFormat.OpenXml.Wordprocessing;
using ElectroPrognizer.DataLayer;
using ElectroPrognizer.DataModel.Entities;
using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Services.Models;
using Microsoft.Win32;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ApplicationContext _db;

        public IExcelConsumptionReader ExcelConsumptionReader { get; set; }
        public IXmlReaderService XmlReaderService { get; set; }
        public IEnergyConsumptionSaverService EnergyConsumptionSaverService { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _db = new ApplicationContext();

            Closing += MainWindow_Closing;

            Init();
        }

        private void Init()
        {
            LoadingBarWrapper.Visibility = Visibility.Hidden;
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            _db.Dispose();
        }

        private void LoadComnsumption_Click(object sender, RoutedEventArgs e)
        {
            //var openFileDialog = new OpenFileDialog
            //{
            //    Multiselect = true,
            //    Filter = "Файлы Excel|*.xlsx"
            //};

            //if (openFileDialog.ShowDialog() == true)
            //{
            //    ExcelConsumptionReader.LoadFileContent(openFileDialog.FileNames);
            //}

            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Файлы xml|*.xml"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                //LoadingBar.Visibility = Visibility.Visible;

                var worker = new BackgroundWorker();

                worker.WorkerReportsProgress = true;
                worker.DoWork += worker_DoWork;
                worker.ProgressChanged += worker_ProgressChanged;

                worker.RunWorkerAsync(openFileDialog.FileNames);



                //LoadingBar.Visibility = Visibility.Hidden;
                //var progressBarTask = Task.Run(() =>
                //{
                //    while (!isReady)
                //    {
                //        LoadingBar.Value = percent;
                //        Task.Delay(500);
                //    }
                //});

                //loadingTask.GetAwaiter().GetResult();
                //isReady = true;
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var energyConsumptions = XmlReaderService.ParseXml((string[])e.Argument);

            var progress = new SaverProgressModel();

            var loadingTask = Task.Run(() => EnergyConsumptionSaverService.SaveToDatabase(energyConsumptions, ref progress));

            int percent = 0;

            while (!progress.IsCompleted)
            {
                if (!progress.IsInitialized)
                    continue;

                percent = progress.TotalCount == 0
                    ? 0
                    : progress.CurrentIndex * 100 / progress.TotalCount;

                (sender as BackgroundWorker)!.ReportProgress(percent, progress);
                Thread.Sleep(500);
            }

            (sender as BackgroundWorker)!.ReportProgress(percent, progress);
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var progressSate = (SaverProgressModel)e.UserState!;

            if (!progressSate.IsCompleted)
                LoadingBarWrapper.Visibility = Visibility.Visible;
            else
                LoadingBarWrapper.Visibility = Visibility.Hidden;

            LoadingBar.Value = e.ProgressPercentage;
            LoadingBarText.Text = $"{progressSate.CurrentIndex} / {progressSate.TotalCount}";
        }
    }
}
