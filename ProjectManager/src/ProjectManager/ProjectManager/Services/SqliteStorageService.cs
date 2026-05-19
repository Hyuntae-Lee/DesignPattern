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
        private readonly Settings _settings;
        private readonly ProjectStorage _projectStorage;
        private readonly ProjectResourceStorage _resourceStorage;

        public SqliteStorageService(
            string dbPath,
            Settings settings,
            ProjectStorage projectStorage,
            ProjectResourceStorage resourceStorage)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _projectStorage = projectStorage ?? throw new ArgumentNullException(nameof(projectStorage));
            _resourceStorage = resourceStorage ?? throw new ArgumentNullException(nameof(resourceStorage));

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
                    "CREATE TABLE IF NOT EXISTS projects (" +
                    "  id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "  name TEXT" +
                    ");");
                ExecNonQuery(conn,
                    "CREATE TABLE IF NOT EXISTS tasks (" +
                    "  id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "  project_id INTEGER," +
                    "  name TEXT," +
                    "  begin_time TEXT," +
                    "  end_time TEXT," +
                    "  assignee_name TEXT," +
                    "  FOREIGN KEY(project_id) REFERENCES projects(id) ON DELETE CASCADE" +
                    ");");
            }
        }

        // Settings ------------------------------------------------------

        public void SaveSettings()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    UpsertSetting(conn, tx, "BeginTime", FormatDate(_settings.BeginTime));
                    UpsertSetting(conn, tx, "EndTime", FormatDate(_settings.EndTime));
                    tx.Commit();
                }
            }
        }

        public void LoadSettings()
        {
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
                _settings.BeginTime = bt;
            if (values.TryGetValue("EndTime", out var e) && TryParseDate(e, out var et))
                _settings.EndTime = et;
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

        public void SaveResources()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    ExecNonQuery(conn, tx, "DELETE FROM resources;");

                    foreach (var r in _resourceStorage.projectResources)
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

        public void LoadResources()
        {
            _resourceStorage.projectResources.Clear();

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT name, affiliation FROM resources;", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _resourceStorage.projectResources.Add(new ProjectResource
                        {
                            Name = reader.IsDBNull(0) ? null : reader.GetString(0),
                            Affiliation = reader.IsDBNull(1) ? null : reader.GetString(1),
                        });
                    }
                }
            }
        }

        // Projects (with tasks) -----------------------------------------

        public void SaveProjects()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    ExecNonQuery(conn, tx, "DELETE FROM tasks;");
                    ExecNonQuery(conn, tx, "DELETE FROM projects;");

                    foreach (var project in _projectStorage.Projects)
                    {
                        long projectId;
                        using (var cmd = new SQLiteCommand(
                            "INSERT INTO projects (name) VALUES (@n); SELECT last_insert_rowid();", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@n", project.Name ?? string.Empty);
                            projectId = (long)cmd.ExecuteScalar();
                        }

                        foreach (var t in project.ProjectTasks)
                        {
                            using (var cmd = new SQLiteCommand(
                                "INSERT INTO tasks (project_id, name, begin_time, end_time, assignee_name) " +
                                "VALUES (@p, @n, @b, @e, @a);", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@p", projectId);
                                cmd.Parameters.AddWithValue("@n", t.Name ?? string.Empty);
                                cmd.Parameters.AddWithValue("@b", FormatDate(t.BeginTime));
                                cmd.Parameters.AddWithValue("@e", FormatDate(t.EndTime));
                                cmd.Parameters.AddWithValue("@a", (object)t.Assignee?.Name ?? DBNull.Value);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    tx.Commit();
                }
            }
        }

        public void LoadProjects()
        {
            _projectStorage.Projects.Clear();

            var lookup = new Dictionary<string, ProjectResource>(StringComparer.Ordinal);
            foreach (var r in _resourceStorage.projectResources)
            {
                if (!string.IsNullOrEmpty(r.Name) && !lookup.ContainsKey(r.Name))
                    lookup[r.Name] = r;
            }

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();

                var projectsById = new Dictionary<long, Project>();
                using (var cmd = new SQLiteCommand("SELECT id, name FROM projects;", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt64(0);
                        var project = new Project
                        {
                            Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                        };
                        projectsById[id] = project;
                        _projectStorage.Projects.Add(project);
                    }
                }

                using (var cmd = new SQLiteCommand(
                    "SELECT project_id, name, begin_time, end_time, assignee_name FROM tasks;", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(0)) continue;
                        var projectId = reader.GetInt64(0);
                        if (!projectsById.TryGetValue(projectId, out var project)) continue;

                        var task = new ProjectTask
                        {
                            Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                        };

                        if (!reader.IsDBNull(2) && TryParseDate(reader.GetString(2), out var bt))
                            task.BeginTime = bt;
                        if (!reader.IsDBNull(3) && TryParseDate(reader.GetString(3), out var et))
                            task.EndTime = et;

                        if (!reader.IsDBNull(4))
                        {
                            var assigneeName = reader.GetString(4);
                            if (lookup.TryGetValue(assigneeName, out var resource))
                                task.Assignee = resource;
                            else
                                task.Assignee = new ProjectResource { Name = assigneeName };
                        }

                        project.ProjectTasks.Add(task);
                    }
                }
            }
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
