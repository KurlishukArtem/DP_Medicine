using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Medicine_DP
{
    public static class HashPassword
    {
        public static string Hash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
