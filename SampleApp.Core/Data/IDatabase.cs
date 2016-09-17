using System.Collections.Generic;

namespace SampleApp.Core.Data
{
    public interface IDatabase
    {
        /// <summary>
        /// Executes the specified SQL statement and returns the number of rows affected.
        /// </summary>
        /// <param name="sqlQuery">SQL statement to execute.</param>
        /// <returns>Number of rows affected.</returns>
        int ExecuteSql(string sqlQuery);

        /// <summary>
        /// Executes the specified SQL statement and returns the first column of the first row.
        /// </summary>
        /// <typeparam name="TFirstDbColumnType">Column type of the first database column in the table.</typeparam>
        /// <param name="sqlQuery">SQL statement to execute.</param>
        /// <returns>Returns the first column of the first row.</returns>
        TFirstDbColumnType ExecuteSql<TFirstDbColumnType>(string sqlQuery);

        /// <summary>
        /// Executes the specified SQL statement and returns the result set.
        /// </summary>
        /// <param name="sqlQuery">SQL statement to execute.</param>
        /// <param name="sqlResults">Returns the result set from the SQL.</param>
        /// <returns>Number of rows affected.</returns>
        int ExecuteSql(string sqlQuery, out IList<DatabaseRow> sqlResults);

        /// <summary>
        /// Executes the stored procedure specified by the name and the corresponding parameters and returns the number of rows affected.
        /// </summary>
        /// <param name="storedProcedureName">Stored procedure name to be executed.</param>
        /// <param name="sqlParameterList">Parameters needed for the specified stored procedure to be executed.</param>
        /// <returns>Number of rows affected.</returns>
        int ExecuteStoredProcedure(string storedProcedureName, SqlParameterList sqlParameterList);

        /// <summary>
        /// Executes the stored procedure specified by the name and the corresponding parameters and returns the result set.
        /// </summary>
        /// <param name="storedProcedureName">Stored procedure name to be executed.</param>
        /// <param name="sqlParameterList">Parameters needed for the specified stored procedure to be executed.</param>
        /// <param name="sqlResults">Returns the result set from the stored procedure.</param>
        /// <returns>Number of rows affected.</returns>
        int ExecuteStoredProcedure(string storedProcedureName, SqlParameterList sqlParameterList, out IList<DatabaseRow> sqlResults);

        /// <summary>
        /// Executes the stored procedure specified by the name and the corresponding parameters and returns the first column of the first row.
        /// </summary>
        /// <typeparam name="TFirstDbColumnType">Column type of the first database column in the table.</typeparam>
        /// <param name="storedProcedureName">Stored procedure name to be executed.</param>
        /// <param name="sqlParameterList">Parameters needed for the specified stored procedure to be executed.</param>
        /// <returns>Returns the first column of the first row.</returns>
        TFirstDbColumnType ExecuteStoredProcedure<TFirstDbColumnType>(string storedProcedureName, SqlParameterList sqlParameterList);

    }
}
