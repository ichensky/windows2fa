using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
//using OtpNet;

namespace Windows2FA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isOpenAddAccount = false;

        public MainWindow()
        {
            InitializeComponent();
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
            isOpenAddAccount = false;
            var text = ((AddAccount)sender).Code.Text;
            if (!Qr.IsValid(text))
            {
                DB.Instance.Data.Add(text);
                DB.Instance.Save();
                var list = DB.Instance.Data.Select(x => new Qr(text, false));
                this.QRs.DataContext = new ObservableCollection<Qr>(list);
            }
        }

        private void DeleteCode(object sender, RoutedEventArgs e)
        {

        }

        private void CopyCode(object sender, RoutedEventArgs e)
        {

        }
    }
}
