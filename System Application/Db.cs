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

            using (var conn = new SQLiteConnection(ConnectionString()))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(Schema, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                SeedAdminIfEmpty(conn);
                SeedMenuIfEmpty(conn);
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
        /// Seed the coffee menu (7 categories + the shop's products) on a fresh
        /// database so the cashier screen is usable immediately. Products are given a
        /// placeholder price of 20 and stock of 100 — the owner adjusts both later in
        /// the Products screen.
        /// </summary>
        private static void SeedMenuIfEmpty(SQLiteConnection conn)
        {
            long typeCount;
            using (var check = new SQLiteCommand("SELECT COUNT(*) FROM product_type;", conn))
                typeCount = Convert.ToInt64(check.ExecuteScalar());

            if (typeCount == 0)
            {
                string[,] types =
                {
                    {"1", "قهوة"}, {"2", "شاى"}, {"3", "نسكافيه"}, {"4", "البن"},
                    {"5", "waffle"}, {"6", "الثلاجة"}, {"7", "أخرى"}
                };
                using (var tx = conn.BeginTransaction())
                {
                    for (int i = 0; i < types.GetLength(0); i++)
                        using (var cmd = new SQLiteCommand(
                            "INSERT INTO product_type (type_id, type_name) VALUES (@id, @name);", conn))
                        {
                            cmd.Parameters.AddWithValue("@id", int.Parse(types[i, 0]));
                            cmd.Parameters.AddWithValue("@name", types[i, 1]);
                            cmd.ExecuteNonQuery();
                        }
                    tx.Commit();
                }
            }

            long prodCount;
            using (var check = new SQLiteCommand("SELECT COUNT(*) FROM product;", conn))
                prodCount = Convert.ToInt64(check.ExecuteScalar());

            if (prodCount == 0)
            {
                using (var tx = conn.BeginTransaction())
                {
                    foreach (var raw in MenuData.Split('\n'))
                    {
                        string line = raw.Trim();
                        if (line.Length == 0) continue;
                        int bar = line.IndexOf('|');
                        int typeId = int.Parse(line.Substring(0, bar));
                        string name = line.Substring(bar + 1);
                        using (var cmd = new SQLiteCommand(
                            "INSERT INTO product (Product_name, Price, ProductType_id, Store, MakeAButton) " +
                            "VALUES (@name, 20, @type, 100, 1);", conn))
                        {
                            cmd.Parameters.AddWithValue("@name", name);
                            cmd.Parameters.AddWithValue("@type", typeId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                }
            }
        }

        // typeId|productName — the shop's original menu (price/stock are placeholders).
        private const string MenuData = @"
1|قهوة تركى
1|قهوة تركى دوبل
1|قهوة اسبريسو
1|قهوة اسبريسو دوبل
1|قهوة بندق
1|قهوة بالكريم الايرلندى
1|قهوة فرنساوى
1|قهوة فرنساوى دوبل
1|قهوة كراميل
1|قهوة لوز
1|قهوة فستق
1|قهوة شيكولاته
1|قهوة فانيليا
1|قهوة ميكس
1|قهوة نكهات دوبل
1|قهوة قرفة
3|نسكافيه
3|نسكافيه بلاك
3|نسكافيه كراميل
3|نسكافيه جولد
3|كوفى ميكس
3|كابتشينو ماكينة
3|كابتشينو كلاسيك
3|كابتشينو بندق
3|كابتشينو فانيليا
3|كابتشينو موكا
3|كابتشينو شيكولاته
3|نسكويك
3|لاتيه
3|موكا
3|هوت شوكلت
3|شوكو ميلك
3|ماكياتو
3|فرابينو بالفانيليا
3|فرابينو بالكراميل
2|شاى
2|شاى لاتيه شيكولاته
2|شاى تفاح
2|شاى مانجو
2|شاى توت
2|شاى فانيليا
2|شاى مشمش
2|شاى مانجو و خوخ
2|شاى توت برى
2|شاى توت و فراولة
2|شاى خوخ
2|شاى خوخ و فواكه استوائية
2|شاى فراولة
2|شاى فراولة و كيوى
2|شاى فراولة و رمان
2|شاى توت و رمان
2|شاى عدنى
2|شاى ياسمين
2|شاى اخضر
2|شاى اخضر بالياسمين
2|شاى إيرل جراى
2|شاى بالليمون
7|ينسون
7|كركديه
7|نعناع
7|نعناع بالكاموميل
7|جنزبيل
7|كراويه
7|ليمون بالجنزبيل
7|قرفة
7|تليو
7|جنزبيل بالقرفة
5|waffle basic
2|شاى فواكة برية
2|شاى خوخ وورد
4|سادة فاتح
4|سادة وسط
4|سادة غامق
4|سادة محروق
4|محوج فاتح
4|محوج وسط
4|محوج غامق
4|محوج محروق
4|كولومبى
4|حبشى
3|نسكافية فانيليا
3|نسكافية بندق
2|شاى بالنعناع
2|شاى بالقرنفل
3|لاتية كراميل
2|شاى مغربى
1|قهوة كولومبى
1|قهوة حبشى
6|مياة كبير
6|مياة صغير
6|كانز كبير
6|فروتز
6|لبن
2|اضافة حليب
3|نسكافية موكا
1|قهوة تركى محوج
1|قهوة تركى محوج دوبل
1|قهوة بالحليب
";

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
