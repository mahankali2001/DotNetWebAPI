using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace SampleApp.Core.Context
{
    public class DatabaseContext : IDatabaseContext
    {
        private static readonly int DEFAULT_COMMAND_TIME_OUT = 500;
        private RepositoryContext context;

        public DatabaseContext(RepositoryContext context)
        {
            this.context = context;
        }
        public DbParameter CreateParameter(string name, object value)
        {
            var param =  new SqlParameter(name, value);

            if (value is DataTable)
            {
                param.SqlDbType = SqlDbType.Structured;
            }

            return param;
        }
        public DbParameter CreateParameter(string name, object value, DbType dbType)
        {
            return new SqlParameter(name, value) { DbType = dbType };
        }

        public object ExecuteScalar(string sql, CommandType commandType, params object[] parameters)
        {
            int currentTimeout = ((IObjectContextAdapter)context).ObjectContext.CommandTimeout ?? DEFAULT_COMMAND_TIME_OUT;
            using (DbCommand command = this.context.Database.Connection.CreateCommand())
            {
                DbConnection connection = ((IObjectContextAdapter)this.context).ObjectContext.Connection;
                try
                {

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }
                    else
                    {
                        connection = null;
                    }

                    command.CommandTimeout = currentTimeout;
                    command.CommandText = sql;
                    command.CommandType = commandType;
                    command.Parameters.AddRange(parameters);
                    return command.ExecuteScalar();
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public int ExecuteCommand(string sql, CommandType commandType, params object[] parameters)
        {
            int currentTimeout = ((IObjectContextAdapter)context).ObjectContext.CommandTimeout ?? DEFAULT_COMMAND_TIME_OUT;
            return this.ExecuteCommandWithTimeout(sql, commandType, currentTimeout, parameters);
        }
        public int ExecuteCommandWithTimeout(string sql, CommandType commandType, int timeoutInSeconds, params object[] parameters)
        {
            using (DbCommand command = this.context.Database.Connection.CreateCommand())
            {
                DbConnection connection = ((IObjectContextAdapter)this.context).ObjectContext.Connection;
                try
                {

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }
                    else
                    {
                        connection = null;
                    }

                    command.CommandTimeout = timeoutInSeconds;
                    command.CommandText = sql;
                    command.CommandType = commandType;
                    command.Parameters.AddRange(parameters);
                    return command.ExecuteNonQuery();
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
        }
        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return this.context.Database.SqlQuery<TElement>(sql, parameters);
        }
        public DataTable ExecuteReader(string sql, CommandType commandType, params object[] parameters)
        {
            int currentTimeout = ((IObjectContextAdapter)context).ObjectContext.CommandTimeout ?? DEFAULT_COMMAND_TIME_OUT;
            return this.ExecuteReaderWithTimeout(sql, commandType, currentTimeout, parameters);
        }
        public DataTable ExecuteReaderWithTimeout(string sql, CommandType commandType, int timeoutInSeconds, params object[] parameters)
        {
            DataTable [] tables = this.ExecuteReaderWithTimeoutForMultiResultSet(sql, commandType, timeoutInSeconds, parameters);
            return tables.Length > 0 ? tables[0] : null;
        }
        public DataTable[] ExecuteReaderWithTimeoutForMultiResultSet(string sql, CommandType commandType, int timeoutInSeconds, params object[] parameters)
        {
            using (DbCommand command = this.context.Database.Connection.CreateCommand())
            {
                DbConnection connection = ((IObjectContextAdapter)this.context).ObjectContext.Connection;
                try
                {

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }
                    else
                    {
                        connection = null;
                    }

                    command.CommandTimeout = timeoutInSeconds;
                    command.CommandText = sql;
                    command.CommandType = commandType;
                    if(parameters!=null && parameters.Length>0)
                        command.Parameters.AddRange(parameters);

                    List<DataTable> tables = new List<DataTable>();
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        do
                        {
                            DataTable table = new DataTable();
                            table.Load(reader);
                            tables.Add(table);
                        } while (!reader.IsClosed);
                        
                    }
                    return tables.ToArray();
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
        }
        public DataTable[] ExecuteReaderForMultiResultSet(string sql, CommandType commandType, params object[] parameters)
        {
            int currentTimeout = ((IObjectContextAdapter)context).ObjectContext.CommandTimeout ?? DEFAULT_COMMAND_TIME_OUT;
            return this.ExecuteReaderWithTimeoutForMultiResultSet(sql, commandType, currentTimeout, parameters);
        }

    }
}