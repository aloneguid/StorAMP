using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NetBox.Extensions;
using Serilog;
using StorAmp.Core.Model;
using StorAmp.Core.Services.Az;

namespace StorAmp.Core.ViewModel.Azure
{
   public class AzureAccountControlViewModel : ViewModelBase, IUserTokenInteraction
   {
      private readonly MultiTenantAzure _azure;
      private bool _verified;

      public AzureAccountControlViewModel()
      {
         Account = new ConnectedAzureAccount();
         _azure = MultiTenantAzure.FromAccount(Account);
         _azure.UserTokenInteraction = this;

         InitialiseAsync().Forget();
      }

      private async Task InitialiseAsync()
      {
         //kick off client initialisation
         await _azure.ListAllSubscriptionsAsync();
      }

      public ConnectedAzureAccount Account { get; private set; }

      private string _UserCode;
      public string UserCode
      {
         get => _UserCode;
         set { Set(() => UserCode, ref _UserCode, value); }
      }


      private string _VerificationUrl;
      public string VerificationUrl
      {
         get => _VerificationUrl;
         set { Set(() => VerificationUrl, ref _VerificationUrl, value); }
      }

      private RelayCommand _CompleteAuthCommand;
      public RelayCommand CompleteAuthCommand
      {
         get
         {
            return _CompleteAuthCommand
                ?? (_CompleteAuthCommand = new RelayCommand(
                () =>
                {
                   //create azure record and save it
                   Account.DisplayName = _azure.UserInfo.DisplayableId;

                   ConnectedAccount.AzureAccounts.Add(Account);
                   ConnectedAccount.Save();

                   EventLog.LogEvent("azureAccountAuthComplete", "name: {name}", _azure.UserInfo.DisplayableId);
                },
                () => _verified));
         }
      }

      public Task CompleteAuthAsync()
      {
         _verified = true;
         CompleteAuthCommand?.RaiseCanExecuteChanged();
         return Task.CompletedTask;
      }

   }
}
