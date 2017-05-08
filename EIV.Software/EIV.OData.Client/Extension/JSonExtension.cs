
namespace EIV.OData.Client.Extension
{
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;
    public static class JSonExtension
    {
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

        public static JObject DictionaryToJSon(this IDictionary<string, object> source)
        {
            if (source == null)
            {
                return null;
            }
            // try ... catch here!
            JObject content = JObject.FromObject(source);
            if (content != null)
            {
                return content;
            }

            return null;
        }

        public static string ArrayToJSon(this object source)
        {
            if (source == null)
            {
                return null;
            }
            // try ... catch here!
            JArray content = (JArray) JToken.FromObject(source);
            if (content != null)
            {
                return content.ToString();
            }

            return null;
        }

        // test this .....
        public static JObject ObjectToJSonObject(this object source)
        {
            if (source == null)
            {
                return null;
            }
            // try ... catch here!
            JObject content = (JObject) JToken.FromObject(source);

            return content;
        }
    }
}