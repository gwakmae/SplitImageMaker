using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;

namespace SplitImageMaker.Helpers
{
    public static class ClipboardHelper
    {
        /// <summary>
        /// 클립보드에서 이미지의 '완벽한 복사본'을 생성하여 반환합니다.
        /// </summary>
        /// <returns>데이터 오염으로부터 안전한, 완전히 독립된 이미지 소스</returns>
        public static ImageSource GetImageFromClipboard()
        {
            try
            {
                // 1. WPF 이미지 확인
                if (System.Windows.Clipboard.ContainsImage())
                {
                    var source = System.Windows.Clipboard.GetImage();
                    if (source == null) return null;

                    // ✅ FIX: RenderTargetBitmap을 사용하여 이미지의 완벽한 '깊은 복사'를 수행합니다.
                    // 이 방법은 원본 데이터와 완전히 단절된 새로운 이미지 데이터를 생성하여 데이터 오염을 원천 차단합니다.
                    var renderTarget = new RenderTargetBitmap(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, PixelFormats.Pbgra32);
                    var drawingVisual = new DrawingVisual();
                    using (var drawingContext = drawingVisual.RenderOpen())
                    {
                        drawingContext.DrawImage(source, new Rect(0, 0, source.PixelWidth, source.PixelHeight));
                    }
                    renderTarget.Render(drawingVisual);

                    // 새로 만든 복사본을 고정하여 성능을 최적화합니다.
                    renderTarget.Freeze();
                    return renderTarget;
                }

                // 2. Windows Forms 이미지 확인 (폴백)
                if (System.Windows.Forms.Clipboard.ContainsImage())
                {
                    using (var winFormsImage = System.Windows.Forms.Clipboard.GetImage())
                    {
                        if (winFormsImage != null)
                            return ConvertBitmapToBitmapSource((Bitmap)winFormsImage);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"클립보드에서 이미지를 가져올 수 없습니다: {ex.Message}");
                return null;
            }
        }

        private static BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
        {
            if (bitmap == null) return null;
            IntPtr hBitmap = IntPtr.Zero;
            try
            {
                hBitmap = bitmap.GetHbitmap();
                var source = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                source.Freeze();
                return source;
            }
            finally
            {
                if (hBitmap != IntPtr.Zero) DeleteObject(hBitmap);
            }
        }

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static bool HasImage()
        {
            try
            {
                return System.Windows.Clipboard.ContainsImage() || System.Windows.Forms.Clipboard.ContainsImage();
            }
            catch { return false; }
        }
    }

    public class ClipboardWatcher : IDisposable
    {
        private readonly HwndSource _hwndSource;
        public event EventHandler ClipboardChanged;
        public ClipboardWatcher(Window window)
        {
            var src = (HwndSource)PresentationSource.FromVisual(window);
            _hwndSource = src ?? throw new InvalidOperationException("HwndSource null");
            _hwndSource.AddHook(WndProc);
            AddClipboardFormatListener(_hwndSource.Handle);
        }
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_CLIPBOARDUPDATE = 0x031D;
            if (msg == WM_CLIPBOARDUPDATE) { ClipboardChanged?.Invoke(this, EventArgs.Empty); handled = true; }
            return IntPtr.Zero;
        }
        public void Dispose()
        {
            RemoveClipboardFormatListener(_hwndSource.Handle);
            _hwndSource.RemoveHook(WndProc);
        }
        [DllImport("user32.dll", SetLastError = true)] private static extern bool AddClipboardFormatListener(IntPtr hwnd);
        [DllImport("user32.dll", SetLastError = true)] private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);
    }
}