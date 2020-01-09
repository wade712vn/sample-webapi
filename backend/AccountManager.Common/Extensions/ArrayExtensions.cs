﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AccountManager.Common.Extensions
{
    public static class ArrayExtensions
    {
        public static string Serialize<T>(this T[] array)
        {
            try
            {
                if (array == null) throw new ArgumentNullException();
                return JsonConvert.SerializeObject(array).Replace("[", "{").Replace("]", "}").Replace("\"", string.Empty);
            }
            catch
            {
                return null;
            }
        }
    }
}
