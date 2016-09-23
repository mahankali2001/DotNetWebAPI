using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleApp.Core.Entities.Common
{
        public class PagedResult<T> : IEnumerable where T : class
        {
            //These are the main results.
            public IList<T> Results { get; set; }

            //Curent page number 
            public int CurrentPage { get; set; }

            //Total number of pages 
            public int PageCount { get; set; }

            //Size of each page
            public int PageSize { get; set; }

            //Number of rows returned.
            public int RowCount { get; set; }

            public IEnumerator GetEnumerator()
            {
                //if (Results != null)
                return Results.GetEnumerator();
            }
        }
}
