using System.Windows;
using NetBox.Extensions;
using StorAmp.Core.Services;
using System.Collections.Generic;
using MahApps.Metro;
using System.Linq;
using ColorScheme = StorAmp.Core.Model.ColorScheme;
using System.Windows.Media;
using System.IO;
using Config.Net;
using StorAmp.Core;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using StorAmp.Wpf;

namespace CloudExplorer.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IThemeManagerService
   {
      public App()
      {
         ServiceLocator.Register<IThemeManagerService>(this);

         GlobalInit.Initialise();

         GlobalShared.Initialise(
            typeof(App).FileVersion().ToString(),
            "7b123eae-58de-4ed1-a355-73b973beb88b");

         //init config
         ICloudExplorerSettings config = new ConfigurationBuilder<ICloudExplorerSettings>()
            .UseIniFile(GlobalSettings.GetSettingsFilePath())
            .Build();
         GlobalSettings.Initialise(config);

         RegisterAvalonSchemas();
      }

      public void ApplyColorScheme(string baseColorName, ColorScheme colorScheme)
      {
         ThemeManager.ChangeThemeBaseColor(Application.Current, baseColorName);
         ThemeManager.ChangeThemeColorScheme(Application.Current, colorScheme.Name);
      }

      public IEnumerable<string> GetBaseColorNames()
      {
         return ThemeManager.BaseColors.ToList();
      }

      public IEnumerable<ColorScheme> GetColorSchemes()
      {
         return ThemeManager
            .ColorSchemes
            .Select(cs => new ColorScheme { Name = cs.Name, Color = ((SolidColorBrush)cs.ShowcaseBrush).Color.ToString() })
            .ToList();
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
