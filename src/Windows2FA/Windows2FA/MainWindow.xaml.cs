using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Windows2FA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isOpenAddAccount = false;
        bool isOpenAbout = false;
        bool isShowCodes = false;

        public MainWindow()
        {
            InitializeComponent();
            this.QRs.DataContext = DB.Instance.GetQrs();
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1),
            };
            timer.Tick +=(s,e)=> DB.Instance.UpdateReminingSecondsQrs();
            timer.Start();
        }

        private void AddAccount_Click(object sender, RoutedEventArgs e)
        {
            if (!isOpenAddAccount)
            {
                var addAccount = new AddAccount();
                addAccount.Closed += (s, e) => isOpenAddAccount = false;
                addAccount.Show();
                isOpenAddAccount = true;
            }
        }

        private void DeleteCode(object sender, RoutedEventArgs e)
        {
           var context = (Qr)((FrameworkElement)sender).DataContext;
           DB.Instance.RemoveQr(context);
        }

        private void CopyCode(object sender, RoutedEventArgs e)
        {
            var context = (Qr)((FrameworkElement)sender).DataContext;
            context.SetCodeToClipboard();
        }

        private void ShowCodes_Click(object sender, RoutedEventArgs e)
        {
            isShowCodes = !isShowCodes;
            DB.Instance.ShowCodes(isShowCodes);
        }

        private void CopyNextCode(object sender, RoutedEventArgs e)
        {
            var context = (Qr)((FrameworkElement)sender).DataContext;
            context.SetNextCodeToClipboard();
        }

        private void OpenDbPath_Click(object sender, RoutedEventArgs e)
        {
            DB.Instance.OpenDbPath();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            if (!isOpenAbout)
            {
                var addAccount = new About();
                addAccount.Closed += (s, e) => isOpenAbout = false;
                addAccount.Show();
                isOpenAbout = true;
            }
        }
    }
}
