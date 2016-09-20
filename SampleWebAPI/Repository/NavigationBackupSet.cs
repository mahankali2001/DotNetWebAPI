using System;
using System.Collections.Generic;
using System.Reflection;
//using System.Data.Metadata.Edm;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SampleWebAPI.Repository
{

    /// <summary>
    /// Helper used to back up and nullify the navigation properties of entities before performing an EF operation like Save or Delete
    /// The backed up properties are then restored after performing the EF operation
    /// </summary>
    /// <remarks>
    /// The navigation properties need to be nullified and backed for Add / Attach, as otherwise EF will attach the Navigation properties along with the main entity 
    /// The navigation properties need to be nullified and backed for Delete, as otherwise EF will set the Navigation properties to null on the main entity
    /// </remarks>
    internal static class NavigationBackup<T> where T : class
    {
        static private Type EntityType;
        static private List<PropertyInfo> PropertiesToBackup = new List<PropertyInfo>();

        static NavigationBackup()
        {
            EntityType = typeof(T);
            foreach (var propInfo in EntityType.GetProperties())
            {
                if (!propInfo.PropertyType.IsGenericType)
                {
                    if (!propInfo.PropertyType.IsClass || propInfo.PropertyType == typeof(string))
                    {
                        continue;
                    }

                    var notMappedAttributes = propInfo.GetCustomAttributes(typeof(NotMappedAttribute), true);
                    if (notMappedAttributes != null && notMappedAttributes.Length != 0)
                    {
                        continue;
                    }

                    var complexTypeAttributes = propInfo.PropertyType.GetCustomAttributes(typeof(ComplexTypeAttribute), true);
                    if (complexTypeAttributes != null && complexTypeAttributes.Length != 0)
                    {
                        continue;
                    }
                    PropertiesToBackup.Add(propInfo);
                }
                else if (propInfo.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>))
                {
                    PropertiesToBackup.Add(propInfo);
                }
            }
        }

        internal static NavigationBackupSet BackupAndReset(T entity)
        {
            if (entity == null || PropertiesToBackup.Count == 0)
            {
                return null;
            }
            object[] propertyValues = new object[PropertiesToBackup.Count];
            for (int index = 0; index < PropertiesToBackup.Count; index++)
            {
                var propInfo = PropertiesToBackup[index];
                propertyValues[index] = propInfo.GetValue(entity, null);
                propInfo.SetValue(entity, null, null);
            }
            return new NavigationBackupSet(propertyValues);
        }

        internal static void Restore(T entity, NavigationBackupSet backup)
        {
            if (entity == null || backup == null || backup.Values == null
                || backup.Values.Length == 0 || backup.Values.Length != PropertiesToBackup.Count)
            {
                return;
            }

            for (int index = 0; index < PropertiesToBackup.Count; index++)
            {
                var propInfo = PropertiesToBackup[index];
                propInfo.SetValue(entity, backup.Values[index], null);
            }
        }
    }

    internal class NavigationBackupSet
    {
        internal NavigationBackupSet(object[] values)
        {
            this.Values = values;
        }

        internal object[] Values { get; private set; }
    }
}
