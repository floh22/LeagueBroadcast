using System.Windows;

namespace LeagueBroadcast.Client.MVVM.Core.Behavior
{
    public class SmoothScrollViewer : System.Windows.Controls.ScrollViewer
    {
        public SmoothScrollViewer()
        {
            Loaded += ScrollViewer_Loaded;
        }

        private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollInfo = new ScrollInfoAdapter(ScrollInfo);
        }
    }

}
