// -----------------------------------------------------------------------
// <copyright file="IDatabaseContext.cs" company="XYZ">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SampleApp.Core.Context
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
using System.Data;
using System.Data.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IDatabaseContext
    {
        DbParameter CreateParameter(string name, object value);
        DbParameter CreateParameter(string name, object value, DbType dbType);
        int ExecuteCommand(string sql, CommandType commandType, params object[] parameters);
        object ExecuteScalar(string sql, CommandType commandType, params object[] parameters);
        int ExecuteCommandWithTimeout(string sql, CommandType commandType, int timeoutInSeconds, params object[] parameters);
        IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters);
        DataTable ExecuteReaderWithTimeout(string sql, CommandType commandType, int timeoutInSeconds, params object[] parameters);
        DataTable ExecuteReader(string sql, CommandType commandType, params object[] parameters);
        DataTable[] ExecuteReaderWithTimeoutForMultiResultSet(string sql, CommandType commandType, int timeoutInSeconds, params object[] parameters);
        DataTable[] ExecuteReaderForMultiResultSet(string sql, CommandType commandType, params object[] parameters);
    }
}
