using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace WpfApp1
{
    public class DatabaseHelper
    {
        private string connectionString = "Server=localhost;Database=sidekick_db;Uid=root;Pwd=Mutsinda#01;";

        public bool AddTask(string title, int reminderDays)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO security_tasks (title, reminder_days) VALUES (@title, @reminderDays);";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@reminderDays", reminderDays);

                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetAllTasksFormatted()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT id, title, reminder_days, is_completed FROM security_tasks ORDER BY created_at DESC;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                return "📋 No cybersecurity tasks found in the database tracking architecture.";
                            }

                            sb.AppendLine("📋 **Current Database Security Tasks:**\n");
                            while (reader.Read())
                            {
                                int id = reader.GetInt32("id");
                                string title = reader.GetString("title");
                                int days = reader.GetInt32("reminder_days");
                                bool completed = reader.GetBoolean("is_completed");

                                string status = completed ? "✅ [Completed]" : "⏳ [Pending]";
                                string reminderText = days > 0 ? $"({days}-day alert interval)" : "(No alerts)";

                                sb.AppendLine($"ID {id}: {status} {title} {reminderText}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "❌ Connection failure. Could not fetch tasks from local MySQL instance. Error: " + ex.Message;
            }

            return sb.ToString();
        }
    }
}