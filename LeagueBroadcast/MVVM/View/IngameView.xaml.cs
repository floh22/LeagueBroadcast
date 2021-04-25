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

namespace LeagueBroadcast.MVVM.View
{
    /// <summary>
    /// Interaction logic for IngameView.xaml
    /// </summary>
    public partial class IngameView : UserControl
    {
        public IngameView()
        {
            InitializeComponent();

            //IDEA: 4 Way Split Event Types: Objective, Player, Team, Other
            //Scroll Up/Down to get to team overview. Maybe different page?
        }
    }
}
