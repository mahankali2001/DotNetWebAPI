using System;

namespace SampleApp.Core.Data
{
    public interface IModel
    {
        int Id { get; set; } // maps to auto incremented id.

        DateTime CreateDateTime { get; set; } // maps to CreateDateTime.

        DateTime ChangedDateTime { get; set; } // maps to ChangedDateTime.

        int ChangedById { get; set; } // maps to ChangedById.

        bool IsFilled { get; set; }

        bool IsValidated { get; set; }
    }
}
