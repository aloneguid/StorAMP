using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NetBox.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StorAmp.Core.Model;
using StorAmp.Core.Model.AppInsights;
using StorAmp.Core.Services.ApplicationInsights;

namespace StorAmp.Core.ViewModel
{
   public class ApplicationInsightsPanelViewModel : ViewModelBase, IAccountPaneViewModel
   {
      private readonly AppInsightsClient _client;

      public ApplicationInsightsPanelViewModel()
      {

      }

      public ApplicationInsightsPanelViewModel(ConnectedAccount account)
      {
         Account = account;

         _client = new AppInsightsClient(
            account.StorageConnectionString.Get("appId"),
            account.StorageConnectionString.Get("apiKey"));
      }

      public ConnectedAccount Account { get; }

      private async Task ExecuteQueryAsync()
      {
         QueryResult = await _client.ExecuteAsync(QueryText);
      }

      public void RestoreSettings()
      {

      }

      public bool AcceptsDrop(DragData data) => false;

      public void DropDataAsync(DragData data)
      {

      }

      public void RestoreSettings(string settings)
      {

      }

      public string GetSettings() => string.Empty;

      #region [ Commands ]

      private RelayCommand _executeCommand;

      /// <summary>
      /// Gets the MyCommand.
      /// </summary>
      public RelayCommand ExecuteCommand
      {
         get
         {
            return _executeCommand
                ?? (_executeCommand = new RelayCommand(
                () =>
                {
                   ExecuteQueryAsync().Forget();
                }));
         }
      }

      #endregion

      #region [ Properties ]


      private string _queryText;
      public string QueryText
      {
         get => _queryText;
         set { Set(() => QueryText, ref _queryText, value); }
      }


      private QueryResult _jsonResult;
      public QueryResult QueryResult
      {
         get => _jsonResult;
         set { Set(() => QueryResult, ref _jsonResult, value); }
      }

      #endregion
   }
}
