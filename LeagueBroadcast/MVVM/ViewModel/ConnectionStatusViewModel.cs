using LeagueBroadcast.MVVM.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace LeagueBroadcast.MVVM.ViewModel
{
    class ConnectionStatusViewModel : ObservableObject
    {
        private string _textContent;

        public string TextContent
        {
            get { return _textContent; }
            set { _textContent = value; OnPropertyChanged(); }
        }

        private SolidColorBrush _textColor;

        public SolidColorBrush TextColor
        {
            get { return _textColor; }
            set { _textColor = value; OnPropertyChanged(); }
        }


        private SolidColorBrush _borderColor;

        public SolidColorBrush BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; OnPropertyChanged(); }
        }

        private double _borderThickness;

        public double BorderThickness
        {
            get { return _borderThickness; }
            set { _borderThickness = value; OnPropertyChanged(); }
        }


        private SolidColorBrush _backgroundColor;

        public SolidColorBrush BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; OnPropertyChanged(); }
        }

        public ConnectionStatusViewModel() { }

        public ConnectionStatusViewModel(Color textColor, string textContent, Color borderColor, double borderThickness, Color backgroundColor)
        {
            TextColor = new SolidColorBrush(textColor);
            TextContent = textContent;
            BorderColor = new SolidColorBrush(borderColor);
            BorderThickness = borderThickness;
            BackgroundColor = new SolidColorBrush(backgroundColor);
        }

        public static ConnectionStatusViewModel DISCONNECTED = new(Colors.White, "DISCONNECTED", Color.FromRgb(251, 105, 98), 3, Color.FromRgb(208, 126, 126));
        public static ConnectionStatusViewModel LCU = new(Colors.White, "CLIENT CONNECTED", Color.FromRgb(168, 228, 239), 3, Color.FromRgb(126, 172, 180));
        public static ConnectionStatusViewModel CONNECTED = new(Colors.White, "CONNECTED", Color.FromRgb(121, 222, 121), 3, Color.FromRgb(12, 192, 120));
    }
}
