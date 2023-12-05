using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using StoreManager.Abstract.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StoreManager.Abstract.Classes
{
    public abstract class AdminStoreInteraction : IStore
    {
        private string _connectionString = ConfigurationManager.ConnectionStrings["conString"].ConnectionString;
        private string connectionString
        {
            get { return _connectionString; }
            set { _connectionString = ConfigurationManager.ConnectionStrings["conString"].ConnectionString; }
        }

        public Role myRole { get; set; }
        public bool isOk { get; set; }
        public bool isExist { get; set; }

        protected AdminStoreInteraction(string userName, string password, string contactInfo)
        {
            myRole = Role.admin;
            isExist = CheckUserExistence(userName);
            if (contactInfo.Equals(string.Empty))
            {
                //Try Sign In
                if (isExist)
                {
                    string passwordHash = HashPassword(password);
                    SignInFunction(userName, passwordHash);
                }
            }
            else
            {
                //Try SignUp
                if (!isExist)
                {
                    string passwordHash = HashPassword(password);
                    CallSignUpProcedure(userName, passwordHash, myRole.ToString(), contactInfo);
                }
            }
        }

        void SignInFunction(string userName, string passwordHash)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("SignIn", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add("result", OracleDbType.Boolean, System.Data.ParameterDirection.ReturnValue);
                        command.Parameters.Add("p_UserName", OracleDbType.Varchar2).Value = userName;
                        command.Parameters.Add("p_PasswordHash", OracleDbType.Varchar2).Value = passwordHash;

                        command.ExecuteNonQuery();

                        OracleBoolean oracleBooleanResult = (OracleBoolean)command.Parameters["result"].Value;

                        bool signInResult = oracleBooleanResult.Equals(OracleBoolean.True);

                        if (signInResult)
                        {
                            isOk = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка: " + ex.Message);
                }
            }
        }

        void CallSignUpProcedure(string userName, string passwordHash, string userRole, string contactInfo)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("SignUp", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        // Добавляем параметры
                        command.Parameters.Add("p_UserName", OracleDbType.Varchar2).Value = userName;
                        command.Parameters.Add("p_PasswordHash", OracleDbType.Varchar2).Value = passwordHash;
                        command.Parameters.Add("p_ContactInfo", OracleDbType.Varchar2).Value = contactInfo;
                        command.Parameters.Add("p_UserRole", OracleDbType.Varchar2).Value = userRole;

                        command.ExecuteNonQuery();
                    }
                    isOk = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка: " + ex.Message);
                }
            }
        }

        static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private bool CheckUserExistence(string UserName)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("BEGIN :result := CheckUserExistence(:p_UserName); END;", connection))
                {
                    command.Parameters.Add("result", OracleDbType.Boolean, System.Data.ParameterDirection.ReturnValue);
                    command.Parameters.Add("p_UserName", OracleDbType.Varchar2).Value = UserName;

                    command.ExecuteNonQuery();

                    OracleBoolean oracleBooleanResult = (OracleBoolean)command.Parameters["result"].Value;

                    bool userExists = oracleBooleanResult.Equals(OracleBoolean.True);

                    return userExists;
                }
            }
        }

        public bool UpdateProduct(int productID, string productName, int cost, int CategoryId, byte[] fileData, string fileName, int? descriptionID)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("UpdateProduct", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_ProductID", OracleDbType.Int64).Value = productID;
                        command.Parameters.Add("p_ProductName", OracleDbType.Varchar2).Value = productName;
                        command.Parameters.Add("p_DescriptionID", OracleDbType.Int64).Value = descriptionID;
                        command.Parameters.Add("p_ProductImage", OracleDbType.Blob).Value = fileData;
                        command.Parameters.Add("p_ProductImageFileName", OracleDbType.Varchar2).Value = fileName;
                        command.Parameters.Add("p_Cost", OracleDbType.Int64).Value = cost;
                        command.Parameters.Add("p_CategoryID", OracleDbType.Int64).Value = CategoryId;

                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Помилка: ", ex.Message);
            }
            return false;
        }

        public bool DeleteProduct(int productId)
        {
            bool result = false;

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("BEGIN :result := DeleteProduct(:p_productID); END;", connection))
                {
                    command.Parameters.Add("result", OracleDbType.Boolean, ParameterDirection.Output).Size = 100;
                    command.Parameters.Add("p_productID", OracleDbType.Decimal).Value = productId;

                    command.ExecuteNonQuery();

                    result = ((OracleBoolean)command.Parameters["result"].Value).IsTrue;

                    if (result)
                    {
                        MessageBox.Show("Предмет видалено");
                    }
                    else
                    {
                        MessageBox.Show("Помилка у вдаленні предмета");
                    }
                }
                connection.Close();
            }
            return result;
        }

        public void CreateCategory(string categoryName, string CategoryDescription)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("AddCategory", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.Add("p_CategoryName", OracleDbType.Varchar2).Value = categoryName;
                    command.Parameters.Add("p_CategoryDescription", OracleDbType.Varchar2).Value = CategoryDescription;

                    command.ExecuteNonQuery();
                }
            }
        }

        public void CreateDescription(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            string fileName = fileInfo.Name;
            string fileExtension = fileInfo.Extension;
            string fileType = GetFileType(fileExtension);
            byte[] fileData = File.ReadAllBytes(filePath);

            OracleConnection connection = new OracleConnection(connectionString);
            connection.Open();

            OracleCommand command = new OracleCommand("AddDescription", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("p_FileName", OracleDbType.Varchar2, fileName, ParameterDirection.Input); // Ім'я файлу
            command.Parameters.Add("p_FileType", OracleDbType.Varchar2, fileType, ParameterDirection.Input); // Тип файлу
            command.Parameters.Add("p_FileExtension", OracleDbType.Varchar2, fileExtension, ParameterDirection.Input); // Розширення файлу
            command.Parameters.Add("p_FileData", OracleDbType.Blob, fileData, ParameterDirection.Input); // Дані файлу
            command.Parameters.Add("p_Info", OracleDbType.Varchar2, "Some Info", ParameterDirection.Input); // Інформація


            command.ExecuteNonQuery();

            connection.Close();
        }

        public DataTable GetDataFromView(string viewName)
        {
            DataTable resultTable = new DataTable();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand($"SELECT * FROM {viewName}", connection))
                {
                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        adapter.Fill(resultTable);
                    }
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

        public void CreateProduct(string productName, int cost, int CategoryId, string filePath, int? descriptionID)
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("AddProduct", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("p_ProductName", OracleDbType.Varchar2).Value = productName;
                    command.Parameters.Add("p_DescriptionID", OracleDbType.Int64).Value = descriptionID;
                    command.Parameters.Add("p_ProductImage", OracleDbType.Blob).Value = fileData;
                    command.Parameters.Add("p_ProductImageFileName", OracleDbType.Varchar2).Value = Path.GetFileName(filePath);
                    command.Parameters.Add("p_Cost", OracleDbType.Int64).Value = cost;
                    command.Parameters.Add("p_CategoryID", OracleDbType.Int64).Value = CategoryId;

                    command.ExecuteNonQuery();
                }
            }
        }

        public byte[] GetDecroptionData(int descriptionId)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("BEGIN :result := GetDescritpionDataByID(:p_DescriptionID); END;", connection))
                {
                    command.Parameters.Add("result", OracleDbType.Blob, ParameterDirection.ReturnValue);
                    command.Parameters.Add("p_DescriptionID", OracleDbType.Decimal).Value = descriptionId;

                    command.ExecuteNonQuery();

                    OracleBlob blob = (OracleBlob)command.Parameters["result"].Value;

                    if (blob != null && blob.Length > 0)
                    {
                        byte[] fileData = new byte[blob.Length];
                        blob.Read(fileData, 0, Convert.ToInt32(blob.Length));
                        return fileData;
                    }
                    else
                    {
                        MessageBox.Show("Помилка. Дані не знайдено!");
                    }
                }
            }
            return null;
        }

        public byte[] GetProductImageData(int productId)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("BEGIN :result := GetProductImageByID(:p_ProductID); END;", connection))
                {
                    command.Parameters.Add("result", OracleDbType.Blob, ParameterDirection.ReturnValue);
                    command.Parameters.Add("p_ProductID", OracleDbType.Decimal).Value = productId;

                    command.ExecuteNonQuery();

                    OracleBlob blob = (OracleBlob)command.Parameters["result"].Value;

                    if (blob != null && blob.Length > 0)
                    {
                        byte[] fileData = new byte[blob.Length];
                        blob.Read(fileData, 0, Convert.ToInt32(blob.Length));
                        return fileData;
                    }
                    else
                    {
                        MessageBox.Show("Помилка. Дані не знайдено!");
                    }
                }
            }
            return null;
        }

        public void CreateSupplier(string companyName, string contactInfo, string supplierAddress)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("AddSupplier", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("p_CompanyName", OracleDbType.Varchar2).Value = companyName;
                    command.Parameters.Add("p_ContactInfo", OracleDbType.Varchar2).Value = contactInfo;
                    command.Parameters.Add("p_SupplierAddress", OracleDbType.Varchar2).Value = supplierAddress;

                    command.ExecuteNonQuery();
                }
            }
        }

        public int? HasShipment(int orderID)
        {
            OracleConnection connection = new OracleConnection(connectionString);
            connection.Open();

            OracleCommand command = new OracleCommand("SELECT HasShipment(:p_OrderID) FROM DUAL", connection);
            command.CommandType = CommandType.Text;
            command.Parameters.Add("p_OrderID", OracleDbType.Int32, orderID, ParameterDirection.Input);

            object result = command.ExecuteScalar();
            int? shipmentID = null;
            if (result != DBNull.Value)
            {
                shipmentID = Convert.ToInt32(result);
            }

            connection.Close();

            return shipmentID;
        }

        public void CreateShipments(string shipmentsStatuc, int OrderID)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("AddShipment", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("p_ShipmentStatus", OracleDbType.Varchar2).Value = shipmentsStatuc;
                    command.Parameters.Add("p_OrderID", OracleDbType.Int64).Value = OrderID;

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateShipments(string shipmentsStatuc, int shipmentsID)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("UpdateShipmentStatus", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("p_ShipmentID", OracleDbType.Int64).Value = shipmentsID;
                    command.Parameters.Add("p_ShipmentStatus", OracleDbType.Varchar2).Value = shipmentsStatuc;

                    command.ExecuteNonQuery();
                }
            }
        }

        public void CreateWarehouses(string warehouseName, string Location, int Capacity, int Availability)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("AddWarehouse", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("p_WarehouseName", OracleDbType.Varchar2).Value = warehouseName;
                    command.Parameters.Add("p_Location", OracleDbType.Varchar2).Value = Location;
                    command.Parameters.Add("p_Capacity", OracleDbType.Int64).Value = Capacity;
                    command.Parameters.Add("p_Availability", OracleDbType.Int64).Value = Availability;

                    command.ExecuteNonQuery();
                }
            }
        }

        public void CreateInventory(int productID, int QuantityOnHand, int WarehouseId)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("AddInventory", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("p_ProductID", OracleDbType.Int64).Value = productID;
                    command.Parameters.Add("p_QuantityOnHand", OracleDbType.Int64).Value = QuantityOnHand;
                    command.Parameters.Add("p_WarehouseID", OracleDbType.Int64).Value = WarehouseId;

                    command.ExecuteNonQuery();
                }
            }
        }

        public void SaveFileToDisk(string fileName, byte[] fileData)
        {
            File.WriteAllBytes(fileName, fileData);
        }

        public void OpenFileWithDefaultApplication(string fileName)
        {
            System.Diagnostics.Process.Start(fileName);
        }
    }
}
