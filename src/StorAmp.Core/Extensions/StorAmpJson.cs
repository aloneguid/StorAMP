using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace StorAmp.Core.Extensions
{
   public static class StorAmpJson
   {
      private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
      {
         Formatting = Formatting.Indented,
         SerializationBinder = new JsonTypeSerialisationBinder(),
         TypeNameHandling = TypeNameHandling.Auto,
         NullValueHandling = NullValueHandling.Ignore,
         DefaultValueHandling = DefaultValueHandling.Ignore
      };

      public static void WriteToFile(object value, string path)
      {
         string json = JsonConvert.SerializeObject(value, _settings);

         File.WriteAllText(path, json);
      }

      public static T ReadFromFile<T>(string path)
      {
         if(!File.Exists(path))
            return default;

         string json = File.ReadAllText(path);

         try
         {
            return JsonConvert.DeserializeObject<T>(json, _settings);
         }
         catch(JsonSerializationException)
         {
            return default;
         }
      }
   }
}
