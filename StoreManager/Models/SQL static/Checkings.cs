using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using StoreManager.DB_classes;
using StoreManager.Models.Abstract.Interfaces;
using StoreManager.Models.Guest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StoreManager.Models.SQL_static
{
    public static class Checkings
    {
        private static string _connectionString = ConfigurationManager.ConnectionStrings["conString"].ConnectionString;
        private static string connectionString
        {
            get { return _connectionString; }
            set { _connectionString = ConfigurationManager.ConnectionStrings["conString"].ConnectionString; }
        }
        private static string _imagePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources\\Standart profile image\\StandartProfileImage.png");
        public static BinaryContent standartImage = new BinaryContent(_imagePath) { ContentID = 1 };
        public static User guest = new User();
        private static void CreateStandartImage()
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "InsertOrUpdateBinaryContent";

                    command.Parameters.Add("p_ContentID", OracleDbType.Int32).Value = standartImage.ContentID;
                    command.Parameters.Add("p_FileName", OracleDbType.Varchar2).Value = standartImage.FileName;
                    command.Parameters.Add("p_FileType", OracleDbType.Varchar2).Value = standartImage.FileType;
                    command.Parameters.Add("p_FileExtension", OracleDbType.Varchar2).Value = standartImage.FileExtension;

                    OracleParameter paramContent = new OracleParameter("p_Content", OracleDbType.Blob);
                    paramContent.Direction = ParameterDirection.Input;
                    paramContent.Value = standartImage.Content;
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
                            byte[] content = reader.IsDBNull(reader.GetOrdinal("Content")) ? null : (byte[])reader.GetValue(reader.GetOrdinal("Content"));

                            standartImage = new BinaryContent(contentID, fileName, fileType, fileExtension, uploadDate, modificationDate, content);
                        }

                        reader.Close();
                    }
                }
            }
        }

        public static void CreateGuestIfNotExist()
        {
            CreateStandartImage();

            guest = User.CreateGuest();

            if (!CheckUserNameExistence(guest.UserName))
            {
                CreateGuestUser(guest);
            }
            var acc = new StoreForGuest();
            guest = acc.user;
        }

        private static void CreateGuestUser(User userGuest)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

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

                        command.Parameters.Add("p_UserName", OracleDbType.Varchar2).Value = userGuest.UserName;
                        command.Parameters.Add("p_PasswordHash", OracleDbType.Varchar2).Value = userGuest.PasswordHash;
                        command.Parameters.Add("p_Email", OracleDbType.Varchar2).Value = userGuest.Email;
                        command.Parameters.Add("p_ContentID", OracleDbType.Int64).Value = userGuest.BinaryContent.ContentID;
                        command.Parameters.Add("p_UserRole", OracleDbType.Varchar2).Value = userGuest.UserRole;
                        command.Parameters.Add("p_BirthDate", OracleDbType.Date).Value = new OracleDate(userGuest.BirthDate);
                        command.Parameters.Add("p_PhoneNumber", OracleDbType.Varchar2).Value = userGuest.PhoneNumber;

                        command.ExecuteNonQuery();

                        userGuest.UserID = int.Parse(paramUserID.Value.ToString());

                        OracleDate oracleDateValue = (OracleDate)paramUserCreatingDate.Value;
                        if (!oracleDateValue.IsNull)
                            userGuest.CreatingDate = oracleDateValue.Value;
                    }   
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
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
        private static bool CheckUserNameExistence(string UserName, OracleConnection connection)
        {
            try
            {
                using (OracleCommand command = new OracleCommand("BEGIN :result := CheckUserNameExistence(:p_UserName); END;", connection))
                {
                    command.Parameters.Add("result", OracleDbType.Boolean, ParameterDirection.ReturnValue);
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
            return true;
        }
        private static bool CheckUserPhoneNumberExistence(string PhoneNumber, OracleConnection connection)
        {
            try
            {
                using (OracleCommand command = new OracleCommand("BEGIN :result := CheckUserPhoneNumberExistence(:p_PhoneNumber); END;", connection))
                {
                    command.Parameters.Add("result", OracleDbType.Boolean, ParameterDirection.ReturnValue);
                    command.Parameters.Add("p_PhoneNumber", OracleDbType.Varchar2).Value = PhoneNumber;

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
            return true;
        }
        private static bool CheckUserEmailExistence(string Email, OracleConnection connection)
        {
            try
            {
                using (OracleCommand command = new OracleCommand("BEGIN :result := CheckUserEmailExistence(:p_PhoneNumber); END;", connection))
                {
                    command.Parameters.Add("result", OracleDbType.Boolean, ParameterDirection.ReturnValue);
                    command.Parameters.Add("p_Email", OracleDbType.Varchar2).Value = Email;

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
            return true;
        }

        /*public static bool CheckUserExistence(string UserName, string Email, string PhoneNumber)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                if (CheckUserNameExistence(UserName, connection))
                {
                    return true;
                }
                if (CheckUserEmailExistence(Email, connection))
                {
                    return true;
                }
                if (CheckUserPhoneNumberExistence(PhoneNumber, connection))
                {
                    return true;
                }

                connection.Close();
            }
            return false;
        }*/
        public static bool CheckUserNameExistence(string UserName)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                if (CheckUserNameExistence(UserName, connection))
                {
                    return true;
                }
                connection.Close();
            }
            return false;
        }
        public static bool CheckUserEmailExistence(string Email)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                if (CheckUserEmailExistence(Email, connection))
                {
                    return true;
                }
                connection.Close();
            }
            return false;
        }
        public static bool CheckUserPhoneNumberExistence(string PhoneNumber)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                if (CheckUserPhoneNumberExistence(PhoneNumber, connection))
                {
                    return true;
                }

                connection.Close();
            }
            return false;
        }
    }
}
