using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N4_BTCM.Controller
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public int? SupplierID { get; set; }
        public DateTime ImportDate { get; set; }
        public string ImagePath { get; set; }
    }

    public class Suppliers
    {
        public int ID { get; set; }
        public string TenNCC { get; set; }
        public string DiaChi { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
    }

    internal class QuanLySP
    {
        private DBConnection dbConn = new DBConnection();

        public List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();
            using (SqlConnection conn = dbConn.GetConnection())
            {
                if (conn == null) return products; // If connection fails, return empty list

                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Products", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product
                            {
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                                Unit = reader["Unit"].ToString(),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                SupplierID = reader["SupplierID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["SupplierID"]),
                                ImportDate = Convert.ToDateTime(reader["ImportDate"]),
                                ImagePath = reader["ImagePath"].ToString()
                            };
                            products.Add(product);
                        }
                    }
                }
            }
            return products;
        }

        public List<Product> SearchProducts(string keyword)
        {
            List<Product> products = new List<Product>();
            using (SqlConnection conn = dbConn.GetConnection())
            {
                if (conn == null) return products;

                string query = @"
            SELECT * FROM Products 
            WHERE 
                ProductID LIKE @Keyword OR 
                Name LIKE @Keyword OR 
                Description LIKE @Keyword OR 
                Unit LIKE @Keyword OR 
                CAST(UnitPrice AS NVARCHAR) LIKE @Keyword OR 
                CAST(Quantity AS NVARCHAR) LIKE @Keyword";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product
                            {
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                                Unit = reader["Unit"].ToString(),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                SupplierID = reader["SupplierID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["SupplierID"]),
                                ImportDate = Convert.ToDateTime(reader["ImportDate"]),
                                ImagePath = reader["ImagePath"].ToString()
                            };
                            products.Add(product);
                        }
                    }
                }
            }
            return products;
        }


        // Thêm thuốc mới
        public bool AddProduct(Product product)
        {
            using (SqlConnection conn = dbConn.GetConnection())
            {
                if (conn == null) return false;

                using (SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO Products (Name, Description, UnitPrice, Unit, Quantity, SupplierID, ImportDate, ImagePath)
                    VALUES (@Name, @Description, @UnitPrice, @Unit, @Quantity, @SupplierID, @ImportDate, @ImagePath);
                    ", conn))
                {
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Description", product.Description ?? (object)DBNull.Value); // Handle null Description
                    cmd.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);
                    cmd.Parameters.AddWithValue("@Unit", product.Unit);
                    cmd.Parameters.AddWithValue("@Quantity", product.Quantity);
                    cmd.Parameters.AddWithValue("@SupplierID", product.SupplierID ?? (object)DBNull.Value); // Handle null SupplierID
                    cmd.Parameters.AddWithValue("@ImportDate", product.ImportDate);
                    cmd.Parameters.AddWithValue("@ImagePath", product.ImagePath ?? (object)DBNull.Value);  // Handle null ImagePath

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool UpdateProduct(Product product)
        {
            using (SqlConnection conn = dbConn.GetConnection())
            {
                if (conn == null) return false;

                using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE Products 
                    SET Name = @Name, Description = @Description, UnitPrice = @UnitPrice, 
                        Unit = @Unit, Quantity = @Quantity, SupplierID = @SupplierID, 
                        ImportDate = @ImportDate, ImagePath = @ImagePath
                    WHERE ProductID = @ProductID", conn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", product.ProductID);
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Description", product.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);
                    cmd.Parameters.AddWithValue("@Unit", product.Unit);
                    cmd.Parameters.AddWithValue("@Quantity", product.Quantity);
                    cmd.Parameters.AddWithValue("@SupplierID", product.SupplierID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImportDate", product.ImportDate);
                    cmd.Parameters.AddWithValue("@ImagePath", product.ImagePath ?? (object)DBNull.Value);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool DeleteProduct(int productId)
        {
            using (SqlConnection conn = dbConn.GetConnection())
            {
                if (conn == null) return false;

                using (SqlCommand cmd = new SqlCommand("UPDATE Products SET Quantity = 0 WHERE ProductID = @ProductID", conn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public List<Suppliers> GetAllSuppliers()
        {
            List<Suppliers> supplies = new List<Suppliers>();
            using (SqlConnection conn = dbConn.GetConnection())
            {
                if (conn == null) return supplies;

                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Suppliers", conn)) // Sử dụng bảng Suppliers
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Suppliers ncc = new Suppliers
                            {
                                ID = Convert.ToInt32(reader["SupplierID"]), // Sử dụng SupplierID
                                TenNCC = reader["Name"].ToString(), // Sử dụng Name
                                DiaChi = reader["Address"].ToString(), // Sử dụng Address
                                SDT = reader["Phone"].ToString(),   // Sử dụng Phone
                                Email = reader["Email"].ToString(),
                                ImagePath = reader["ImagePath"].ToString()
                            };
                            supplies.Add(ncc);
                        }
                    }
                }
            }
            return supplies;
        }

        public bool AddNcc(Suppliers supplier)
        {
            using (SqlConnection conn = dbConn.GetConnection())
            {
                if (conn == null) return false;

                using (SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO Suppliers (Name, Address, Phone, Email, ImagePath)
                    VALUES (@SupplierID, @Name, @Address, @Phone, @Email,@ImagePath);
                    ", conn))
                {
                    cmd.Parameters.AddWithValue("@Name", supplier.TenNCC);
                    cmd.Parameters.AddWithValue("@Address", supplier.DiaChi);
                    cmd.Parameters.AddWithValue("@Phone", supplier.SDT);
                    cmd.Parameters.AddWithValue("@Email", supplier.Email);
                    cmd.Parameters.AddWithValue("@ImagePath", supplier.ImagePath ?? (object)DBNull.Value);  // Handle null ImagePath

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        // Cập nhật thông tin loại thuốc
        public bool UpdateNcc(Suppliers supplies)
        {
            using (SqlConnection conn = dbConn.GetConnection())
            {
                if (conn == null) return false;

                using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE Suppliers 
                    SET Name = @Name, Address = @Address, Phone = @Phone, Email = @Email,ImagePath = @ImagePath
                    WHERE SupplierID = @SupplierID", conn))
                {
                    cmd.Parameters.AddWithValue("@SupplierID", supplies.ID);
                    cmd.Parameters.AddWithValue("@Name", supplies.TenNCC);
                    cmd.Parameters.AddWithValue("@Address", supplies.DiaChi);
                    cmd.Parameters.AddWithValue("@Phone", supplies.SDT);
                    cmd.Parameters.AddWithValue("@Email", supplies.Email);
                    cmd.Parameters.AddWithValue("@ImagePath", supplies.ImagePath ?? (object)DBNull.Value);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
        public List<Suppliers> SearchSuppliers(string keyword)
        {
            List<Suppliers> result = new List<Suppliers>();
            using (SqlConnection conn = dbConn.GetConnection())
            {
                if (conn == null) return result;

                string query = @"
            SELECT * FROM Suppliers 
            WHERE 
                SupplierID LIKE @Keyword OR 
                Name LIKE @Keyword OR 
                Address LIKE @Keyword OR 
                Phone LIKE @Keyword OR 
                Email LIKE @Keyword";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Suppliers supplier = new Suppliers
                            {
                                ID = Convert.ToInt32(reader["SupplierID"]),
                                TenNCC = reader["Name"].ToString(),
                                DiaChi = reader["Address"].ToString(),
                                SDT = reader["Phone"].ToString(),
                                Email = reader["Email"].ToString(),
                                ImagePath = reader["ImagePath"].ToString()
                            };
                            result.Add(supplier);
                        }
                    }
                }
            }
            return result;
        }

    }
}
