using System;
using System.Collections.Generic;
using System.Linq;
using SampleApp.Core.Entities.Common;

namespace SampleApp.Core.Utility
{
    public static class Pager<T> where T : class 
    {
        //Page index should be 1 to fetch the first page 
        //Page size should be the size of the page to be retrieved.

        public static PagedResult<T> GetResult(IQueryable<T> query, int pageIndex, int pageSize)
        {
            var result = new PagedResult<T> { CurrentPage = pageIndex, PageSize = pageSize, RowCount = query.Count() };
            double pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);
            int skip = (pageIndex - 1) * pageSize;
            result.Results = query.Skip(skip).Take(pageSize).ToList();
            return result;
        }

        public static PagedResult<T> GetResult(IEnumerable<T> query, int pageIndex, int pageSize)
        {
            var result = new PagedResult<T> { CurrentPage = pageIndex, PageSize = pageSize, RowCount = query.Count() };
            double pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);
            int skip = (pageIndex - 1) * pageSize;
            result.Results = query.Skip(skip).Take(pageSize).ToList();
            return result;
        }

        public static Func<T, object> GetOrderByExpression(string sortColumn)
        {
            Func<T, object> orderByExpr = null;
            if (!String.IsNullOrEmpty(sortColumn))
            {
                Type sponsorResultType = typeof(T);

                if (sponsorResultType.GetProperties().Any(prop => prop.Name == sortColumn))
                {
                    System.Reflection.PropertyInfo pinfo = sponsorResultType.GetProperty(sortColumn);
                    orderByExpr = (data => pinfo.GetValue(data, null));
                }
            }
            return orderByExpr;
        }

        public static IEnumerable<T> OrderByDir<T>(IEnumerable<T> source, string dir, Func<T, object> orderByColumn)
        {
            return dir.ToUpper().Equals("ASCENDING") ? source.OrderBy(orderByColumn) : source.OrderByDescending(orderByColumn);
        }

    }
}