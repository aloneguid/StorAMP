using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Humanizer;
using StorAmp.Core.Model.Messages;
using StorAmp.Core.Services;
using StorAmp.Core.Tasks;
using EventLog = Serilog.EventLog;

namespace StorAmp.Core.ViewModel
{
   public class TaskListViewModel : ViewModelBase, ITaskManagerService
   {
      public event Action<BackgroundTask> OnNewTask;

      public ObservableCollection<BackgroundTask> Tasks { get; } = new ObservableCollection<BackgroundTask>();

      public TaskListViewModel()
      {
         if(!IsInDesignMode)
         {
            ServiceLocator.Register<ITaskManagerService>(this);
            Messenger.Default.Register<BackgroundTaskMessage>(this, TaskAdded);
         }
      }

      private void TaskAdded(BackgroundTaskMessage message)
      {
         if(GlobalSettings.Default.OpenTaskPaneOnTaskAdded)
         {
            ShowDetails = true;
         }
      }

      public async Task ScheduleAsync(BackgroundTask task)
      {
         EventLog.LogEvent("scheduleTask", "{type}", task.TypeName);

         task.Status = BackgroundTaskStatus.Scheduled;
         
         // send message with MVVM messenger
         Messenger.Default.Send(new BackgroundTaskMessage(task));

         Tasks.Insert(0, task);
         OnNewTask?.Invoke(task);
         //ExecuteWithTracking(task).Forget();
         await Task.Factory.StartNew(() => ExecuteWithTracking(task), TaskCreationOptions.LongRunning);
      }

      private async Task ExecuteWithTracking(BackgroundTask task)
      {
         try
         {
            task.Status = BackgroundTaskStatus.Running;
            UpdateVisualStatus();

            await task.ExecuteAsync();
            task.Status = BackgroundTaskStatus.Succeeded;

            //remove from list on success
            //Tasks.Remove(task);
         }
         catch(Exception ex)
         {
            task.Error = ex;
            task.Status = BackgroundTaskStatus.Failed;
         }
         finally
         {
            //ClearAllCompletedCommand.RaiseCanExecuteChanged();
            UpdateVisualStatus();
         }
      }

      private void UpdateVisualStatus()
      {
         int failedCount = Tasks.Count(t => t.Status == BackgroundTaskStatus.Failed);
         int runningCount = Tasks.Count(t => t.Status == BackgroundTaskStatus.Running);

         string status = runningCount == 0
            ? Strings.TaskManager_NoTasks
            : string.Format(Strings.TaskManager_NTasks, Strings.TaskManager_Task.ToQuantity(runningCount));

         if(failedCount > 0)
         {
            status += ", ";
            status += string.Format(Strings.TaskManager_NFailed, failedCount);
         }

         ShortStatusText = status;
         IsRunning = runningCount > 0;
         HasTasks = Tasks.Count > 0;
      }

      #region [ Commands ]

      private RelayCommand _showDetailsCommand;
      public RelayCommand ShowDetailsCommand
      {
         get
         {
            return _showDetailsCommand
                ?? (_showDetailsCommand = new RelayCommand(
                () =>
                {
                   ShowDetails = !ShowDetails;
                },
                () => true));
         }
      }

      private RelayCommand _clearTasksCommand;
      public RelayCommand ClearTasksCommand
      {
         get
         {
            return _clearTasksCommand
                ?? (_clearTasksCommand = new RelayCommand(
                () =>
                {
                   Tasks.Clear();
                   UpdateVisualStatus();
                }));
         }
      }

      #endregion

      #region [ Properties ]

      private bool _showDetails;
      public bool ShowDetails
      {
         get => _showDetails;
         set { Set(() => ShowDetails, ref _showDetails, value); }
      }


      private string _shortStatusText;
      public string ShortStatusText
      {
         get => _shortStatusText;
         set { Set(() => ShortStatusText, ref _shortStatusText, value); }
      }


      private bool _isRunning;
      public bool IsRunning
      {
         get => _isRunning;
         set { Set(() => IsRunning, ref _isRunning, value); }
      }


      private bool _hasTasks;
      public bool HasTasks
      {
         get => _hasTasks;
         set { Set(() => HasTasks, ref _hasTasks, value); }
      }

      #endregion
   }
}
