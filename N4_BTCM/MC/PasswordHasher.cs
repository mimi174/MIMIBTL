// File: N4_BTCM/PasswordHasher.cs
using System;
using System.Security.Cryptography;
using System.Text;

namespace N4_BTCM
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 20;
        private const int Iterations = 10000;

        public static string HashPassword(string password)
        {
            // Tạo một salt ngẫu nhiên
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);

            // Tạo hash bằng PBKDF2
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            // Kết hợp salt và hash
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // Chuyển đổi sang chuỗi Base64 để lưu vào database
            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Chuyển đổi chuỗi Base64 đã băm thành byte array
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);

            // Lấy salt từ hashBytes
            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Lấy hash gốc từ hashBytes
            byte[] storedHash = new byte[HashSize];
            Array.Copy(hashBytes, SaltSize, storedHash, 0, HashSize);

            // Tạo hash mới từ password đã nhập và salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
            byte[] newHash = pbkdf2.GetBytes(HashSize);

            // So sánh hai hash
            for (int i = 0; i < HashSize; i++)
            {
                if (newHash[i] != storedHash[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}