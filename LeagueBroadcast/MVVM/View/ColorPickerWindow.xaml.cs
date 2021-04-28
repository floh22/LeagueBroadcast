using LeagueBroadcast.ChampSelect.Data.Config;
using LeagueBroadcast.Common;
using LeagueBroadcast.MVVM.ViewModel;
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
        public ColorPickerWindow()
        {
            InitializeComponent();

        }

        public ColorPickerWindow(TeamConfigViewModel configVM) : this()
        {
            VM = (ColorPickerViewModel)DataContext;
            VM.SelectedColor = configVM.Color;
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

        private void RInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            var parseRes = Byte.TryParse(textBox.Text, out var rValue);
            if(!parseRes)
            {
                textBox.Text = VM.R + "";
                return;
            }

            if(VM == null)
                VM = (ColorPickerViewModel)DataContext;
            VM.R = rValue;
        }

        private void BInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            var parseRes = Byte.TryParse(textBox.Text, out var bValue);
            if (!parseRes)
            {
                textBox.Text = VM.B + "";
                return;
            }

            if (VM == null)
                VM = (ColorPickerViewModel)DataContext;

            VM.B = bValue;
        }

        private void GInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            var parseRes = Byte.TryParse(textBox.Text, out var gValue);
            if (!parseRes)
            {
                textBox.Text = VM.G + "";
                return;
            }

            if (VM == null)
                VM = (ColorPickerViewModel)DataContext;

            VM.G = gValue;
        }
    }
}
