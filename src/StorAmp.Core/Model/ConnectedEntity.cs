using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using StorAmp.Core.ViewModel;

namespace StorAmp.Core.Model
{
   public abstract class ConnectedEntity : ViewModelBase
   {
      [JsonProperty("id")]
      public string Id { get; set; } = Guid.NewGuid().ToString();


      private string _DisplayName;
      [JsonProperty("name")]
      public string DisplayName
      {
         get => _DisplayName;
         set { Set(() => DisplayName, ref _DisplayName, value); }
      }

      [JsonProperty("children")]
      private ConnectedEntity[] _jsonChildren
      {
         get => Children?.Count > 0 ? Children.ToArray() : null;
         set
         {
            Children = new ObservableCollection<ConnectedEntity>();
            if(value != null)
            {
               foreach(ConnectedEntity i in value)
               {
                  Children.Add(i);
               }
            }
         }
      }



      [JsonIgnore]
      public ObservableCollection<ConnectedEntity> Children { get; set; } = new ObservableCollection<ConnectedEntity>();

      public ConnectedEntity FindById(string id)
      {
         if(this.Id == id)
            return this;

         return Children?.Select(c => c.FindById(id)).Where(c => c != null).FirstOrDefault();
      }

      public bool DeleteById(string id)
      {
         foreach(ConnectedEntity child in Children)
         {
            if(child.Id == id)
            {
               Children.Remove(child);
               return true;
            }

            if(child.DeleteById(id))
            {
               return true;
            }
         }

         return false;
      }

      public ConnectedAccount FindConnectedAccountById(string id)
      {
         return FindById(id) as ConnectedAccount;
      }

      public HierarchicalResource ToHierarchicalResource()
      {
         return null;
      }
   }
}
