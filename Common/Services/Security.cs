using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    public class Security: ISecurity
    {
        public string Hash(string hash)
        {
            var sha1 = new SHA1CryptoServiceProvider();
            var sha1data = sha1.ComputeHash(Encoding.UTF8.GetBytes(hash));
            return Convert.ToBase64String(sha1data);
        }
    }
}
