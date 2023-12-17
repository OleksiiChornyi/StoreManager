using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using StoreManager.DB_classes;
using StoreManager.Models.Abstract.Interfaces;
using StoreManager.Models.Data;
using StoreManager.Models.SQL_static;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StoreManager.Models.Abstract.Classes
{
    public abstract class AllUsersInteractions : IStore
    {
        protected string _connectionString = ConfigurationManager.ConnectionStrings["conString"].ConnectionString;
        protected string connectionString
        {
            get { return _connectionString; }
            set { _connectionString = ConfigurationManager.ConnectionStrings["conString"].ConnectionString; }
        }
        public User user { get; set; }

        protected AllUsersInteractions(User user, bool isSignIn)
        {
            //isOk = false;
            this.user = user;
            if (isSignIn)
            {
                SignInFunction(user);
            }
            else
            {
                CallSignUpProcedure(user);
            }
        }

        protected void SignInFunction(User user)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("SignIn", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_UserName", OracleDbType.Varchar2).Value = user.UserName;
                        command.Parameters.Add("p_PasswordHash", OracleDbType.Varchar2).Value = user.PasswordHash;

                        OracleParameter paramUserData = new OracleParameter("p_UserData", OracleDbType.RefCursor);
                        paramUserData.Direction = ParameterDirection.Output;
                        command.Parameters.Add(paramUserData);

                        OracleParameter paramContentData = new OracleParameter("p_ContentData", OracleDbType.RefCursor);
                        paramContentData.Direction = ParameterDirection.Output;
                        command.Parameters.Add(paramContentData);

                        command.ExecuteNonQuery();

                        int contentID = 0;

                        using (OracleDataReader userDataReader = paramUserData.Value as OracleRefCursor == null ? null : ((OracleRefCursor)paramUserData.Value).GetDataReader())
                        {
                            if (userDataReader != null)
                            {
                                while (userDataReader.Read())
                                {
                                    user.UserID = userDataReader.IsDBNull(userDataReader.GetOrdinal("UserID")) ? default(int) : userDataReader.GetInt32(userDataReader.GetOrdinal("UserID"));
                                    user.Email = userDataReader.IsDBNull(userDataReader.GetOrdinal("Email")) ? string.Empty : userDataReader.GetString(userDataReader.GetOrdinal("Email"));

                                    Role userRole;
                                    Enum.TryParse(userDataReader.IsDBNull(userDataReader.GetOrdinal("UserRole")) ? string.Empty : userDataReader.GetString(userDataReader.GetOrdinal("UserRole")), out userRole);
                                    user.UserRole = userRole;

                                    user.OrderCount = userDataReader.IsDBNull(userDataReader.GetOrdinal("OrderCount")) ? default(int) : userDataReader.GetInt32(userDataReader.GetOrdinal("OrderCount"));

                                    user.BirthDate = userDataReader.IsDBNull(userDataReader.GetOrdinal("BirthDate")) ? default(DateTime) : userDataReader.GetDateTime(userDataReader.GetOrdinal("BirthDate"));
                                    user.CreatingDate = userDataReader.IsDBNull(userDataReader.GetOrdinal("CreatingDate")) ? default(DateTime) : userDataReader.GetDateTime(userDataReader.GetOrdinal("CreatingDate"));

                                    user.PhoneNumber = userDataReader.IsDBNull(userDataReader.GetOrdinal("PhoneNumber")) ? string.Empty : userDataReader.GetString(userDataReader.GetOrdinal("PhoneNumber"));

                                    contentID = userDataReader.IsDBNull(userDataReader.GetOrdinal("ContentID")) ? default(int) : userDataReader.GetInt32(userDataReader.GetOrdinal("ContentID"));

                                    user.IsAutorize = true;
                                }
                            }
                        }
                        if (contentID != 0)
                        {
                            using (OracleDataReader contentDataReader = paramContentData.Value as OracleRefCursor == null ? null : ((OracleRefCursor)paramContentData.Value).GetDataReader())
                            {
                                if (contentDataReader != null)
                                {
                                    while (contentDataReader.Read())
                                    {
                                        string fileName = contentDataReader.IsDBNull(contentDataReader.GetOrdinal("FileName")) ? string.Empty : contentDataReader.GetString(contentDataReader.GetOrdinal("FileName"));
                                        string fileType = contentDataReader.IsDBNull(contentDataReader.GetOrdinal("FileType")) ? string.Empty : contentDataReader.GetString(contentDataReader.GetOrdinal("FileType"));
                                        string fileExtension = contentDataReader.IsDBNull(contentDataReader.GetOrdinal("FileExtension")) ? string.Empty : contentDataReader.GetString(contentDataReader.GetOrdinal("FileExtension"));
                                        DateTime uploadDate = contentDataReader.IsDBNull(contentDataReader.GetOrdinal("UploadDate")) ? default(DateTime) : contentDataReader.GetDateTime(contentDataReader.GetOrdinal("UploadDate"));
                                        DateTime modificationDate = contentDataReader.IsDBNull(contentDataReader.GetOrdinal("ModificationDate")) ? default(DateTime) : contentDataReader.GetDateTime(contentDataReader.GetOrdinal("ModificationDate"));
                                        byte[] content = contentDataReader.IsDBNull(contentDataReader.GetOrdinal("Content")) ? null : (byte[])contentDataReader.GetValue(contentDataReader.GetOrdinal("Content"));

                                        user.BinaryContent = new BinaryContent(contentID, fileName, fileType, fileExtension, uploadDate, modificationDate, content);
                                    }
                                }
                            }
                        }
                        else
                        {
                            user.BinaryContent = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidDataException(ex.Message);
                }

                connection.Close();
            }
        }
        public List<Product> GetProductDataFromDatabase()
        {
            List<Product> products = new List<Product>();
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("BEGIN :result := GetProductData; END;", connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int productId = Convert.ToInt32(reader["ProductID"]);
                            string sku = reader["SKU"].ToString();
                            string productName = reader["ProductName"].ToString();
                            int cost = Convert.ToInt32(reader["Cost"]);
                            int salesCount = Convert.ToInt32(reader["SalesCount"]);

                            int? descriptionId = reader["DescriptionID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["DescriptionID"]);
                            string descriptionFileName = reader["DescriptionFileName"] == DBNull.Value ? null : reader["DescriptionFileName"].ToString();
                            string descriptionFileType = reader["DescriptionFileType"] == DBNull.Value ? null : reader["DescriptionFileType"].ToString();
                            string descriptionFileExtension = reader["DescriptionFileExtension"] == DBNull.Value ? null : reader["DescriptionFileExtension"].ToString();
                            byte[] descriptionFileData = reader["FileData"] == DBNull.Value ? null : (byte[])reader["FileData"];
                            DateTime? descriptionUploadDate = reader["DescriptionUploadDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DescriptionUploadDate"]);
                            DateTime? descriptionModificationDate = reader["DescriptionModificationDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DescriptionModificationDate"]);
                            string descriptionInfo = reader["Info"] == DBNull.Value ? null : reader["Info"].ToString();
                            Description description = null;
                            if (descriptionId != null)
                                description = new Description((int)descriptionId, descriptionFileName, descriptionFileType, descriptionFileExtension,
                                    descriptionFileData, (DateTime)descriptionUploadDate, (DateTime)descriptionModificationDate, descriptionInfo);

                            int categoryId = Convert.ToInt32(reader["CategoryID"]);
                            string categoryName = reader["CategoryName"].ToString();
                            string categoryDescription = reader["CategoryDescription"].ToString();
                            int? parentCategoryId = reader["ParentCategoryID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["ParentCategoryID"]);
                            Category category = new Category(categoryId, categoryName, categoryDescription, parentCategoryId);


                            int contentId = Convert.ToInt32(reader["ContentID"]);
                            string binaryContentFileName = reader["BinaryContentFileName"] == DBNull.Value ? null : reader["BinaryContentFileName"].ToString();
                            string binaryContentFileType = reader["BinaryContentFileType"] == DBNull.Value ? null : reader["BinaryContentFileType"].ToString();
                            string binaryContentFileExtension = reader["BinaryContentFileExtension"] == DBNull.Value ? null : reader["BinaryContentFileExtension"].ToString();
                            DateTime binaryContentUploadDate = Convert.ToDateTime(reader["BinaryContentUploadDate"]);
                            DateTime binaryContentModificationDate = Convert.ToDateTime(reader["BinaryContentModificationDate"]);
                            byte[] binaryContentData = reader.IsDBNull(reader.GetOrdinal("Content")) ? null : (byte[])reader.GetValue(reader.GetOrdinal("Content"));
                            BinaryContent binaryContent = new BinaryContent(contentId, binaryContentFileName, binaryContentFileType, binaryContentFileExtension, binaryContentUploadDate, binaryContentModificationDate, binaryContentData);

                            products.Add(new Product(productId, productName, description, binaryContent, cost, category, salesCount));
                        }

                    }
                }
            }
            return products;
        }

        protected BinaryContent CreateBinaryContent(BinaryContent content, OracleConnection connection)
        {
            try
            {
                using (OracleCommand command = new OracleCommand("CreateBinaryContent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    OracleParameter paramContentData = new OracleParameter("p_cursor", OracleDbType.RefCursor);
                    paramContentData.Direction = ParameterDirection.Output;
                    command.Parameters.Add(paramContentData);

                    command.Parameters.Add("p_FileName", OracleDbType.Varchar2).Value = content.FileName;
                    command.Parameters.Add("p_FileType", OracleDbType.Varchar2).Value = content.FileType;
                    command.Parameters.Add("p_FileExtension", OracleDbType.Varchar2).Value = content.FileExtension;

                    OracleParameter paramContent = new OracleParameter("p_Content", OracleDbType.Blob);
                    paramContent.Direction = ParameterDirection.Input;
                    paramContent.Value = content.Content;
                    command.Parameters.Add(paramContent);


                    command.ExecuteNonQuery();

                    using (OracleDataReader contentDataReader = paramContentData.Value as OracleRefCursor == null ? null : ((OracleRefCursor)paramContentData.Value).GetDataReader())
                    {
                        if (contentDataReader != null)
                        {
                            while (contentDataReader.Read())
                            {
                                int ContentID = contentDataReader.IsDBNull(contentDataReader.GetOrdinal("ContentID")) ? default(int) : contentDataReader.GetInt32(contentDataReader.GetOrdinal("ContentID"));

                                string fileName = contentDataReader.IsDBNull(contentDataReader.GetOrdinal("FileName")) ? string.Empty : contentDataReader.GetString(contentDataReader.GetOrdinal("FileName"));
                                string fileType = contentDataReader.IsDBNull(contentDataReader.GetOrdinal("FileType")) ? string.Empty : contentDataReader.GetString(contentDataReader.GetOrdinal("FileType"));
                                string fileExtension = contentDataReader.IsDBNull(contentDataReader.GetOrdinal("FileExtension")) ? string.Empty : contentDataReader.GetString(contentDataReader.GetOrdinal("FileExtension"));
                                DateTime uploadDate = contentDataReader.IsDBNull(contentDataReader.GetOrdinal("UploadDate")) ? default(DateTime) : contentDataReader.GetDateTime(contentDataReader.GetOrdinal("UploadDate"));
                                DateTime modificationDate = contentDataReader.IsDBNull(contentDataReader.GetOrdinal("ModificationDate")) ? default(DateTime) : contentDataReader.GetDateTime(contentDataReader.GetOrdinal("ModificationDate"));
                                byte[] contentData = contentDataReader.IsDBNull(contentDataReader.GetOrdinal("Content")) ? null : (byte[])contentDataReader.GetValue(contentDataReader.GetOrdinal("Content"));

                                content = new BinaryContent(ContentID, fileName, fileType, fileExtension, uploadDate, modificationDate, contentData);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            return content;
        }

        protected void CallSignUpProcedure(User user)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                if (user.BinaryContent?.Content != null)
                    user.BinaryContent= CreateBinaryContent(user.BinaryContent, connection);
                
                int? contentID = null;
                if (user.BinaryContent?.ContentID != 0)
                    contentID = user.BinaryContent?.ContentID;
                else
                    contentID = Checkings.standartImage.ContentID;
                try
                {
                    using (OracleCommand command = new OracleCommand("SignUp", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        OracleParameter paramUserID = new OracleParameter("p_UserID", OracleDbType.Int32);
                        paramUserID.Direction = ParameterDirection.Output;
                        command.Parameters.Add(paramUserID);

                        OracleParameter paramUserCreatingDate = new OracleParameter("p_CreatingDate", OracleDbType.Date);
                        paramUserCreatingDate.Direction = ParameterDirection.Output;
                        command.Parameters.Add(paramUserCreatingDate);

                        command.Parameters.Add("p_UserName", OracleDbType.Varchar2).Value = user.UserName;
                        command.Parameters.Add("p_PasswordHash", OracleDbType.Varchar2).Value = user.PasswordHash;
                        command.Parameters.Add("p_Email", OracleDbType.Varchar2).Value = user.Email;
                        command.Parameters.Add("p_ContentID", OracleDbType.Int64).Value = contentID;
                        command.Parameters.Add("p_UserRole", OracleDbType.Varchar2).Value = user.UserRole;
                        command.Parameters.Add("p_BirthDate", OracleDbType.Date).Value = user.BirthDate;
                        command.Parameters.Add("p_PhoneNumber", OracleDbType.Varchar2).Value = user.PhoneNumber;

                        command.ExecuteNonQuery();

                        user.UserID = int.Parse(paramUserID.Value.ToString());

                        OracleDate oracleDateValue = (OracleDate)paramUserCreatingDate.Value;
                        if (!oracleDateValue.IsNull)
                            user.CreatingDate = oracleDateValue.Value;

                        user.IsAutorize = true;

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
        }

        private void UpdateBinaryContent(BinaryContent content, OracleConnection connection)
        {
            using (OracleCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "InsertOrUpdateBinaryContent";

                command.Parameters.Add("p_ContentID", OracleDbType.Int32).Value = content.ContentID;
                command.Parameters.Add("p_FileName", OracleDbType.Varchar2).Value = content.FileName;
                command.Parameters.Add("p_FileType", OracleDbType.Varchar2).Value = content.FileType;
                command.Parameters.Add("p_FileExtension", OracleDbType.Varchar2).Value = content.FileExtension;

                OracleParameter paramContent = new OracleParameter("p_Content", OracleDbType.Blob);
                paramContent.Direction = ParameterDirection.Input;
                paramContent.Value = content.Content;
                command.Parameters.Add(paramContent);

                OracleParameter outCursorParam = new OracleParameter();
                outCursorParam.ParameterName = "p_Cursor";
                outCursorParam.OracleDbType = OracleDbType.RefCursor;
                outCursorParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(outCursorParam);

                command.ExecuteNonQuery();

                if (outCursorParam.Value != DBNull.Value)
                {
                    OracleDataReader reader = ((OracleRefCursor)outCursorParam.Value).GetDataReader();

                    while (reader.Read())
                    {
                        int contentID = reader.IsDBNull(reader.GetOrdinal("ContentID")) ? default(int) : reader.GetInt32(reader.GetOrdinal("ContentID"));
                        string fileName = reader.IsDBNull(reader.GetOrdinal("FileName")) ? string.Empty : reader.GetString(reader.GetOrdinal("FileName"));
                        string fileType = reader.IsDBNull(reader.GetOrdinal("FileType")) ? string.Empty : reader.GetString(reader.GetOrdinal("FileType"));
                        string fileExtension = reader.IsDBNull(reader.GetOrdinal("FileExtension")) ? string.Empty : reader.GetString(reader.GetOrdinal("FileExtension"));
                        DateTime uploadDate = reader.IsDBNull(reader.GetOrdinal("UploadDate")) ? default(DateTime) : reader.GetDateTime(reader.GetOrdinal("UploadDate"));
                        DateTime modificationDate = reader.IsDBNull(reader.GetOrdinal("ModificationDate")) ? default(DateTime) : reader.GetDateTime(reader.GetOrdinal("ModificationDate"));
                        byte[] contentData = reader.IsDBNull(reader.GetOrdinal("Content")) ? null : (byte[])reader.GetValue(reader.GetOrdinal("Content"));

                        content = new BinaryContent(contentID, fileName, fileType, fileExtension, uploadDate, modificationDate, contentData);
                    }

                    reader.Close();
                }
            }
        }

        public BinaryContent UpdateBinaryContent(BinaryContent content)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                if (user.BinaryContent?.Content != null)
                    UpdateBinaryContent(content, connection);

                connection.Close();
            }

            return content;
        }
        public BinaryContent CreateBinaryContent(BinaryContent content)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                if (content.Content != null)
                    content = CreateBinaryContent(content, connection);

                connection.Close();
            }

            return content;
        }

        protected int GetUserIdByUserName(string userName)
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
        private void UpdateUserDate(OracleConnection connection)
        {
            using (OracleCommand command = new OracleCommand("UpdateUserData", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("p_UserID", OracleDbType.Int64).Value = user.UserID;
                command.Parameters.Add("p_UserName", OracleDbType.Varchar2).Value = user.UserName;
                command.Parameters.Add("p_Email", OracleDbType.Varchar2).Value = user.Email;
                command.Parameters.Add("p_BirthDate", OracleDbType.Date).Value = user.BirthDate;
                command.Parameters.Add("p_PhoneNumber", OracleDbType.Varchar2).Value = user.PhoneNumber;

                OracleParameter paramUserData = new OracleParameter("p_Cursor", OracleDbType.RefCursor);
                paramUserData.Direction = ParameterDirection.Output;
                command.Parameters.Add(paramUserData);

                command.ExecuteNonQuery();

                using (OracleDataReader userDataReader = paramUserData.Value as OracleRefCursor == null ? null : ((OracleRefCursor)paramUserData.Value).GetDataReader())
                {
                    if (userDataReader != null)
                    {
                        while (userDataReader.Read())
                        {
                            user.UserID = userDataReader.IsDBNull(userDataReader.GetOrdinal("UserID")) ? default(int) : userDataReader.GetInt32(userDataReader.GetOrdinal("UserID"));
                            user.Email = userDataReader.IsDBNull(userDataReader.GetOrdinal("Email")) ? string.Empty : userDataReader.GetString(userDataReader.GetOrdinal("Email"));

                            Role userRole;
                            Enum.TryParse(userDataReader.IsDBNull(userDataReader.GetOrdinal("UserRole")) ? string.Empty : userDataReader.GetString(userDataReader.GetOrdinal("UserRole")), out userRole);
                            user.UserRole = userRole;

                            user.OrderCount = userDataReader.IsDBNull(userDataReader.GetOrdinal("OrderCount")) ? default(int) : userDataReader.GetInt32(userDataReader.GetOrdinal("OrderCount"));

                            user.BirthDate = userDataReader.IsDBNull(userDataReader.GetOrdinal("BirthDate")) ? default(DateTime) : userDataReader.GetDateTime(userDataReader.GetOrdinal("BirthDate"));
                            user.CreatingDate = userDataReader.IsDBNull(userDataReader.GetOrdinal("CreatingDate")) ? default(DateTime) : userDataReader.GetDateTime(userDataReader.GetOrdinal("CreatingDate"));

                            user.PhoneNumber = userDataReader.IsDBNull(userDataReader.GetOrdinal("PhoneNumber")) ? string.Empty : userDataReader.GetString(userDataReader.GetOrdinal("PhoneNumber"));

                            user.IsAutorize = true;
                        }
                    }
                }
            }
        }
        public void UpdateUserDate()
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                if (user.BinaryContent?.Content != null)
                    UpdateUserDate(connection);

                connection.Close();
            }
        }
        public List<Category> CreateCategoryHierarchy()
        {
            List<Category> result = new List<Category>();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM CategoryHierarchyView";

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Category categoryItem = new Category
                            {
                                CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                                ParentCategoryID = reader.IsDBNull(reader.GetOrdinal("ParentCategoryID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ParentCategoryID")),
                                CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName")) ? string.Empty : reader.GetString(reader.GetOrdinal("CategoryName")),
                                CategoryDescription = reader.IsDBNull(reader.GetOrdinal("CategoryDescription")) ? string.Empty : reader.GetString(reader.GetOrdinal("CategoryDescription"))
                            };

                            result.Add(categoryItem);
                        }
                    }
                }
            }

            return result;
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

        private async void CreateOrderItems(List<StoreCartInteraction.OrderItem> orderItems, int orderNumber, OracleConnection connection)
        {
            foreach (var item in orderItems)
            {
                OracleCommand command = new OracleCommand("AddOrderItem", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("p_OrderNumber", OracleDbType.Int64, orderNumber, ParameterDirection.Input);
                command.Parameters.Add("p_ProductID", OracleDbType.Varchar2, item.product.ProductID, ParameterDirection.Input);
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

        private async void CreatePayment(Payment payment, bool isBankCard, OracleConnection connection)
        {
            int? cashId = null;
            int? bankCardID = null;
            if (isBankCard)
            {
                bankCardID = CreateNewBankCardID(connection);
                CreateBankCard((int)bankCardID, payment.OrderNumber, payment.BankCardID.ToString(), connection);
            }
            else
            {
                cashId = CreateNewCashID(connection);
                CreateCash((int)cashId, payment.OrderNumber, connection);
            }

            OracleCommand command = new OracleCommand("AddPayment", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("p_UserId", OracleDbType.Int64, this.user.UserID, ParameterDirection.Input);
            command.Parameters.Add("p_OrderNumber", OracleDbType.Int64, payment.OrderNumber, ParameterDirection.Input);
            command.Parameters.Add("p_TotalPrice", OracleDbType.Int64, payment.TotalPrice, ParameterDirection.Input);
            command.Parameters.Add("p_CashID", OracleDbType.Int64, cashId, ParameterDirection.Input);
            command.Parameters.Add("p_BankCardID", OracleDbType.Int64, bankCardID, ParameterDirection.Input);

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }
        private async void CreateTransaction(Transaction transaction, OracleConnection connection)
        {
            OracleCommand command = new OracleCommand("AddTransaction", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("p_UserID", OracleDbType.Int64, this.user.UserID, ParameterDirection.Input);
            command.Parameters.Add("p_OrderNumber", OracleDbType.Int64, transaction.OrderNumber, ParameterDirection.Input);
            command.Parameters.Add("p_TransactionType", OracleDbType.Varchar2, transaction.TransactionDate, ParameterDirection.Input);
            command.Parameters.Add("p_Description", OracleDbType.Varchar2, transaction.Description, ParameterDirection.Input);

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        public bool CreateOrder(int orderNumber, List<StoreCartInteraction.OrderItem> orderItems, int totalPrice, bool isBankCard, string BankCardNumber)
        {
            OracleConnection connection = new OracleConnection(connectionString);
            connection.Open();
            try
            {
                string description = "Categorios: ";
                foreach (var item in orderItems)
                {
                    description += item.product.Category.CategoryName + ", ";
                }
                var payment = new Payment() { OrderNumber = orderNumber, BankCardID = BankCardNumber == null ? 0 : int.Parse(BankCardNumber), TotalPrice = totalPrice };
                CreatePayment(payment, isBankCard, connection);
                CreateTransaction(new Transaction() { OrderNumber = orderNumber, TransactionType = "buy", Description = description }, connection);
                CreateOrderItems(orderItems, orderNumber, connection);

                OracleCommand command = new OracleCommand("AddOrder", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("p_OrderNumber", OracleDbType.Decimal, orderNumber, ParameterDirection.Input);
                command.Parameters.Add("p_UserID", OracleDbType.Decimal, this.user.UserID, ParameterDirection.Input);

                command.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }

            connection.Close();
            return true;
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
        public DataTable SelectMyUserData()
        {
            DataTable dataTable = new DataTable();
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                int userID = user.UserID;

                string sql = @"
                SELECT
                    O.OrderDate,
                    O.OrderStatus,
                    U.UserName,
                    P.CashID,
                    P.BankCardID,
                    BC.BankNumber,
                    O.OrderNumber
                FROM
                    Orders O
                JOIN
                    Users U ON O.UserID = U.UserID
                LEFT JOIN
                    Payments P ON O.OrderNumber = P.OrderNumber
                LEFT JOIN
                    BankCard BC ON P.BankCardID = BC.BankCardID
                WHERE
                    O.UserID = :p_UserID";

                using (OracleCommand command = new OracleCommand(sql, connection))
                {
                    command.Parameters.Add("p_UserID", OracleDbType.Int32).Value = userID;

                    try
                    {
                        OracleDataAdapter adapter = new OracleDataAdapter(command);
                        adapter.Fill(dataTable);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading data: {ex.Message}");
                    }
                }
                connection.Close();
            }
            return dataTable;
        }
        public (string, byte[]) GetProductImage(int productID)
        {
            string fileName = string.Empty;
            byte[] fileData = null;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                try
                {
                    using (OracleCommand command = new OracleCommand("BEGIN :result := GetProductImage(:p_ProductID); END;", connection))
                    {
                        command.Parameters.Add("result", OracleDbType.RefCursor, ParameterDirection.ReturnValue);
                        command.Parameters.Add("p_ProductID", OracleDbType.Int64).Value = productID;

                        command.ExecuteNonQuery();

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                fileName = reader["ProductImageFileName"].ToString();
                                OracleBlob blob = reader.GetOracleBlob(0);

                                fileData = new byte[blob.Length];
                                blob.Read(fileData, 0, Convert.ToInt32(blob.Length));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            return (fileName, fileData);
        }

        public (string, byte[]) GetDescriptionData(int descriptionID)
        {
            string fileName = string.Empty;
            byte[] fileData = null;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                try
                {
                    using (OracleCommand command = new OracleCommand("BEGIN :result := GetDescriptionData(:p_DescriptionID); END;", connection))
                    {
                        command.Parameters.Add("result", OracleDbType.RefCursor, ParameterDirection.ReturnValue);
                        command.Parameters.Add("p_DescriptionID", OracleDbType.Int64).Value = descriptionID;

                        command.ExecuteNonQuery();

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                fileName = reader["FileName"].ToString();
                                OracleBlob blob = reader.GetOracleBlob(0);

                                fileData = new byte[blob.Length];
                                blob.Read(fileData, 0, Convert.ToInt32(blob.Length));
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            return (fileName, fileData);
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
