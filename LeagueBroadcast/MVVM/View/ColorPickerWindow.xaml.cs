using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.MVVM.ViewModel;
using LeagueBroadcast.OperatingSystem;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LeagueBroadcast.MVVM.View
{
    /// <summary>
    /// Interaction logic for ColorPickerWindow.xaml
    /// </summary>
    public partial class ColorPickerWindow : Window
    {
        private TeamConfigViewModel _teamConfigVM;
        private PickBanConfigViewModel _pickBanVM;
        private ColorPickerViewModel VM;

        private string mapSide;
        private bool isDefault = false;

        public ColorPickerWindow(TeamConfigViewModel configVM, string mapSide)
        {
            this.mapSide = mapSide;
            InitializeComponent();
            VM = (ColorPickerViewModel)DataContext;
            VM.SelectedColor = configVM.Color;
            VM.UpdateColorValues();
            _teamConfigVM = configVM;
        }

        public ColorPickerWindow(PickBanConfigViewModel pickBanVM, string mapSide)
        {
            this.mapSide = mapSide;
            this._pickBanVM = pickBanVM;
            this.isDefault = true;
            InitializeComponent();
            VM = (ColorPickerViewModel)DataContext;
            VM.SelectedColor = mapSide == "blue" ? ConfigController.Component.PickBan.DefaultBlueColor.ToColor() : ConfigController.Component.PickBan.DefaultRedColor.ToColor();
            VM.UpdateColorValues();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (_teamConfigVM == null && _pickBanVM == null)
            {
                Log.Warn("Tried updating team color without reference to team to update");
                Close();
                return;
            }

            if (isDefault)
            {
                if ((mapSide == "blue" ? _pickBanVM.DefaultBlueColor : _pickBanVM.DefaultRedColor) == VM.SelectedColor)
                {
                    Log.Warn("Tried updating color to same color. Ingoring color change");
                    Close();
                }

                if (mapSide == "blue")
                {
                    _pickBanVM.DefaultBlueColor = VM.SelectedColor;
                }
                else
                {
                    _pickBanVM.DefaultRedColor = VM.SelectedColor;
                }

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


            BroadcastController.Instance.IGController.gameState.UpdateTeamColors();

            Close();
        }

        private bool IsTextAccepted(TextBox sender, String text)
        {
            string toCheck = "";
            if(sender.SelectedText != "")
            {
                string nonSelected = sender.Text.Remove(sender.SelectionStart, sender.SelectedText.Length);
                toCheck = nonSelected.Insert(sender.SelectionStart, text);
            } else
            {
                toCheck = sender.Text.Insert(sender.CaretIndex, text);
            }
            return (int.TryParse(toCheck, out int res) && res >= 0 && res < 256) ;
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
