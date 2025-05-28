using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;

namespace N4_BTCM.Controller
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int RoleID { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserStatus Status { get; set; }
        public string ImagePath { get; set; }
    }

    public enum UserStatus
    {
        Inactive = 0,
        Active = 1
    }

    internal class QuanLyNguoiDung
    {
        private DBConnection dbConn = new DBConnection();

        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            using (SqlConnection conn = dbConn.GetConnection())
            {
                if (conn == null) return users;

                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Users WHERE RoleID = 3", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new User
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                Username = reader["Username"].ToString(),
                                Fullname = reader["FullName"]?.ToString(),
                                Email = reader["Email"]?.ToString(),
                                Phone = reader["Phone"]?.ToString(),
                                Address = reader["Address"]?.ToString(),
                                RoleID = Convert.ToInt32(reader["RoleID"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                Status = (UserStatus)Convert.ToInt32(reader["Status"]),
                                ImagePath = reader["ImagePath"].ToString()
                            };
                            users.Add(user);
                        }
                    }
                }
            }
            return users;
        }

        public bool AddUser(User user)
        {
            using (SqlConnection conn = dbConn.GetConnection())
            {
                if (conn == null) return false;

                string rawPassword = RemoveDiacritics(user.Fullname)
                            .Replace(" ", "")
                            .ToLower()
                            + "123";
                string passwordHash = PasswordHasher.HashPassword(rawPassword);

                string query = @"INSERT INTO Users 
                    (Username, PasswordHash, FullName, Email, Phone, Address, RoleID, CreatedAt, Status, ImagePath)
                    VALUES 
                    (@Username, @PasswordHash, @FullName, @Email, @Phone, @Address, @RoleID, @CreatedAt, @Status,@ImagePath)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", user.Fullname);
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    cmd.Parameters.AddWithValue("@FullName", user.Fullname);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Phone", user.Phone);
                    cmd.Parameters.AddWithValue("@Address", user.Address);
                    cmd.Parameters.AddWithValue("@RoleID",3);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now); // Tự động lấy thời gian hiện tại
                    cmd.Parameters.AddWithValue("@Status", (int)UserStatus.Active);
                    cmd.Parameters.AddWithValue("@ImagePath", user.ImagePath ?? (object)DBNull.Value);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public static string RemoveDiacritics(string text)
        {
            string normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }


        public bool UpdateUser(User user)
        {
            using (SqlConnection conn = dbConn.GetConnection())
            {
                if (conn == null) return false;

                string query = @"UPDATE Users 
                    SET FullName = @FullName, Email = @Email, Phone = @Phone,
                        Address = @Address,ImagePath = @ImagePath
                    WHERE UserID = @UserID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FullName", user.Fullname);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Phone", user.Phone);
                    cmd.Parameters.AddWithValue("@Address", user.Address);
                    cmd.Parameters.AddWithValue("@UserID", user.UserID);
                    cmd.Parameters.AddWithValue("@ImagePath", user.ImagePath ?? (object)DBNull.Value);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeactivateUser(int userID)
        {
            using (SqlConnection conn = dbConn.GetConnection())
            {
                if (conn == null) return false;

                string query = "UPDATE Users SET Status = @Status WHERE UserID = @UserID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.Parameters.AddWithValue("@Status", (int)UserStatus.Inactive);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool ActivateUser(int userID)
        {
            using (SqlConnection conn = dbConn.GetConnection())
            {
                if (conn == null) return false;

                string query = "UPDATE Users SET Status = @Status WHERE UserID = @UserID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.Parameters.AddWithValue("@Status", (int)UserStatus.Active);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool IsValueExists(string columnName, string value, int? excludeUserId = null)
        {
            using (SqlConnection conn = dbConn.GetConnection())
            {
                string query = $"SELECT COUNT(*) FROM Users WHERE {columnName} = @value";
                if (excludeUserId.HasValue)
                {
                    query += " AND UserID != @excludeUserId";
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@value", value);
                    if (excludeUserId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@excludeUserId", excludeUserId.Value);
                    }
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }
        public bool ResetPassword(int userId, string newPassword)
        {
            using (SqlConnection conn = dbConn.GetConnection())
            {
                string passwordHash = PasswordHasher.HashPassword(newPassword);
                string query = "UPDATE Users SET PasswordHash = @PasswordHash WHERE UserID = @UserID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public List<User> GetAllStaffs()
        {
            List<User> users = new List<User>();
            using (SqlConnection conn = dbConn.GetConnection())
            {
                if (conn == null) return users;

                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Users WHERE RoleID = 2", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new User
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                Username = reader["Username"].ToString(),
                                Fullname = reader["FullName"]?.ToString(),
                                Email = reader["Email"]?.ToString(),
                                Phone = reader["Phone"]?.ToString(),
                                Address = reader["Address"]?.ToString(),
                                RoleID = Convert.ToInt32(reader["RoleID"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                Status = (UserStatus)Convert.ToInt32(reader["Status"]),
                                ImagePath = reader["ImagePath"].ToString()

                            };
                            users.Add(user);
                        }
                    }
                }
            }
            return users;
        }
        public bool AddStaff(User user)
        {
            using (SqlConnection conn = dbConn.GetConnection())
            {
                if (conn == null) return false;

                string rawPassword = RemoveDiacritics(user.Fullname)
                            .Replace(" ", "") // xóa khoảng trắng
                            .ToLower()        // tùy chọn: viết thường toàn bộ
                            + "123";
                string passwordHash = PasswordHasher.HashPassword(rawPassword);

                string query = @"INSERT INTO Users 
                    (Username, PasswordHash, FullName, Email, Phone, Address, RoleID, CreatedAt, Status,ImagePath)
                    VALUES 
                    (@Username, @PasswordHash, @FullName, @Email, @Phone, @Address, @RoleID, @CreatedAt, @Status,@ImagePath)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", user.Fullname);
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    cmd.Parameters.AddWithValue("@FullName", user.Fullname);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Phone", user.Phone);
                    cmd.Parameters.AddWithValue("@Address", user.Address);
                    cmd.Parameters.AddWithValue("@RoleID", 2);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Status", (int)UserStatus.Active);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        public List<User> SearchUsers(string keywordOrId)
        {
            List<User> users = new List<User>();
            using (SqlConnection conn = dbConn.GetConnection())
            {
                if (conn == null) return users;

                // Xác định có phải là UserID không
                bool isUserIdSearch = int.TryParse(keywordOrId, out int userId);

                string query;
                SqlCommand cmd;

                if (isUserIdSearch)
                {
                    query = "SELECT * FROM Users WHERE UserID = @UserID";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                }
                else
                {
                    query = @"
                SELECT * FROM Users 
                WHERE 
                    UserID LIKE @Keyword OR 
                    Username LIKE @Keyword OR 
                    FullName LIKE @Keyword OR 
                    Email LIKE @Keyword OR 
                    Phone LIKE @Keyword OR 
                    Address LIKE @Keyword";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keywordOrId + "%");
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User user = new User
                        {
                            UserID = Convert.ToInt32(reader["UserID"]),
                            Username = reader["Username"].ToString(),
                            Fullname = reader["FullName"]?.ToString(),
                            Email = reader["Email"]?.ToString(),
                            Phone = reader["Phone"]?.ToString(),
                            Address = reader["Address"]?.ToString(),
                            RoleID = Convert.ToInt32(reader["RoleID"]),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                            Status = (UserStatus)Convert.ToInt32(reader["Status"]),
                            ImagePath = reader["ImagePath"].ToString()

                        };
                        users.Add(user);
                    }
                }
            }
            return users;
        }
    }
}
