using System;
using System.IO;
using System.Text;
using Microsoft.Win32;
using Corner_Application; // shared Db (data-folder path)

namespace CornerPos.Licensing
{
    /// <summary>Outcome of a license evaluation at startup.</summary>
    public enum LicenseState { Trial, Licensed, Expired }

    /// <summary>Snapshot of the current licensing situation.</summary>
    public sealed class LicenseStatus
    {
        public LicenseState State { get; set; }
        /// <summary>Whole trial days remaining (0 once activated — a fixed key is perpetual).</summary>
        public int DaysLeft { get; set; }

        /// <summary>True while the trial is about to run out (drives the login-time warning).</summary>
        public bool WarnSoon =>
            State == LicenseState.Trial && DaysLeft <= LicenseManager.WarnThresholdDays;

        public bool CanUseApp => State != LicenseState.Expired;
    }

    /// <summary>
    /// Offline licensing for CornerPos, fixed-key model.
    ///
    /// Every install activates with the SAME hardcoded key (<see cref="FixedKey"/>). There
    /// is no expiry and no per-customer key: entering the correct string activates the app
    /// permanently on that machine. Before a key is entered the app runs a 7-day trial.
    ///
    /// Trade-off (chosen deliberately): the key is the same for everyone, so if it leaks
    /// anyone can activate. Change <see cref="FixedKey"/> and rebuild to invalidate keys
    /// already handed out.
    ///
    /// The trial start date is mirrored to two places (an HKCU registry value and a hidden
    /// file in the data folder) and cross-checked, so deleting just one of them does not
    /// reset the trial. Wiping BOTH still resets it (basic-tier protection).
    /// </summary>
    public static class LicenseManager
    {
        public const int TrialDays = 7;
        public const int WarnThresholdDays = 5;

        /// <summary>
        /// The single activation key shared by every install. Compared case-insensitively.
        /// Change this string (and rebuild) to invalidate all previously distributed keys.
        /// </summary>
        public const string FixedKey = "CORNER-MAS3-9578-NASU-56L8";

        private const string RegistryKeyPath = @"Software\CornerPos";
        private const string RegistryValueName = "Seed";
        private const long TrialMask = 0x5F3759DF5A17C0DEL; // obfuscation only, not secret

        private static string LicenseFile => Path.Combine(Db.DataDirectory, "license.key");
        private static string TrialFile => Path.Combine(Db.DataDirectory, ".trialstate");

        /// <summary>
        /// Decide the current licensing state. Called at startup and after a key is entered.
        /// Never throws — any failure degrades to the trial path.
        /// </summary>
        public static LicenseStatus Evaluate()
        {
            // 1) Already activated with the fixed key -> licensed forever.
            if (IsActivated())
                return new LicenseStatus { State = LicenseState.Licensed, DaysLeft = 0 };

            // 2) Otherwise run/continue the 7-day trial.
            DateTime start = ReadOrStartTrial();
            int used = (int)Math.Floor((Today - start.Date).TotalDays);
            int left = TrialDays - used;
            if (left > 0)
                return new LicenseStatus { State = LicenseState.Trial, DaysLeft = left };

            return new LicenseStatus { State = LicenseState.Expired };
        }

        /// <summary>
        /// Validate a key the user typed/pasted. If it matches the fixed key, persist the
        /// activation. Returns true on success; otherwise <paramref name="error"/> carries
        /// an Arabic message describing why it was rejected.
        /// </summary>
        public static bool TryActivate(string keyText, out string error)
        {
            error = null;
            keyText = (keyText ?? "").Trim();
            if (keyText.Length == 0) { error = "من فضلك أدخل مفتاح الترخيص."; return false; }

            if (!KeyMatches(keyText))
            {
                error = "مفتاح الترخيص غير صحيح.";
                return false;
            }

            try
            {
                Directory.CreateDirectory(Db.DataDirectory);
                File.WriteAllText(LicenseFile, FixedKey, Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                error = "تعذر حفظ المفتاح: " + ex.Message;
                return false;
            }
        }

        private static bool IsActivated()
        {
            try { return File.Exists(LicenseFile) && KeyMatches(File.ReadAllText(LicenseFile, Encoding.UTF8)); }
            catch { return false; }
        }

        private static bool KeyMatches(string k) =>
            string.Equals((k ?? "").Trim(), FixedKey, StringComparison.OrdinalIgnoreCase);

        private static DateTime Today => DateTime.UtcNow.Date;

        // ---- Trial marker: mirrored across registry + hidden file, cross-checked ----

        private static DateTime ReadOrStartTrial()
        {
            DateTime? fromReg = ReadRegistryStart();
            DateTime? fromFile = ReadFileStart();

            // Earliest known start wins, so recreating one marker at "now" cannot shorten
            // (or reset) a trial while the other marker still remembers the real start.
            DateTime? start = null;
            if (fromReg.HasValue) start = fromReg;
            if (fromFile.HasValue && (!start.HasValue || fromFile.Value < start.Value)) start = fromFile;

            if (!start.HasValue) start = DateTime.UtcNow; // genuine first run

            // Re-mirror to heal a missing/tampered side.
            WriteRegistryStart(start.Value);
            WriteFileStart(start.Value);
            return start.Value;
        }

        private static DateTime? ReadRegistryStart()
        {
            try
            {
                using (RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
                {
                    if (k == null) return null;
                    return DecodeTicks(k.GetValue(RegistryValueName) as string);
                }
            }
            catch { return null; }
        }

        private static void WriteRegistryStart(DateTime start)
        {
            try
            {
                using (RegistryKey k = Registry.CurrentUser.CreateSubKey(RegistryKeyPath))
                    if (k != null) k.SetValue(RegistryValueName, EncodeTicks(start), RegistryValueKind.String);
            }
            catch { /* best effort */ }
        }

        private static DateTime? ReadFileStart()
        {
            try { return File.Exists(TrialFile) ? DecodeTicks(File.ReadAllText(TrialFile)) : null; }
            catch { return null; }
        }

        private static void WriteFileStart(DateTime start)
        {
            try
            {
                Directory.CreateDirectory(Db.DataDirectory);
                File.WriteAllText(TrialFile, EncodeTicks(start));
                File.SetAttributes(TrialFile, FileAttributes.Hidden);
            }
            catch { /* best effort */ }
        }

        private static string EncodeTicks(DateTime dt)
        {
            long v = dt.ToUniversalTime().Ticks ^ TrialMask;
            return Convert.ToBase64String(BitConverter.GetBytes(v));
        }

        private static DateTime? DecodeTicks(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            try
            {
                long v = BitConverter.ToInt64(Convert.FromBase64String(s), 0) ^ TrialMask;
                return new DateTime(v, DateTimeKind.Utc);
            }
            catch { return null; }
        }
    }
}
