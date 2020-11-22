using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
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
            if (!Qr.IsValid(this.Code.Text))
            {
                this.ErrorHint.Content = "Please enter valid uri.";
                this.ErrorHint.Visibility = Visibility.Visible;
                return;
            }

            DB.Instance.AddQr(new Qr(this.Code.Text));

            this.Close();
        }

        public Result Decode(string path)
        {
            Bitmap image;
            try
            {
                image = (Bitmap)Image.FromFile(path);
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
            var dialog = new OpenFileDialog
            {
                Filter = "IMG Files| *.jpg;*jpeg;*png",
                ValidateNames = true,
                Multiselect = false
            };
            var dr = dialog.ShowDialog();

            if (dr.HasValue && dr.Value)
            {
                try
                {
                    this.Code.Text = Decode(dialog.FileName).Text;
                    if (!Qr.IsValid(this.Code.Text))
                    {
                        this.ErrorHint.Content = "Please enter valid uri.";
                        this.ErrorHint.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.ErrorHint.Visibility = Visibility.Hidden;
                    }
                }
                catch (Exception ex)
                {
                    this.ErrorHint.Visibility = Visibility.Visible;
                    this.ErrorHint.Content = ex.Message;
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
