using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Newtonsoft.Json;

namespace StorAmp.Wpf.Converters
{
   public class VsCodeIconThemeConverter : IValueConverter
   {
      public class VsCodeEntry
      {
         public string name { get; set; }
         public string[] fileExtensions { get; set; }
         public string[] fileNames { get; set; }
      }

      private static readonly Dictionary<string, VsCodeEntry> _fileNameToVsCodeEntry =
         new Dictionary<string, VsCodeEntry>(StringComparer.OrdinalIgnoreCase);
      private static readonly Dictionary<string, VsCodeEntry> _extToVsCodeEntry =
         new Dictionary<string, VsCodeEntry>(StringComparer.OrdinalIgnoreCase);

      public VsCodeIconThemeConverter()
      {
         ReadVsCodeIconMap();
      }

      private static void ReadVsCodeIconMap()
      {
         const string resName = "StorAmp.Wpf.Media.vscode.ext.json";

         string json;
         using(Stream s = typeof(VsCodeIconThemeConverter).Assembly.GetManifestResourceStream(resName))
         {
            using(var r = new StreamReader(s))
            {
               json = r.ReadToEnd();
            }
         }

         //extensions have no dots in them
         VsCodeEntry[] entries = JsonConvert.DeserializeObject<VsCodeEntry[]>(json);
         foreach(VsCodeEntry entry in entries)
         {
            if(entry.fileNames != null)
            {
               foreach(string fn in entry.fileNames)
               {
                  _fileNameToVsCodeEntry[fn] = entry;
               }
            }

            if(entry.fileExtensions != null)
            {
               foreach(string extNoDot in entry.fileExtensions)
               {
                  _extToVsCodeEntry["." + extNoDot] = entry;
               }
            }
         }
      }

      public object Convert(object value, Type targetType, object parameter, string culture)
      {
         if(value == null)
            return null;

         if(targetType == typeof(ImageSource))
         {
            string name = value.ToString();

            if(_fileNameToVsCodeEntry.TryGetValue(name, out VsCodeEntry entry) ||
               _extToVsCodeEntry.TryGetValue(Path.GetExtension(name), out entry))
            {
               return $"ext/{entry.name}".GetImageSource();
            }
         }

         return null;
      }

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
   }

}
