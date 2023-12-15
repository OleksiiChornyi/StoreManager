using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using StoreManager.DB_classes;
using StoreManager.Models.Abstract.Interfaces;
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
        public bool isOk { get; set; }

        protected AllUsersInteractions(User user, bool isSignIn)
        {
            isOk = false;
            this.user = user;
            if (isSignIn)
            {
                SignInFunction(user);
            }
            else
            {
                CallSignUpProcedure(user);
            }
            /*if (isOk)
            {
                userId = GetUserIdByUserName(userName);
            }*/
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

                                    user.PhoneNumber = userDataReader.IsDBNull(userDataReader.GetOrdinal("PhoneNumber")) ? string.Empty : userDataReader.GetString(userDataReader.GetOrdinal("PhoneNumber"));

                                    contentID = userDataReader.IsDBNull(userDataReader.GetOrdinal("ContentID")) ? default(int) : userDataReader.GetInt32(userDataReader.GetOrdinal("ContentID"));
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
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
        }

        private void CreateBinaryContent(BinaryContent content, OracleConnection connection)
        {
            int ContentID = -1;

            try
            {
                using (OracleCommand command = new OracleCommand("CreateBinaryContent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    OracleParameter paramContentID = new OracleParameter("p_ContentID", OracleDbType.Int32);
                    paramContentID.Direction = ParameterDirection.Output;
                    command.Parameters.Add(paramContentID);

                    command.Parameters.Add("p_FileName", OracleDbType.Varchar2).Value = content.FileName;
                    command.Parameters.Add("p_FileType", OracleDbType.Varchar2).Value = content.FileType;
                    command.Parameters.Add("p_FileExtension", OracleDbType.Varchar2).Value = content.FileExtension;

                    OracleParameter paramContent = new OracleParameter("p_Content", OracleDbType.Blob);
                    paramContent.Direction = ParameterDirection.Input;
                    paramContent.Value = content.Content;
                    command.Parameters.Add(paramContent);


                    command.ExecuteNonQuery();

                    if (paramContentID.Value != null)
                        int.TryParse(paramContentID.Value.ToString(), out ContentID);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            content.ContentID = ContentID;
        }

        protected void CallSignUpProcedure(User user)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                if (user.BinaryContent?.Content != null)
                    CreateBinaryContent(user.BinaryContent, connection);

                int? contentID = null;
                if (user.BinaryContent?.ContentID != -1)
                    contentID = user.BinaryContent?.ContentID;
                try
                {
                    using (OracleCommand command = new OracleCommand("SignUp", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        OracleParameter paramUserID = new OracleParameter("p_UserID", OracleDbType.Int32);
                        paramUserID.Direction = ParameterDirection.Output;
                        command.Parameters.Add(paramUserID);

                        command.Parameters.Add("p_UserName", OracleDbType.Varchar2).Value = user.UserName;
                        command.Parameters.Add("p_PasswordHash", OracleDbType.Varchar2).Value = user.PasswordHash;
                        command.Parameters.Add("p_Email", OracleDbType.Varchar2).Value = user.Email;
                        command.Parameters.Add("p_ContentID", OracleDbType.Int64).Value = contentID;
                        command.Parameters.Add("p_UserRole", OracleDbType.Varchar2).Value = user.UserRole;
                        command.Parameters.Add("p_BirthDate", OracleDbType.Date).Value = user.BirthDate;
                        command.Parameters.Add("p_PhoneNumber", OracleDbType.Varchar2).Value = user.PhoneNumber;


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
        public List<Category> CreateCategoryHierarchy()
        {
            List<Category> result = new List<Category>();

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
