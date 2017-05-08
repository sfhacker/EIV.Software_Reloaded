

namespace EIV.Demo.WebService.Filters
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    public static class Extensions
    {
        public static IEnumerable<T> SelectManyRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            var result = source.SelectMany(selector);
            if (!result.Any())
            {
                return result;
            }
            return result.Concat(result.SelectManyRecursive(selector));
        }

        public static string ObjectToJSon(this object source)
        {
            if (source == null)
            {
                return null;
            }
            // try ... catch here!
            JObject content = JObject.FromObject(source);
            if (content != null)
            {
                return content.ToString();
            }

            return null;
        }

        public static object ObjectToJSonObject(this object source)
        {
            if (source == null)
            {
                return null;
            }
            // try ... catch here!
            JObject content = (JObject) JToken.FromObject(source);

            return content;
        }

        public static ObservableCollection<T> JSonToCollection<T>(this string source) where T : class
        {
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }

            JArray content = (JArray) JToken.Parse(source);

            var result = content.ToObject<T[]>();

            return new ObservableCollection<T>(result);
        }
    }
}