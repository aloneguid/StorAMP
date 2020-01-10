using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using StorAmp.Core.Model.Account;
using StorAmp.Core.ViewModel.Aws;
using StorAmp.Core.ViewModel.Azure;

namespace StorAmp.Core.ViewModel.Account
{

   public class SideBarViewModel : ViewModelBase
   {
      public SideBarViewModel()
      {
         RootItems.Add(new ConnectedEntitiesViewModel());
         RootItems.Add(new MicrosoftAzure());
         RootItems.Add(new AmazonAws());
      }

      public Task InitialiseAsync()
      {
         return Task.CompletedTask;
      }

      public ObservableCollection<HierarchicalResource> RootItems { get; } = new ObservableCollection<HierarchicalResource>();


      private string _NodeFilter;
      public string NodeFilter
      {
         get => _NodeFilter;
         set
         {
            Set(() => NodeFilter, ref _NodeFilter, value);
            foreach(HierarchicalResource hr in RootItems)
            {
               hr.FilterVisibility(value);
            }
         }
      }

   }
}
