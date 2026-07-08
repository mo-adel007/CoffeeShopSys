using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CornerKeyGen
{
    /// <summary>
    /// Corner license key generator (developer tool — never shipped).
    ///
    /// Signs license tokens with an ECDSA P-256 PRIVATE key. CornerPos verifies them
    /// with the matching PUBLIC key baked into LicenseManager, so keys cannot be forged
    /// without this tool + its private key file.
    ///
    /// The private key lives OUTSIDE the repository at:
    ///   %USERPROFILE%\.cornerpos-keygen\private.key
    ///
    /// Usage:
    ///   dotnet run --project KeyGen -- keygen
    ///       Create a new key pair. Prints the PUBLIC X/Y to paste into LicenseManager.
    ///       (Regenerating invalidates every previously issued key.)
    ///
    ///   dotnet run --project KeyGen -- issue "Customer Name" 365
    ///   dotnet run --project KeyGen -- issue "Customer Name" 2027-01-31
    ///       Mint a key expiring in N days, or on an explicit yyyy-MM-dd date.
    /// </summary>
    internal static class Program
    {
        private static string KeyDir =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                         ".cornerpos-keygen");
        private static string PrivateFile => Path.Combine(KeyDir, "private.key");

        private static int Main(string[] args)
        {
            try
            {
                if (args.Length == 0) { PrintUsage(); return 1; }

                switch (args[0].ToLowerInvariant())
                {
                    case "keygen": return KeyGen();
                    case "issue": return Issue(args);
                    default: PrintUsage(); return 1;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: " + ex.Message);
                return 2;
            }
        }

        private static int KeyGen()
        {
            if (File.Exists(PrivateFile))
            {
                Console.Write("A private key already exists at " + PrivateFile +
                              ".\nOverwrite it (invalidates all issued keys)? [y/N] ");
                string ans = Console.ReadLine();
                if (!string.Equals(ans?.Trim(), "y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Aborted.");
                    return 1;
                }
            }

            using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            ECParameters full = ecdsa.ExportParameters(true);

            Directory.CreateDirectory(KeyDir);
            // Store D, X, Y (base64) — private material kept out of the repo.
            File.WriteAllLines(PrivateFile, new[]
            {
                Convert.ToBase64String(full.D),
                Convert.ToBase64String(full.Q.X),
                Convert.ToBase64String(full.Q.Y)
            });

            string x = Convert.ToBase64String(full.Q.X);
            string y = Convert.ToBase64String(full.Q.Y);

            Console.WriteLine("Key pair created.");
            Console.WriteLine("Private key saved to: " + PrivateFile);
            Console.WriteLine();
            Console.WriteLine("Paste the PUBLIC key into CornerPos/Licensing/LicenseManager.cs:");
            Console.WriteLine();
            Console.WriteLine("        private const string PublicKeyX = \"" + x + "\";");
            Console.WriteLine("        private const string PublicKeyY = \"" + y + "\";");
            Console.WriteLine();
            return 0;
        }

        private static int Issue(string[] args)
        {
            if (args.Length < 3)
            {
                Console.Error.WriteLine("Usage: issue \"Customer Name\" <days | yyyy-MM-dd>");
                return 1;
            }
            if (!File.Exists(PrivateFile))
            {
                Console.Error.WriteLine("No private key found. Run 'keygen' first.");
                return 1;
            }

            string customer = args[1].Trim();
            DateTime expiry;
            if (int.TryParse(args[2], out int days))
                expiry = DateTime.UtcNow.Date.AddDays(days);
            else if (!DateTime.TryParseExact(args[2], "yyyy-MM-dd",
                         CultureInfo.InvariantCulture, DateTimeStyles.None, out expiry))
            {
                Console.Error.WriteLine("Third argument must be a day count or a yyyy-MM-dd date.");
                return 1;
            }

            string[] lines = File.ReadAllLines(PrivateFile);
            var p = new ECParameters
            {
                Curve = ECCurve.NamedCurves.nistP256,
                D = Convert.FromBase64String(lines[0]),
                Q = new ECPoint
                {
                    X = Convert.FromBase64String(lines[1]),
                    Y = Convert.FromBase64String(lines[2])
                }
            };

            // Payload: v1|yyyyMMdd|customer   (signed as UTF-8 bytes)
            string payloadText = "v1|" + expiry.ToString("yyyyMMdd") + "|" + customer;
            byte[] payload = Encoding.UTF8.GetBytes(payloadText);

            byte[] sig;
            using (var ecdsa = ECDsa.Create())
            {
                ecdsa.ImportParameters(p);
                sig = ecdsa.SignData(payload, HashAlgorithmName.SHA256); // IEEE-P1363 default
            }

            string token = ToBase64Url(payload) + "." + ToBase64Url(sig);

            Console.WriteLine("Customer : " + customer);
            Console.WriteLine("Expires  : " + expiry.ToString("yyyy-MM-dd"));
            Console.WriteLine("License key:");
            Console.WriteLine();
            Console.WriteLine(token);
            Console.WriteLine();
            return 0;
        }

        private static string ToBase64Url(byte[] bytes) =>
            Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

        private static void PrintUsage()
        {
            Console.WriteLine("Corner license key generator");
            Console.WriteLine("  keygen                              create a new key pair");
            Console.WriteLine("  issue \"Customer\" <days|yyyy-MM-dd>   mint a signed license key");
        }
    }
}
