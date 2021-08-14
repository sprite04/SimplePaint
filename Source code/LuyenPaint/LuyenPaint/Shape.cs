using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace LuyenPaint
{
    public abstract class Shape
    {
        public PointF Begin { get; set; }
        public PointF End { get; set; }
        public Color Color { get; set; }
        public Color ForeColor { get; set; }
        public string Name { get; set; }
        public int LineWidth { get; set; }
        public bool IsSelected { get; set; }
        public bool IsSolid { get; set; }
        public DashStyle DashStyle { get; set; }
        public HatchStyle HatchStyle { get; set; }
        public abstract GraphicsPath GraphicsPath { get; }
        public abstract bool IsHit(PointF point);
        public abstract void Draw(Graphics graphics);
        public abstract void Move(PointF distance);
        public override string ToString()
        {
            return $"{Name} [({Begin.X}, {Begin.Y}); ({End.X}, {End.Y})]";
        }
    }
}
