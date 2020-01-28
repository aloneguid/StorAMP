using System.Windows;
using NetBox.Extensions;
using StorAmp.Core.Services;
using System.Collections.Generic;
using MahApps.Metro;
using System.Linq;
using ColorScheme = StorAmp.Core.Model.ColorScheme;
using System.Windows.Media;
using Config.Net;
using StorAmp.Core;
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


   }
}
