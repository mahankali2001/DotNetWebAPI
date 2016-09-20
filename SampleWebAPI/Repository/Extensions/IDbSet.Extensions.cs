using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace SampleWebAPI.Repository.Extenstion
{
    internal static class IDbSetExtension
    {
        private static readonly DateTime SqlMinDate = new DateTime(1753, 1, 1, 0, 0, 0, 0);
        public static object GetDateOrDbNull(this DateTime date)
        {
            return date == DateTime.MinValue || date == SqlMinDate ? DBNull.Value : (object)date;
        }

        public static DateTime? GetNullableDate(this DateTime date)
        {
            return date == DateTime.MinValue || date == SqlMinDate ? (DateTime?)null : date;
        }
        public static DataTable ToIdTable(this IEnumerable<int> listOfIds)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(int));

            if (listOfIds == null)
            {
                return table;
            }

            foreach (int id in listOfIds)
            {
                DataRow row = table.NewRow();
                row[0] = id;
                table.Rows.Add(row);
            }

            return table;
        }
        public static DataTable ToIdTable(this IEnumerable<long> listOfIds)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(int));  // todo: need to change to long after adding the big int datatype

            if (listOfIds == null)
            {
                return table;
            }

            foreach (int id in listOfIds)
            {
                DataRow row = table.NewRow();
                row[0] = id;
                table.Rows.Add(row);
            }

            return table;
        }


    }
}
