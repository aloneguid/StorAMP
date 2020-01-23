using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using StorAmp.Core;
using StorAmp.Core.Services;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NetBox.Extensions;
using StorAmp.Wpf;
using StorAmp.Core.Model;
using StorAmp.Core.ViewModel;
using System.Collections.Specialized;
using StorAmp.Wpf.Wpf;
using StorAmp.Wpf.Interop;
using EventLog = Serilog.EventLog;
using StorAmp.Wpf.Services;
using StorAmp.Core.ViewModel.Msg;
using StorAmp.Wpf.Wpf.Msg;
using StorAmp.Core.ViewModel.Redis;
using StorAmp.Wpf.Wpf.Redis;

namespace CloudExplorer.Wpf
{

   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow
      : MetroWindow, IProgressReportingService, IUIDispatcher
   {
      private ProgressDialogController _progressController;

      public MainWindow()
      {
         InitializeComponent();

         ServiceLocator.Register<IProgressReportingService>(this);
         ServiceLocator.Register<IDialogService>(this);
         ServiceLocator.Register<IAppDialogsService>(this);
         ServiceLocator.Register<IUIDispatcher>(this);
         ServiceLocator.Register<ISystemService>(new SystemService());

         Version version = GetType().FileVersion();
         string printVersion;
#if DEBUG
         printVersion = "Next";
#else
         printVersion = $"{version.Major}.{version.Minor}.{version.Build}";
#endif
         Title = string.Format(Res.WindowTitle, Res.ProductName, printVersion);

         CheckInstallStatusAsync().Forget();
         CheckReleaseNotesShownAsync().Forget();
      }

      private async Task CheckInstallStatusAsync()
      {
         DateTime installDate = GlobalSettings.Default.InstallDate;
         if(installDate == DateTime.MinValue)
         {
            GlobalSettings.Default.InstallDate = DateTime.UtcNow;
         }
         else
         {
            double daysUsed = (DateTime.UtcNow - GlobalSettings.Default.InstallDate).TotalDays;

            if(daysUsed > 5 && !GlobalSettings.Default.ReviewLeft)
            {
               if(await ServiceLocator.GetInstance<IDialogService>().AskYesNoAsync(
                  "Leave me a review!",
                  $"You've been using {Res.ProductName} for quite a while, {(int)daysUsed} day(s) now. It's a completely free product and leaving a review will help us motivated to work on this completely for free. Would you like to leave us a review now?"))
               {
                  //const string url = "https://www.microsoft.com/store/apps/9NKV1D43NLL3";
                  const string url = "ms-windows-store://pdp/?ProductId=9NKV1D43NLL3";

                  var p = new Process();
                  p.StartInfo.UseShellExecute = true;
                  p.StartInfo.FileName = url;
                  p.Start();

               }

               GlobalSettings.Default.ReviewLeft = true;
            }
         }
      }

      private async Task CheckReleaseNotesShownAsync()
      {
         string currentVersion = GetType().FileVersion().ToString();

         if(GlobalSettings.Default.ReleaseNotesLastShownForVersion != currentVersion)
         {
            await Task.Delay(TimeSpan.FromSeconds(5));
            await ShowReleaseNotesAsync();
         }

         GlobalSettings.Default.ReleaseNotesLastShownForVersion = currentVersion;
      }

      private MainViewModel ViewModel => (MainViewModel)DataContext;

      protected override void OnSourceInitialized(EventArgs e)
      {
         base.OnSourceInitialized(e);

         this.SetPlacement(GlobalSettings.Default.MainWindowPlacementXml);

         ViewModel.ActiveStorageAccounts.CollectionChanged += ActiveStorageAccounts_CollectionChanged;
         ViewModel.PropertyChanged += ViewModel_PropertyChanged;

         //add existing accounts
         foreach(IAccountPaneViewModel vm in ViewModel.ActiveStorageAccounts)
         {
            NonRecycledTabs.Children.Add(CreateElementForViewModel(vm));
         }

         //update visibility
         UpdateNonRecycledTabVisibility();
      }

      protected override void OnClosing(CancelEventArgs e)
      {
         GlobalSettings.Default.MainWindowPlacementXml = this.GetPlacement();
         ViewModel.Dispose();

         base.OnClosing(e);
      }

      private void SettingsFlyoutHeaderButton_Click(object sender, RoutedEventArgs e)
      {
         settingsFlyout.IsOpen = !settingsFlyout.IsOpen;

         EventLog.LogEvent("openFlyout", "{name}", "settings");
      }

      private void AboutFlyoutHeaderButton_Click(object sender, RoutedEventArgs e)
      {
         aboutFlyout.IsOpen = !aboutFlyout.IsOpen;

         EventLog.LogEvent("openFlyout", "{name}", "about");
      }

      private async Task ShowReleaseNotesAsync()
      {
         await this.ShowDialogAsync("Release Notes", new ReleaseNotes(), null, null);
      }

      private void ShowReleaseNotes_Click(object sender, RoutedEventArgs e)
      {
         ShowReleaseNotesAsync().Forget();

         EventLog.LogEvent("openFlyout", "{name}", "releaseNotes");
      }

      public async Task StartProgressAsync(string title, string content, double min, double max)
      {
         _progressController = await this.ShowProgressAsync(title, content, true);

         _progressController.SetCancelable(true);
         _progressController.Minimum = min;
         _progressController.Maximum = max;
      }

      public async Task UpdateProgressAsync(string message, double progress)
      {
         if(message != null)
            _progressController.SetMessage(message);

         _progressController.SetProgress(progress);

         if(progress >= _progressController.Maximum)
         {
            await _progressController.CloseAsync();
            _progressController = null;
         }
      }
      #region [ Tab Switching ]

      //this whole region is made to enable super-fast tab switching, and it may look ugly and move to a component

      private void ActiveStorageAccounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         Debug.WriteLine("action: " + e.Action);

         if(e.Action == NotifyCollectionChangedAction.Add)
         {
            foreach(IAccountPaneViewModel vm in e.NewItems)
            {
               Debug.WriteLine("adding " + vm.Account.DisplayName);

               NonRecycledTabs.Children.Add(CreateElementForViewModel(vm));
            }
         }
         else if(e.Action == NotifyCollectionChangedAction.Remove)
         {
            foreach(IAccountPaneViewModel vm in e.OldItems)
            {
               Debug.WriteLine("removing " + vm.Account.DisplayName);

               int idx = GetNonRecycledTabIndexForViewModel(vm);
               if(idx != -1)
                  NonRecycledTabs.Children.RemoveAt(idx);
            }
         }
      }

      private UIElement CreateElementForViewModel(IAccountPaneViewModel viewModel)
      {
         if(viewModel is BlobStoragePanelViewModel)
            return new BlobStoragePanel { DataContext = viewModel };

         if(viewModel is ApplicationInsightsPanelViewModel)
            return new ApplicationInsightsPanel { DataContext = viewModel };

         if(viewModel is MessengerViewModel)
            return new MessengerPanel { DataContext = viewModel };

         if(viewModel is RedisViewModel)
            return new RedisPanel { DataContext = viewModel };

         return null;
      }

      private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         if(e.PropertyName == nameof(ViewModel.SelectedAccount))
         {
            UpdateNonRecycledTabVisibility();
         }
      }

      private void UpdateNonRecycledTabVisibility()
      {
         int idx = GetNonRecycledTabIndexForViewModel(ViewModel.SelectedAccount);
         if(idx != -1)
         {
            ActiveNonRecycledTabIndex = idx;
         }
      }

      private int GetNonRecycledTabIndexForViewModel(IAccountPaneViewModel viewModel)
      {
         for(int i = 0; i < NonRecycledTabs.Children.Count; i++)
         {
            var element = (FrameworkElement)NonRecycledTabs.Children[i];
            var elementContext = (IAccountPaneViewModel)element.DataContext;
            if(viewModel == elementContext)
            {
               return i;
            }
         }

         return -1;
      }

      private int ActiveNonRecycledTabIndex
      {
         set
         {
            for(int i = 0; i < NonRecycledTabs.Children.Count; i++)
            {
               UIElement element = NonRecycledTabs.Children[i];
               element.Visibility = i == value ? Visibility.Visible : Visibility.Collapsed;
            }
         }
      }

#endregion

      private void FeedbackHeaderButton_Click(object sender, RoutedEventArgs e)
      {
         "https://github.com/aloneguid/StorAMP/issues".ShellOpen();
      }

      private void TabControl_DragOver(object sender, DragEventArgs e)
      {
         IAccountPaneViewModel pane = GetPaneUnderCursor(e);
         if(pane != null && pane.AcceptsDrop(e.ToDragData()))
         {
            e.Effects = DragDropEffects.Copy | DragDropEffects.Move;
         }
         else
         {
            e.Effects = DragDropEffects.None;
         }
      }

      private void TabControl_Drop(object sender, DragEventArgs e)
      {
         IAccountPaneViewModel targetPane = GetPaneUnderCursor(e);
         if(targetPane == null)
            return;

         DragData data = e.ToDragData();
         if(data == null)
            return;

         targetPane.DropDataAsync(data);
      }

      private IAccountPaneViewModel GetPaneUnderCursor(DragEventArgs e)
      {
         //get the visual element that is under the mouse
         /*HitTestResult r = VisualTreeHelper.HitTest(Tabs, e.GetPosition(Tabs));
         if(!(r.VisualHit is FrameworkElement fe))
            return null;

         return fe.DataContext as IAccountPaneViewModel;*/
         return null;
      }

      private void SideBar_ConnectedAccountDoubleTapped(ConnectedAccount connectedAccount)
      {
         ViewModel.AddTab(connectedAccount);
      }

      public void Invoke(Action action)
      {
         Dispatcher.BeginInvoke(action);
      }

   }
}
