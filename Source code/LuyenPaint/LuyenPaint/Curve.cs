using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuyenPaint
{
    public class Curve : Shape
    {
        public List<PointF> Points = new List<PointF>();
        public List<PointF> POINT = new List<PointF>();
        public PointF BEGIN;
        public PointF END;
        public Curve()
        {
            Name = "Curve";
        }
        public override GraphicsPath GraphicsPath
        {
            get
            {
                GraphicsPath path = new GraphicsPath();
                path.AddCurve(Points.ToArray());
                return path;
            }
        }
        public override void Draw(Graphics graphics)
        {
            using(GraphicsPath path=GraphicsPath)
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
            bool res = false;
            using(GraphicsPath path=GraphicsPath)
            {
                Pen pen = new Pen(Color, LineWidth+3);
                res=path.IsOutlineVisible(point, pen);
            }
            return res;
        }

        public override void Move(PointF distance)
        {
            for (int i = 0; i < Points.Count; i++)
                Points[i] = new PointF(Points[i].X+distance.X, Points[i].Y + distance.Y);
            Begin = Points[0];
            End = Points[Points.Count - 1];
        }
    }
}
