using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Windows2FA
{
    public class Qr
    {
        public string Url { get; private set; }

        public string Issuer { get; private set; }

        public string Label { get; private set; }

        public string Code { get; private set; }

        public string NextCode { get; private set; }

        public int ReminigSeconds { get; private set; }

        public Qr(string url, bool isShowCodes = false) {

            url = url.Trim();
            var uri = new Uri(url);
            this.Url = url;
            this.Label = uri.AbsolutePath.TrimStart('/');
            var parsed = HttpUtility.ParseQueryString(uri.Query);
            this.Issuer = parsed["issuer"];
            var secret = parsed["secret"];

            if (isShowCodes)
            {
                var key = Base32Encoding.ToBytes(secret);
                var totp = new Totp(key);
                this.Code = totp.ComputeTotp();
                this.ReminigSeconds = totp.RemainingSeconds();
                this.NextCode = totp.ComputeTotp(DateTime.UtcNow.AddSeconds(30));
            }
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
    }
}
