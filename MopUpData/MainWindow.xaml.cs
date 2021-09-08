﻿using System;
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
        }

        private async void sendRequest_Click(object sender, RoutedEventArgs e)
        {
            _district = district.Text;
            _status = status.Text;
            _userName = userName.Text;
            _password = password.Text;
            RunFSE();
            textResult.Text = textResult.Text.Insert(0, $"\n FSE response: { apiHelper.apiResponse }");
        }
        //_userName, _password, _district, _status,
        async Task RunFSE ()
        {
<<<<<<< HEAD
            var result = await apiHelper.CallFSE(_userName, _password, _district, _status, true);
=======
            var result = await apiHelper.CallFSE("_userName, _password, _district, _status, true);
>>>>>>> 3ed61c90e4d4486b03bb863cd54bc89d24525c23
         }

        async Task UpdateFSE()
        {
<<<<<<< HEAD
            var result = await apiHelper.UpdateTasks(_userName, _password);
=======
            var result = await apiHelper.UpdateTasks("_userName, _password,");
>>>>>>> 3ed61c90e4d4486b03bb863cd54bc89d24525c23
        }

        private async void post_Click(object sender, RoutedEventArgs e)
        {
            _userName = userName.Text;
            _password = password.Text;
            UpdateFSE();
            textResult.Text = textResult.Text.Insert(0, $"\n FSE response: { apiHelper.apiResponse }");
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
