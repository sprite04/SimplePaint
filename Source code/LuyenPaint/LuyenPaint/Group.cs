using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuyenPaint
{
    public class Group: Shape
    {
        public List<Shape> Shapes = new List<Shape>();
        public Group()
        {
            Name = "Group";
        }

        public void Add(Shape shape)
        {
            Shapes.Add(shape);
        }
        public GraphicsPath[] GraphicsPaths
        {
            get
            {
                GraphicsPath[] paths = new GraphicsPath[Shapes.Count];
                for(int i=0; i<Shapes.Count;i++)
                {
                    GraphicsPath path = new GraphicsPath();
                    if(Shapes[i] is Line line)
                    {
                        path = line.GraphicsPath;
                    }
                    else if(Shapes[i] is Rectangle rectangle)
                    {
                        if(Shapes[i] is Square square)
                        {
                            path = square.GraphicsPath;
                        }
                        else
                        {
                            path = rectangle.GraphicsPath;
                        }
                    }
                    else if(Shapes[i] is Ellipse ellipse)
                    {
                        if(Shapes[i] is Circle circle)
                        {
                            path = circle.GraphicsPath;
                        }
                        else
                        {
                            path = ellipse.GraphicsPath;
                        }
                    }
                    else if(Shapes[i] is Curve curve)
                    {
                        path = curve.GraphicsPath;
                    }
                    else if(Shapes[i] is Polygon polygon)
                    {
                        path = polygon.GraphicsPath;
                    }
                    else if(Shapes[i] is Group group)
                    {
                        for (int j = 0; j < group.GraphicsPaths.Length; j++)
                        {
                            path.AddPath(group.GraphicsPaths[j], false);
                        }
                    }
                    paths[i] = path;
                }
                return paths;
            }
        }

        public override GraphicsPath GraphicsPath => throw new NotImplementedException();

        public override void Draw(Graphics graphics)
        {
            GraphicsPath[] paths = GraphicsPaths;
            for (int i = 0; i < paths.Length; i++)
            {
                using (GraphicsPath path = paths[i])
                {
                    if (Shapes[i] is FillShape shape)
                    {
                        if (shape.Fill)
                        {
                            Brush brush;
                            if (shape.IsSolid)
                            {
                                brush = new SolidBrush(shape.ForeColor);
                            }
                            else
                            {
                                brush = new HatchBrush(shape.HatchStyle, shape.Color, shape.ForeColor);
                            }
                            graphics.FillPath(brush, path);
                            Pen pen = new Pen(shape.Color, shape.LineWidth)
                            {
                                DashStyle = shape.DashStyle
                            };
                            graphics.DrawPath(pen, path);
                        }
                        else
                        {
                            using (Pen pen = new Pen(shape.Color, shape.LineWidth) { DashStyle = shape.DashStyle })
                            {
                                graphics.DrawPath(pen, path);
                            }
                        }
                    }
                    else if (Shapes[i] is Group group)
                    {
                        group.Draw(graphics);
                    }
                    else
                    {
                        using (Pen pen = new Pen(Shapes[i].Color, Shapes[i].LineWidth) { DashStyle = Shapes[i].DashStyle })
                        {
                            graphics.DrawPath(pen, path);
                        }
                    }
                }
            }
        }

        public override bool IsHit(PointF point)
        {
            GraphicsPath[] paths = GraphicsPaths;
            for (int i = 0; i < paths.Length; i++)
            {
                using (GraphicsPath path = paths[i])
                {
                    if (Shapes[i] is FillShape shape)
                    {
                        if (shape.Fill)
                        {
                            using (Brush brush = new SolidBrush(shape.Color))
                            {
                                if (path.IsVisible(point))
                                {
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            using (Pen pen = new Pen(shape.Color, shape.LineWidth + 3))
                            {
                                if (path.IsOutlineVisible(point, pen))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    else if (!(Shapes[i] is Group))
                    {
                        using (Pen pen = new Pen(Shapes[i].Color, Shapes[i].LineWidth + 3))
                        {
                            if (path.IsOutlineVisible(point, pen))
                            {
                                return true;
                            }
                        }
                    }
                    else if (Shapes[i] is Group group)
                    {
                        return group.IsHit(point);
                    }
                }
            }
            return false;
        }

        public override void Move(PointF distance)
        {
            for (int i = 0; i < Shapes.Count; i++)
            {
                if (Shapes[i] is Curve curve)
                {
                    curve.Begin = new PointF(curve.Begin.X + distance.X, curve.Begin.Y + distance.Y);
                    curve.End = new PointF(curve.End.X + distance.X, curve.End.Y + distance.Y);

                    for (int j = 0; j < curve.Points.Count; j++)
                    {
                        curve.Points[j] = new PointF(curve.Points[j].X + distance.X, curve.Points[j].Y + distance.Y);
                    }
                }
                else if (Shapes[i] is Polygon polygon)
                {
                    polygon.Begin = new PointF(polygon.Begin.X + distance.X, polygon.Begin.Y + distance.Y);
                    polygon.End = new PointF(polygon.End.X + distance.X, polygon.End.Y + distance.Y);

                    for (int j = 0; j < polygon.Points.Count; j++)
                    {
                        polygon.Points[j] = new PointF(polygon.Points[j].X + distance.X, polygon.Points[j].Y + distance.Y);
                    }
                }
                else if (Shapes[i] is Group group)
                {
                    group.Move(distance);
                }
                else
                {
                    Shapes[i].Begin = new PointF(Shapes[i].Begin.X + distance.X, Shapes[i].Begin.Y + distance.Y);
                    Shapes[i].End = new PointF(Shapes[i].End.X + distance.X, Shapes[i].End.Y + distance.Y);
                }
            }
            Begin = new PointF(Begin.X + distance.X, Begin.Y + distance.Y);
            End = new PointF(End.X + distance.X, End.Y + distance.Y);
        }
    }
}
