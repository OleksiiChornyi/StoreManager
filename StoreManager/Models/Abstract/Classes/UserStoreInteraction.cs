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
    }
}
