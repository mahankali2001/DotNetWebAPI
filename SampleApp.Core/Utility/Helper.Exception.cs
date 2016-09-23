using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;

namespace SampleApp.Core.Utility
{
    public static class Helper
    {
        internal static void IffFalse(bool condition, Action falseAction)
        {
            if (!condition)
            {
                falseAction();
            }
        }

        internal static void IffTrue(bool condition, Action trueAction)
        {
            if (condition)
            {
                trueAction();
            }
        }

        internal static bool IsValidId(int? id)
        {
            return (id != null) && (id.Value > 0);
        }

        internal static bool IsValidId(int id)
        {
            return id > 0;
        }

        internal static bool IsValidId(long id)
        {
            return id > 0;
        }

    }
}