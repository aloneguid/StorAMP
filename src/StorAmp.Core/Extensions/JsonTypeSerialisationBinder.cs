using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Serialization;
using StorAmp.Core.Model;

namespace StorAmp.Core.Extensions
{
   class JsonTypeSerialisationBinder : DefaultSerializationBinder
   {
      private static readonly Dictionary<Type, string> _typeToName = new Dictionary<Type, string>();
      private static readonly Dictionary<string, Type> _nameToType = new Dictionary<string, Type>();

      static JsonTypeSerialisationBinder()
      {
         Add<ConnectedAccount>("account");
         Add<ConnectedFolder>("folder");
      }

      private static void Add<T>(string name)
      {
         _typeToName[typeof(T)] = name;
         _nameToType[name] = typeof(T);
      }

      public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
      {
         if(_typeToName.TryGetValue(serializedType, out string name))
         {
            typeName = name;
            assemblyName = null;
         }
         else
         {
            base.BindToName(serializedType, out assemblyName, out typeName);
         }
      }

      public override Type BindToType(string assemblyName, string typeName)
      {
         if(_nameToType.TryGetValue(typeName, out Type t))
            return t;

         return base.BindToType(assemblyName, typeName);
      }
   }
}
