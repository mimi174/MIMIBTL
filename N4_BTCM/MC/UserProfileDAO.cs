using System;
using System.Data.SqlClient;
using N4_BITCM;

namespace N4_BTCM
{
    public class UserProfileDAO
    {
        private readonly DBConnection db = new DBConnection();

        /// <summary>
        /// Lấy thông tin hồ sơ người dùng theo ID.
        /// </summary>
        public UserProfile GetUserProfileById(int userId)
        {
            UserProfile user = null;

            using (SqlConnection conn = db.GetConnection())
            {

                string query = "SELECT * FROM UserProfiles WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", userId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new UserProfile
                        {
                            Id = (int)reader["Id"],
                            HoTen = reader["HoTen"].ToString(),
                            NgaySinh = reader["NgaySinh"] != DBNull.Value ? Convert.ToDateTime(reader["NgaySinh"]) : DateTime.MinValue,
                            GioiTinh = reader["GioiTinh"].ToString(),
                            SoDienThoai = reader["SoDienThoai"].ToString(),
                            Email = reader["Email"].ToString(),
                            DiaChi = reader["DiaChi"].ToString(),
                            Avatar = reader["Avatar"] != DBNull.Value ? (byte[])reader["Avatar"] : null
                        };
                    }
                }
            }

            return user;
        }

        /// <summary>
        /// Cập nhật thông tin hồ sơ người dùng.
        /// </summary>
        public bool UpdateUserProfile(UserProfile profile)
        {
            using (SqlConnection conn = db.GetConnection())
            {
                string query = @"UPDATE UserProfiles SET 
                                    HoTen = @HoTen,
                                    NgaySinh = @NgaySinh,
                                    GioiTinh = @GioiTinh,
                                    SoDienThoai = @SoDienThoai,
                                    Email = @Email,
                                    DiaChi = @DiaChi,
                                    Avatar = @Avatar
                                WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@HoTen", profile.HoTen);
                cmd.Parameters.AddWithValue("@NgaySinh", profile.NgaySinh);
                cmd.Parameters.AddWithValue("@GioiTinh", profile.GioiTinh);
                cmd.Parameters.AddWithValue("@SoDienThoai", profile.SoDienThoai);
                cmd.Parameters.AddWithValue("@Email", profile.Email);
                cmd.Parameters.AddWithValue("@DiaChi", profile.DiaChi);
                cmd.Parameters.AddWithValue("@Avatar", (object)profile.Avatar ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Id", profile.Id);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
