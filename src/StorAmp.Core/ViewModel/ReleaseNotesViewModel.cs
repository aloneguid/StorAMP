using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using GalaSoft.MvvmLight;
using NetBox.Extensions;

namespace StorAmp.Core.ViewModel
{
   public class ReleaseNotesViewModel : ViewModelBase
   {
      public ObservableCollection<Release> Releases { get; } = new ObservableCollection<Release>();

      public ReleaseNotesViewModel()
      {
         ParseReleaseNotes();
      }

      public class Release : ViewModelBase
      {
         public ObservableCollection<string> Items { get; } = new ObservableCollection<string>();

         public Release(string line)
         {
            Title = line.Trim('v');
         }

         private string _Title;
         public string Title
         {
            get => _Title;
            set { Set(() => Title, ref _Title, value); }
         }
      }

      private void ParseReleaseNotes()
      {
         string file = typeof(GlobalInit).GetSameFolderEmbeddedResourceFileAsText("releasenotes.txt");

         Release r = null;

         foreach(string line in file.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
         {
            if(line.StartsWith("v"))
            {
               if(r != null)
               {
                  Releases.Add(r);
               }

               r = new Release(line);
            }
            else
            {
               if(r != null)
               {
                  string item = line.Trim();
                  r.Items.Add(item);
               }
            }
         }

         if(r != null)
         {
            Releases.Add(r);
         }
      }
   }
}
