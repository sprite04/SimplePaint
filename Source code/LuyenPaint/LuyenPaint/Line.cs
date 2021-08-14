using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuyenPaint
{
    public class Line : Shape
    {
        public Line()
        {
            Name = "Line";
        }
        public override GraphicsPath GraphicsPath
        {
            get
            {
                GraphicsPath path = new GraphicsPath();
                path.AddLine(Begin, End);
                return path;
            }
        }

        public override void Draw(Graphics graphics)
        {
            using (GraphicsPath path = GraphicsPath)
            {
                Pen pen = new Pen(Color, LineWidth)
                {
                    DashStyle = DashStyle
                };
                graphics.DrawPath(pen, path);
            }
        }

        public override bool IsHit(PointF point)
        {
            bool res=false;
            using (GraphicsPath path = GraphicsPath)
            {
                Pen pen = new Pen(Color, LineWidth + 3);
                res = path.IsOutlineVisible(point, pen);
            }
            return res;
        }

        public override void Move(PointF distance)
        {
            Begin = new PointF(Begin.X + distance.X, Begin.Y + distance.Y);
            End = new PointF(End.X + distance.X, End.Y + distance.Y);
        }
    }
}
