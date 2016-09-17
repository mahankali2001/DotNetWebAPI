using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using SampleApp.Core.Logger;

namespace SampleApp.Core.Data
{
    class MsSqlServerImpl : IDatabase
    {
        private static readonly ILogger logger = LogManager.GetLogger(typeof(MsSqlServerImpl));
        private static MsSqlServerImpl instance;
        private static readonly string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
        private static readonly PerformanceTimer performanceTimer = new PerformanceTimer();

        #region Constructors

        protected MsSqlServerImpl()
        {
        }

        #endregion

        #region Internal methods

        internal SqlConnection OpenConnection()
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ApplicationException("Connection string is null or empty.");
            }

            SqlConnection sqlConnection = new SqlConnection(connectionString);

            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }

            return sqlConnection;
        }

        internal void PopulateSqlParameters(SqlCommand sqlCommand, SqlParameterList sqlParameterList)
        {
            if (sqlParameterList == null)
                return;

            if (sqlParameterList.sqlParameters.Count <= 0)
                return;

            // Add a Sql parameter with direction as return value to capture the identity column value in case of INSERT's 
            // or if the stored procedures return a value.
            SqlParameter returnValueSqlParameter = new SqlParameter
            {
                Name = SqlProperties.SP_PARAMETER_RETURN_VALUE_NAME,
                Direction = SqlProperties.SqlParameterDirection.ReturnValue
            };
            sqlParameterList.sqlParameters.Insert(0, returnValueSqlParameter);

            foreach (SqlParameter sqlParameter in sqlParameterList.sqlParameters)
            {
                System.Data.SqlClient.SqlParameter sqlParam = new System.Data.SqlClient.SqlParameter(sqlParameter.Name, sqlParameter.Value);

                switch (sqlParameter.Direction)
                {
                    case SqlProperties.SqlParameterDirection.Input:
                        sqlParam.Direction = ParameterDirection.Input;
                        break;
                    case SqlProperties.SqlParameterDirection.Output:
                        sqlParam.Direction = ParameterDirection.Output;
                        break;
                    case SqlProperties.SqlParameterDirection.InputOuput:
                        sqlParam.Direction = ParameterDirection.InputOutput;
                        break;
                    case SqlProperties.SqlParameterDirection.ReturnValue:
                        sqlParam.Direction = ParameterDirection.ReturnValue;
                        break;
                    default:
                        sqlParam.Direction = ParameterDirection.Input;
                        break;
                }

                if (sqlParameter.DbType != null)
                {
                    switch (sqlParameter.DbType.FullName)
                    {
                        case "System.DateTime":
                            DateTime dateTime = (DateTime)sqlParameter.Value;
                            if (dateTime.Year < 1900)
                                sqlParam.Value = DBNull.Value;
                            break;

                        default:
                            break;
                    }
                }

                sqlCommand.Parameters.Add(sqlParam);
                StringBuilder logMessage = new StringBuilder("PopulateSqlParameters(): SQL Parameter = [Name=");
                logMessage.Append(sqlParameter.Name);
                logMessage.Append(", Value=");
                logMessage.Append(sqlParameter.Value);
                logMessage.Append(", Direction=");
                logMessage.Append(sqlParameter.Direction);
                logger.Debug(logMessage.ToString());
            }
        }

        #endregion

        #region Static methods

        public static MsSqlServerImpl Instance
        {
            get
            {
                if (instance == null)
                {
                    logger.Debug("Creating MS SQL Server database implementation instance.");
                    instance = new MsSqlServerImpl();
                }

                return instance;
            }
        }

        #endregion

        #region IDatabase Members

        /// <summary>
        /// Executes the specified SQL statement and returns the number of rows affected.
        ///  This uses ExecuteNonQuery() method.
        /// </summary>
        /// <param name="sqlQuery">SQL statement to execute.</param>
        /// <returns>Number of rows affected.</returns>
        public int ExecuteSql(string sqlQuery)
        {
            try
            {
                string logMessage = string.Format("MsSqlServerImpl.ExecuteSql(): SQL statement = {0}", sqlQuery);
                performanceTimer.Start(logMessage);
                using (SqlConnection sqlConnection = OpenConnection())
                {
                    using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection, null))
                    {
                        sqlCommand.CommandType = CommandType.Text;
                        logger.Debug(logMessage);
                        int result = sqlCommand.ExecuteNonQuery();
                        performanceTimer.Stop();
                        return result;
                    }
                }
            }
            catch (Exception)
            {
                performanceTimer.Reset();
                throw;
            }
        }

        /// <summary>
        /// Executes the specified SQL statement and returns the first column of the first row.
        ///  This uses ExecuteScalar() method.
        /// </summary>
        /// <typeparam name="TFirstDbColumnType">Column type of the first database column in the table.</typeparam>
        /// <param name="sqlQuery">SQL statement to execute.</param>
        /// <returns>Returns the first column of the first row.</returns>
        public TFirstDbColumnType ExecuteSql<TFirstDbColumnType>(string sqlQuery)
        {
            try
            {
                string logMessage = string.Format("MsSqlServerImpl.ExecuteSql<TFirstDbColumnType>(): SQL statement = {0}", sqlQuery);
                performanceTimer.Start(logMessage);
                using (SqlConnection sqlConnection = OpenConnection())
                {
                    using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection, null))
                    {
                        sqlCommand.CommandType = CommandType.Text;
                        logger.Debug(logMessage);
                        TFirstDbColumnType result = (TFirstDbColumnType)sqlCommand.ExecuteScalar();
                        performanceTimer.Stop();
                        return result;
                    }
                }
            }
            catch (Exception)
            {
                performanceTimer.Reset();
                throw;
            }
        }

        /// <summary>
        /// Executes the specified SQL statement and returns the result set.
        ///  This uses SqlDataAdapter.Fill() method.
        /// </summary>
        /// <param name="sqlQuery">SQL statement to execute.</param>
        /// <param name="sqlResults">Returns the result set from the SQL.</param>
        /// <returns>Number of rows affected.</returns>
        public int ExecuteSql(string sqlQuery, out IList<DatabaseRow> sqlResults)
        {
            sqlResults = new List<DatabaseRow>();

            try
            {
                string logMessage = string.Format("MsSqlServerImpl.ExecuteSql(out IList<DatabaseRow): SQL statement = {0}", sqlQuery);
                performanceTimer.Start(logMessage);
                using (SqlConnection sqlConnection = OpenConnection())
                {
                    using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection, null))
                    {
                        DataTable dataTable = new DataTable();
                        sqlCommand.CommandType = CommandType.Text;
                        logger.Debug(logMessage);
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                        sqlDataAdapter.Fill(dataTable);

                        if (dataTable.Rows.Count <= 0)
                            return dataTable.Rows.Count;

                        foreach (DataRow dataTableRow in dataTable.Rows)
                        {
                            DatabaseRow databaseRow = new DatabaseRow();
                            foreach (DataColumn dataColumn in dataTable.Columns)
                            {
                                object o = dataTableRow[dataColumn];
                                if (o is DBNull)
                                {
                                    databaseRow.Columns.Add(dataColumn.ColumnName,
                                                            new DatabaseColumn() { Name = dataColumn.ColumnName, Value = null });
                                }
                                else
                                {
                                    databaseRow.Columns.Add(dataColumn.ColumnName,
                                                            new DatabaseColumn()
                                                            {
                                                                Name = dataColumn.ColumnName,
                                                                Value = dataTableRow[dataColumn]
                                                            });
                                }
                            }

                            sqlResults.Add(databaseRow);
                        }
                    }
                }
                performanceTimer.Stop();
            }
            catch (Exception)
            {
                performanceTimer.Reset();
                throw;
            }

            return sqlResults.Count;
        }

        /// <summary>
        /// Executes the stored procedure specified by the name and the corresponding parameters and returns the number of rows affected.
        ///  This uses ExecuteNonQuery() method.
        /// </summary>
        /// <param name="storedProcedureName">Stored procedure name to be executed.</param>
        /// <param name="sqlParameterList">Parameters needed for the specified stored procedure to be executed.</param>
        /// <returns>Number of rows affected.</returns>
        public int ExecuteStoredProcedure(string storedProcedureName, SqlParameterList sqlParameterList)
        {
            try
            {
                string logMessage = string.Format("MsSqlServerImpl.ExecuteStoredProcedure(): Stored procedure = {0}", storedProcedureName);
                performanceTimer.Start(logMessage);
                using (SqlConnection sqlConnection = OpenConnection())
                {
                    using (SqlCommand sqlCommand = new SqlCommand(storedProcedureName, sqlConnection, null))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        logger.Debug(logMessage);
                        PopulateSqlParameters(sqlCommand, sqlParameterList);
                        int result = sqlCommand.ExecuteNonQuery();
                        performanceTimer.Stop();
                        return result;
                    }
                }
            }
            catch (Exception)
            {
                performanceTimer.Reset();
                throw;
            }
        }

        /// <summary>
        /// Executes the stored procedure specified by the name and the corresponding parameters and returns the result set.
        ///  This uses SqlDataAdapter.Fill() method.
        /// </summary>
        /// <param name="storedProcedureName">Stored procedure name to be executed.</param>
        /// <param name="sqlParameterList">Parameters needed for the specified stored procedure to be executed.</param>
        /// <param name="sqlResults">Returns the result set from the stored procedure.</param>
        /// <returns>Number of rows affected.</returns>
        public int ExecuteStoredProcedure(string storedProcedureName, SqlParameterList sqlParameterList, out IList<DatabaseRow> sqlResults)
        {
            sqlResults = new List<DatabaseRow>();

            try
            {
                string logMessage = string.Format("MsSqlServerImpl.ExecuteStoredProcedure(out IList<DatabaseRow): Stored procedure = {0}", storedProcedureName);
                performanceTimer.Start(logMessage);
                using (SqlConnection sqlConnection = OpenConnection())
                {
                    using (SqlCommand sqlCommand = new SqlCommand(storedProcedureName, sqlConnection, null))
                    {
                        DataTable dataTable = new DataTable();
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        logger.Debug(logMessage);
                        PopulateSqlParameters(sqlCommand, sqlParameterList);
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                        sqlDataAdapter.Fill(dataTable);

                        if (dataTable.Rows.Count <= 0)
                            return dataTable.Rows.Count;

                        foreach (DataRow dataTableRow in dataTable.Rows)
                        {
                            DatabaseRow databaseRow = new DatabaseRow();
                            foreach (DataColumn dataColumn in dataTable.Columns)
                            {
                                object o = dataTableRow[dataColumn];
                                if (o is DBNull)
                                {
                                    databaseRow.Columns.Add(dataColumn.ColumnName, new DatabaseColumn()
                                                                                       {
                                                                                           Name = dataColumn.ColumnName,
                                                                                           Value = null,
                                                                                           DataType = dataColumn.DataType
                                                                                       });
                                }
                                else
                                {
                                    databaseRow.Columns.Add(dataColumn.ColumnName, new DatabaseColumn()
                                                                                       {
                                                                                           Name = dataColumn.ColumnName,
                                                                                           Value = dataTableRow[dataColumn],
                                                                                           DataType = dataColumn.DataType
                                                                                       });
                                }
                            }

                            sqlResults.Add(databaseRow);
                        }
                    }
                }

                performanceTimer.Stop();
            }
            catch (Exception)
            {
                performanceTimer.Reset();
                throw;
            }

            return sqlResults.Count;
        }

        /// <summary>
        /// Executes the stored procedure specified by the name and the corresponding parameters and returns the first column of the first row.
        ///  This uses ExecuteScalar() method.
        /// </summary>
        /// <typeparam name="TFirstDbColumnType">Column type of the first database column in the table.</typeparam>
        /// <param name="storedProcedureName">Stored procedure name to be executed.</param>
        /// <param name="sqlParameterList">Parameters needed for the specified stored procedure to be executed.</param>
        /// <returns>Returns the first column of the first row.</returns>
        public TFirstDbColumnType ExecuteStoredProcedure<TFirstDbColumnType>(string storedProcedureName, SqlParameterList sqlParameterList)
        {
            try
            {
                string logMessage = string.Format("MsSqlServerImpl.ExecuteStoredProcedure<TFirstDbColumnType>(): Stored procedure = {0}", storedProcedureName);
                using (SqlConnection sqlConnection = OpenConnection())
                {
                    using (SqlCommand sqlCommand = new SqlCommand(storedProcedureName, sqlConnection, null))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        logger.Debug(logMessage);
                        PopulateSqlParameters(sqlCommand, sqlParameterList);
                        object resultObject = sqlCommand.ExecuteScalar();
                        if (resultObject == null)
                            return default(TFirstDbColumnType);

                        return (TFirstDbColumnType) resultObject;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
