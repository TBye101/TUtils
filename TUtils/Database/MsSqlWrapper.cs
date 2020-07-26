
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using TUtils.IO;

namespace TUtils.Database
{
    /// <summary>
    /// An implementation of our database wrapper for MS SQL.
    /// </summary>
    public class MsSqlWrapper : IDatabaseWrapper, IDisposable
    {
        private SqlConnection Connection { get; }
        private static readonly object SyncObject = new object();

        public MsSqlWrapper(SqlConnection dbConnection)
        {
            this.Connection = dbConnection;
            this.Connection.Open();
        }

        private void LogSQLCommand<FunctionType1, FunctionType2>(string nonQuerySQL, Func<FunctionType1, FunctionType2> resultValidator, params SQLParam[] parameters)
        {
            try
            {
                Log.LogFile.Write(LogLevel.Information, "Executing non-query SQL: " + nonQuerySQL);
                Log.LogFile.Write(LogLevel.Information, "Using result validator method: " + resultValidator.Method.Name);

                foreach (SQLParam item in parameters)
                {
                    string paramInfo = "Parameter: " + item.Name + ", Value: " + item.Value;
                    Log.LogFile.Write(LogLevel.Debug, paramInfo);
                }
            }
            catch (System.Exception e)
            {
                Log.LogFile.WriteException(e, "An error occured while logging an SQL command's information");
            }
        }

        public bool AttemptNonQuery(string nonQuery, Func<int, bool> resultValidator, params SQLParam[] parameters)
        {
            this.LogSQLCommand(nonQuery, resultValidator, parameters);
            lock (SyncObject)
            {
                SqlTransaction transaction = null;
                try
                {
                    transaction = this.Connection.BeginTransaction();
                    using SqlCommand command = this.CreateSqlCommand(nonQuery, transaction, parameters);
                    int rowsAffected = command.ExecuteNonQuery();
                    bool validResult = resultValidator.Invoke(rowsAffected);

                    if (validResult)
                        transaction.Commit();
                    else
                        transaction.Rollback();
                    
                    return validResult;
                }
                catch (System.Exception e)
                {
                    Log.LogFile.WriteException(e, "An error occured while attempting to execute a non-query SQL statement");

                    if (transaction != null)
                        transaction.Rollback();

                    return false;
                }
                finally
                {
                    if (transaction != null)
                        transaction.Dispose();
                }   
            }
        }

        /// <summary>
        /// Initializes a SqlCommand with parameters.
        /// It is the responsibility of the caller to dispose of the SQlCommand object properly.
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private SqlCommand CreateSqlCommand(string sqlText, SqlTransaction transaction, params SQLParam[] parameters)
        {
            SqlCommand command = new SqlCommand(sqlText, this.Connection, transaction);
            Array.ForEach(parameters, 
            (parameter) =>
            {
                if (parameter.Value is ulong)
                    parameter.Value = (long)parameter.Value;
                command.Parameters.AddWithValue("@" + parameter.Name, parameter.Value);
            });
            return command;
        }

        public List<ExportedDataType> SelectData<ExportedDataType>(string selectQuery, Func<IDataRow, ExportedDataType> dataParser, params SQLParam[] parameters)
        {
            this.LogSQLCommand(selectQuery, dataParser, parameters);

            lock (SyncObject)
            {
                if (selectQuery == null || selectQuery.Equals(string.Empty) || dataParser == null)
                    throw new ArgumentNullException("A parameter was unacceptably null or empty");

                try
                {
                    using SqlCommand command = this.CreateSqlCommand(selectQuery, null, parameters);
                    using SqlDataAdapter dataSource = new SqlDataAdapter(command);

                    using DataTable dataTable = new DataTable();
                    dataSource.Fill(dataTable);
                    List<ExportedDataType> selectedData = new List<ExportedDataType>(dataTable.Rows.Count);
                    
                    foreach (DataRow item in dataTable.Rows)
                    {
                        IDataRow wrappedRow = new MsSqlDataRow(item);
                        ExportedDataType parsedItem = dataParser.Invoke(wrappedRow);
                        selectedData.Add(parsedItem);
                    }

                    return selectedData;
                }
                catch (System.Exception e)
                {
                    Log.LogFile.WriteException(e, "An error occured while executing a SQL query");
                    return new List<ExportedDataType>();
                }
            }
        }

        public void Dispose()
        {
            this.Connection?.Dispose();
        }
    }
}