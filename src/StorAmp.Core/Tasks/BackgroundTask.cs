using System;
using System.ComponentModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace StorAmp.Core.Tasks
{
   public enum BackgroundTaskStatus
   {
      Scheduled,

      Running,

      Succeeded,

      Failed
   }

   public abstract class BackgroundTask : ViewModelBase
   {
      private DateTime _lastUpdateTime = DateTime.UtcNow;

      protected BackgroundTask(string typeName)
      {
         CreatedDate = DateTime.Now;
         TypeName = typeName;
      }

      public abstract Task ExecuteAsync();

      protected void UpdateProgress(long current, long max)
      {
         ProgressPercentage = 100.0 * current / max;
         ProgressPercentageInt = (int)ProgressPercentage;
      }

      #region [ Properties ]

      private DateTime _createdDate;
      public DateTime CreatedDate
      {
         get => _createdDate;
         set { Set(() => CreatedDate, ref _createdDate, value); }
      }


      private string _typeName;
      public string TypeName
      {
         get => _typeName;
         set { Set(() => TypeName, ref _typeName, value); }
      }


      private string _abstract;
      public string Abstract
      {
         get => _abstract;
         set { Set(() => Abstract, ref _abstract, value); }
      }

      private string _message;
      public string Message
      {
         get => _message;
         set
         {
            Set(() => Message, ref _message, value);
         }
      }

      private bool _isIndeterminate;
      public bool IsIndeterminate
      {
         get => _isIndeterminate;
         set { Set(() => IsIndeterminate, ref _isIndeterminate, value); }
      }

      private double _progress;
      public double ProgressPercentage
      {
         get => _progress;
         set { Set(() => ProgressPercentage, ref _progress, value); }
      }


      private int _progressInt;
      public int ProgressPercentageInt
      {
         get => _progressInt;
         set { Set(() => ProgressPercentageInt, ref _progressInt, value); }
      }

      private BackgroundTaskStatus _status;
      public BackgroundTaskStatus Status
      {
         get => _status;
         set
         {
            Set(() => Status, ref _status, value);
            IsComplete = Status == BackgroundTaskStatus.Succeeded || Status == BackgroundTaskStatus.Failed;
         }
      }

      private bool _isComplete;
      public bool IsComplete
      {
         get => _isComplete;
         set { Set(() => IsComplete, ref _isComplete, value); }
      }


      private Exception _error;
      public Exception Error
      {
         get => _error;
         set { Set(() => Error, ref _error, value); HasError = value != null; }
      }


      private bool _hasError;
      public bool HasError
      {
         get => _hasError;
         set { Set(() => HasError, ref _hasError, value); }
      }

      #endregion
   }
}
