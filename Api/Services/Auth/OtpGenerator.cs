using OtpNet;
using System;
using System.Text;

namespace Api.Services.Infrastructure
{
    internal class OtpGenerator : IOtpGenerator
    {
        public string Generate(string key, int expireTimeMinutes = 2, int digitsCount = 6)
        {
            var ttl = TimeSpan.FromMinutes(expireTimeMinutes);
            var otp = new Totp(Encoding.UTF8.GetBytes(key), (int)ttl.TotalSeconds, totpSize: digitsCount);

            return otp.ComputeTotp();
        }

        public bool Verify(string key, string token, int expireTimeMinutes = 2, int digitsCount = 6)
        {
            var ttl = TimeSpan.FromMinutes(expireTimeMinutes);
            var otp = new Totp(Encoding.UTF8.GetBytes(key), (int)ttl.TotalSeconds, totpSize: digitsCount);
            return otp.VerifyTotp(token, out _);
        }
    }
}