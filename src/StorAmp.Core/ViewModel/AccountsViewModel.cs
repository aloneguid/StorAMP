using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using StorAmp.Core.Model;
using GalaSoft.MvvmLight.Command;
using Serilog;
using System.Diagnostics;
using StorAmp.Core.Model.Account;
using GalaSoft.MvvmLight;
using NetBox.Extensions;
using System.Threading.Tasks;
using System;

namespace StorAmp.Core.ViewModel
{
   public class AccountsViewModel : ViewModelBase
   {
      public event Action<bool> Committed;
      private readonly bool _isNew;

      private readonly Dictionary<AccountField, PropertyChangedEventHandler> _fieldToHandler = new Dictionary<AccountField, PropertyChangedEventHandler>();

      public ObservableCollection<AccountConnectionType> ConnectionTypes { get; } = new ObservableCollection<AccountConnectionType>();

      public ObservableCollection<AccountField> AccountFields { get; } = new ObservableCollection<AccountField>();

      public ObservableCollection<AccountDefinition> AccountDefinitions { get; } = AccountDefinition.AllDefinitions;

      public AccountsViewModel(ConnectedFolder connectedFolder, ConnectedAccount connectedAccount)
      {
         _isNew = connectedAccount == null;
         ConnectedFolder = connectedFolder;
         ConnectedAccount = connectedAccount;

         //bind to a subtype - https://stackoverflow.com/a/8344798/80858
      }

      private async Task TestAccountAsync()
      {
         AccountDefinition worker = AccountDefinition.GetByPrefix(ConnectedAccount.Prefix);

         IsValidating = true;
         Exception ex = await worker.ValidateInteractiveAsync(ConnectedAccount.Id, ConnectedAccount.StorageConnectionString);
         ValidationError = ex?.Message;
         ValidationErrorDetails = ex?.ToString();
         IsValidating = false;
         ValidationPassed = ex == null;
         CommitCommand?.RaiseCanExecuteChanged();
      }
      private void UnbindHandlers()
      {
         foreach(KeyValuePair<AccountField, PropertyChangedEventHandler> handler in _fieldToHandler)
         {
            handler.Key.PropertyChanged -= handler.Value;
         }

         _fieldToHandler.Clear();
      }
      
      private void UpdateAccountFields()
      {
         AccountFields.Clear();
         if(SelectedConnectionType == null)
            return;

         foreach(AccountField accountField in SelectedConnectionType.Fields)
         {
            ConnectedAccount.StorageConnectionString.Parameters.TryGetValue(accountField.Name, out string fieldValue);

            accountField.Value = fieldValue ?? string.Empty;

            if(accountField is HiddenAccountField)
            {
               ConnectedAccount.StorageConnectionString.Parameters[accountField.Name] = accountField.Value;
            }

            PropertyChangedEventHandler eh = (sender, pcea) =>
            {
               if(pcea.PropertyName == nameof(AccountField.Value) && sender is AccountField af)
               {
                  //update connection string parameter
                  ConnectedAccount.StorageConnectionString.Parameters[af.Name] = af.Value;

                  IsDirty = true;
                  CommitCommand?.RaiseCanExecuteChanged();

                  Debug.WriteLine($"{ConnectedAccount.DisplayName}: {af.Name} => {af.Value}");
               }
            };

            accountField.PropertyChanged += eh;
            _fieldToHandler[accountField] = eh;
            AccountFields.Add(accountField);
         }
      }

      #region [ Commands ]

      private RelayCommand<AccountDefinition> _addCommand;
      public RelayCommand<AccountDefinition> AddCommand
      {
         get
         {
            return _addCommand
                ?? (_addCommand = new RelayCommand<AccountDefinition>(
                (definition) =>
                {
                   var account = new ConnectedAccount(definition.Prefix) { System = definition.Type };

                   EventLog.LogEvent(
                      "add account",
                      "prefix: {Prefix}, type: {Type}",
                      definition.Prefix, definition.Type);

                   ConnectedAccount = account;
                }));
         }
      }

      private RelayCommand _commitCommand;
      public RelayCommand CommitCommand
      {
         get
         {
            return _commitCommand
                ?? (_commitCommand = new RelayCommand(
                () =>
                {
                   if(_isNew)
                   {
                      GlobalState.LastConnectedFolder.Children.Add(ConnectedAccount);
                   }
                   ConnectedAccount.Save();
                   Committed?.Invoke(true);
                },
                () => ValidationPassed));
         }
      }

      private RelayCommand _testCommand;
      public RelayCommand TestCommand
      {
         get
         {
            return _testCommand
                ?? (_testCommand = new RelayCommand(
                () =>
                {
                   TestAccountAsync().Forget();
                }));
         }
      }

      #endregion

      #region [ Properties ]

      private ConnectedAccount _selectedAccount;

      public ConnectedFolder ConnectedFolder { get; }

      public ConnectedAccount ConnectedAccount
      {
         get => _selectedAccount;
         set
         {
            UnbindHandlers();

            if(_selectedAccount != null)
            {
               _selectedAccount.PropertyChanged -= SelectedAccountPropertyChanged;
            }

            Set(() => ConnectedAccount, ref _selectedAccount, value);

            //populate connection types
            ConnectionTypes.Clear();
            if(value != null)
            {
               value.PropertyChanged += SelectedAccountPropertyChanged;

               AccountDefinition aa = AccountDefinition.GetByPrefix(value.Prefix);
               ConnectionTypes.AddRange(aa.ConnectionTypes);
               //set generic helper properties
               HasConnectionTypeOptions = aa.ConnectionTypes.Count > 1;

               //set selected connection type
               AccountConnectionType activeType = ConnectionTypes.FirstOrDefault(ct => ct.Token == value.ConnectionTypeToken);
               SelectedConnectionType = activeType ?? ConnectionTypes.First();
            }

            AccountTypeSelected = value != null;
         }
      }

      private void SelectedAccountPropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         if(e.PropertyName == nameof(Model.ConnectedAccount.DisplayName))
         {
            IsDirty = true;
         }
      }

      private bool _hasConnectionTypeOptions;
      public bool HasConnectionTypeOptions
      {
         get => _hasConnectionTypeOptions;
         set { Set(() => HasConnectionTypeOptions, ref _hasConnectionTypeOptions, value); }
      }


      private AccountConnectionType _selectedConnectionType;
      public AccountConnectionType SelectedConnectionType
      {
         get => _selectedConnectionType;
         set
         {
            Set(() => SelectedConnectionType, ref _selectedConnectionType, value);

            if(value != null)
            {
               ConnectedAccount.ConnectionTypeToken = value.Token;
            }

            CommitCommand?.RaiseCanExecuteChanged();
            AddCommand?.RaiseCanExecuteChanged();

            IsDirty = false;
            IsValid = true;
            ValidationPassed = false;
            UpdateAccountFields();
         }
      }


      private bool _isDirty;
      public bool IsDirty
      {
         get => _isDirty;
         set { Set(() => IsDirty, ref _isDirty, value); CommitCommand?.RaiseCanExecuteChanged(); }
      }


      private bool _isValidating;
      public bool IsValidating
      {
         get => _isValidating;
         set { Set(() => IsValidating, ref _isValidating, value); }
      }


      private string _validationError;
      public string ValidationError
      {
         get => _validationError;
         set { Set(() => ValidationError, ref _validationError, value); IsValid = value == null; }
      }


      private string _validationErrorDetails;
      public string ValidationErrorDetails
      {
         get => _validationErrorDetails;
         set { Set(() => ValidationErrorDetails, ref _validationErrorDetails, value); }
      }


      private bool _isValid;
      public bool IsValid
      {
         get => _isValid;
         set { Set(() => IsValid, ref _isValid, value); }
      }


      private bool _validationPassed;
      public bool ValidationPassed
      {
         get => _validationPassed;
         set { Set(() => ValidationPassed, ref _validationPassed, value); }
      }

      private bool _accountTypeSelected;
      public bool AccountTypeSelected
      {
         get => _accountTypeSelected;
         set { Set(() => AccountTypeSelected, ref _accountTypeSelected, value); }
      }

      #endregion
   }
}
