using System;
using System.Data;

namespace CornerPos
{
    /// <summary>
    /// Shift-number assignment, replicating the legacy Form1/user_shift logic: a
    /// cashier reuses the current max shift number if they were the last user on it,
    /// otherwise a new shift (max + 1) is opened and a user_shift row is recorded.
    /// The user_shift table is cleared by Close Day, which resets numbering.
    /// Admins are not assigned a shift (they use 0).
    /// </summary>
    internal static class Shift
    {
        public static int AssignForCashier(string userName)
        {
            var dt = Data.Query("SELECT UserName, ShiftNumber FROM user_shift;");
            int maxShift = 0;
            string lastUser = null;
            foreach (DataRow r in dt.Rows)
            {
                int sn;
                int.TryParse(Convert.ToString(r["ShiftNumber"]), out sn);
                if (sn > maxShift) { maxShift = sn; lastUser = Convert.ToString(r["UserName"]); }
            }

            if (maxShift > 0 && userName == lastUser)
                return maxShift; // same cashier continues the open shift

            int shift = maxShift + 1;
            Data.Execute("INSERT INTO user_shift (UserName, ShiftNumber) VALUES (@u, @s);",
                ("@u", userName), ("@s", shift.ToString()));
            return shift;
        }
    }
}
