using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using StorAmp.Core.Model;
using StorAmp.Core.Services;
using NetBox.Extensions;
using Serilog;
using GalaSoft.MvvmLight;

namespace StorAmp.Core.ViewModel
{
   public class SettingsViewModel : ViewModelBase
   {

      public SettingsViewModel()
      {
         if(ThemeManager != null)
         {
            BaseColorNames.AddRange(ThemeManager.GetBaseColorNames());
            ColorSchemes.AddRange(ThemeManager.GetColorSchemes());

            RestoreTheme();
         }

         ConfigFolderPath = GlobalSettings.GetSettingsDir();
      }

      private IThemeManagerService ThemeManager => ServiceLocator.GetInstance<IThemeManagerService>();

      public ObservableCollection<string> BaseColorNames { get; set; } = new ObservableCollection<string>();

      public ObservableCollection<ColorScheme> ColorSchemes { get; set; } = new ObservableCollection<ColorScheme>();

      private void RestoreTheme()
      {
         string baseColor = GlobalSettings.Default.ThemeBaseColor;
         string accent = GlobalSettings.Default.ThemeAccent;

         ActiveBaseColor = baseColor;
         ActiveColorScheme = ColorSchemes.FirstOrDefault(cs => cs.Name == accent);

         ApplyTheme();
      }

      private void ApplyTheme()
      {
         if(ActiveBaseColor == null || ActiveColorScheme == null)
            return;

         ThemeManager.ApplyColorScheme(ActiveBaseColor, ActiveColorScheme);

         GlobalSettings.Default.ThemeBaseColor = ActiveBaseColor;
         GlobalSettings.Default.ThemeAccent = ActiveColorScheme?.Name;

         Log.Information("applied theme {BaseColor}/{Accent}", ActiveBaseColor, ActiveColorScheme.Name);
      }

      #region [ Properties ]


      private string _activeBaseColor;
      public string ActiveBaseColor
      {
         get => _activeBaseColor;
         set { Set(() => ActiveBaseColor, ref _activeBaseColor, value); ApplyTheme(); }
      }


      private ColorScheme _activeColorScheme;
      public ColorScheme ActiveColorScheme
      {
         get => _activeColorScheme;
         set { Set(() => ActiveColorScheme, ref _activeColorScheme, value); ApplyTheme(); }
      }


      private string _configFolderPath;
      public string ConfigFolderPath
      {
         get => _configFolderPath;
         set { Set(() => ConfigFolderPath, ref _configFolderPath, value); }
      }

      #endregion
   }
}
