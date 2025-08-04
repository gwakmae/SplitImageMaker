using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects; // 추가됨

namespace SplitImageMaker.Helpers
{
    /// <summary>
    /// 비트맵 이미지 처리를 위한 헬퍼 클래스
    /// 이미지 크기 조절, 변환, 저장 등의 기능을 제공합니다.
    /// </summary>
    public static class BitmapHelper
    {
        /// <summary>
        /// 비트맵 소스를 지정된 배율로 크기 조절합니다.
        /// </summary>
        /// <param name="source">원본 비트맵 소스</param>
        /// <param name="scale">배율 (1.0 = 원본 크기)</param>
        /// <returns>크기가 조절된 비트맵 소스</returns>
        public static BitmapSource ScaleBitmap(BitmapSource source, double scale)
        {
            if (source == null || scale <= 0)
                return source;

            try
            {
                var scaledBitmap = new TransformedBitmap(source,
                    new ScaleTransform(scale, scale));
                scaledBitmap.Freeze();
                return scaledBitmap;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"이미지 크기 조절 오류: {ex.Message}");
                return source;
            }
        }

        /// <summary>
        /// 비트맵을 최대 폭과 높이에 맞게 비례적으로 크기 조절합니다.
        /// </summary>
        /// <param name="source">원본 비트맵 소스</param>
        /// <param name="maxWidth">최대 폭</param>
        /// <param name="maxHeight">최대 높이</param>
        /// <returns>크기가 조절된 비트맵 소스</returns>
        public static BitmapSource ResizeBitmap(BitmapSource source, int maxWidth, int maxHeight)
        {
            if (source == null || maxWidth <= 0 || maxHeight <= 0)
                return source;

            try
            {
                double scaleX = (double)maxWidth / source.PixelWidth;
                double scaleY = (double)maxHeight / source.PixelHeight;
                double scale = Math.Min(scaleX, scaleY);

                if (scale >= 1.0)
                    return source;

                return ScaleBitmap(source, scale);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"이미지 리사이즈 오류: {ex.Message}");
                return source;
            }
        }

        /// <summary>
        /// 비트맵을 정확한 크기로 조절합니다 (비율 무시).
        /// </summary>
        /// <param name="source">원본 비트맵 소스</param>
        /// <param name="width">새로운 폭</param>
        /// <param name="height">새로운 높이</param>
        /// <returns>크기가 조절된 비트맵 소스</returns>
        public static BitmapSource ResizeBitmapExact(BitmapSource source, int width, int height)
        {
            if (source == null || width <= 0 || height <= 0)
                return source;

            try
            {
                double scaleX = (double)width / source.PixelWidth;
                double scaleY = (double)height / source.PixelHeight;

                var scaledBitmap = new TransformedBitmap(source,
                    new ScaleTransform(scaleX, scaleY));
                scaledBitmap.Freeze();
                return scaledBitmap;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"정확한 크기 조절 오류: {ex.Message}");
                return source;
            }
        }

        /// <summary>
        /// 비트맵을 회전시킵니다.
        /// </summary>
        /// <param name="source">원본 비트맵 소스</param>
        /// <param name="angle">회전 각도 (도)</param>
        /// <returns>회전된 비트맵 소스</returns>
        public static BitmapSource RotateBitmap(BitmapSource source, double angle)
        {
            if (source == null)
                return source;

            try
            {
                var rotatedBitmap = new TransformedBitmap(source,
                    new RotateTransform(angle));
                rotatedBitmap.Freeze();
                return rotatedBitmap;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"이미지 회전 오류: {ex.Message}");
                return source;
            }
        }

        /// <summary>
        /// 이미지의 색상을 흑백으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 비트맵 소스</param>
        /// <returns>흑백으로 변환된 비트맵 소스</returns>
        public static BitmapSource ConvertToGrayscale(BitmapSource source)
        {
            if (source == null)
                return source;

            try
            {
                var grayBitmap = new FormatConvertedBitmap(source, PixelFormats.Gray8, null, 0);
                grayBitmap.Freeze();
                return grayBitmap;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"흑백 변환 오류: {ex.Message}");
                return source;
            }
        }

        /// <summary>
        /// 이미지가 너무 큰지 확인합니다.
        /// </summary>
        /// <param name="source">확인할 비트맵 소스</param>
        /// <param name="maxDimension">최대 차원 (기본값: 4000px)</param>
        /// <returns>크기 제한 초과 여부</returns>
        public static bool IsImageTooLarge(BitmapSource source, int maxDimension = 4000)
        {
            if (source == null)
                return false;

            return source.PixelWidth > maxDimension || source.PixelHeight > maxDimension;
        }

        /// <summary>
        /// 비트맵 소스를 파일로 저장합니다.
        /// </summary>
        /// <param name="source">저장할 비트맵 소스</param>
        /// <param name="filePath">저장할 파일 경로</param>
        /// <param name="quality">JPEG 품질 (1-100)</param>
        public static void SaveBitmapSource(BitmapSource source, string filePath, int quality = 95)
        {
            if (source == null || string.IsNullOrEmpty(filePath))
                return;

            try
            {
                BitmapEncoder encoder;
                string extension = Path.GetExtension(filePath).ToLower();

                switch (extension)
                {
                    case ".png":
                        encoder = new PngBitmapEncoder();
                        break;
                    case ".jpg":
                    case ".jpeg":
                        var jpegEncoder = new JpegBitmapEncoder();
                        jpegEncoder.QualityLevel = Math.Max(1, Math.Min(100, quality));
                        encoder = jpegEncoder;
                        break;
                    default:
                        encoder = new PngBitmapEncoder();
                        break;
                }

                encoder.Frames.Add(BitmapFrame.Create(source));

                string directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"이미지 저장 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 메모리에서 비트맵을 해제합니다.
        /// </summary>
        public static void FreeMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        /// <summary>
        /// 지정된 배율로 비트맵을 고품질로 리사이즈합니다.
        /// </summary>
        /// <param name="source">원본 비트맵 소스</param>
        /// <param name="scaleX">가로 배율</param>
        /// <param name="scaleY">세로 배율</param>
        /// <returns>고품질로 리사이즈된 비트맵 소스</returns>
        public static BitmapSource ResizeHighQuality(BitmapSource source, double scaleX, double scaleY)
        {
            if (source == null) return null;

            try
            {
                // DrawingGroup을 사용하여 렌더링 품질 제어
                var group = new DrawingGroup();
                group.BitmapEffect = new BlurBitmapEffect { Radius = 0 }; // 고화질 유지 (블러 효과 제거)

                var drawingVisual = new DrawingVisual();
                using (var drawingContext = drawingVisual.RenderOpen())
                {
                    drawingContext.PushGuidelineSet(new GuidelineSet());
                    // 이미지 그리기, 대상 크기에 맞게 스케일 적용
                    drawingContext.DrawImage(source, new Rect(0, 0, source.PixelWidth * scaleX, source.PixelHeight * scaleY));
                    drawingContext.Pop();
                }

                // RenderTargetBitmap을 사용하여 렌더링된 비주얼을 비트맵으로 변환
                var renderTarget = new RenderTargetBitmap(
                    (int)(source.PixelWidth * scaleX),
                    (int)(source.PixelHeight * scaleY),
                    300, 300, PixelFormats.Pbgra32); // DPI를 300으로 설정하여 고화질 유지

                renderTarget.Render(drawingVisual);
                renderTarget.Freeze(); // 비트맵 고정하여 성능 최적화
                return renderTarget;
            }
            catch
            {
                // 오류 발생 시 원본 소스 반환 또는 null 처리 (여기서는 원본 소스 반환)
                return source;
            }
        }
    }
}