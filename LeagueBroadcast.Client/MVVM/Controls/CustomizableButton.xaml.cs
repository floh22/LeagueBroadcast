using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LeagueBroadcast.Client.MVVM.Controls
{
    /// <summary>
    /// Standard button with extensions
    /// </summary>
    public partial class CustomizableButton : Button
    {
        static readonly Brush? DefaultHoverBackgroundValue = new BrushConverter().ConvertFromString("#FFBEE6FD") as Brush;

        public CustomizableButton()
        {
        }

        public Brush HoverBackground
        {
            get { return (Brush)GetValue(HoverBackgroundProperty); }
            set { SetValue(HoverBackgroundProperty, value); }
        }
        public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.Register(
          "HoverBackground", typeof(Brush), typeof(CustomizableButton), new PropertyMetadata(DefaultHoverBackgroundValue));
    }
}
