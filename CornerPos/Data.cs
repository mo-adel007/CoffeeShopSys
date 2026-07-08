using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Corner_Application; // shared Db

namespace CornerPos
{
    /// <summary>
    /// Thin ADO.NET helper over the portable SQLite database so screens don't repeat
    /// the open / parameterize / execute boilerplate. Every call opens and closes its
    /// own connection against <see cref="Db.ConnectionString"/>. All parameters are
    /// bound (never string-concatenated) so nothing here is injectable.
    /// </summary>
    internal static class Data
    {
        public static DataTable Query(string sql, params (string name, object value)[] ps)
        {
            using (var conn = new SQLiteConnection(Db.ConnectionString()))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    Bind(cmd, ps);
                    var dt = new DataTable();
                    using (var da = new SQLiteDataAdapter(cmd)) da.Fill(dt);
                    return dt;
                }
            }
        }

        public static int Execute(string sql, params (string name, object value)[] ps)
        {
            using (var conn = new SQLiteConnection(Db.ConnectionString()))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    Bind(cmd, ps);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static object Scalar(string sql, params (string name, object value)[] ps)
        {
            using (var conn = new SQLiteConnection(Db.ConnectionString()))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    Bind(cmd, ps);
                    return cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>Run several statements in one transaction (all-or-nothing).</summary>
        public static void Batch(IEnumerable<(string sql, (string name, object value)[] ps)> statements)
        {
            using (var conn = new SQLiteConnection(Db.ConnectionString()))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    foreach (var s in statements)
                        using (var cmd = new SQLiteCommand(s.sql, conn))
                        {
                            Bind(cmd, s.ps);
                            cmd.ExecuteNonQuery();
                        }
                    tx.Commit();
                }
            }
        }

        private static void Bind(SQLiteCommand cmd, (string name, object value)[] ps)
        {
            if (ps == null) return;
            foreach (var p in ps)
                cmd.Parameters.AddWithValue(p.name, p.value ?? System.DBNull.Value);
        }
    }
}
