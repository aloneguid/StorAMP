using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using StorAmp.Core.Model;

namespace StorAmp.Wpf.Controls
{
   /// <summary>
   /// Interaction logic for WinUISymbol.xaml
   /// </summary>
   public partial class WinUISymbol : UserControl, INotifyPropertyChanged
   {
      private Symbol _symbol;

      public WinUISymbol()
      {
         InitializeComponent();

         DataContext = this;

         Symbol = Symbol.Accept;
      }

      public Symbol Symbol
      {
         get => _symbol;
         set
         {
            _symbol = value;

            Char = new string((char)(int)value, 1);
         }
      }

      private string _char;
      public string Char
      {
         get => _char;
         set
         {
            _char = value;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Char)));
         }
      }

      public event PropertyChangedEventHandler PropertyChanged;
   }
}
