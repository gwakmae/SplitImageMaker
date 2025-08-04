using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace SplitImageMaker.Models
{
    public class PanelInfo : INotifyPropertyChanged
    {
        private BitmapSource _image;
        private bool _isSelected;

        public int Row { get; set; }
        public int Column { get; set; }

        // ✅ FIX: GridService와의 호환성을 위해 속성 복원
        public double WidthRatio { get; set; }
        public double HeightRatio { get; set; }

        public BitmapSource Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ImageDimensionsText));
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        public string DisplayText => $"Panel {Row + 1}-{Column + 1}";
        public string ImageDimensionsText => Image != null ? $"{Image.PixelWidth} x {Image.PixelHeight}" : "Empty";

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}