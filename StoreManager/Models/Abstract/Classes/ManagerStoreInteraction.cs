using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using StoreManager.DB_classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StoreManager.Models.Abstract.Classes
{
    public abstract class ManagerStoreInteraction : AllUsersInteractions
    {
        protected ManagerStoreInteraction(User user, bool isSignIn) : base(user, isSignIn)
        { }
        public bool UpdateProduct(Product product)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("UpdateProduct", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_ProductID", OracleDbType.Int64).Value = product.ProductID;
                        command.Parameters.Add("p_ProductName", OracleDbType.Varchar2).Value = product.ProductName;
                        command.Parameters.Add("p_DescriptionID", OracleDbType.Int64).Value = product.Description?.DescriptionID;
                        command.Parameters.Add("p_ContentID", OracleDbType.Int64).Value = product.BinaryContent.ContentID;
                        command.Parameters.Add("p_Cost", OracleDbType.Int64).Value = product.Price;
                        command.Parameters.Add("p_CategoryID", OracleDbType.Int64).Value = product.Category.CategoryID;

                        command.ExecuteNonQuery();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
            return false;
        }
        public void CreateCategory(Category category)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("AddCategory", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add("p_CategoryName", OracleDbType.Varchar2).Value = category.CategoryName;
                        command.Parameters.Add("p_CategoryDescription", OracleDbType.Varchar2).Value = category.CategoryDescription;
                        command.Parameters.Add("p_ParentCategoryID", OracleDbType.Int64).Value = category.ParentCategoryID;

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
        }
        public void CreateDescription(Description description)
        {
            OracleConnection connection = new OracleConnection(connectionString);
            connection.Open();
            try
            {
                OracleCommand command = new OracleCommand("AddDescription", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("p_FileName", OracleDbType.Varchar2, description.FileName, ParameterDirection.Input); // Ім'я файлу
                command.Parameters.Add("p_FileType", OracleDbType.Varchar2, description.FileType, ParameterDirection.Input); // Тип файлу
                command.Parameters.Add("p_FileExtension", OracleDbType.Varchar2, description.FileExtension, ParameterDirection.Input); // Розширення файлу
                command.Parameters.Add("p_FileData", OracleDbType.Blob, description.FileData, ParameterDirection.Input); // Дані файлу
                command.Parameters.Add("p_Info", OracleDbType.Varchar2, description.Info, ParameterDirection.Input); // Інформація


                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            connection.Close();
        }
        public bool UpdateCategory(Category category)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("UpdateCategory", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("UpdateCategory", OracleDbType.Int64).Value = category.CategoryID;
                        command.Parameters.Add("p_CategoryName", OracleDbType.Varchar2).Value = category.CategoryName;
                        command.Parameters.Add("p_CategoryDescription", OracleDbType.Varchar2).Value = category.CategoryDescription;
                        command.Parameters.Add("p_ParentCategoryID", OracleDbType.Int64).Value = category.ParentCategoryID;

                        command.ExecuteNonQuery();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
            return false;
        }
        public bool UpdateDescription(Description description)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("UpdateDescription", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_DescriptionID", OracleDbType.Int64).Value = description.DescriptionID;
                        command.Parameters.Add("p_FileName", OracleDbType.Varchar2).Value = description.FileName;
                        command.Parameters.Add("p_FileType", OracleDbType.Varchar2).Value = description.FileType;
                        command.Parameters.Add("p_FileExtension", OracleDbType.Varchar2).Value = description.FileExtension;
                        command.Parameters.Add("p_Info", OracleDbType.Varchar2).Value = description.Info;

                        command.ExecuteNonQuery();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
            return false;
        }
        public bool DeleteCategory(int categoryID)
        {
            bool isOkDeleted = false;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("DeleteCategory", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_CategoryID", OracleDbType.Int64).Value = categoryID;
                        OracleParameter isDeleted = new OracleParameter("p_IsDeleted", OracleDbType.Boolean);
                        isDeleted.Direction = ParameterDirection.Output;
                        command.Parameters.Add(isDeleted);

                        command.ExecuteNonQuery();

                        isOkDeleted = ((OracleBoolean)isDeleted.Value).IsTrue;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
            return isOkDeleted;
        }
        public bool DeleteDescription(int descriptionID)
        {
            bool isOkDeleted = false;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("DeleteDescription", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_DescriptionID", OracleDbType.Int64).Value = descriptionID;
                        OracleParameter isDeleted = new OracleParameter("p_IsDeleted", OracleDbType.Boolean);
                        isDeleted.Direction = ParameterDirection.Output;
                        command.Parameters.Add(isDeleted);

                        command.ExecuteNonQuery();

                        isOkDeleted = ((OracleBoolean)isDeleted.Value).IsTrue;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
            return isOkDeleted;
        }
        public bool DeleteInventory(int inventoryID)
        {
            bool isOkDeleted = false;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("DeleteInventory", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_InventoryID", OracleDbType.Int64).Value = inventoryID;
                        OracleParameter isDeleted = new OracleParameter("p_IsDeleted", OracleDbType.Boolean);
                        isDeleted.Direction = ParameterDirection.Output;
                        command.Parameters.Add(isDeleted);

                        command.ExecuteNonQuery();

                        isOkDeleted = ((OracleBoolean)isDeleted.Value).IsTrue;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
            return isOkDeleted;
        }
        public bool DeleteWarehouses(int warehouseID)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("DeleteInventoryAndWarehouseID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_WarehouseID", OracleDbType.Int64).Value = warehouseID;

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    return false;
                }

                connection.Close();
            }
            return true;
        }
        public DataTable GetCategoryHierarchy()
        {
            DataTable resultTable = new DataTable();
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                try
                {
                    string sqlQuery = @"
                SELECT 
                    CONNECT_BY_ROOT CategoryID AS RootCategoryID,
                    CategoryID,
                    CategoryName,
                    CONNECT_BY_ISLEAF AS IsLeaf,
                    SYS_CONNECT_BY_PATH(CategoryName, '/') AS CategoryPath
                FROM 
                    Categories
                START WITH 
                    ParentCategoryID IS NULL
                CONNECT BY 
                    PRIOR CategoryID = ParentCategoryID
                ORDER SIBLINGS BY 
                    CategoryID";

                    using (OracleDataAdapter adapter = new OracleDataAdapter(sqlQuery, connection))
                    {
                        adapter.Fill(resultTable);
                    }
                    resultTable.Columns.Remove("CategoryID");
                    resultTable.Columns.Remove("RootCategoryID");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            return resultTable;
        }
        public static string GetFileType(string fileExtension)
        {
            var fileTypes = new Dictionary<string, string>()
            {
                {".txt", "Text"},
                {".doc", "Word"},
                {".docx", "Word"},
                {".xls", "Excel"},
                {".xlsx", "Excel"},
                {".ppt", "PowerPoint"},
                {".pptx", "PowerPoint"},
                {".pdf", "PDF"},
                {".jpg", "Image"},
                {".jpeg", "Image"},
                {".png", "Image"},
                {".gif", "Image"},
                {".mp3", "Audio"},
                {".wav", "Audio"},
                {".mp4", "Video"},
                {".avi", "Video"},
                {".zip", "Archive"},
                {".rar", "Archive"},
                {".7z", "Archive"}
            };

            if (fileTypes.ContainsKey(fileExtension))
            {
                return fileTypes[fileExtension];
            }
            else
            {
                return "Unknown";
            }
        }

        public void CreateProduct(Product product)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                int? binaryContentID = null;
                if (product.BinaryContent != null)
                {
                    product.BinaryContent = CreateBinaryContent(product.BinaryContent, connection);
                    binaryContentID = product.BinaryContent.ContentID;
                }
                try
                {
                    using (OracleCommand command = new OracleCommand("AddProduct", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_ProductName", OracleDbType.Varchar2).Value = product.ProductName;
                        command.Parameters.Add("p_DescriptionID", OracleDbType.Int64).Value = product.Description?.DescriptionID;
                        command.Parameters.Add("p_ContentID", OracleDbType.Int64).Value = binaryContentID;
                        command.Parameters.Add("p_Cost", OracleDbType.Int64).Value = product.Price;
                        command.Parameters.Add("p_CategoryID", OracleDbType.Int64).Value = product.Category.CategoryID;

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
        }

        public void CreateSupplier(string companyName, string contactInfo, string supplierAddress)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("AddSupplier", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_CompanyName", OracleDbType.Varchar2).Value = companyName;
                        command.Parameters.Add("p_ContactInfo", OracleDbType.Varchar2).Value = contactInfo;
                        command.Parameters.Add("p_SupplierAddress", OracleDbType.Varchar2).Value = supplierAddress;

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
        }
        public bool UpdateSupplier(Supplier supplier)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("UpdateSupplier", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_SupplierID", OracleDbType.Int32).Value = supplier.SupplierID;
                        command.Parameters.Add("p_CompanyName", OracleDbType.Varchar2).Value = supplier.CompanyName;
                        command.Parameters.Add("p_ContactInfo", OracleDbType.Varchar2).Value = supplier.ContactInfo;
                        command.Parameters.Add("p_SupplierAddress", OracleDbType.Varchar2).Value = supplier.SupplierAddress;

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    return false;
                }

                connection.Close();
            }
            return true;
        }

        public int? HasShipment(int orderID)
        {
            OracleConnection connection = new OracleConnection(connectionString);
            connection.Open();
            int? shipmentID = null;

            try
            {
                OracleCommand command = new OracleCommand("SELECT HasShipment(:p_OrderID) FROM DUAL", connection);
                command.CommandType = CommandType.Text;
                command.Parameters.Add("p_OrderID", OracleDbType.Int32, orderID, ParameterDirection.Input);

                object result = command.ExecuteScalar();
                if (result != DBNull.Value)
                {
                    shipmentID = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            connection.Close();

            return shipmentID;
        }

        public void CreateShipments(string shipmentsStatuc, int OrderID)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("AddShipment", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_ShipmentStatus", OracleDbType.Varchar2).Value = shipmentsStatuc;
                        command.Parameters.Add("p_OrderID", OracleDbType.Int64).Value = OrderID;

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
        }

        public void UpdateShipments(string shipmentsStatuc, int shipmentsID)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("UpdateShipmentStatus", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_ShipmentID", OracleDbType.Int64).Value = shipmentsID;
                        command.Parameters.Add("p_ShipmentStatus", OracleDbType.Varchar2).Value = shipmentsStatuc;

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
        }

        public void CreateWarehouses(Warehouse warehouse)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("AddWarehouse", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_WarehouseName", OracleDbType.Varchar2).Value = warehouse.WarehoseName;
                        command.Parameters.Add("p_Location", OracleDbType.Varchar2).Value = warehouse.Location;
                        command.Parameters.Add("p_Capacity", OracleDbType.Int64).Value = warehouse.Capacity;
                        command.Parameters.Add("p_Availability", OracleDbType.Int64).Value = warehouse.Availability;

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
        }
        public bool UpdateWareHouse(Warehouse warehouse)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("UpdateWarehouse", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_InventoryID", OracleDbType.Int64).Value = warehouse.WarehoseID;
                        command.Parameters.Add("p_WarehouseName", OracleDbType.Varchar2).Value = warehouse.WarehoseName;
                        command.Parameters.Add("p_Location", OracleDbType.Varchar2).Value = warehouse.Location;
                        command.Parameters.Add("p_Capacity", OracleDbType.Int64).Value = warehouse.Capacity;
                        command.Parameters.Add("p_Availability", OracleDbType.Int64).Value = warehouse.Availability;

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    return false;
                }

                connection.Close();
            }
            return true;
        }

        public void CreateInventory(Inventory inventory)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("AddInventory", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_ProductID", OracleDbType.Int64).Value = inventory.ProductID;
                        command.Parameters.Add("p_QuantityOnHand", OracleDbType.Int64).Value = inventory.QuantityOnHand;
                        command.Parameters.Add("p_WarehouseID", OracleDbType.Int64).Value = inventory.WareHouseID;

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
        }
        public bool UpdateInventory(Inventory inventory)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("UpdateInventory", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_InventoryID", OracleDbType.Int64).Value = inventory.InventoryID;
                        command.Parameters.Add("p_ProductID", OracleDbType.Int64).Value = inventory.ProductID;
                        command.Parameters.Add("p_QuantityOnHand", OracleDbType.Int64).Value = inventory.QuantityOnHand;
                        command.Parameters.Add("p_WarehouseID", OracleDbType.Int64).Value = inventory.WareHouseID;

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    return false;
                }

                connection.Close();
            }
            return true;
        }
        private string GetUsernameFromConnectionString()
        {
            OracleConnectionStringBuilder builder = new OracleConnectionStringBuilder(connectionString);

            string username = builder.ContainsKey("UserID")
                ? builder["UserID"] as string
                : builder.ContainsKey("User ID")
                    ? builder["User ID"] as string
                    : null;


            return username;
        }
        public DataTable GetDatabaseObjects(string objectTableName, string objectTypeColumnName, string objectType)
        {
            DataTable resultTable = new DataTable();
            string owner = GetUsernameFromConnectionString();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleCommand command = connection.CreateCommand())
                {
                    if (objectType.Equals("TABLE", StringComparison.OrdinalIgnoreCase))
                    {
                        command.CommandText = $"SELECT TABLE_NAME FROM ALL_TABLES WHERE OWNER = :owner";
                    }
                    else if (objectType.Equals("PROCEDURE", StringComparison.OrdinalIgnoreCase) || objectType.Equals("FUNCTION", StringComparison.OrdinalIgnoreCase))
                    {
                        command.CommandText = $"SELECT {objectTypeColumnName} FROM ALL_PROCEDURES WHERE OBJECT_TYPE = :objectType AND OWNER = :owner";
                        command.Parameters.Add("objectType", OracleDbType.Varchar2).Value = objectType;
                    }
                    else
                    {
                        command.CommandText = $"SELECT {objectTypeColumnName} FROM {objectTableName} WHERE OWNER = :owner";
                    }

                    command.Parameters.Add("owner", OracleDbType.Varchar2).Value = owner;

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        resultTable.Load(reader);
                    }
                }
            }

            return resultTable;
        }

    }
}
