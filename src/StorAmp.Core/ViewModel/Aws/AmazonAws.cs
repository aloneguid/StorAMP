using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Amazon.Runtime;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using Serilog;
using Storage.Net.Amazon.Aws;
using StorAmp.Core.Model;

namespace StorAmp.Core.ViewModel.Aws
{

   public class AmazonAws : HierarchicalResource
   {
      private readonly HierarchicalResource _awsCliProfiles = new HierarchicalResource("AWS CLI Profiles", "cli");

      public AmazonAws() : base("Amazon AWS", "cloud-aws")
      {
         IsExpanded = false;

         Children.Add(_awsCliProfiles);

         RefreshAwsCliProfiles();

         CommandGroups.Add(new HierarchicalResourceCommandGroup(
            new HierarchicalResourceCommand("Refresh", Symbol.Refresh, RefreshCliProfilesCommand)));
      }

      private RelayCommand _RefreshCliProfilesCommand;
      public RelayCommand RefreshCliProfilesCommand
      {
         get
         {
            return _RefreshCliProfilesCommand
                ?? (_RefreshCliProfilesCommand = new RelayCommand(
                () =>
                {
                   RefreshAwsCliProfiles();
                }));
         }
      }

      public void RefreshAwsCliProfiles()
      {
         _awsCliProfiles.Children.Clear();

         try
         {
            foreach(string profileName in AwsCliCredentials.EnumerateProfiles())
            {
               AWSCredentials creds = AwsCliCredentials.GetCredentials(profileName);

               _awsCliProfiles.Children.Add(new AwsCliProfile(profileName, creds));
            }
         }
         catch(Exception ex)
         {
            Log.Error(ex, "discovery failed");
         }
      }

      #region [ Static Caching ]

      private static ObservableCollection<AwsEnvironment> _allEnvironments;

      public static ObservableCollection<AwsEnvironment> AllEnvironments =>
         _allEnvironments ?? (_allEnvironments = LoadAllEnvironemnts());

      private static ObservableCollection<AwsEnvironment> LoadAllEnvironemnts()
      {
         var result = new ObservableCollection<AwsEnvironment>();

         string path = GlobalSettings.GetAwsFilePath();
         if(File.Exists(path))
         {
            string json = File.ReadAllText(path);
            AwsEnvironment[] envs = JsonConvert.DeserializeObject<AwsEnvironment[]>(json);
            foreach(AwsEnvironment env in envs)
            {
               result.Add(env);
            }
         }

         return result;
      }

      public static void Save()
      {
         string json = JsonConvert.SerializeObject(_allEnvironments.ToArray(), Formatting.Indented);
         File.WriteAllText(GlobalSettings.GetAwsFilePath(), json);
      }

      #endregion

   }
}
