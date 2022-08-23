using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace KAP.ToolCreateMap
{
    public class KDLUtils
    {
        #region Enum

        /// <summary>
        /// loop all values in enum
        /// </summary>
        public static void ForeachEnum<T>(Action<T> action) where T : Enum
        {
            foreach (T enumValue in Enum.GetValues(typeof(T)))
            {
                action?.Invoke(enumValue);
            }
        }

        public static int GetEnumLength<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Length;
        }

        public static T ParseEnum<T>(string strValue) where T : Enum
        {
            if (string.IsNullOrEmpty(strValue))
            {
                var defaultValue = (T)((object)0);
                return (T)defaultValue;
            }

            try
            {
                return (T)Enum.Parse(typeof(T), strValue, true);
            }
            catch (Exception)
            {
                Debug.LogError(string.Format("Something Wrong!!! Cannot parse enum {0}: {1}", typeof(T).ToString(), strValue));

                var defaultValue = (T)((object)0);
                return defaultValue;
            }
        }

        #endregion
    }
}

