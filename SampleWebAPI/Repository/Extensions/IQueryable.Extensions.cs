using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace SampleWebAPI.Repository.Extenstion
{
    internal static class IQueryableExtension
    {
        internal static IQueryable<T> Includes<T>(this IQueryable<T> queryable, ICollection<string> includes) where T : class
        {
            if (includes == null || includes.Count == 0)
            {
                return queryable;
            }

            foreach (var include in includes)
            {
                if (string.IsNullOrEmpty(include))
                {
                    continue;
                }

                queryable = queryable.Include(include);
            }

            return queryable;
        }
    }
}
