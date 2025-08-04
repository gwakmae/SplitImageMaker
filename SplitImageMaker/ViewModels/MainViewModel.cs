using SplitImageMaker.Models;
using SplitImageMaker.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace SplitImageMaker.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ImageService _imageService = new ImageService();
        private readonly GridService _gridService = new GridService();
        private readonly FileService _fileService = new FileService();

        private GridConfiguration _gridConfig = new GridConfiguration();
        private PanelInfo _selectedPanel;
        private string _statusText = "Ready. Select a panel and press Ctrl+V to paste an image.";
        private BitmapSource _combinedPreviewImage;

        public MainViewModel()
        {
            Panels = new ObservableCollection<PanelInfo>();
            CreateGrid();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<PanelInfo> Panels { get; }
        public GridConfiguration GridConfig { get => _gridConfig; set { _gridConfig = value; OnPropertyChanged(); } }
        public PanelInfo SelectedPanel
        {
            get => _selectedPanel;
            set
            {
                if (_selectedPanel != null) _selectedPanel.IsSelected = false;
                _selectedPanel = value;
                if (_selectedPanel != null) _selectedPanel.IsSelected = true;
                OnPropertyChanged();
            }
        }
        public string StatusText { get => _statusText; set { _statusText = value; OnPropertyChanged(); } }
        public BitmapSource CombinedPreviewImage { get => _combinedPreviewImage; private set { _combinedPreviewImage = value; OnPropertyChanged(); } }

        public void CreateGrid()
        {
            // ✅ FIX: 패널을 생성하기 전에 비율 리스트의 크기를 동기화하여 오류를 방지합니다.
            GridConfig.SetEqualRatios();

            // ✅ FIX: 메모리 누수 방지를 위해 이전 패널의 이벤트 구독 해제
            foreach (var panel in Panels)
            {
                panel.PropertyChanged -= Panel_PropertyChanged;
            }

            Panels.Clear();
            var newPanels = _gridService.CreatePanels(GridConfig);
            foreach (var panel in newPanels)
            {
                // ✅ NEW: 새 패널의 속성 변경 이벤트를 구독하여 캡션 변경 감지
                panel.PropertyChanged += Panel_PropertyChanged;
                Panels.Add(panel);
            }
            UpdateCombinedPreview();
        }

        // ✅ NEW: 패널 속성 변경 시 호출될 이벤트 핸들러
        private void Panel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // 캡션이 변경되면 미리보기 업데이트
            if (e.PropertyName == nameof(PanelInfo.Caption))
            {
                UpdateCombinedPreview();
            }
        }

        public void PasteImage()
        {
            if (SelectedPanel == null)
            {
                StatusText = "Please select a panel first before pasting.";
                return;
            }

            var img = _imageService.GetImageFromClipboard();
            if (img == null)
            {
                StatusText = "No image on clipboard.";
                return;
            }

            SelectedPanel.Image = img;
            StatusText = $"Pasted image to {SelectedPanel.DisplayText}.";
            UpdateCombinedPreview();
        }

        public void SaveCombinedImage()
        {
            var finalImage = _imageService.CombineImages(Panels, GridConfig.Rows, GridConfig.Columns);
            if (finalImage == null) { StatusText = "There are no images to combine."; return; }
            bool success = _fileService.SaveCombinedImage(finalImage);
            StatusText = success ? "Combined image saved successfully." : "Save cancelled or failed.";
        }

        private void UpdateCombinedPreview()
        {
            CombinedPreviewImage = _imageService.CombineImages(Panels, GridConfig.Rows, GridConfig.Columns);
        }
    }
}