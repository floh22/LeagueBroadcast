using LeagueBroadcast.Common.Events;
using LeagueBroadcast.Utils;
using System.Collections.Generic;
using System.Windows.Media;

namespace LeagueBroadcast.Client.Utils
{
    public class ClientConnectionStatus : ObservableObject
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

        public ClientConnectionStatus(Color textColor, string textContent, Color borderColor, double borderThickness, Color backgroundColor)
        {
            TextColor = new SolidColorBrush(textColor);
            TextContent = textContent;
            BorderColor = new SolidColorBrush(borderColor);
            BorderThickness = borderThickness;
            BackgroundColor = new SolidColorBrush(backgroundColor);
        }

        public static ClientConnectionStatus DISCONNECTED { get; set; } = new(Colors.White, "DISCONNECTED", Color.FromRgb(251, 105, 98), 3, Color.FromRgb(208, 126, 126));
        public static ClientConnectionStatus CONNECTING { get; set; } = new(Colors.White, "CONNECTING", Color.FromRgb(251, 218, 98), 3, Color.FromRgb(208, 196, 126));
        public static ClientConnectionStatus CONNECTED { get; set; } = new(Colors.White, "CONNECTED", Color.FromRgb(168, 228, 239), 3, Color.FromRgb(126, 172, 180));
        public static ClientConnectionStatus PREGAME { get; set; } = new(Colors.White, "CHAMP SELECT", Color.FromRgb(121, 222, 121), 3, Color.FromRgb(121, 222, 121));
        public static ClientConnectionStatus INGAME { get; set; } = new(Colors.White, "INGAME", Color.FromRgb(121, 222, 121), 3, Color.FromRgb(121, 222, 121));
        public static ClientConnectionStatus POSTGAME { get; set; } = new(Colors.White, "POST GAME", Color.FromRgb(121, 222, 121), 3, Color.FromRgb(121, 222, 121));

        public static Dictionary<ConnectionStatus, ClientConnectionStatus> ConnectionStatusMap { get; set; } = new Dictionary<ConnectionStatus, ClientConnectionStatus>() {
            {ConnectionStatus.Disconnected, DISCONNECTED},
            {ConnectionStatus.Connected, CONNECTED},
            {ConnectionStatus.Connecting, CONNECTING},
            {ConnectionStatus.PreGame, PREGAME},
            {ConnectionStatus.Ingame, INGAME},
            {ConnectionStatus.PostGame, POSTGAME},
        };
    }
}
