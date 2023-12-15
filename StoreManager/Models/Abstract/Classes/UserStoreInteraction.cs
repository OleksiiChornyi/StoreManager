using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using StoreManager.Models.Abstract.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StoreManager.Models.Abstract.Classes.StoreCartInteraction;
using System.Windows;
using StoreManager.DB_classes;

namespace StoreManager.Models.Abstract.Classes
{
    public abstract class UserStoreInteraction : AllUsersInteractions
    {
        protected UserStoreInteraction(User user, bool isSignIn) : base(user, isSignIn)
        { }

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

        /*        public byte[] GetDecroptionData(int descriptionId)
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
                }*/

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
            foreach (var item in orderItems)
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

            command.Parameters.Add("p_UserId", OracleDbType.Int64, this.user.UserID, ParameterDirection.Input);
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
                command.Parameters.Add("p_UserID", OracleDbType.Decimal, this.user.UserID, ParameterDirection.Input);

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
