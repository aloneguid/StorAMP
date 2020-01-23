using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using StorAmp.Core.Model;
using StorAmp.Core.ViewModel.Msg;
using StorAmp.Core.ViewModel.Redis;

namespace StorAmp.Core.ViewModel
{
   public class MainViewModel : ViewModelBase, INotifyPropertyChanged, IDisposable
   {
      public ObservableCollection<IAccountPaneViewModel> ActiveStorageAccounts { get; set; } = new ObservableCollection<IAccountPaneViewModel>();

      public ICommand AddTabCommand => new RelayCommand<ConnectedAccount>((sa) =>
      {
         AddTab(sa);
      });

      public ICommand CloseTabCommand => new RelayCommand(
      () =>
      {
         ActiveStorageAccounts.Remove(SelectedAccount);

         SelectedAccount = ActiveStorageAccounts.LastOrDefault();
      });

      public MainViewModel()
      {
         if(!IsInDesignMode)
         {
            RestoreTabs();

            /*if(ActiveStorageAccounts.Count == 0)
            {
               AddTab(RegisteredAccounts.First());
            }*/

            if(SelectedAccount == null)
            {
               SelectedAccount = ActiveStorageAccounts.LastOrDefault();
            }
         }
      }

      public void AddTab(ConnectedAccount connectedAccount, string settings = null, bool makeActive = true)
      {
         if(connectedAccount == null)
            return;

         if(connectedAccount.System == "msg")
         {
            ActiveStorageAccounts.Add(new MessengerViewModel(connectedAccount));
         }
         else if(connectedAccount.System == "ai")
         {
            ActiveStorageAccounts.Add(new ApplicationInsightsPanelViewModel(connectedAccount));
         }
         else if(connectedAccount.System == "redis")
         {
            ActiveStorageAccounts.Add(new RedisViewModel(connectedAccount));
         }
         else
         {
            ActiveStorageAccounts.Add(new BlobStoragePanelViewModel(connectedAccount));
         }

         if(makeActive)
         {
            SelectedAccount = ActiveStorageAccounts.Last();
         }
         ActiveStorageAccounts.Last().RestoreSettings(settings);
      }

      private void SaveTabs()
      {
         string s = TabSettings.ToString(ActiveStorageAccounts.Select(
            asa => new TabSettings(
               asa.Account.Id,
               asa.Account.Id == SelectedAccount.Account.Id,
               asa.GetSettings())));

         GlobalSettings.Default.OpenTabs = s;
      }

      private void RestoreTabs()
      {
         string tabs = GlobalSettings.Default.OpenTabs;
         if(tabs == null)
            return;

         IReadOnlyCollection<TabSettings> ts = TabSettings.FromString(tabs);

         foreach(TabSettings t in ts)
         {
            ConnectedAccount sa = ConnectedAccount.RootFolder.FindConnectedAccountById(t.AccountId);
            if(sa == null)
               continue;

            AddTab(sa, t.Settings, false);

            if(t.IsActive)
            {
               SelectedAccount = ActiveStorageAccounts.Last();
            }
         }
      }

      public void Dispose()
      {
         SaveTabs();
      }

      #region [ Properties ]



      private IAccountPaneViewModel _selectedAccount;
      public IAccountPaneViewModel SelectedAccount
      {
         get => _selectedAccount;
         set { Set(() => SelectedAccount, ref _selectedAccount, value); }
      }

      private ConnectedAccount _selectedRegisteredAccount;
      public ConnectedAccount SelectedRegisteredAccount
      {
         get => _selectedRegisteredAccount;
         set { Set(() => SelectedRegisteredAccount, ref _selectedRegisteredAccount, value); }
      }

      #endregion

   }
}
