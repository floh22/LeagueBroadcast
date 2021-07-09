using LeagueBroadcast.MVVM.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LeagueBroadcast.MVVM.Controls
{
    //Modified from https://github.com/Tyrrrz/LightBulb/blob/master/LightBulb/Views/Controls/HotKeyTextBox.cs
    public class HotKeyTextBox : TextBox
    {
        public static readonly DependencyProperty HotKeyProperty =
            DependencyProperty.Register(nameof(HotKey), typeof(HotKey), typeof(HotKeyTextBox),
                new FrameworkPropertyMetadata(default(HotKey), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, HotKeyChanged));

        private static void HotKeyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is HotKeyTextBox control)
            {
                control.Text = control.HotKey.ToString();
            }
        }

        public HotKey HotKey
        {
            get => (HotKey)GetValue(HotKeyProperty);
            set => SetValue(HotKeyProperty, value);
        }

        public HotKeyTextBox()
        {
            IsReadOnly = true;
            IsReadOnlyCaretVisible = false;
            IsUndoEnabled = false;

            if (ContextMenu is not null)
                ContextMenu.Visibility = Visibility.Collapsed;

            Text = HotKey.ToString();
        }

        private static bool HasKeyChar(Key key) =>
            // A - Z
            key >= Key.A && key <= Key.Z ||
            // 0 - 9
            key >= Key.D0 && key <= Key.D9 ||
            // Numpad 0 - 9
            key >= Key.NumPad0 && key <= Key.NumPad9 ||
            // The rest
            key == Key.OemQuestion || key == Key.OemQuotes || key == Key.OemPlus || key == Key.OemOpenBrackets || key == Key.OemCloseBrackets || 
            key == Key.OemMinus || key == Key.DeadCharProcessed || key == Key.Oem1 || key == Key.Oem5 || key == Key.Oem7 || key == Key.OemPeriod || key == Key.OemComma || key == Key.Add || 
            key == Key.Divide || key == Key.Multiply || key == Key.Subtract || key == Key.Oem102 || key == Key.Decimal;

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            e.Handled = true;

            // Get modifiers and key data
            var modifiers = Keyboard.Modifiers;
            var key = e.Key;

            // If nothing was pressed - return
            if (key == Key.None)
                return;

            // If Alt is used as modifier - the key needs to be extracted from SystemKey
            if (key == Key.System)
                key = e.SystemKey;

            // If Delete/Backspace/Escape is pressed without modifiers - clear current value and return
            if ((key == Key.Delete || key == Key.Back || key == Key.Escape) && modifiers == ModifierKeys.None)
            {
                HotKey = HotKey.None;
                return;
            }

            // If no actual key was pressed - return
            if (key == Key.LeftCtrl ||
                key == Key.RightCtrl ||
                key == Key.LeftAlt ||
                key == Key.RightAlt ||
                key == Key.LeftShift ||
                key == Key.RightShift ||
                key == Key.LWin ||
                key == Key.RWin ||
                key == Key.Clear ||
                key == Key.OemClear ||
                key == Key.Apps)
            {
                return;
            }

            // If Enter/Space/Tab is pressed without modifiers - return
            if ((key == Key.Enter || key == Key.Space || key == Key.Tab) && modifiers == ModifierKeys.None)
                return;

            // If key has a character and pressed without modifiers or only with Shift - return
            if (HasKeyChar(key) && (modifiers == ModifierKeys.None || modifiers == ModifierKeys.Shift))
                return;

            // Set value
            HotKey = new HotKey(key, modifiers);
        }
    }
}
