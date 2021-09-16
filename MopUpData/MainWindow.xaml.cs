using System;
using System.Collections.Generic;
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
using MopUpData.Helpers;
using MopUpData.Models;

namespace MopUpData
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _status;
        private string _userName;
        private string _password;
        private bool _sandBox;
        private APIHelper apiHelper = new APIHelper();
        public MainWindow()
        {
            InitializeComponent();
        }

        // Called by click "Send request" button. Captured credentials and the Status value that will be used for get query and as a TaskColor value
        private async void sendRequest_Click(object sender, RoutedEventArgs e)
        {
            _status = status.Text;
            _userName = userName.Text;
            _password = password.Text;
            _sandBox = sandBox.IsChecked.GetValueOrDefault();
            var watch = System.Diagnostics.Stopwatch.StartNew();

            Progress<ProgressBarModel> progress = new Progress<ProgressBarModel>();
            progress.ProgressChanged += ReportProgress;

            try
            {
                apiHelper.CallFSE("q160659@amerensb", "Sep@2021", _status, _sandBox);
                watch.Stop();
                var timeExecution = watch.ElapsedMilliseconds;
                TimeSpan t = TimeSpan.FromMilliseconds(timeExecution);
                string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s", t.Hours,t.Minutes,t.Seconds);
                textResult.Text = textResult.Text.Insert(0, $"\n FSE response: { apiHelper.apiResponse }  Executed  {t}" );
            }
            catch (OperationCanceledException)
            {
                textResult.Text = textResult.Text.Insert(0, $"\n FSE response: { apiHelper.apiResponse }");
            }

        }
        //_userName, _password, _district, _status,

        private async void post_Click(object sender, RoutedEventArgs e)
        {
            _userName = userName.Text;
            _password = password.Text;
            textResult.Text = textResult.Text.Insert(0, $"\n FSE response: { apiHelper.apiResponse }");

            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var results = await apiHelper.UpdateTasksInParallel();
                watch.Stop();
                var timeExecution = watch.ElapsedMilliseconds;
                TimeSpan t = TimeSpan.FromMilliseconds(timeExecution);
                string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s", t.Hours, t.Minutes, t.Seconds);
                textResult.Text = textResult.Text.Insert(0, $"\n FSE response: { results }  Executed  {t}");
            }
            catch (OperationCanceledException)
            {
                textResult.Text = textResult.Text.Insert(0, $"\n FSE response: { apiHelper.apiResponse }");
            }
        }

        private void ReportProgress(object sender, ProgressBarModel e)
        {
            dashboardProgress.Value = e.PercentageComplete;
            PrintResults(e.SitesDownloaded);
        }

        private void PrintResults(List<int> results)
        {
            textResult.Text = "";
            foreach (var item in results)
            {
                textResult.Text += $"";
            }
        }
        private void status_Loaded(object sender, RoutedEventArgs e)
        {
            status.Items.Add("Unassigned");
            status.Items.Add("Scheduled");
            status.Items.Add("Dispatched");
            status.Items.Add("Arrived");
            status.Items.Add("EnRoute"); 
            status.Items.Add("On Hold");
            status.Items.Add("Accepted");
            status.Items.Add("Cancelled");
            status.Items.Add("Complete");
            status.Items.Add("Incomplete"); 
            status.Items.Add("Rejected");
        }


    }
}
