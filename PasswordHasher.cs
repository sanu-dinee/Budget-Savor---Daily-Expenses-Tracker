using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace budgetSavour
{
    internal class PasswordHasher
    {
        public static string HashPassword(string password,out string salt)
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create()) { 
                 rng.GetBytes(saltBytes);
            }
            salt=Convert.ToBase64String(saltBytes);

            using(var pbkdf2= new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = pbkdf2.GetBytes(32);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool verifyPassword(string inputPassword, string storedPassword,string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);

            using (var pbkdf2 = new Rfc2898DeriveBytes(inputPassword, saltBytes, 10000, HashAlgorithmName.SHA256))
            {
                byte[] inputHash = pbkdf2.GetBytes(32);
                string inputHashBase64 = Convert.ToBase64String(inputHash);
                return inputHashBase64== storedPassword;
            }
        }
    }
}
