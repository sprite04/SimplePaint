using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuyenPaint
{
    public class Ellipse : FillShape
    {
        public Ellipse()
        {
            Name = "Ellipse";
        }
        public override GraphicsPath GraphicsPath
        {
            get
            {
                GraphicsPath path = new GraphicsPath();
                if (Begin.X < End.X && Begin.Y < End.Y)
                {
                    path.AddEllipse(new RectangleF(Begin.X, Begin.Y, End.X - Begin.X, End.Y - Begin.Y));
                }
                else if (Begin.X < End.X && End.Y < Begin.Y)
                {
                    path.AddEllipse(new RectangleF(Begin.X, End.Y, End.X - Begin.X, Begin.Y - End.Y));
                }
                else if (End.X < Begin.X && Begin.Y < End.Y)
                {
                    path.AddEllipse(new RectangleF(End.X, Begin.Y, Begin.X - End.X, End.Y - Begin.Y));
                }
                else
                {
                    path.AddEllipse(new RectangleF(End.X, End.Y, Begin.X - End.X, Begin.Y - End.Y));
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
                    Pen pen = new Pen(Color, LineWidth)
                    { 
                        DashStyle=DashStyle
                    };
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
            Begin = new PointF(Begin.X + distance.X, Begin.Y + distance.Y);
            End = new PointF(End.X + distance.X, End.Y + distance.Y);
        }
    }
}
