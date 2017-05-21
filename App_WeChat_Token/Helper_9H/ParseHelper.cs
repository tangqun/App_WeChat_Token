using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Helper_9H
{
    public static class ParseHelper
    {
        public static long ToLong(this object val, long defVal = default(long))
        {
            return val.To<long>(defVal);
        }

        public static DateTime ToDateTime(this object val, DateTime defVal = default(DateTime))
        {
            return val.To<DateTime>(defVal);
        }

        public static int ToInt(this object val, int defVal = default(int))
        {
            return val.To<int>(defVal);
        }
        
        public static T To<T>(this object val, T defVal = default(T))
        {
            if (val == null)
                return (T)defVal;
            if (val is T)
                return (T)val;

            Type type = typeof(T);
            try
            {
                if (type.BaseType == typeof(Enum))
                {
                    return (T)Enum.Parse(type, val.ToString(), true);
                }
                else
                {
                    return (T)Convert.ChangeType(val, type);
                }
            }
            catch
            {
                return defVal;
            }
        }
    }
}
