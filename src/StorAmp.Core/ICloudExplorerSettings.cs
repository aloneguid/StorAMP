using System;
using System.ComponentModel;
using System.IO;
using Config.Net;

namespace StorAmp.Core
{
   public interface ICloudExplorerSettings : INotifyPropertyChanged
   {
      string MainWindowPlacementXml { get; set; }

      [Option(DefaultValue = true)]
      bool OpenTaskPaneOnTaskAdded { get; set; }

      [Option(DefaultValue = "Light")]
      string ThemeBaseColor { get; set; }

      [Option(DefaultValue = "Steel")]
      string ThemeAccent { get; set; }

      bool HumanisedBlobChangeDates { get; set; }

      bool AlternateRowColours { get; set; }

      [Option(DefaultValue = true)]
      bool FoldersFirst { get; set; }

      [Option(DefaultValue = 10)]
      int MaxParallelUploads { get; set; }

      string OpenTabs { get; set; }

      DateTime InstallDate { get; set; }

      bool ReviewLeft { get; set; }

      string ReleaseNotesLastShownForVersion { get; set; }

      [Option(DefaultValue = ".jpg, .jpeg, .png, .gif, .bmp, .ico")]
      string ImageExtensions { get; }

      [Option(DefaultValue = ".dat, .wmv, .3g2, .3gp, .3gp2, .3gpp, .amv, .asf, .avi, .bin, .cue, .divx, .dv, .flv, .gxf, .iso, .m1v, .m2v, .m2t, .m2ts, .m4v, .mkv, .mov, .mp2, .mp2v, .mp4, .mp4v, .mpa, .mpe, .mpeg, .mpeg1, .mpeg2, .mpeg4, .mpg, .mpv2, .mts, .nsv, .nuv, .ogg, .ogm, .ogv, .ogx, .ps, .rec, .rm, .rmvb, .tod, .ts, .tts, .vob, .vro, .webm")]
      string VideoExtensions { get; }

      #region [ Blob List ]

      [Option(DefaultValue = true)]
      bool SingleClickNavigation { get; set; }

      #endregion

      #region [ Grid Splitters ]

      [Option(Alias = "GridSplitters.SideBar", DefaultValue = "Auto")]
      string SideBarColumnWidth { get; set; }

      [Option(Alias = "GridSplitters.Messenger", DefaultValue = "Auto")]
      string MessengerColumnWidth { get; set; }

      #endregion

      #region [ Azure ]

      [Option(Alias = "Azure.GroupByResourceGroup", DefaultValue = true)]
      bool AzureGroupResourcesByResourceGroup { get; set; }

      [Option(Alias = "Azure.ShowEmptySubscriptions")]
      bool AzureShowEmptySubscriptions { get; set; }

      #endregion
   }

   public static class GlobalSettings
   {
      private const string Subfolder = "StorAmp";

      public static ICloudExplorerSettings Default { get; private set; }

      public static void Initialise(ICloudExplorerSettings instance)
      {
         Default = instance;
      }

      #region [ File stuff, to move ]

      private static string _basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      public static void SetBasePath(string path)
      {
         _basePath = path;
      }

      public static string GetFilePath(params string[] name)
      {
         string dir = Path.Combine(_basePath, Subfolder);

         for(int i = 0; i < name.Length - 1; i++)
         {
            dir = Path.Combine(dir, name[i]);
         }

         if(!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

         return Path.Combine(dir, name[name.Length - 1]);
      }

      public static string GetSettingsDir()
      {
         return Path.Combine(_basePath, Subfolder);
      }

      public static string GetSettingsFilePath()
      {
         string path = GetFilePath("app.ini");

         MigrateFileByMovement(path, Path.Combine(_basePath, "storamp.ini"));

         return path;
      }

      public static string GetAzureFilePath()
      {
         return GetFilePath("azure.json");
      }

      public static string GetAwsFilePath()
      {
         return GetFilePath("aws.json");
      }

      public static string GetAadCachepath(string identity)
      {
         return GetFilePath("aad", identity + ".dep");
      }

      public static string GetAccountsJsonPath()
      {
         string path = GetFilePath("accounts.json");

         MigrateFileByMovement(path, Path.Combine(_basePath, "storamp.accounts.json"), GetFilePath("accounts.ini"));

         return path;
      }

      private static void MigrateFileByMovement(string currentPath, params string[] oldPaths)
      {
         if(oldPaths?.Length == 0)
            return;

         foreach(string oldPath in oldPaths)
         {
            if(File.Exists(oldPath))
            {
               File.Move(oldPath, currentPath);
               break;
            }
         }
      }

      #endregion

   }
}
