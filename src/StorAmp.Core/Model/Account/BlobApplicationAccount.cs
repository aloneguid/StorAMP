using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Storage.Net;
using Storage.Net.Blobs;
using Storage.Net.ConnectionString;
using StorAmp.Core.Services;

namespace StorAmp.Core.Model.Account
{

   public abstract class BlobApplicationAccount : AccountDefinition
   {
      private readonly List<ActionGroup> _actionGroups = new List<ActionGroup>();

      private readonly Dictionary<string, ExtraColumnView> _viewNameToExtraColumnView =
         new Dictionary<string, ExtraColumnView>();

      protected BlobApplicationAccount(string type, string prefix, string displayName, params AccountConnectionType[] connectionTypes) 
         : base(type, prefix, displayName, connectionTypes)
      {
      }

      protected BlobApplicationAccount(
         string prefix,
         string displayName,
         params AccountField[] fields)
         : base(prefix, displayName, fields)
      {

      }

      [JsonIgnore]
      protected IAppDialogsService AppDialogs => ServiceLocator.GetInstance<IAppDialogsService>();

      public IReadOnlyCollection<ActionGroup> GetExecutableBlobActionGroups(IBlobStorage storage, Blob blob)
      {
         if(storage == null || blob == null)
            return new List<ActionGroup>();

         var result = new List<ActionGroup>();

         foreach(ActionGroup ag in _actionGroups)
         {
            IConnectedAccountAction[] actions = ag.Actions.Where(a => a.CanExecute(storage, blob)).ToArray();
            if(actions.Length > 0)
            {
               result.Add(new ActionGroup(ag.Name, actions));
            }
         }

         return result;
      }

      public IReadOnlyCollection<ActionGroup> GetExecutableAccountActionGroups(ConnectedAccount connectedAccount)
      {
         var result = new List<ActionGroup>();

         foreach(ActionGroup ag in _actionGroups)
         {
            IConnectedAccountAction[] actions = ag.Actions.Where(a => a.CanExecute(connectedAccount)).ToArray();
            if(actions.Length > 0)
            {
               result.Add(new ActionGroup(ag.Name, actions));
            }
         }

         return result;
      }

      public void AddColumnView(ExtraColumnView view)
      {
         _viewNameToExtraColumnView[view.Name] = view;
      }

      public ExtraColumnView MatchColumnView(Blob anyBlob)
      {
         return _viewNameToExtraColumnView.Values.FirstOrDefault(v => v.IsMatch(anyBlob));
      }

      public void AddActionGroup(ActionGroup group)
      {
         _actionGroups.Add(group);
      }

      public override async Task<Exception> TestAndGetErrorAsync(string accountId, StorageConnectionString connectionString)
      {
         try
         {
            IBlobStorage bs = StorageFactory.Blobs.FromConnectionString(connectionString.ToString());
            await bs.ListAsync(maxResults: 1, recurse: false);
            return null;
         }
         catch(Exception ex)
         {
            return ex;
         }
      }
   }

}
