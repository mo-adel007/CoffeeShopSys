using System;
using System.Security.Cryptography;

namespace Corner_Application
{
    /// <summary>
    /// Password hashing for the local login store.
    ///
    /// Uses PBKDF2 (Rfc2898DeriveBytes, built into .NET Framework 4.5 — no external
    /// dependency). Passwords are never stored or compared in plaintext. Because the
    /// hash is one-way, the admin panel can RESET a user's password but can never
    /// display the existing one.
    ///
    /// Stored format (single TEXT column, dot-separated):
    ///     PBKDF2.{iterations}.{base64 salt}.{base64 hash}
    /// The "PBKDF2" tag lets us detect and transparently upgrade any legacy
    /// plaintext rows on first successful login (see Verify).
    /// </summary>
    public static class Security
    {
        private const int Iterations = 100000; // PBKDF2 rounds (SHA1 on .NET 4.5)
        private const int SaltSize = 16;       // bytes
        private const int HashSize = 32;       // bytes

        /// <summary>Create a salted PBKDF2 hash string for a new/updated password.</summary>
        public static string Hash(string password)
        {
            if (password == null) password = string.Empty;

            byte[] salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                hash = pbkdf2.GetBytes(HashSize);
            }

            return "PBKDF2." + Iterations + "." +
                   Convert.ToBase64String(salt) + "." +
                   Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Returns true if <paramref name="password"/> matches <paramref name="stored"/>.
        /// Handles the PBKDF2 format; also returns true for a legacy plaintext row that
        /// equals the password (so pre-existing databases keep working while callers
        /// re-hash on success).
        /// </summary>
        public static bool Verify(string password, string stored)
        {
            if (stored == null) return false;
            if (password == null) password = string.Empty;

            if (!stored.StartsWith("PBKDF2."))
            {
                // Legacy plaintext value — allow, caller should re-hash on success.
                return stored == password;
            }

            string[] parts = stored.Split('.');
            if (parts.Length != 4) return false;

            int iterations;
            if (!int.TryParse(parts[1], out iterations)) return false;

            byte[] salt, expected;
            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expected = Convert.FromBase64String(parts[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            byte[] actual;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                actual = pbkdf2.GetBytes(expected.Length);
            }

            return FixedTimeEquals(actual, expected);
        }

        /// <summary>True if <paramref name="stored"/> is NOT already a PBKDF2 hash.</summary>
        public static bool IsLegacyPlaintext(string stored)
        {
            return stored != null && !stored.StartsWith("PBKDF2.");
        }

        // Constant-time comparison (CryptographicOperations.FixedTimeEquals is not
        // available on .NET Framework 4.5, so implement it here).
        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }
    }
}
