using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Storage.Net.Blobs;
using StorAmp.Core.Model.Account;
using StorAmp.Wpf.Controls;

namespace StorAmp.Wpf.Wpf.Blobs
{
   static class UiBuilder
   {
      public static void BuildMenu(ContextMenu menu, int originalCount, BlobApplicationAccount account, IBlobStorage storage, Blob blob)
      {
         //clear up anything extra
         while(menu.Items.Count > originalCount)
         {
            menu.Items.RemoveAt(menu.Items.Count - 1);
         }

         //add relevant items
         IReadOnlyCollection<ActionGroup> actionGroups = account.GetExecutableBlobActionGroups(storage, blob);

         if(actionGroups.Count > 0)
         {
            foreach(ActionGroup ag in actionGroups)
            {
               menu.Items.Add(new Separator());

               foreach(IConnectedAccountAction action in ag.Actions)
               {
                  var mi = new MenuItem
                  {
                     Header = action.Name,
                     Icon = new WinUISymbol { Symbol = action.Icon, HorizontalAlignment = HorizontalAlignment.Center }
                  };

                  object[] argRange = action.ArgumentRange;

                  if(argRange == null)
                  {
                     mi.Click += (sender, e) => action.ExecuteAsync(storage, blob, null);
                  }
                  else
                  {
                     foreach(object arg in argRange)
                     {
                        var mii = new MenuItem
                        {
                           Header = arg

                        };
                        mii.Click += (sender, e) => action.ExecuteAsync(storage, blob, arg);
                        mi.Items.Add(mii);
                     }
                  }

                  menu.Items.Add(mi);
               }

            }
         }
      }
   }
}
