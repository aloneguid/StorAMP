using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using StorAmp.Core.Model;
using StorAmp.Core.Services;
using IDialogService = StorAmp.Core.Services.IDialogService;

namespace StorAmp.Core.ViewModel
{

   public class HierarchicalResource : ViewModelBase
   {
      public HierarchicalResource(string name = null, string iconPath = null, bool useBuiltInCommands = true)
      {
         if(name != null)
         {
            DisplayName = name;
         }

         IconPath = iconPath;
         IsVisible = true;

         if(useBuiltInCommands)
         {
            CommandGroups.Add(new HierarchicalResourceCommandGroup(
               new HierarchicalResourceCommand("Copy name", Symbol.Copy, () =>
               {
                  ServiceLocator.SystemService.SetClipboardText(DisplayName);
                  return Task.CompletedTask;
               }),
               new HierarchicalResourceCommand("expand all", Symbol.Upload, () =>
               {
                  this.SetExpandedRecursively(true);
                  return Task.CompletedTask;
               }),
               new HierarchicalResourceCommand("collapse all", Symbol.Download, () =>
               {
                  this.SetExpandedRecursively(false);
                  return Task.CompletedTask;
               })));
         }
      }

      private string _DisplayName;
      public string DisplayName
      {
         get => _DisplayName;
         set { Set(() => DisplayName, ref _DisplayName, value); }
      }

      public string IconPath { get; }

      public object Tag { get; set; }

      public ObservableCollection<HierarchicalResource> Children { get; set; } = new ObservableCollection<HierarchicalResource>();

      private bool _IsExpanded;
      public bool IsExpanded
      {
         get => _IsExpanded;
         set { Set(() => IsExpanded, ref _IsExpanded, value); }
      }

      public bool IsAutoexpandable { get; set; } = true;

      public void SetExpandedRecursively(bool value)
      {
         if(value &&!IsAutoexpandable)
            return;
         this.IsExpanded = value;
         foreach(HierarchicalResource child in Children)
         {
            child.SetExpandedRecursively(value);
         }
      }


      private bool _IsLoading;
      public bool IsLoading
      {
         get => _IsLoading;
         set { Set(() => IsLoading, ref _IsLoading, value); }
      }


      private bool _Visible;
      public bool IsVisible
      {
         get => _Visible;
         set { Set(() => IsVisible, ref _Visible, value); }
      }

      public virtual void FilterVisibility(string text)
      {
         foreach(HierarchicalResource child in Children)
         {
            child.FilterVisibility(text);
         }

         int visibleCount = Children.Count(c => c.IsVisible);
         IsVisible = visibleCount > 0 || DisplayName.Contains(text, StringComparison.OrdinalIgnoreCase);
      }

      private bool _HasError;
      public bool HasError
      {
         get => _HasError;
         set { Set(() => HasError, ref _HasError, value); }
      }


      private Exception _Error;
      public Exception Error
      {
         get => _Error;
         set { Set(() => Error, ref _Error, value); HasError = value != null; }
      }

      [JsonIgnore]
      public ObservableCollection<HierarchicalResourceCommandGroup> CommandGroups { get; } =
         new ObservableCollection<HierarchicalResourceCommandGroup>();

      public HierarchicalResourceCommandGroup AddCommandGroup(params HierarchicalResourceCommand[] commands)
      {
         var group = new HierarchicalResourceCommandGroup(commands);
         CommandGroups.Add(group);
         return group;
      }

      public virtual object GetDoubleTapResult()
      {
         return null;
      }

      protected IDialogService Dialogs => ServiceLocator.GetInstance<IDialogService>();

      protected IAppDialogsService AppDialogs => ServiceLocator.GetInstance<IAppDialogsService>();
   }
}
