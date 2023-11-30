using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace EarthCo.Models
{
    public class VerifySignature
    {
        private const string SIGNATURE = "intuit-signature";
        private const string ALGORITHM = "HmacSHA256";

        public bool IsRequestValid(Dictionary<string, string> headers, string payload, string verifier)
        {
            if (!headers.TryGetValue(SIGNATURE, out string signature))
            {
                return false;
            }

            try
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(verifier);
                using (var hmac = new HMACSHA256(keyBytes))
                {
                    byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
                    byte[] hashBytes = hmac.ComputeHash(payloadBytes);
                    string hash = Convert.ToBase64String(hashBytes);
                    return hash.Equals(signature);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}