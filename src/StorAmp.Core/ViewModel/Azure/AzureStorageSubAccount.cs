using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Storage.Fluent;
using Serilog;
using StorAmp.Core.Model;

namespace StorAmp.Core.ViewModel.Azure
{
   public abstract class AzureStorageSubAccount : HierarchicalResource
   {
      protected AzureStorageSubAccount(IStorageAccount storageAccount, string name, string iconPath) : base(name, iconPath)
      {
         StorageAccount = storageAccount;

         AddCommandGroup(new HierarchicalResourceCommand("save", Symbol.SaveLocal, SaveAsync));
      }

      private Task SaveAsync()
      {
         ConnectedAccount ca = CreateConnectedAccount();

         EventLog.LogEvent("saveAzureAccount", "prefix: {prefix}", ca.Prefix);

         GlobalState.LastConnectedFolder.Children.Add(ca);
         ConnectedAccount.Save();
         return Task.CompletedTask;
      }

      protected abstract ConnectedAccount CreateConnectedAccount();

      public override object GetDoubleTapResult()
      {
         EventLog.LogEvent("doubleTapAzureAccount", "name: {name}", this.DisplayName);

         return CreateConnectedAccount();
      }

      public override void FilterVisibility(string text)
      {
         IsVisible = string.IsNullOrEmpty(text);
      }

      public IStorageAccount StorageAccount { get; }

      public string PrimarySharedKey { get; set; }

      public string SecondarySharedKey { get; set; }
   }
}
