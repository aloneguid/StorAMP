using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Storage.Net.Blobs;

namespace StorAmp.Core.Model.Account
{
   public interface IConnectedAccountAction
   {
      string Name { get; }

      Symbol Icon { get; }

      bool CanExecute(IBlobStorage storage, Blob blob);

      bool CanExecute(ConnectedAccount connectedAccount);

      object[] ArgumentRange { get; }

      Task ExecuteAsync(IBlobStorage storage, Blob blob, object arg);

      Task ExecuteAsync(ConnectedAccount connectedAccount, object arg);
   }

   public abstract class ConnectedAccountAction : IConnectedAccountAction
   {
      public abstract string Name { get; }

      public abstract Symbol Icon { get; }

      public virtual object[] ArgumentRange => null;

      public virtual bool CanExecute(IBlobStorage storage, Blob blob) => false;
      public virtual bool CanExecute(ConnectedAccount connectedAccount) => false;
      public virtual Task ExecuteAsync(IBlobStorage storage, Blob blob, object arg) => Task.CompletedTask;
      public virtual Task ExecuteAsync(ConnectedAccount connectedAccount, object arg) => Task.CompletedTask;
   }
}
