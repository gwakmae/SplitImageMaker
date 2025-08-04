#nullable disable
using SplitImageMaker.Models;
using SplitImageMaker.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace SplitImageMaker.Views
{
    public class IsSelectedToBrushConverter : IValueConverter { public object Convert(object value, Type t, object p, CultureInfo c) => (bool)value ? Brushes.DodgerBlue : Brushes.Gray; public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException(); }
    public class IsSelectedToThicknessConverter : IValueConverter { public object Convert(object value, Type t, object p, CultureInfo c) => new Thickness((bool)value ? 3 : 1); public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException(); }
    public class NullToVisibilityConverter : IValueConverter { public object Convert(object value, Type t, object p, CultureInfo c) => value == null ? Visibility.Visible : Visibility.Collapsed; public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException(); }

    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) { RebuildGridStructure(); }
        private void Window_KeyDown(object sender, KeyEventArgs e) { if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.V) _viewModel.PasteImage(); }

        private void SetGrid_Click(object sender, RoutedEventArgs e) { _viewModel.CreateGrid(); RebuildGridStructure(); }
        private void PasteImage_Click(object sender, RoutedEventArgs e) { _viewModel.PasteImage(); }
        private void SaveCombinedImage_Click(object sender, RoutedEventArgs e) { _viewModel.SaveCombinedImage(); }

        private void RebuildGridStructure()
        {
            MainSplitGrid.Children.Clear(); MainSplitGrid.RowDefinitions.Clear(); MainSplitGrid.ColumnDefinitions.Clear();
            var config = _viewModel.GridConfig;
            for (int i = 0; i < config.Rows; i++) MainSplitGrid.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < config.Columns; i++) MainSplitGrid.ColumnDefinitions.Add(new ColumnDefinition());
            foreach (var panelModel in _viewModel.Panels)
            {
                var panelContent = CreateBoundPanelContent(panelModel);
                Grid.SetRow(panelContent, panelModel.Row); Grid.SetColumn(panelContent, panelModel.Column);
                MainSplitGrid.Children.Add(panelContent);
            }
        }

        private FrameworkElement CreateBoundPanelContent(PanelInfo panelModel)
        {
            var border = new Border { Background = Brushes.WhiteSmoke, Margin = new Thickness(1), Cursor = Cursors.Hand, Tag = panelModel };
            border.SetBinding(Border.BorderBrushProperty, new Binding("IsSelected") { Source = panelModel, Converter = new IsSelectedToBrushConverter() });
            border.SetBinding(Border.BorderThicknessProperty, new Binding("IsSelected") { Source = panelModel, Converter = new IsSelectedToThicknessConverter() });
            var grid = new Grid();
            var image = new Image { Stretch = Stretch.Uniform };
            image.SetBinding(Image.SourceProperty, new Binding("Image") { Source = panelModel });
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
            grid.Children.Add(image);
            var textBlock = new TextBlock { Text = $"{panelModel.DisplayText}", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Foreground = Brushes.LightGray, IsHitTestVisible = false };
            textBlock.SetBinding(VisibilityProperty, new Binding("Image") { Source = panelModel, Converter = new NullToVisibilityConverter() });
            grid.Children.Add(textBlock);
            border.Child = grid;
            border.MouseLeftButtonDown += (s, e) => { if (s is FrameworkElement fe && fe.Tag is PanelInfo pm) _viewModel.SelectedPanel = pm; };
            return border;
        }
    }
}
