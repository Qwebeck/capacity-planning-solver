using System;
using System.IO;
using Newtonsoft.Json;

namespace Tools.Utils
{
    public static class JsonUtils
    {
        public static T LoadJson<T>(string serializedModel)
        {
            using var r = new StreamReader(serializedModel);
            var json = r.ReadToEnd();
            return JsonConvert.DeserializeObject<T>(json) ?? throw new InvalidOperationException();
        }
    }
}