using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using StoreManager.DB_classes;
using StoreManager.Models.Abstract.Interfaces;
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

        public static void CreateGuestIfNotExist()
        {
            string guestName = "Guest";
            string email = null;
            string phoneNumber = null;
            if (!CheckUserExistence(guestName, email, phoneNumber))
            {
                string password = "1111";
                string passwordHash = HashPassword(password);
                string userRole = Role.guest.ToString();
                DateTime birthDate = DateTime.Now;
                CreateGuestUser(guestName, passwordHash, email, userRole, birthDate, phoneNumber);
            }
        }

        private static void CreateGuestUser(string userName, string passwordHash, string email, string userRole, DateTime birthDate, string phoneNumber)
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

                        command.Parameters.Add("p_UserName", OracleDbType.Varchar2).Value = userName;
                        command.Parameters.Add("p_PasswordHash", OracleDbType.Varchar2).Value = passwordHash;
                        command.Parameters.Add("p_Email", OracleDbType.Varchar2).Value = email;
                        command.Parameters.Add("p_ContentID", OracleDbType.Int64).Value = null;
                        command.Parameters.Add("p_UserRole", OracleDbType.Varchar2).Value = userRole;
                        command.Parameters.Add("p_BirthDate", OracleDbType.Date).Value = new OracleDate(birthDate);
                        command.Parameters.Add("p_PhoneNumber", OracleDbType.Varchar2).Value = phoneNumber;

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

        public static bool CheckUserNameExistence(string UserName)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

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

                connection.Close();
            }
            return true;
        }

        public static bool CheckUserExistence(string UserName, string Email, string PhoneNumber)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("BEGIN :result := CheckUserExistence(:p_UserName, :p_PhoneNumber, :p_Email); END;", connection))
                    {
                        command.Parameters.Add("result", OracleDbType.Boolean, System.Data.ParameterDirection.ReturnValue);
                        command.Parameters.Add("p_UserName", OracleDbType.Varchar2).Value = UserName;
                        command.Parameters.Add("p_PhoneNumber", OracleDbType.Varchar2).Value = Email;
                        command.Parameters.Add("p_Email", OracleDbType.Varchar2).Value = PhoneNumber;

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
    }
}
