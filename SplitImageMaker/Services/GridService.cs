using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SplitImageMaker.Models;

namespace SplitImageMaker.Services
{
    public class GridService
    {
        public ObservableCollection<PanelInfo> CreatePanels(GridConfiguration config)
        {
            var panels = new ObservableCollection<PanelInfo>();

            for (int r = 0; r < config.Rows; r++)
            {
                for (int c = 0; c < config.Columns; c++)
                {
                    panels.Add(new PanelInfo
                    {
                        Row = r,
                        Column = c,
                        WidthRatio = config.ColumnRatios[c],
                        HeightRatio = config.RowRatios[r]
                    });
                }
            }

            return panels;
        }

        public void SetupGrid(Grid mainGrid, GridConfiguration config)
        {
            mainGrid.Children.Clear();
            mainGrid.RowDefinitions.Clear();
            mainGrid.ColumnDefinitions.Clear();

            // 행/열 정의
            for (int i = 0; i < config.Rows; i++)
            {
                mainGrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(config.RowRatios[i], GridUnitType.Star)
                });
            }

            for (int i = 0; i < config.Columns; i++)
            {
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(config.ColumnRatios[i], GridUnitType.Star)
                });
            }
        }

        public void AddGridSplitters(Grid mainGrid, GridConfiguration config)
        {
            // 세로 분할선
            for (int c = 0; c < config.Columns - 1; c++)
            {
                var vSplitter = new GridSplitter
                {
                    Width = 5,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Background = Brushes.LightGray
                };
                Grid.SetColumn(vSplitter, c);
                Grid.SetRowSpan(vSplitter, config.Rows);
                mainGrid.Children.Add(vSplitter);
            }

            // 가로 분할선
            for (int r = 0; r < config.Rows - 1; r++)
            {
                var hSplitter = new GridSplitter
                {
                    Height = 5,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Background = Brushes.LightGray
                };
                Grid.SetRow(hSplitter, r);
                Grid.SetColumnSpan(hSplitter, config.Columns);
                mainGrid.Children.Add(hSplitter);
            }
        }
    }
}
