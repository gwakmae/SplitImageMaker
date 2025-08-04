using System.Windows;

namespace SplitImageMaker.Models
{
    public class SelectionArea
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public bool IsActive { get; set; }

        public Rect GetRect() => new Rect(StartPoint, EndPoint);
        public double Width => GetRect().Width;
        public double Height => GetRect().Height;
    }
}
