using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SplitImageMaker.Models
{
    public class GridConfiguration : INotifyPropertyChanged // INotifyPropertyChanged 인터페이스 추가
    {
        public int Rows { get; set; } = 2;
        public int Columns { get; set; } = 2;
        public List<double> ColumnRatios { get; set; } = new List<double> { 1.0, 1.0 };
        public List<double> RowRatios { get; set; } = new List<double> { 1.0, 1.0 };
        public int OutputCount { get; set; } = 4;

        // 새로 추가: 최종 출력 이미지 크기 설정
        private int _outputWidth = 1920;
        public int OutputWidth
        {
            get => _outputWidth;
            set { _outputWidth = value; OnPropertyChanged(); OnPropertyChanged(nameof(AspectRatio)); OnPropertyChanged(nameof(AspectRatioText)); }
        }

        private int _outputHeight = 1080;
        public int OutputHeight
        {
            get => _outputHeight;
            set { _outputHeight = value; OnPropertyChanged(); OnPropertyChanged(nameof(AspectRatio)); OnPropertyChanged(nameof(AspectRatioText)); }
        }

        private bool _maintainAspectRatio = true;
        public bool MaintainAspectRatio
        {
            get => _maintainAspectRatio;
            set { _maintainAspectRatio = value; OnPropertyChanged(); }
        }

        public void SetEqualRatios()
        {
            ColumnRatios = Enumerable.Repeat(1.0, Columns).ToList();
            RowRatios = Enumerable.Repeat(1.0, Rows).ToList();

            // 속성 변경 알림 (INotifyPropertyChanged 구현 시)
            OnPropertyChanged(nameof(WidthRatioText));
            OnPropertyChanged(nameof(HeightRatioText));
        }

        // ✅ 읽기 전용 속성들 (OneWay 바인딩용)
        public string GetWidthRatioText() => string.Join(":", ColumnRatios.Select(r => r.ToString("F1")));
        public string GetHeightRatioText() => string.Join(":", RowRatios.Select(r => r.ToString("F1")));

        // ✅ 쓰기 가능한 속성들 추가 (TwoWay 바인딩용)
        public string WidthRatioText
        {
            get => GetWidthRatioText();
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    try
                    {
                        var ratios = value.Split(':').Select(r => double.Parse(r.Trim())).ToList();
                        if (ratios.Count > 0)
                        {
                            ColumnRatios = ratios;
                            Columns = ratios.Count;
                            OnPropertyChanged();
                            OnPropertyChanged(nameof(Columns));
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"비율 형식 오류: {ex.Message}");
                    }
                }
            }
        }

        public string HeightRatioText
        {
            get => GetHeightRatioText();
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    try
                    {
                        var ratios = value.Split(':').Select(r => double.Parse(r.Trim())).ToList();
                        if (ratios.Count > 0)
                        {
                            RowRatios = ratios;
                            Rows = ratios.Count;
                            OnPropertyChanged();
                            OnPropertyChanged(nameof(Rows));
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"비율 형식 오류: {ex.Message}");
                    }
                }
            }
        }

        // 종횡비 계산
        public double AspectRatio => (double)OutputWidth / OutputHeight;
        public string AspectRatioText => $"{OutputWidth}:{OutputHeight} ({AspectRatio:F2})";

        // INotifyPropertyChanged 구현 (필요한 경우)
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}