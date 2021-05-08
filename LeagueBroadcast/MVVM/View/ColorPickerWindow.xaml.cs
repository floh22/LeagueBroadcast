using LeagueBroadcast.ChampSelect.Data.Config;
using LeagueBroadcast.Common;
using LeagueBroadcast.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LeagueBroadcast.MVVM.View
{
    /// <summary>
    /// Interaction logic for ColorPickerWindow.xaml
    /// </summary>
    public partial class ColorPickerWindow : Window
    {
        private TeamConfigViewModel _teamConfigVM;
        private ColorPickerViewModel VM;

        public ColorPickerWindow(TeamConfigViewModel configVM)
        {
            InitializeComponent();
            VM = (ColorPickerViewModel)DataContext;
            VM.SelectedColor = configVM.Color;
            VM.UpdateColorValues();
            _teamConfigVM = configVM;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (_teamConfigVM == null)
            {
                Log.Warn("Tried updating team color without reference to team to update");
                Close();
                return;
            }
            if (_teamConfigVM.Color == VM.SelectedColor)
            {
                Log.Warn("Tried updating color to same color. Ingoring color change");
                Close();
                return;
            }
            _teamConfigVM.Color = VM.SelectedColor;
            Close();
        }

        private bool IsTextAccepted(TextBox sender, String text)
        {
            return (int.TryParse(sender.Text + text, out int res) && res > 0 && res < 256) || (text == "0" && sender.Text.Length == 0);
        }

        private void PreviewTextInputHandler(Object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAccepted((TextBox)sender, e.Text);
        }
 
        private void PastingHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAccepted((TextBox)sender, text)) e.CancelCommand();
            }
            else e.CancelCommand();
        }
    }
}
