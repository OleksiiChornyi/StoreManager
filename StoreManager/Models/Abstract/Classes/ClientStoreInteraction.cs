using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using StoreManager.Models.Abstract.Interfaces;
using StoreManager.Client;
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
using static StoreManager.Models.Abstract.Classes.StoreCartInteraction;
using StoreManager.ViewModels.Data;

namespace StoreManager.Models.Abstract.Classes
{
    public abstract class ClientStoreInteraction : IStore
    {
        private static string _connectionString = ConfigurationManager.ConnectionStrings["conString"].ConnectionString;
        private static string connectionString {
            get { return _connectionString; }
            set { _connectionString = ConfigurationManager.ConnectionStrings["conString"].ConnectionString; } 
        }

        public Role myRole { get; set; }
        public bool isOk { get; set; }
        public bool isExist { get; set; }
        private int userId { get; set; }

        protected ClientStoreInteraction(string userName, string password, string contactInfo, Role userRole)
        {
            myRole = userRole;
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
            if (CheckUserExistence(userName))
            {
                userId = GetUserIdByUserName(userName);
            }
        }

        public static void CreateGuestIfNotExist()
        {
            string guestName = "Guest";
            if (!CheckUserExistence(guestName))
            {
                string password = "1111";
                Role userRole = Role.guest;
                string contactInfo = "Guest";
                string passwordHash = HashPassword(password);
                CreateGuestUser(guestName, passwordHash, userRole.ToString(), contactInfo);
            }
        }

        public static Role? GetUserRole(string userName)
        {

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                try
                {
                    using (OracleCommand command = new OracleCommand("SELECT GetUserRole(:p_UserName) FROM DUAL", connection))
                    {
                        command.Parameters.Add("p_UserName", OracleDbType.Varchar2).Value = userName;

                        string userRole = command.ExecuteScalar().ToString();

                        if (!string.IsNullOrEmpty(userRole))
                        {

                            Role roleEnum;
                            if (Enum.TryParse(userRole, true, out roleEnum))
                            {
                                return roleEnum;
                            }
                            else
                            {
                                MessageBox.Show("Error getting a user role");
                                return null;
                            }
                        }
                        else
                        {
                            MessageBox.Show($"User {userName} not found.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
            return null;
        }
        public List<CategoryItem> CreateCategoryHierarchy()
        {
            List<CategoryItem> result = new List<CategoryItem>();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT CategoryID, ParentCategoryID FROM CategoryHierarchyView";

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CategoryItem categoryItem = new CategoryItem
                            {
                                CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                                ParentCategoryID = reader.IsDBNull(reader.GetOrdinal("ParentCategoryID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ParentCategoryID"))
                            };

                            result.Add(categoryItem);
                        }
                    }
                }
            }

            return result;

        }
        private static void CreateGuestUser(string userName, string passwordHash, string userRole, string contactInfo)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("SignUp", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_UserName", OracleDbType.Varchar2).Value = userName;
                        command.Parameters.Add("p_PasswordHash", OracleDbType.Varchar2).Value = passwordHash;
                        command.Parameters.Add("p_ContactInfo", OracleDbType.Varchar2).Value = contactInfo;
                        command.Parameters.Add("p_UserRole", OracleDbType.Varchar2).Value = userRole;

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

        void SignInFunction(string userName, string passwordHash)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
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
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
        }

        void CallSignUpProcedure(string userName, string passwordHash, string userRole, string contactInfo)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("SignUp", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

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
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
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

        private static bool CheckUserExistence(string UserName)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
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
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
            return true;
        }

        private int GetUserIdByUserName(string userName)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("SELECT GetUserIDByUsername(:p_UserName) FROM DUAL", connection))
                    {
                        command.Parameters.Add("p_UserName", OracleDbType.Varchar2).Value = userName;

                        string userIdString = command.ExecuteScalar().ToString();

                        if (!string.IsNullOrEmpty(userIdString))
                        {
                            return int.Parse(userIdString);
                        }
                        else
                        {
                            MessageBox.Show($"User {userName} was not found.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
            return -1;
        }

        public DataTable GetDataFromView(string viewName)
        {
            DataTable resultTable = new DataTable();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand($"SELECT * FROM {viewName}", connection))
                    {
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(resultTable);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
            return resultTable;
        }

        public string GetDescriptionName(int productId)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("BEGIN :result := GetDescriptionName(:p_productID); END;", connection))
                    {
                        command.Parameters.Add("result", OracleDbType.Varchar2, ParameterDirection.Output).Size = 100;
                        command.Parameters.Add("p_productID", OracleDbType.Decimal).Value = productId;

                        command.ExecuteNonQuery();

                        string result = command.Parameters["result"].Value.ToString();

                        if (!string.IsNullOrEmpty(result))
                        {
                            return result;
                        }
                        else
                        {
                            MessageBox.Show("Error. Data not found!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }

            return null;
        }

        public byte[] GetDecroptionData(int descriptionId)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
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
                            MessageBox.Show("Error. Data not found!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
            return null;
        }

        public byte[] GetProductImageData(int productId)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
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
                            MessageBox.Show("Error. Data not found!ы");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
            return null;
        }

        public void SaveFileToDisk(string fileName, byte[] fileData)
        {
            File.WriteAllBytes(fileName, fileData);
        }

        public void OpenFileWithDefaultApplication(string fileName)
        {
            System.Diagnostics.Process.Start(fileName);
        }

        public int GetNewRandomOrderNumber()
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "BEGIN :result := GetUniqueRandomOrderNumber; END;";
                        command.CommandType = CommandType.Text;

                        command.Parameters.Add("result", OracleDbType.Decimal).Direction = ParameterDirection.ReturnValue;

                        command.ExecuteNonQuery();

                        int result = ((OracleDecimal)command.Parameters["result"].Value).ToInt32();

                        return result;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
            return -1;
        }

        private async void CreateOrderItems(List<OrderItem> orderItems, OracleConnection connection)
        {
            foreach(var item in orderItems)
            {
                OracleCommand command = new OracleCommand("AddOrderItem", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("p_OrderNumber", OracleDbType.Int64, item.orderNumber, ParameterDirection.Input);
                command.Parameters.Add("p_ProductID", OracleDbType.Varchar2, item.productId, ParameterDirection.Input);
                command.Parameters.Add("p_Quantity", OracleDbType.Varchar2, item.quantity, ParameterDirection.Input);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        private int CreateNewCashID(OracleConnection connection)
        {
            int ID = -1;
            try
            {
                using (OracleCommand command = new OracleCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "BEGIN :result := GetNextCashID; END;";
                    command.CommandType = CommandType.Text;

                    command.Parameters.Add("result", OracleDbType.Decimal).Direction = ParameterDirection.ReturnValue;

                    command.ExecuteNonQuery();

                    ID = ((OracleDecimal)command.Parameters["result"].Value).ToInt32();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            return ID;
        }

        private int CreateNewBankCardID(OracleConnection connection)
        {
            int ID = -1;
            try
            {
                using (OracleCommand command = new OracleCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "BEGIN :result := GetNextBackCardID; END;";
                    command.CommandType = CommandType.Text;

                    command.Parameters.Add("result", OracleDbType.Decimal).Direction = ParameterDirection.ReturnValue;

                    command.ExecuteNonQuery();

                    ID = ((OracleDecimal)command.Parameters["result"].Value).ToInt32();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            return ID;
        }

        private async void CreateBankCard(int bankCardID, int oderNumber, string BankCardNumber, OracleConnection connection)
        {
            OracleCommand command = new OracleCommand("AddBankCard", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("p_OrderNumber", OracleDbType.Int64, oderNumber, ParameterDirection.Input);
            command.Parameters.Add("p_CardNumber", OracleDbType.Varchar2, BankCardNumber, ParameterDirection.Input);
            command.Parameters.Add("p_BankCardID", OracleDbType.Int64, bankCardID, ParameterDirection.Input);

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        private async void CreateCash(int cashID, int oderNumber, OracleConnection connection)
        {
            OracleCommand command = new OracleCommand("AddCash", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("p_OrderNumber", OracleDbType.Int64, oderNumber, ParameterDirection.Input);
            command.Parameters.Add("p_CashID", OracleDbType.Int64, cashID, ParameterDirection.Input);

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        private async void CreatePayment(int orderNumber, int totalPrice, bool isBankCard, string BankCardNumber, OracleConnection connection)
        {
            int? cashId = null;
            int? bankCardID = null;
            if (isBankCard)
            {
                bankCardID = CreateNewBankCardID(connection);
                CreateBankCard((int)bankCardID, orderNumber, BankCardNumber, connection);
            }
            else
            {
                cashId = CreateNewCashID(connection);
                CreateCash((int)cashId, orderNumber, connection);
            }

            OracleCommand command = new OracleCommand("AddPayment", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("p_UserId", OracleDbType.Int64, this.userId, ParameterDirection.Input);
            command.Parameters.Add("p_OrderNumber", OracleDbType.Int64, orderNumber, ParameterDirection.Input);
            command.Parameters.Add("p_TotalPrice", OracleDbType.Int64, totalPrice, ParameterDirection.Input);
            command.Parameters.Add("p_CashID", OracleDbType.Int64, cashId, ParameterDirection.Input);
            command.Parameters.Add("p_BankCardID", OracleDbType.Int64, bankCardID, ParameterDirection.Input);

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        public void CreateOrder(int orderNumber, List<OrderItem> orderItems, int totalPrice, bool isBankCard, string BankCardNumber)
        {
            OracleConnection connection = new OracleConnection(connectionString);
            connection.Open();
            try
            {
                CreatePayment(orderNumber, totalPrice, isBankCard, BankCardNumber, connection);
                CreateOrderItems(orderItems, connection);

                OracleCommand command = new OracleCommand("AddOrder", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("p_OrderNumber", OracleDbType.Decimal, orderNumber, ParameterDirection.Input);
                command.Parameters.Add("p_UserID", OracleDbType.Decimal, this.userId, ParameterDirection.Input);

                command.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            connection.Close();
        }
    }
}
