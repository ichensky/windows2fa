using System;
using System.Collections.Generic;
using System.Text;

namespace Windows2FA
{
    public class Qr
    {
        public string Uri { get; set; }

        public string Protocol { get; set; }

        public string Type { get; set; }

        public string Label { get; set; }

        public string Secrete { get; set; }

        public string Code { get; set; }

        public string NextCode { get; set; }

        public string Issuer { get; set; }

        public static bool IsValid(Uri uri) {

            if (uri.Scheme != "otpauth")
            {
                return false;
            }
            if (uri.Host!= "totp" || uri.Host!="hotp")
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
