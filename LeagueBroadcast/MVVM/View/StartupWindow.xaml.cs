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
    /// Interaction logic for StartupWindow.xaml
    /// </summary>
    public partial class StartupWindow : Window
    {
        public StartupWindow()
        {
            InitializeComponent();
        }

        public StartupViewModel GETDataContext()
        {
            return (StartupViewModel) DataContext;
        }

        private void UpdateNow_Click(object sender, RoutedEventArgs e)
        {
            var ctx = (StartupViewModel) this.DataContext;
            ctx.Update?.Invoke(null, EventArgs.Empty);

        }

        private void UpdateSkip_Click(object sender, RoutedEventArgs e)
        {
            var ctx = (StartupViewModel)this.DataContext;
            ctx.SkipUpdate?.Invoke(null, EventArgs.Empty);
        }
    }
}
