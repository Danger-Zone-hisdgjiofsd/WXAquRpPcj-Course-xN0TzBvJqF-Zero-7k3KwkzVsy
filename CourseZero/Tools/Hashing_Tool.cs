using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CourseZero.Hashing
{
    public class Hashing_Tool
    {
        public static (string salt, string hashed_pw) Hash_Password_by_Random_Salt(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));
            return (Convert.ToBase64String(salt), hashed);
        }
        public static string Hash_Password(string password, string salt_in_base64)
        {
            byte[] salt = Convert.FromBase64String(salt_in_base64);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));
            return hashed;
        }
        public static string Random_String(int length)
        {
            if (length * 3 % 4 != 0)
                throw new Exception("length has to be divisible by 0.75");
            byte[] result = new byte[(int)(length*0.75)];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(result);
            }
            return Convert.ToBase64String(result);
        }
    }
}
