using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuyenPaint
{
    public class Circle:Ellipse
    {
        public float Length { get; set; }
        public Circle()
        {
            Name = "Circle";
        }

        public override GraphicsPath GraphicsPath
        {
            get
            {
                GraphicsPath path = new GraphicsPath();
                Length = (Math.Abs(Begin.X - End.X));
                if(Begin.X<End.X && Begin.Y<End.Y)
                {
                    path.AddEllipse(new RectangleF(Begin.X, Begin.Y, Length, Length));
                }
                else if(Begin.X<End.X && End.Y<Begin.Y)
                {
                    path.AddEllipse(new RectangleF(Begin.X, Begin.Y-Length, Length, Length));
                    End = new PointF(Begin.X, Begin.Y - Length);
                }
                else if(End.X<Begin.X && Begin.Y<End.Y)
                {
                    path.AddEllipse(new RectangleF(Begin.X-Length, Begin.Y, Length, Length));
                    End = new PointF(Begin.X - Length, Begin.Y);
                }
                else
                {
                    path.AddEllipse(new RectangleF(Begin.X-Length, Begin.Y-Length, Length, Length));
                    End = new PointF(Begin.X - Length, Begin.Y - Length);
                }
                return path;
            }
        }
        
    }
}
