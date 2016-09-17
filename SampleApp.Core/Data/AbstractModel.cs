using System;

namespace SampleApp.Core.Data
{
    [Serializable]
    public abstract class AbstractModel : IModel
    {
        protected AbstractModel()
        {
            IsFilled = false;
            IsValidated = false;
        }

        protected TType GetValueOrDefault<TType>(TType typeValue) where TType : class
        {
            //if (typeof(TType).IsGenericType && typeof(TType).GetGenericTypeDefinition() == typeof(Nullable<>) && Equals(typeValue, null))
            if (Equals(typeValue, null))
                return default(TType);

            return typeValue;
        }

        public virtual IModel CloneMembers()
        {
            return MemberwiseClone() as IModel;
        }

        #region IModel Members

        public virtual int Id { get; set; } // maps to auto incremented id.

        public virtual DateTime CreateDateTime { get; set; } // maps to CreateDateTime.

        public virtual DateTime ChangedDateTime { get; set; } // maps to ChangedDateTime.

        public virtual int ChangedById { get; set; } // maps to ChangedById.

        public virtual bool IsFilled { get; set; }

        public virtual bool IsValidated { get; set; }

        #endregion

    }
}
