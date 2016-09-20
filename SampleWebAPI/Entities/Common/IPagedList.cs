using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleWebAPI.Entities.Common
{
    public interface IPagedList
    {
        int TotalCount { get; }
        int PageCount { get; }
        int Page { get; }
        int PageSize { get; }
    }
}
