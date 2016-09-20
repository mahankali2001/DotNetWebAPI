using System;
using System.Collections.Generic;
using System.Linq;
using SampleWebAPI.Entities.Common;

namespace SampleWebAPI.Utility
{
    internal static class Pager<T> where T : class 
    {
        //Page index should be 1 to fetch the first page 
        //Page size should be the size of the page to be retrieved.

        internal static PagedResult<T> GetResult(IQueryable<T> query, int pageIndex, int pageSize)
        {
            var result = new PagedResult<T> {CurrentPage = pageIndex, PageSize = pageSize, RowCount = query.Count()};
            double pageCount = (double) result.RowCount/pageSize;
            result.PageCount = (int) Math.Ceiling(pageCount);
            int skip = (pageIndex - 1)*pageSize;
            result.Results = query.Skip(skip).Take(pageSize).ToList();
            return result;
        }

        internal static PagedResult<T> GetResult(IEnumerable<T> query, int pageIndex, int pageSize)
        {
            var result = new PagedResult<T> { CurrentPage = pageIndex, PageSize = pageSize, RowCount = query.Count() };
            double pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);
            int skip = (pageIndex - 1) * pageSize;
            result.Results = query.Skip(skip).Take(pageSize).ToList();
            return result;
        }

    }
}