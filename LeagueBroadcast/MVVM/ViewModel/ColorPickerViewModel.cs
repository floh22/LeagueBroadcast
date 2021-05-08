using LeagueBroadcast.MVVM.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace LeagueBroadcast.MVVM.ViewModel
{
    class ColorPickerViewModel : ObservableObject
    {

        private string _r;

        public string R
        {
            get { return _r; }
            set { _r = value; OnPropertyChanged(); UpdateColor(); }
        }

        private string _g;

        public string G
        {
            get { return _g; }
            set { _g = value; OnPropertyChanged(); UpdateColor(); }
        }

        private string _b;

        public string B
        {
            get { return _b; }
            set { _b = value; OnPropertyChanged(); UpdateColor(); }
        }

        private Color _selectedColor;

        public Color SelectedColor
        {
            get { return _selectedColor; }
            set { _selectedColor = value; OnPropertyChanged(); UpdateColorValues(); }
        }

        private SolidColorBrush _colorBrush;

        public SolidColorBrush ColorBrush
        {
            get { return _colorBrush; }
            set { _colorBrush = value; OnPropertyChanged(); }
        }

        public ColorPickerViewModel()
        {

        }

        public void UpdateColor()
        {
            byte r, g, b;
            if (R.Length == 0)
            {
                r = 0;
            }
            else
            {
                r = byte.Parse(R);
            }

            if (G.Length == 0)
            {
                g = 0;
            }
            else
            {
                g = byte.Parse(G);
            }

            if (B.Length == 0)
            {
                b = 0;
            }
            else
            {
                b = byte.Parse(B);
            }

            _selectedColor = Color.FromRgb(r, g, b);
            ColorBrush = new SolidColorBrush(SelectedColor);
            OnPropertyChanged("SelectedColor");
        }

        public void UpdateColorValues()
        {
            _r = SelectedColor.R.ToString();
            _g = SelectedColor.G.ToString();
            _b = SelectedColor.B.ToString();
            ColorBrush = new SolidColorBrush(SelectedColor);
            OnPropertyChanged("R");
            OnPropertyChanged("G");
            OnPropertyChanged("B");
        }
    }
}
