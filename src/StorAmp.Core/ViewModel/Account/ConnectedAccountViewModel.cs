using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using NetBox.Extensions;
using Storage.Net;
using StorAmp.Core.Model;
using StorAmp.Core.Model.Account;
using StorAmp.Core.Services;

namespace StorAmp.Core.ViewModel.Account
{
   public class ConnectedAccountViewModel : ConnectedEntityViewModel
   {
      private readonly ConnectedAccount _ca;

      public ConnectedAccountViewModel(ConnectedAccount ca) : base(ca, null)
      {
         _ca = ca ?? throw new System.ArgumentNullException(nameof(ca));
         Tag = ca;

         AddActionCommands(ca);

         CommandGroups.Add(new HierarchicalResourceCommandGroup(
            new HierarchicalResourceCommand("Edit", Symbol.Edit, EditAccountCommand)));

         HierarchicalResourceCommandGroup copyGroup = AddCommandGroup(
            new HierarchicalResourceCommand("copy connection string", Symbol.Copy, () =>
            {
               ServiceLocator.SystemService.SetClipboardText(_ca.ConnectionString);
               return Task.CompletedTask;
            }));

         if(_ca.StorageConnectionString.Parameters.TryGetValue(KnownParameter.KeyOrPassword, out string keyOrPassword))
         {
            copyGroup.Commands.Add(new HierarchicalResourceCommand("copy key/password", Symbol.Copy, () =>
            {
               ServiceLocator.SystemService.SetClipboardText(keyOrPassword);
               return Task.CompletedTask;
            }));
         }

      }

      private void AddActionCommands(ConnectedAccount ca)
      {
         if(!(ca.Definition is BlobApplicationAccount baa))
            return;

         IReadOnlyCollection<ActionGroup> actionGroups = baa.GetExecutableAccountActionGroups(ca);

         foreach(ActionGroup ag in actionGroups)
         {
            var hrg = new HierarchicalResourceCommandGroup(
               ag.Actions.Select(
                  cmd => new HierarchicalResourceCommand(cmd.Name, cmd.Icon, () => cmd.ExecuteAsync(ca, null) )).ToArray()
               );

            CommandGroups.Add(hrg);
         }
      }

      private RelayCommand _EditAccountCommand;
      public RelayCommand EditAccountCommand
      {
         get
         {
            return _EditAccountCommand
                ?? (_EditAccountCommand = new RelayCommand(
                () =>
                {
                   AppDialogs.ShowConnectedAccountDialogAsync(null, _ca).Forget();
                }));
         }
      }

      public override object GetDoubleTapResult() => _ca;

   }
}