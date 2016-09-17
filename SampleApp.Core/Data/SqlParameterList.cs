using System.Collections.Generic;

namespace SampleApp.Core.Data
{
    public class SqlParameterList
    {
        internal IList<SqlParameter> sqlParameters = new List<SqlParameter>();

        public void Add<TParameterType>(string parameterName, object parameterValue)
        {
            sqlParameters.Add(new SqlParameter() { Name = parameterName, Value = parameterValue, DbType = typeof(TParameterType) });
        }

        public void Add<TParameterType>(string parameterName, object parameterValue, SqlProperties.SqlParameterDirection parameterDirection)
        {
            sqlParameters.Add(new SqlParameter() { Name = parameterName, Value = parameterValue, Direction = parameterDirection, DbType = typeof(TParameterType) });
        }

    }
}
