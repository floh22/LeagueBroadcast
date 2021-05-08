using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LeagueBroadcast.MVVM.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LocationChanged += new EventHandler(Window_LocationChanged);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void ExitApp_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeApp_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            foreach (Window win in this.OwnedWindows)
            {
                win.Top = (this.Top + this.Height / 2) - win.Height / 2;
                win.Left = (this.Left + this.Width / 2) - win.Width / 2;
            }
        }

        public void SetHomeSelected()
        {
            HomeButton.IsChecked = true;
        }

        public void SetPickBanSelected()
        {
            PBButton.IsChecked = true;
        }

        public void SetIngameSelected()
        {
            IGButton.IsChecked = true;
        }

        public void SetPostGameSelected()
        {
            PGButton.IsChecked = true;
        }
    }
}
