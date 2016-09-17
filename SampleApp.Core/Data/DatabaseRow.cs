using System;
using System.Collections.Generic;

namespace SampleApp.Core.Data
{
    public class DatabaseRow
    {
        private IDictionary<string, DatabaseColumn> columns = new Dictionary<string, DatabaseColumn>();
        public IDictionary<string, DatabaseColumn> Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        public TColumnType GetColumnValue<TColumnType>(string columnName)
        {
            if (Columns == null)
                return default(TColumnType);

            if (Columns.ContainsKey(columnName) == false)
                return default(TColumnType);

            DatabaseColumn dbColumn = Columns[columnName];

            if (Convert.IsDBNull(dbColumn.Value))
                return default(TColumnType);

            if (dbColumn.Value == null)
                return default(TColumnType);

           // return (TColumnType)dbColumn.Value;
            return (TColumnType)Convert.ChangeType(dbColumn.Value, typeof(TColumnType));
        }
    }
}
