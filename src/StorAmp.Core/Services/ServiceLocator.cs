using System;
using System.Collections.Generic;
using System.Text;

namespace StorAmp.Core.Services
{
   public static class ServiceLocator
   {
      private static readonly Dictionary<Type, object> _serviceTypeToImpl = new Dictionary<Type, object>();

      public static void Register<T>(object instance)
      {
         _serviceTypeToImpl[typeof(T)] = instance;
      }

      public static T GetInstance<T>()
      {
         _serviceTypeToImpl.TryGetValue(typeof(T), out object instance);
         return (T)instance;
      }

      #region [ Syntax Sugar Daddry Helpers ]

      public static IDialogService Dialogs => GetInstance<IDialogService>();

      public static IAppDialogsService AppDialogs => GetInstance<IAppDialogsService>();

      public static ITaskManagerService TaskManager => GetInstance<ITaskManagerService>();

      public static ISystemService SystemService => GetInstance<ISystemService>();

      #endregion
   }
}
