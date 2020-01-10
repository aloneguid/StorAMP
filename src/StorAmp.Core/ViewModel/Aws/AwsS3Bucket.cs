using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using NetBox.Extensions;
using Storage.Net;
using StorAmp.Core.Model;
using StorAmp.Core.Services;
using IDialogService = StorAmp.Core.Services.IDialogService;

namespace StorAmp.Core.ViewModel.Aws
{
   public class AwsS3Bucket : HierarchicalResource
   {
      private readonly string _cliProfileName;
      private readonly string _region;

      public AwsS3Bucket(string name, string cliProfileName, string region) : base(name, "account/aws.s3")
      {
         _cliProfileName = cliProfileName;
         _region = region;

         CommandGroups.Add(new HierarchicalResourceCommandGroup(
            new HierarchicalResourceCommand("Attach", Symbol.Attach, AttachCommand),
            new HierarchicalResourceCommand("Properties", Symbol.Setting, ShowPropertiesCommand)));
      }

      public override object GetDoubleTapResult()
      {
         return new ConnectedAccount(BuildConnectionString())
         {
            DisplayName = DisplayName
         };
      }

      private string BuildConnectionString()
      {
         return StorageFactory.ConnectionStrings.ForAwsS3FromCliProfile(_cliProfileName, DisplayName, _region).ToString();
      }

      private RelayCommand _ShowPropertiesCommand;
      public RelayCommand ShowPropertiesCommand
      {
         get
         {
            return _ShowPropertiesCommand
                ?? (_ShowPropertiesCommand = new RelayCommand(
                () =>
                {
                   IDialogService dialogService = ServiceLocator.GetInstance<IDialogService>();

                   dialogService.ShowPropertiesAsync("Properties",
                      new Dictionary<string, object>
                      {
                         ["Profile Name"] = _cliProfileName,
                         ["Region"] = _region,
                         ["Bucket Name"] = DisplayName,
                         ["Storage.Net Connection String"] = BuildConnectionString()
                      }).Forget();
                }));
         }
      }

      private RelayCommand _AttachCommand;
      public RelayCommand AttachCommand
      {
         get
         {
            return _AttachCommand
                ?? (_AttachCommand = new RelayCommand(
                () =>
                {
                   var ca = new ConnectedAccount(BuildConnectionString()) { DisplayName = DisplayName, ConnectionTypeToken = "awscli" };

                   //ConnectedAccount.AddAndSave(ca);
                }));
         }
      }

   }
}
