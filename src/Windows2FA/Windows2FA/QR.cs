using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Runtime;
using System.Windows;

namespace Windows2FA
{
    public class Qr   : IEquatable<Qr>,IComparable<Qr>
    {
        public string Url { get; private set; }

        public string Issuer { get; private set; }

        public string Label { get; private set; }

        public string Code { get; private set; }

        public string NextCode { get; private set; }

        public int ReminigSeconds { get; private set; }

        private int hash;
        private Totp totp;

        public Qr(string url) {

            url = url.Trim();
            var uri = new Uri(url);
            this.Url = url;
            this.hash = url.GetHashCode();
            this.Label = HttpUtility.UrlDecode(uri.AbsolutePath.TrimStart('/'));
            var parsed = HttpUtility.ParseQueryString(uri.Query);
            this.Issuer = HttpUtility.UrlDecode(parsed["issuer"]);
            var secret = HttpUtility.UrlDecode(parsed["secret"]);
            this.totp = GetTotp(secret);
            UpdateReminingSeconds();
        }
        public void ShowCodes(bool isShowCodes) {
            if (isShowCodes)
            {
                this.Code = totp.ComputeTotp();
                this.NextCode = totp.ComputeTotp(DateTime.UtcNow.AddSeconds(30));
            }
            else { 
            this.Code = null;
            this.NextCode = null;
            }
        }

        public void UpdateReminingSeconds() {
            this.ReminigSeconds = totp.RemainingSeconds();
        }

        private Totp GetTotp(string secret) {
            var key = Base32Encoding.ToBytes(secret);
            var totp = new Totp(key);
            return totp;
        }

        public void SetCodeToClipboard() {
            var code = totp.ComputeTotp();
            Clipboard.SetText(code);
        }

        public void SetNextCodeToClipboard()
        {
            var nextCode = totp.ComputeTotp(DateTime.UtcNow.AddSeconds(30));
            Clipboard.SetText(nextCode);
        }

        public static bool IsValid(string str) {

            if (string.IsNullOrWhiteSpace(str))
            {
                return false;
            }
            var uri = new Uri(str);
            if (!System.Uri.IsWellFormedUriString(str, UriKind.Absolute))
            {
                return false;
            }
            if (uri.Scheme != "otpauth")
            {
                return false;
            }
            if (uri.Host!= "totp")
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(uri.AbsolutePath.TrimStart('/')))
            {
                return false;
            }

            var label = uri.AbsolutePath.TrimStart('/');
            if (string.IsNullOrWhiteSpace(label))
            {
                return false;
            }
            var parsed = HttpUtility.ParseQueryString(uri.Query);
            var issuer = parsed["issuer"];
            if (string.IsNullOrWhiteSpace(issuer))
            {
                return false;
            }
            var secret = parsed["secret"];
            if (string.IsNullOrWhiteSpace(secret))
            {
                return false;
            }
            try
            {
                var key = Base32Encoding.ToBytes(secret);
                var totp = new Totp(key);
                var code = totp.ComputeTotp();
                if (string.IsNullOrWhiteSpace(code))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
         
        // override object.GetHashCode
        public override int GetHashCode()
        {
            return hash;
        }

        public bool Equals(Qr other)
        {
            if (other == null)
            {
                return false;
            }
            var flag =  this.GetHashCode() == other.GetHashCode();
            if (flag)
            {
                flag = this.Url == other.Url;
            }
            return flag;
        }

        public int CompareTo(Qr other)
        {
            if (other==null)
            {
                throw new Exception();
            }
            var val =  this.Issuer.CompareTo(other.Issuer);
            if (val==0)
            {
                val = this.Label.CompareTo(other.Label);
            }
            return val;
        }
    }
}
