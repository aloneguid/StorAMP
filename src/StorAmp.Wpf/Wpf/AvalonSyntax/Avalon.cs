using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using CloudExplorer.Wpf;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace StorAmp.Wpf.Wpf.AvalonSyntax
{
   static class Avalon
   {
      private static bool _initialised;

      public static void Init()
      {
         if(!_initialised)
         {
            _initialised = true;

            RegisterAvalonSchemas();
         }
      }

      private static void RegisterAvalonSchemas()
      {
         RegisterAvalonSchema("JSON", ".json");
         RegisterAvalonSchema("Kusto", ".kusto");
         RegisterAvalonSchema("INI", ".ini", ".inf", ".wer", ".dof");
         RegisterAvalonSchema("YAML", ".yml", ".yaml");
      }

      private static void RegisterAvalonSchema(string name, params string[] extension)
      {
         using(Stream s = typeof(App).Assembly.GetManifestResourceStream($"StorAmp.Wpf.Wpf.AvalonSyntax.{name.ToLower()}.xshd"))
         {
            using(var reader = new XmlTextReader(s))
            {
               IHighlightingDefinition hl = HighlightingLoader.Load(reader, HighlightingManager.Instance);

               HighlightingManager.Instance.RegisterHighlighting(name, extension, hl);
            }
         }

      }

   }
}
