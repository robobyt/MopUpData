using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace MopUpData
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _district;
        private string _status;
        private string _userName;
        private string _password;
        private APIHelper apiHelper = new APIHelper();
        public MainWindow()
        {
            InitializeComponent();

            Binding binding = new Binding("Text");
            binding.Source = apiHelper.apiResponse;
            response.SetBinding(TextBlock.TextProperty, binding);

        }

        private void sendRequest_Click(object sender, RoutedEventArgs e)
        {
            _district = district.Text;
            _status = status.Text;
            _userName = userName.Text;
            _password = password.Text;

            
            RunFSE(apiHelper);
        }
        //_userName, _password, _district, _status
        async Task RunFSE (APIHelper apiHelper)
        {
            var result = await apiHelper.CallFSE(_userName, _password, _district, _status, true);
        }

        async Task UpdateFSE(APIHelper apiHelper)
        {
            var result = await apiHelper.UpdateTasks(_userName, _password);
        }

        private void post_Click(object sender, RoutedEventArgs e)
        {
            _userName = userName.Text;
            _password = password.Text;

            UpdateFSE(apiHelper);
        }


        private void response_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        //private void district_Loaded(object sender, RoutedEventArgs e)
        //{
        //    district.Items.Add("HILLSBORO");
        //    district.Items.Add("PEORIA");
        //}

        private void status_Loaded(object sender, RoutedEventArgs e)
        {
            status.Items.Add("Unassigned");
            status.Items.Add("Scheduled");
            status.Items.Add("Dispatched");
            status.Items.Add("Arrived");
            status.Items.Add("EnRoute");
        }
    }
}
