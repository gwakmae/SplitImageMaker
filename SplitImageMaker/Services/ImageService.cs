using SplitImageMaker.Helpers;
using SplitImageMaker.Models;
using System;
using System.Collections.ObjectModel;
using System.Globalization; // 추가됨
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SplitImageMaker.Services
{
    public class ImageService
    {
        public BitmapSource GetImageFromClipboard()
        {
            try
            {
                return ClipboardHelper.GetImageFromClipboard() as BitmapSource;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting image from clipboard: {ex.Message}");
                return null;
            }
        }

        public BitmapSource CombineImages(ObservableCollection<PanelInfo> panels, int rows, int cols)
        {
            if (panels == null || panels.Count == 0 || panels.All(p => p.Image == null))
                return null;

            try
            {
                // 각 열의 최대 너비 계산
                var colWidths = new int[cols];
                for (int c = 0; c < cols; c++)
                {
                    colWidths[c] = panels.Where(p => p.Column == c && p.Image != null)
                                         .Select(p => p.Image.PixelWidth)
                                         .DefaultIfEmpty(0)
                                         .Max();
                }

                // 각 행의 최대 높이 계산
                var rowHeights = new int[rows];
                for (int r = 0; r < rows; r++)
                {
                    rowHeights[r] = panels.Where(p => p.Row == r && p.Image != null)
                                          .Select(p => p.Image.PixelHeight)
                                          .DefaultIfEmpty(0)
                                          .Max();
                }

                int totalWidth = colWidths.Sum();
                int totalHeight = rowHeights.Sum();

                if (totalWidth == 0 || totalHeight == 0) return null;

                var renderTarget = new RenderTargetBitmap(totalWidth, totalHeight, 96, 96, PixelFormats.Pbgra32);
                var drawingVisual = new DrawingVisual();
                // ✅ FIX: 테두리를 더 잘 보이도록 2픽셀 검은색으로 변경
                var borderPen = new Pen(Brushes.Black, 2);
                borderPen.Freeze();

                // 각 셀의 시작 위치 계산
                var xOffsets = new int[cols];
                for (int c = 1; c < cols; c++) xOffsets[c] = xOffsets[c - 1] + colWidths[c - 1];

                var yOffsets = new int[rows];
                for (int r = 1; r < rows; r++) yOffsets[r] = yOffsets[r - 1] + rowHeights[r - 1];

                using (var drawingContext = drawingVisual.RenderOpen())
                {
                    drawingContext.DrawRectangle(Brushes.White, null, new Rect(0, 0, totalWidth, totalHeight));

                    foreach (var panel in panels)
                    {
                        if (panel?.Image != null)
                        {
                            int x = xOffsets[panel.Column];
                            int y = yOffsets[panel.Row];
                            int cellWidth = colWidths[panel.Column];
                            int cellHeight = rowHeights[panel.Row];

                            drawingContext.DrawImage(panel.Image, new Rect(x, y, panel.Image.PixelWidth, panel.Image.PixelHeight));
                            drawingContext.DrawRectangle(null, borderPen, new Rect(x, y, cellWidth, cellHeight));

                            if (!string.IsNullOrWhiteSpace(panel.Caption))
                            {
                                RenderCaption(drawingContext, panel, x, y, cellWidth, cellHeight);
                            }
                        }
                    }
                }

                renderTarget.Render(drawingVisual);
                renderTarget.Freeze();
                return renderTarget;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Image combining error: {ex.Message}");
                return null;
            }
        }

        public BitmapSource CropImageByRatio(BitmapSource original, PanelInfo panel, GridConfiguration config)
        {
            if (original == null || panel == null || config == null) return null;
            try
            {
                double totalColRatio = config.ColumnRatios.Sum();
                double totalRowRatio = config.RowRatios.Sum();
                double colStartRatio = config.ColumnRatios.Take(panel.Column).Sum() / totalColRatio;
                double colWidthRatio = config.ColumnRatios[panel.Column] / totalColRatio;
                double rowStartRatio = config.RowRatios.Take(panel.Row).Sum() / totalRowRatio;
                double rowHeightRatio = config.RowRatios[panel.Row] / totalRowRatio;
                int x = (int)(original.PixelWidth * colStartRatio);
                int y = (int)(original.PixelHeight * rowStartRatio);
                int width = (int)(original.PixelWidth * colWidthRatio);
                int height = (int)(original.PixelHeight * rowHeightRatio);
                x = Math.Max(0, Math.Min(x, original.PixelWidth));
                y = Math.Max(0, Math.Min(y, original.PixelHeight));
                width = Math.Max(1, Math.Min(width, original.PixelWidth - x));
                height = Math.Max(1, Math.Min(height, original.PixelHeight - y));
                var rect = new Int32Rect(x, y, width, height);
                return new CroppedBitmap(original, rect);
            }
            catch { return original; }
        }

        public BitmapSource CropSelectedArea(BitmapSource original, SelectionArea selection, Size canvasSize)
        {
            if (original == null || selection == null || !selection.IsActive) return null;
            try
            {
                double scaleX = original.PixelWidth / canvasSize.Width;
                double scaleY = original.PixelHeight / canvasSize.Height;
                var rect = selection.GetRect();
                int x = (int)(rect.X * scaleX);
                int y = (int)(rect.Y * scaleY);
                int width = (int)(rect.Width * scaleX);
                int height = (int)(rect.Height * scaleY);
                x = Math.Max(0, Math.Min(x, original.PixelWidth));
                y = Math.Max(0, Math.Min(y, original.PixelHeight));
                width = Math.Max(1, Math.Min(width, original.PixelWidth - x));
                height = Math.Max(1, Math.Min(height, original.PixelHeight - y));
                var cropRect = new Int32Rect(x, y, width, height);
                return new CroppedBitmap(original, cropRect);
            }
            catch { return original; }
        }

        private void RenderCaption(DrawingContext dc, PanelInfo panel, int x, int y, int cellWidth, int cellHeight)
        {
            // ✅ FIX: 폰트를 더 깔끔한 'Segoe UI Black'으로 변경
            var typeface = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Black, FontStretches.Normal);

            // ✅ FIX: 폰트 크기와 여백을 미세 조정하여 균형감 개선
            double fontSize = Math.Max(20, cellHeight * 0.09);
            double padding = cellWidth * 0.05;
            double bottomMargin = fontSize * 0.4;

            var formattedText = new FormattedText(
                panel.Caption,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                fontSize,
                Brushes.White,
                96
            )
            {
                MaxTextWidth = cellWidth - padding,
                TextAlignment = TextAlignment.Center,
                MaxTextHeight = cellHeight * 0.4,
                Trimming = TextTrimming.CharacterEllipsis
            };

            double textX = x + (cellWidth - formattedText.Width) / 2;
            double textY = y + cellHeight - formattedText.Height - bottomMargin;

            var textGeometry = formattedText.BuildGeometry(new Point(textX, textY));

            // ✅ FIX: 외곽선 두께를 줄여 '먹물 번짐' 현상 개선 (기존 0.12 -> 0.08)
            var pen = new Pen(Brushes.Black, fontSize * 0.08)
            {
                LineJoin = PenLineJoin.Round
            };
            pen.Freeze();

            dc.DrawGeometry(Brushes.White, pen, textGeometry);
        }
    }
}