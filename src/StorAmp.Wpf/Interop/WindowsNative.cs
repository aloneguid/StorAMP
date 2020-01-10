using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Win32;

namespace CloudExplorer.Wpf.Interop
{
   public static class WindowsNative
   {
      private static readonly Dictionary<string, bool> _extToHas = new Dictionary<string, bool>();

      public static bool HasAssociatedProgram(string fileName)
      {
         string extension = Path.GetExtension(fileName);

         if(_extToHas.TryGetValue(extension, out bool has))
            return has;

         using(RegistryKey cr = Registry.ClassesRoot)
         {
            using(RegistryKey sub = cr.OpenSubKey(extension))
            {
               has = sub != null;
               _extToHas[extension] = has;
               return has;
            }
         }
      }
   }
}
