using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace StorAmp.Core.ViewModel.Account
{
   public class AddAwsAccountViewModel : ViewModelBase
   {
      #region [ Commands ]

      private RelayCommand _AddCommand;
      public RelayCommand AddCommand
      {
         get
         {
            return _AddCommand
                ?? (_AddCommand = new RelayCommand(
                () =>
                {

                },
                () => true));
         }
      }

      #endregion
   }
}
