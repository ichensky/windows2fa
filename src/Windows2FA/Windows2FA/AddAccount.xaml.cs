using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using ZXing;
using ZXing.Common;

namespace Windows2FA
{
    /// <summary>
    /// Interaction logic for AddAccount.xaml
    /// </summary>
    public partial class AddAccount : Window
    {
        //private const string uriExample = "otpauth://totp/gfsddsa?secret=fdafsda&issuer=Google";

        public AddAccount()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e) {
            if (!Uri.IsWellFormedUriString(this.Code.Text, UriKind.Absolute))
            {
                this.ErrorHint.Visibility = Visibility.Visible;
                return;
            }
            this.Close();
        }

        public Result Decode(string path)
        {
            Bitmap image;
            try
            {
                image = (Bitmap)Bitmap.FromFile(path);
            }
            catch (Exception)
            {
                throw new FileNotFoundException("Resource not found: " + path);
            }

            using (image)
            {
                LuminanceSource source;
                source = new BitmapLuminanceSource(image);
                BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));
                Result result = new MultiFormatReader().decode(bitmap);
                if (result == null)
                {
                    throw new Exception("Unable to decode QR code.");
                }
                return result;
            }
        }

        private void LoadQrCode_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "IMG Files| *.jpg;*jpeg;*png";
            dialog.ValidateNames = true;
            dialog.Multiselect = false;
            var dr = dialog.ShowDialog();

            if (dr.HasValue && dr.Value)
            {
                this.Code.Text = Decode(dialog.FileName).Text;
                if (!Uri.IsWellFormedUriString(this.Code.Text, UriKind.Absolute) || !Qr.IsValid(new Uri(this.Code.Text)))
                {
                    this.ErrorHint.Visibility = Visibility.Visible;
                }
                else {
                    this.ErrorHint.Visibility = Visibility.Hidden;
                }
            }
        }
      
        private void Code_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(Code.Text))
            {
                this.ErrorHint.Visibility = Visibility.Hidden;
            }
        }
    }
}
