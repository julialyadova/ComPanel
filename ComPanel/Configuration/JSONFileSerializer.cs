using System;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace ComPanel.Configuration
{
    public class JSONFileSerializer
    {
        public static void Save<T>(string path, T configuration)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(configuration));
        }

        public static T Load<T>(string path)
        {
            if (!File.Exists(path))
                return default;

            T obj = default;
            try
            {
                obj = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            }
            catch (Exception) { }
            return obj;
        }
    }
}
