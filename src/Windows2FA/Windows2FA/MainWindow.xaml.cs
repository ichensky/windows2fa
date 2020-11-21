using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Windows2FA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isOpenAddAccount = false;
        bool isShowCodes = false;

        public MainWindow()
        {
            InitializeComponent();
            this.QRs.DataContext = DB.Instance.GetQrs(isShowCodes);
        }

        private void AddAccount_Click(object sender, RoutedEventArgs e)
        {
            if (!isOpenAddAccount)
            {
                var addAccount = new AddAccount();
                addAccount.Closed += AddAccount_Closed;
                addAccount.Show();
                isOpenAddAccount = true;
            }
        }

        private void AddAccount_Closed(object sender, EventArgs e)
        {
            this.QRs.DataContext = DB.Instance.GetQrs(isShowCodes);
            isOpenAddAccount = false;
        }

        private void DeleteCode(object sender, RoutedEventArgs e)
        {
           var context = (Qr)((FrameworkElement)sender).DataContext;
           DB.Instance.RemoveQr(context);
           this.QRs.DataContext = DB.Instance.GetQrs(isShowCodes);
        }

        private void CopyCode(object sender, RoutedEventArgs e)
        {
            var context = (Qr)((FrameworkElement)sender).DataContext;
            Clipboard.SetText(context.Code);
        }

        private void ShowCodes_Click(object sender, RoutedEventArgs e)
        {
            isShowCodes = !isShowCodes;
            this.QRs.DataContext = DB.Instance.GetQrs(isShowCodes);
        }

        private void CopyNextCode(object sender, RoutedEventArgs e)
        {
            var context = (Qr)((FrameworkElement)sender).DataContext;
            Clipboard.SetText(context.NextCode);
        }

        private void OpenDbPath_Click(object sender, RoutedEventArgs e)
        {
            DB.Instance.OpenDbPath();
        }
    }
}
