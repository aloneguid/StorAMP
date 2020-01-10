using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Command;
using NetBox.Extensions;
using StorAmp.Core.Services;
using StorAmp.Core.Tasks;
using StorAmp.Core.ViewModel.Msg;

namespace StorAmp.Wpf.Wpf.Msg
{
   /// <summary>
   /// Interaction logic for MessengerPanel.xaml
   /// </summary>
   public partial class MessengerPanel : UserControl
   {
      private StressChannelViewModel _lastStressVm;

      public MessengerPanel()
      {
         InitializeComponent();
      }

      public MessengerViewModel ViewModel => (MessengerViewModel)DataContext;

      private async void StressChannel_Click(object sender, RoutedEventArgs e)
      {
         _lastStressVm = new StressChannelViewModel(ViewModel.Messenger, ViewModel.SelectedChannelName);

         await ServiceLocator.Dialogs.ShowDialogAsync("Stress Messenger Channel",
            new StressChannel { DataContext = _lastStressVm },
            "Stress", ScheduleStressCommand);
      }

      private RelayCommand _ScheduleStressCommand;
      public RelayCommand ScheduleStressCommand
      {
         get
         {
            return _ScheduleStressCommand
                ?? (_ScheduleStressCommand = new RelayCommand(
                () =>
                {
                   ServiceLocator.TaskManager.ScheduleAsync(
                      new StressMessengerChannelTask(ViewModel.Messenger, ViewModel.SelectedChannelName, _lastStressVm.MessagesToSend))
                   .Forget();
                }));
         }
      }
   }
}
