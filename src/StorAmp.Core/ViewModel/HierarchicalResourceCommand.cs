using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Humanizer;
using NetBox.Extensions;
using StorAmp.Core.Model;

namespace StorAmp.Core.ViewModel
{
   public class HierarchicalResourceCommandGroup : ViewModelBase
   {
      public List<HierarchicalResourceCommand> Commands { get; } = new List<HierarchicalResourceCommand>();

      public HierarchicalResourceCommandGroup(params HierarchicalResourceCommand[] commands)
      {
         foreach(HierarchicalResourceCommand command in commands)
         {
            Commands.Add(command);
         }
      }
   }

   public class HierarchicalResourceCommand : ViewModelBase
   {
      public HierarchicalResourceCommand(string displayName, Symbol iconSymbol, Func<Task> command)
         : this(displayName, iconSymbol, new RelayCommand(() => command().Forget(), true))
      {
      }

      public HierarchicalResourceCommand(string displayName, Symbol iconSymbol, ICommand command)
      {
         DisplayName = displayName.Humanize(LetterCasing.Title);
         IconSymbol = iconSymbol;
         Command = command;
      }

      public int Priority { get; set; }

      public string DisplayName { get; set; }

      public Symbol IconSymbol { get; set; }

      public ICommand Command { get; set; }

      public object Parameter { get; set; }
   }
}
