using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using StoreManager.DB_classes;
using StoreManager.Models.Abstract.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StoreManager.Models.Abstract.Classes
{
    public abstract class AdminStoreInteraction : ManagerStoreInteraction
    {
        protected AdminStoreInteraction(User user, bool isSignIn) : base(user, isSignIn)
        { }

        public bool DeleteProduct(int productId)
        {
            bool result = false;

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                try
                {
                    using (OracleCommand command = new OracleCommand("BEGIN :result := DeleteProduct(:p_productID); END;", connection))
                    {
                        command.Parameters.Add("result", OracleDbType.Boolean, ParameterDirection.Output);
                        command.Parameters.Add("p_productID", OracleDbType.Decimal).Value = productId;

                        command.ExecuteNonQuery();

                        result = ((OracleBoolean)command.Parameters["result"].Value).IsTrue;

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
            return result;
        }

        public bool UpdateUserRole(User user)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("UpdateUserRole", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_UserID", OracleDbType.Int64).Value = user.UserID;
                        command.Parameters.Add("p_NewUserRole", OracleDbType.Varchar2).Value = user.UserRole;

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
        public bool DeleteUser(User user)
        {
            bool isOkDeleted = false;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("DeleteUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_UserID", OracleDbType.Int64).Value = user.UserID;
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
        public string GetPasswordHash(User user)
        {
            string passwordHash = "";
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                try
                {
                    using (OracleCommand command = new OracleCommand("BEGIN :result := GetPasswordHash(:userId); END;", connection))
                    {
                        command.CommandType = CommandType.Text;

                        OracleParameter resultParameter = new OracleParameter("result", OracleDbType.Varchar2, 64);
                        resultParameter.Direction = ParameterDirection.ReturnValue;

                        OracleParameter userIdParameter = new OracleParameter("userId", OracleDbType.Int32);
                        userIdParameter.Value = user.UserID;

                        command.Parameters.Add(resultParameter);
                        command.Parameters.Add(userIdParameter);

                        command.ExecuteNonQuery();

                        passwordHash = resultParameter.Value.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                connection.Close();
            }
            return passwordHash;
        }
        public bool DeleteOrderByOrderNumber(int orderNumber)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand("DeleteOrderByOrderNumber ", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_OrderNumber", OracleDbType.Int64).Value = orderNumber;

                        command.ExecuteNonQuery();

                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    return false;
                }

                connection.Close();
            }
        }
    }
}
