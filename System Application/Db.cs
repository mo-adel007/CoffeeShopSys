using System;
using System.Data.SQLite;
using System.IO;

namespace Corner_Application
{
    /// <summary>
    /// Embedded-SQLite data layer bootstrap.
    ///
    /// The application is fully self-contained: it stores everything in a single
    /// portable file, <c>corner.db</c>, created next to the executable on first run.
    /// There is NO MySQL/MariaDB server and NO XAMPP requirement any more — backing up
    /// the shop's data is just copying that one file.
    ///
    /// <see cref="EnsureCreated"/> is called once at startup (Program.Main). It creates
    /// the database file and schema if missing and seeds the initial admin login. All
    /// forms open connections with <see cref="Program.Constring"/>, which now points at
    /// this SQLite file.
    /// </summary>
    public static class Db
    {
        /// <summary>Full path to the portable database file (next to the EXE).</summary>
        public static readonly string DbPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "corner.db");

        /// <summary>ADO.NET connection string for the portable SQLite database.</summary>
        public static string ConnectionString()
        {
            var builder = new SQLiteConnectionStringBuilder
            {
                DataSource = DbPath,
                Version = 3,
                // Foreign-key enforcement stays OFF (SQLite default) to preserve the
                // original app's insert behaviour, which did not depend on cascades.
                FailIfMissing = false
            };
            return builder.ToString();
        }

        /// <summary>
        /// Create the database file, schema and seed data if they do not yet exist.
        /// Safe to call on every launch (all statements are IF NOT EXISTS / idempotent).
        /// </summary>
        public static void EnsureCreated()
        {
            bool freshFile = !File.Exists(DbPath);
            if (freshFile)
            {
                SQLiteConnection.CreateFile(DbPath);
            }

            using (var conn = new SQLiteConnection(Program.Constring))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(Schema, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                SeedAdminIfEmpty(conn);
            }
        }

        /// <summary>
        /// Insert the default administrator login only when the table has no users.
        /// Default credentials: username "adel", password "1234" (stored hashed).
        /// The owner should change this password after first login.
        /// </summary>
        private static void SeedAdminIfEmpty(SQLiteConnection conn)
        {
            long count;
            using (var check = new SQLiteCommand("SELECT COUNT(*) FROM login;", conn))
            {
                count = Convert.ToInt64(check.ExecuteScalar());
            }
            if (count > 0) return;

            using (var insert = new SQLiteCommand(
                "INSERT INTO login (username, Pass, time_work) VALUES (@u, @p, @t);", conn))
            {
                insert.Parameters.AddWithValue("@u", "adel");
                insert.Parameters.AddWithValue("@p", Security.Hash("1234"));
                insert.Parameters.AddWithValue("@t", "admin");
                insert.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// SQLite schema, converted from the original MariaDB dump (System.sql).
        /// int -> INTEGER, double -> REAL, varchar/mediumtext -> TEXT, datetime -> TEXT
        /// (timestamps are written by the app as ISO strings, so text sorts correctly).
        /// AUTO_INCREMENT primary keys become INTEGER PRIMARY KEY AUTOINCREMENT. The
        /// two MariaDB foreign keys (close_day/close_shift -> login) are represented as
        /// plain indexed columns; enforcement was never relied upon by the app.
        /// </summary>
        private const string Schema = @"
CREATE TABLE IF NOT EXISTS all_monthlyexpenses (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  expens_name TEXT,
  Price REAL,
  Date_time TEXT
);

CREATE TABLE IF NOT EXISTS bill (
  id_bill INTEGER PRIMARY KEY AUTOINCREMENT,
  pro_name TEXT,
  pro_price REAL,
  quantity INTEGER,
  tot_price REAL,
  store INTEGER,
  IDproduct INTEGER
);

CREATE TABLE IF NOT EXISTS close_day (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  processT TEXT,
  price REAL,
  reason TEXT,
  proName TEXT,
  quantity INTEGER,
  ReasonOfT TEXT,
  PersonT TEXT,
  UserN TEXT,
  Dtime TEXT,
  Userid INTEGER,
  ShiftNumber INTEGER,
  whoPerson_add TEXT
);
CREATE INDEX IF NOT EXISTS Day_user_idx ON close_day (Userid);

CREATE TABLE IF NOT EXISTS close_shift (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  processT TEXT,
  price REAL,
  reason TEXT,
  proName TEXT,
  quantity INTEGER,
  ReasonOfT TEXT,
  PersonTake TEXT,
  UserN TEXT,
  Dtime TEXT,
  Userid INTEGER,
  ShiftNumber INTEGER,
  WhoPersonAdd TEXT
);
CREATE INDEX IF NOT EXISTS Shift_user_idx ON close_shift (Userid);

CREATE TABLE IF NOT EXISTS day_details (
  id_day INTEGER PRIMARY KEY AUTOINCREMENT,
  DayNum INTEGER,
  TotalSell REAL,
  TotalBuy REAL,
  ProfitDay REAL
);

CREATE TABLE IF NOT EXISTS login (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  username TEXT,
  Pass TEXT,
  time_work TEXT
);

CREATE TABLE IF NOT EXISTS monthly_expenses (
  id_expenses INTEGER PRIMARY KEY AUTOINCREMENT,
  expens_name TEXT,
  Price REAL
);

CREATE TABLE IF NOT EXISTS month_details (
  id_Month INTEGER PRIMARY KEY AUTOINCREMENT,
  MonthNumber INTEGER,
  TotalSell REAL,
  TotalBuy REAL,
  ProfitDay REAL,
  TotalMonthlyExpenses REAL
);

CREATE TABLE IF NOT EXISTS product (
  Product_id INTEGER PRIMARY KEY AUTOINCREMENT,
  Product_name TEXT,
  Price REAL,
  ProductType_id INTEGER,
  Store INTEGER,
  MakeAButton INTEGER
);

CREATE TABLE IF NOT EXISTS product_process (
  id_Process INTEGER PRIMARY KEY AUTOINCREMENT,
  User_Name TEXT,
  Process_type TEXT,
  Product_name TEXT,
  quantity INTEGER,
  price REAL,
  ""DateTime"" TEXT,
  IdProduct INTEGER,
  UserId INTEGER
);

CREATE TABLE IF NOT EXISTS product_type (
  type_id INTEGER PRIMARY KEY AUTOINCREMENT,
  type_name TEXT
);

CREATE TABLE IF NOT EXISTS safe (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  Type TEXT,
  price REAL,
  reason TEXT,
  proName TEXT,
  quantity INTEGER,
  ReasonOfT TEXT,
  PersonTake TEXT,
  UserN TEXT,
  Dtime TEXT,
  Userid INTEGER,
  Who_personAdd TEXT
);

CREATE TABLE IF NOT EXISTS shift_details (
  id_shift INTEGER PRIMARY KEY AUTOINCREMENT,
  ShiftNum INTEGER,
  TotalSell REAL,
  TotalBuy REAL,
  ProfitShift REAL
);

CREATE TABLE IF NOT EXISTS timing (
  Login_Date_time TEXT NOT NULL,
  User_Name TEXT,
  Logout_date_time TEXT,
  User_id INTEGER NOT NULL,
  PRIMARY KEY (Login_Date_time, User_id)
);

CREATE TABLE IF NOT EXISTS user_shift (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  UserName TEXT,
  ShiftNumber TEXT
);
";
    }
}
