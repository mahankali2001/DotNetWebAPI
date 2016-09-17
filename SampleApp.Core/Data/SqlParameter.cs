using System;

namespace SampleApp.Core.Data
{
    internal class SqlParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public SqlProperties.SqlParameterDirection Direction { get; set; }
        public Type DbType { get; set; }
    }
}
