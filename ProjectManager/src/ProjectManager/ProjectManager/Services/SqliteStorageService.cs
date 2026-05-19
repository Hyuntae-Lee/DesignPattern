using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using ProjectManager.Models;

namespace ProjectManager.Services
{
    public class SqliteStorageService
    {
        private readonly string _connectionString;

        public SqliteStorageService(string dbPath)
        {
            var dir = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            _connectionString = "Data Source=" + dbPath + ";Version=3;";

            EnsureSchema();
        }

        private void EnsureSchema()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                ExecNonQuery(conn,
                    "CREATE TABLE IF NOT EXISTS settings (" +
                    "  key TEXT PRIMARY KEY," +
                    "  value TEXT" +
                    ");");
                ExecNonQuery(conn,
                    "CREATE TABLE IF NOT EXISTS resources (" +
                    "  name TEXT PRIMARY KEY," +
                    "  affiliation TEXT" +
                    ");");
                ExecNonQuery(conn,
                    "CREATE TABLE IF NOT EXISTS tasks (" +
                    "  id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "  name TEXT," +
                    "  begin_time TEXT," +
                    "  end_time TEXT," +
                    "  assignee_name TEXT" +
                    ");");
            }
        }

        // Settings ------------------------------------------------------

        public void SaveSettings(Settings settings)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    UpsertSetting(conn, tx, "BeginTime", FormatDate(settings.BeginTime));
                    UpsertSetting(conn, tx, "EndTime", FormatDate(settings.EndTime));
                    tx.Commit();
                }
            }
        }

        public Settings LoadSettings()
        {
            var settings = new Settings();
            var values = new Dictionary<string, string>();

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT key, value FROM settings;", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        values[reader.GetString(0)] = reader.IsDBNull(1) ? null : reader.GetString(1);
                }
            }

            if (values.TryGetValue("BeginTime", out var b) && TryParseDate(b, out var bt))
                settings.BeginTime = bt;
            if (values.TryGetValue("EndTime", out var e) && TryParseDate(e, out var et))
                settings.EndTime = et;

            return settings;
        }

        private static void UpsertSetting(SQLiteConnection conn, SQLiteTransaction tx, string key, string value)
        {
            using (var cmd = new SQLiteCommand(
                "INSERT INTO settings (key, value) VALUES (@k, @v) " +
                "ON CONFLICT(key) DO UPDATE SET value = excluded.value;", conn, tx))
            {
                cmd.Parameters.AddWithValue("@k", key);
                cmd.Parameters.AddWithValue("@v", value);
                cmd.ExecuteNonQuery();
            }
        }

        // Resources -----------------------------------------------------

        public void SaveResources(ProjectResourceStorage storage)
        {
            if (storage == null) throw new ArgumentNullException(nameof(storage));

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    ExecNonQuery(conn, tx, "DELETE FROM resources;");

                    foreach (var r in storage.projectResources)
                    {
                        using (var cmd = new SQLiteCommand(
                            "INSERT OR REPLACE INTO resources (name, affiliation) VALUES (@n, @a);", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@n", r.Name ?? string.Empty);
                            cmd.Parameters.AddWithValue("@a", (object)r.Affiliation ?? DBNull.Value);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    tx.Commit();
                }
            }
        }

        public ProjectResourceStorage LoadResources()
        {
            var storage = new ProjectResourceStorage();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT name, affiliation FROM resources;", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        storage.projectResources.Add(new ProjectResource
                        {
                            Name = reader.IsDBNull(0) ? null : reader.GetString(0),
                            Affiliation = reader.IsDBNull(1) ? null : reader.GetString(1),
                        });
                    }
                }
            }
            return storage;
        }

        // Project (tasks) -----------------------------------------------

        public void SaveProject(Project project)
        {
            if (project == null) throw new ArgumentNullException(nameof(project));

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    ExecNonQuery(conn, tx, "DELETE FROM tasks;");

                    foreach (var t in project.ProjectTasks)
                    {
                        using (var cmd = new SQLiteCommand(
                            "INSERT INTO tasks (name, begin_time, end_time, assignee_name) " +
                            "VALUES (@n, @b, @e, @a);", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@n", t.Name ?? string.Empty);
                            cmd.Parameters.AddWithValue("@b", FormatDate(t.BeginTime));
                            cmd.Parameters.AddWithValue("@e", FormatDate(t.EndTime));
                            cmd.Parameters.AddWithValue("@a", (object)t.Assignee?.Name ?? DBNull.Value);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    tx.Commit();
                }
            }
        }

        public Project LoadProject(ProjectResourceStorage resources = null)
        {
            var project = new Project();

            Dictionary<string, ProjectResource> lookup = null;
            if (resources != null)
            {
                lookup = new Dictionary<string, ProjectResource>(StringComparer.Ordinal);
                foreach (var r in resources.projectResources)
                {
                    if (!string.IsNullOrEmpty(r.Name) && !lookup.ContainsKey(r.Name))
                        lookup[r.Name] = r;
                }
            }

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    "SELECT name, begin_time, end_time, assignee_name FROM tasks;", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var task = new ProjectTask
                        {
                            Name = reader.IsDBNull(0) ? null : reader.GetString(0),
                        };

                        if (!reader.IsDBNull(1) && TryParseDate(reader.GetString(1), out var bt))
                            task.BeginTime = bt;
                        if (!reader.IsDBNull(2) && TryParseDate(reader.GetString(2), out var et))
                            task.EndTime = et;

                        if (!reader.IsDBNull(3))
                        {
                            var assigneeName = reader.GetString(3);
                            if (lookup != null && lookup.TryGetValue(assigneeName, out var resource))
                                task.Assignee = resource;
                            else
                                task.Assignee = new ProjectResource { Name = assigneeName };
                        }

                        project.ProjectTasks.Add(task);
                    }
                }
            }

            return project;
        }

        // Helpers -------------------------------------------------------

        private static void ExecNonQuery(SQLiteConnection conn, string sql)
        {
            using (var cmd = new SQLiteCommand(sql, conn))
                cmd.ExecuteNonQuery();
        }

        private static void ExecNonQuery(SQLiteConnection conn, SQLiteTransaction tx, string sql)
        {
            using (var cmd = new SQLiteCommand(sql, conn, tx))
                cmd.ExecuteNonQuery();
        }

        private static string FormatDate(DateTime dt) =>
            dt.ToString("o", CultureInfo.InvariantCulture);

        private static bool TryParseDate(string s, out DateTime dt) =>
            DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dt);
    }
}
