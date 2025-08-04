using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace SplitImageMaker.Models
{
    public class PanelInfo : INotifyPropertyChanged
    {
        private BitmapSource _image;
        private bool _isSelected;
        private string _caption = string.Empty; // 캡션 필드 추가

        public int Row { get; set; }
        public int Column { get; set; }

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

        // ✅ NEW: 캡션 속성 추가
        public string Caption
        {
            get => _caption;
            set
            {
                _caption = value;
                OnPropertyChanged();
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