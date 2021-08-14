using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuyenPaint
{
    public class Polygon : FillShape
    {
        public List<PointF> Points = new List<PointF>();
        public List<PointF> POINT = new List<PointF>();
        public PointF BEGIN;
        public PointF END;
        public Polygon()
        {
            Name = "Polygon";
        }
        public override GraphicsPath GraphicsPath
        {
            get
            {
                GraphicsPath path = new GraphicsPath();
                {
                    if (Points.Count < 3)
                        path.AddLine(Points[0], Points[1]);
                    else
                    {
                        path.AddPolygon(Points.ToArray());
                    }
                }
                return path;
            }
        }

        public override void Draw(Graphics graphics)
        {
            using(GraphicsPath path=GraphicsPath)
            {
                if(!Fill)
                {
                    Pen pen = new Pen(Color, LineWidth) { DashStyle = DashStyle };
                    graphics.DrawPath(pen, path);
                }
                else
                {
                    Brush brush;
                    if (IsSolid)
                    {
                        brush = new SolidBrush(ForeColor);
                    }
                    else
                    {
                        brush = new HatchBrush(HatchStyle, Color, ForeColor);
                    }
                    graphics.FillPath(brush, path);
                    Pen pen = new Pen(Color, LineWidth)
                    {
                        DashStyle = DashStyle
                    };
                    graphics.DrawPath(pen, path);
                }
            }
        }

        public override bool IsHit(PointF point)
        {
            bool res = false;
            using(GraphicsPath path=GraphicsPath)
            {
                if(!Fill)
                {
                    Pen pen = new Pen(Color, LineWidth + 3);
                    res=path.IsOutlineVisible(point, pen);
                }
                else
                {
                    res=path.IsVisible(point);
                }
            }
            return res;
        }

        public override void Move(PointF distance)
        {
            for (int i = 0; i < Points.Count; i++)
                Points[i] = new PointF(Points[i].X + distance.X, Points[i].Y + distance.Y);
            Begin = Points[0];
            End = Points[Points.Count - 1];
        }
    }
}
