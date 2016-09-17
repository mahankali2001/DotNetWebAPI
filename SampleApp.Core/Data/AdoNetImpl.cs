﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SampleApp.Core.Logger;

namespace SampleApp.Core.Data
{
    public class AdoNetImpl
    {
        #region Members

        private static AdoNetImpl instance;
        private static readonly ILogger logger = LogManager.GetLogger(typeof(AdoNetImpl));
        private static readonly PerformanceTimer performanceTimer = new PerformanceTimer();

        public delegate TModel CreateModelInstanceDelegate<TModel>(DatabaseRow databaseRow);

        #endregion

        #region Constructors

        protected AdoNetImpl()
        {
            Database = MsSqlServerImpl.Instance;
        }

        #endregion

        #region Static methods

        public static AdoNetImpl Instance
        {
            get
            {
                if (instance == null)
                    instance = new AdoNetImpl();

                return instance;
            }
        }

        #endregion

        #region Properties

        public IDatabase Database { get; private set; }

        #endregion

        #region Private methods

        private static IList<TModel> CreateModelListFromSqlResults<TModel>(IList<DatabaseRow> sqlResults, CreateModelInstanceDelegate<TModel> createModelInstanceDelegate) where TModel : IModel
        {
            IList<TModel> models = new List<TModel>();

            if (sqlResults.Count <= 0)
                return models;

            if (createModelInstanceDelegate == null)
                return models;

            foreach (DatabaseRow databaseRow in sqlResults)
            {
                TModel entityModelInstance = createModelInstanceDelegate.Invoke(databaseRow);
                entityModelInstance.IsFilled = true;
                models.Add(entityModelInstance);
            }

            return models;
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Executes the specified SQL statement and returns a list of models.
        /// </summary>
        /// <typeparam name="TModel">Model that needs to be populated from the result set. The model implements the IModel interface.</typeparam>
        /// <param name="sqlStatement">SQL statement to execute.</param>
        /// <param name="createModelInstanceDelegate">Delegate method to map the database result set into the corresponding model. In case of entity models this delegate is always CreateEntityModelInstance.</param>
        /// <returns>A list of models. The list is empty if no results are found.</returns>
        protected IList<TModel> ExecuteSql<TModel>(string sqlStatement, CreateModelInstanceDelegate<TModel> createModelInstanceDelegate) where TModel : IModel
        {
            IList<DatabaseRow> sqlResults;
            Database.ExecuteSql(sqlStatement, out sqlResults);
            return CreateModelListFromSqlResults(sqlResults, createModelInstanceDelegate);
        }

        /// <summary>
        /// Executes the stored procedure specified by the name and the corresponding parameters and returns the number of rows affected.
        /// It also returns the generated id into the "generatedId" out field.
        /// </summary>
        /// <param name="storedProcedureName">Stored procedure name to be executed.</param>
        /// <param name="sqlParameterList">Parameters needed for the specified stored procedure to be executed.</param>
        /// <param name="generatedId">Returns the generated id.</param>
        /// <returns>Number of rows affected.</returns>
        protected int ExecuteInsertStoredProcedure(string storedProcedureName, SqlParameterList sqlParameterList, out int generatedId)
        {
            try
            {
                string logMessage = string.Format("AdoNetImpl.ExecuteInsertStoredProcedure(): Stored procedure = {0}", storedProcedureName);
                performanceTimer.Start(logMessage);
                using (SqlConnection sqlConnection = ((MsSqlServerImpl)Database).OpenConnection())
                {
                    using (SqlCommand sqlCommand = new SqlCommand(storedProcedureName, sqlConnection, null))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        logger.Debug(logMessage);
                        ((MsSqlServerImpl)Database).PopulateSqlParameters(sqlCommand, sqlParameterList);
                        int numberOfRowsAffected = sqlCommand.ExecuteNonQuery();
                        // Populate the returned identity id generated by the SQL execution.
                        generatedId = (int)sqlCommand.Parameters[SqlProperties.SP_PARAMETER_RETURN_VALUE_NAME].Value;
                        performanceTimer.Stop();
                        return numberOfRowsAffected;
                    }
                }
            }
            catch (Exception)
            {
                performanceTimer.Reset();
                throw;
            }
        }

        //[Obsolete("This method is for backwards compatibility only to invoke the existing stored procedures with the naming conventions as <EntityModel>_Exists.", false)]
        ///// <summary>
        ///// This method is for backwards compatibility only to invoke the existing stored procedures with the naming conventions as &lt;EntityModel&gt;_Exists.
        ///// </summary>
        ///// <param name="storedProcedureName">Stored procedure name to be executed.</param>
        ///// <param name="sqlParameters">Parameters needed for the specified stored procedure to be executed.</param>
        ///// <returns>True if the row exists else returns a false.</returns>
        //public bool ExecuteExistsStoredProcedure(string storedProcedureName, SqlParameterList sqlParameterList)
        //{
        //    try
        //    {
        //        string logMessage = string.Format("AbstractEntityDataAccessAdoNet.ExecuteExistsStoredProcedure(): Stored procedure = {0}", storedProcedureName);
        //        performanceTimer.Start(logMessage);
        //        using (SqlConnection sqlConnection = ((MsSqlServerImpl)Database).OpenConnection())
        //        {
        //            using (SqlCommand sqlCommand = new SqlCommand(storedProcedureName, sqlConnection, null))
        //            {
        //                DataTable dataTable = new DataTable();
        //                sqlCommand.CommandType = CommandType.StoredProcedure;
        //                logger.Debug(logMessage);
        //                ((MsSqlServerImpl)Database).PopulateSqlParameters(sqlCommand, sqlParameterList);
        //                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
        //                sqlDataAdapter.Fill(dataTable);
        //                if (dataTable.Rows.Count > 0)
        //                {
        //                    performanceTimer.Stop();
        //                    return true;
        //                }

        //                performanceTimer.Stop();
        //                return false;
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        performanceTimer.Reset();
        //        throw;
        //    }
        //}

        #endregion

        #region Public methods

        /// <summary>
        /// Executes the stored procedure specified by the name and the corresponding parameters and returns a list of models.
        /// </summary>
        /// <typeparam name="TModel">Model that needs to be populated from the result set. The model implements the IModel interface.</typeparam>
        /// <param name="storedProcedureName">Stored procedure name to be executed.</param>
        /// <param name="sqlParameterList">Parameters needed for the specified stored procedure to be executed.</param>
        /// <param name="createModelInstanceDelegate">Delegate method to map the database result set into the corresponding model. In case of entity models this delegate is always CreateEntityModelInstance.</param>
        /// <returns>A list of models. The list is empty if no results are found.</returns>
        public IList<TModel> ExecuteStoredProcedure<TModel>(string storedProcedureName, SqlParameterList sqlParameterList, CreateModelInstanceDelegate<TModel> createModelInstanceDelegate) where TModel : IModel
        {
            IList<DatabaseRow> sqlResults;
            Database.ExecuteStoredProcedure(storedProcedureName, sqlParameterList, out sqlResults);
            return CreateModelListFromSqlResults(sqlResults, createModelInstanceDelegate);
        }

        /// <summary>
        /// Executes the stored procedure specified by the name and the corresponding parameters and returns the number of rows affected.
        /// </summary>
        /// <param name="storedProcedureName">Stored procedure name to be executed.</param>
        /// <param name="sqlParameterList">Parameters needed for the specified stored procedure to be executed.</param>
        /// <returns>Number of rows affected.</returns>
        public int ExecuteStoredProcedure(string storedProcedureName, SqlParameterList sqlParameterList)
        {
            return Database.ExecuteStoredProcedure(storedProcedureName, sqlParameterList);
        }

        /// <summary>
        /// Executes the stored procedure specified by the name and the corresponding parameters and returns the number of rows affected.
        /// It also populates the generated id into the entity model "Id" field.
        /// </summary>
        /// <param name="storedProcedureName">Stored procedure name to be executed.</param>
        /// <param name="sqlParameterList">Parameters needed for the specified stored procedure to be executed.</param>
        /// <param name="model">Model that implements the IEntityModel interface. The "Id" field in the model will be populdated with the generated id.</param>
        /// <returns>Number of rows affected.</returns>
        public int ExecuteInsertStoredProcedure<TModel>(string storedProcedureName, SqlParameterList sqlParameterList, TModel model) where TModel : IModel
        {
            int generatedId;
            int numberOfRowsAffected = ExecuteInsertStoredProcedure(storedProcedureName, sqlParameterList, out generatedId);

            if (numberOfRowsAffected > 0)
                model.Id = generatedId;

            return numberOfRowsAffected;
        }

        #endregion
    }
}