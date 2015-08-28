using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;

namespace MahjongBuddy.Extensions
{
    public static class MyExtensions
    {
        static readonly Random Random = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                var k = Random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T DeepCLone<T>(this T obj)
        {
            T newObj = Activator.CreateInstance<T>();

            foreach (PropertyInfo i in newObj.GetType().GetProperties())
            {
                //"EntitySet" is specific to link and this conditional logic is optional/can be ignored
                if (i.CanWrite && i.PropertyType.Name.Contains("EntitySet") == false)
                {
                    object value = obj.GetType().GetProperty(i.Name).GetValue(obj, null);
                    i.SetValue(newObj, value, null);
                }
            }

            return newObj;
        }
    }    
}