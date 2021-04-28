using LeagueBroadcast.MVVM.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace LeagueBroadcast.MVVM.ViewModel
{
    class ColorPickerViewModel : ObservableObject
    {

        private byte _r;

        public byte R
        {
            get { return _r; }
            set { _r = value; OnPropertyChanged(); UpdateColor(); }
        }

        private byte _g;

        public byte G
        {
            get { return _g; }
            set { _g = value; OnPropertyChanged(); UpdateColor(); }
        }

        private byte _b;

        public byte B
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
            set { _colorBrush = value; }
        }


        public ColorPickerViewModel()
        {

        }

        private void UpdateColor()
        {
            _selectedColor = Color.FromRgb(R, G, B);
            ColorBrush = new SolidColorBrush(SelectedColor);
            OnPropertyChanged("SelectedColor");
        }

        private void UpdateColorValues()
        {
            _r = SelectedColor.R;
            _g = SelectedColor.G;
            _b = SelectedColor.B;
            OnPropertyChanged("R");
            OnPropertyChanged("G");
            OnPropertyChanged("B");
        }
    }
}
