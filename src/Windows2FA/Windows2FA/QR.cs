using OtpNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace Windows2FA
{
    public class Qr
    {
        public string Uri { get; private set; }

        public string Protocol { get; private set; }

        public string Type { get; private set; }

        public string Label { get; private set; }

        public string Secrete { get; private set; }

        public string Code { get; private set; }

        public string ReminigSeconds { get; private set; }

        public string NextCode { get; private set; }

        public string Issuer { get; private set; }

        public Qr(string url, bool isShowCodes) {

            if (isShowCodes)
            {
                var key = Base32Encoding.ToBytes(Secrete);
                {
                    var totp = new Totp(key);
                    var code = totp.ComputeTotp();
                    var reminingSeconds = totp.RemainingSeconds();
                    var nexCode = totp.ComputeTotp(DateTime.UtcNow.AddSeconds(30));
                }
            }
        }

        public static bool IsValid(string str) {

            if (!string.IsNullOrWhiteSpace(str))
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
            return true;
        }
    }
}
