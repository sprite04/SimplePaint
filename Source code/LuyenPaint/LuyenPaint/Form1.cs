using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Drawing.Drawing2D;

namespace LuyenPaint
{
    public partial class Form1 : Form
    {
        private List<Shape> shapes = new List<Shape>();
        private CurrentShape currentShape = CurrentShape.NoDrawing;
        private bool isChangeShape = false;
        private bool isMouseDown = false;
        private bool isSelected = false;
        private bool isMoving = false;
        private bool isDrawCurve = false;
        private bool isDrawPolygon = false;
        private bool isControlKey;
        private Point previousPoint;
        private Brush brushSelected = new SolidBrush(Color.Blue);
        private HatchStyle HatchStyle;
        private bool IsSolid = true;
        private Pen penSelected = new Pen(Color.Blue,0.25f)
        {
            DashStyle = DashStyle.Dash,
        };
        public Form1()
        {
            InitializeComponent();
            this.Size = this.MaximumSize;
            cbDashStyle.SelectedIndex = 0;
            cbFillShape.SelectedIndex = 0;
            cbHatchStyle.SelectedIndex = 0;
            HatchStyle = (HatchStyle)6;
        }

        private void myPanel_Paint(object sender, PaintEventArgs e)
        {
            if(isSelected)
            {
                shapes.ForEach(shape =>
                {
                    shape.Draw(e.Graphics);
                    if(shape.IsSelected==true)
                    {
                        if(shape is Ellipse || shape is Group)
                        {
                            if(!(shape is Circle))
                            {
                                if (shape.Begin.X < shape.End.X && shape.Begin.Y < shape.End.Y)
                                    e.Graphics.DrawRectangle(penSelected, shape.Begin.X, shape.Begin.Y,
                                    shape.End.X - shape.Begin.X, shape.End.Y - shape.Begin.Y);
                                else if (shape.Begin.X < shape.End.X && shape.Begin.Y > shape.End.Y)
                                    e.Graphics.DrawRectangle(penSelected, shape.Begin.X, shape.End.Y,
                                    shape.End.X - shape.Begin.X, shape.Begin.Y - shape.End.Y);
                                else if (shape.Begin.X > shape.End.X && shape.Begin.Y < shape.End.Y)
                                    e.Graphics.DrawRectangle(penSelected, shape.End.X, shape.Begin.Y,
                                     shape.Begin.X - shape.End.X, shape.End.Y - shape.Begin.Y);
                                else
                                    e.Graphics.DrawRectangle(penSelected, shape.End.X, shape.End.Y,
                                     shape.Begin.X - shape.End.X, shape.Begin.Y - shape.End.Y);
                            }
                            else if(shape is Circle circle)
                            {
                                if (circle.Begin.X < circle.End.X && circle.Begin.Y < circle.End.Y)
                                {
                                    e.Graphics.DrawRectangle(penSelected, circle.Begin.X, circle.Begin.Y, circle.Length, circle.Length);
                                }
                                else if (circle.Begin.X < circle.End.X && circle.Begin.Y > circle.End.Y)
                                {
                                    e.Graphics.DrawRectangle(penSelected, circle.Begin.X, circle.End.Y, circle.Length, circle.Length);
                                }

                                else if (circle.Begin.X > circle.End.X && circle.Begin.Y < circle.End.Y)
                                {
                                    e.Graphics.DrawRectangle(penSelected, circle.End.X, circle.Begin.Y, circle.Length, circle.Length);
                                }
                                else
                                {
                                    e.Graphics.DrawRectangle(penSelected, circle.End.X, circle.End.Y, circle.Length, circle.Length);
                                }                          
                            }
                        }
                        else if(shape is Curve curve)
                        {
                            FindMaxMinCurve(curve);
                            e.Graphics.DrawRectangle(penSelected, curve.Begin.X, curve.Begin.Y, curve.End.X- curve.Begin.X, curve.End.Y-curve.Begin.Y);
                            curve.Points.ForEach(point =>
                            {
                                e.Graphics.FillEllipse(brushSelected, point.X - 4, point.Y - 4, 8, 8);
                            });
                        }
                        else if(shape is Polygon polygon)
                        {
                            FindMaxMinPolygon(polygon);
                            e.Graphics.DrawRectangle(penSelected, polygon.Begin.X, polygon.Begin.Y, polygon.End.X - polygon.Begin.X, polygon.End.Y - polygon.Begin.Y);
                            polygon.Points.ForEach(point =>
                            {
                                e.Graphics.FillEllipse(brushSelected, point.X - 4, point.Y - 4, 8, 8);
                            });
                        }
                        else
                        {
                            if(!(shape is Square))
                            {
                                e.Graphics.FillEllipse(brushSelected, shape.Begin.X - 4, shape.Begin.Y - 4, 8, 8);
                                e.Graphics.FillEllipse(brushSelected, shape.End.X - 4, shape.End.Y - 4, 8, 8);
                            }
                            else if(shape is Square square)
                            {
                                e.Graphics.FillEllipse(brushSelected, square.Begin.X - 4, square.Begin.Y - 4, 8, 8);
                                if(square.Begin.X<square.End.X && square.Begin.Y<square.End.Y)
                                    e.Graphics.FillEllipse(brushSelected, square.Begin.X+square.Length - 4, square.Begin.Y+square.Length - 4, 8, 8);
                                else if (square.Begin.X < square.End.X && square.Begin.Y > square.End.Y)
                                    e.Graphics.FillEllipse(brushSelected, square.Begin.X + square.Length - 4, square.Begin.Y - square.Length - 4, 8, 8);
                                else if(square.Begin.X > square.End.X && square.Begin.Y < square.End.Y)
                                    e.Graphics.FillEllipse(brushSelected, square.Begin.X - square.Length - 4, square.Begin.Y + square.Length - 4, 8, 8);
                                else if(square.Begin.X > square.End.X && square.Begin.Y > square.End.Y)
                                    e.Graphics.FillEllipse(brushSelected, square.Begin.X - square.Length - 4, square.Begin.Y - square.Length - 4, 8, 8);
                            }
                        }
                    }
                });
            }
            else
            {
                shapes.ForEach(shape =>
                {
                    shape.Draw(e.Graphics);
                });
            }
        }
        private bool isChangeBegin = false;
        private bool isChangeEnd = false;
        public bool IsHit(PointF point, PointF e)
        {
            bool res = false;
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(new RectangleF(point.X - 4, point.Y - 4, 8, 8));
            res=path.IsVisible(e);
            return res;
        }

        private void UpdateInfo(Shape shape)
        {
            if(!(shape is Group))
            {
                btnColor.BackColor = shape.Color;
                btnColor1.BackColor = shape.ForeColor;
                if (shape.IsSolid)
                    chbSolid.Checked = true;
                else
                    chbSolid.Checked = false;
                cbDashStyle.SelectedIndex = (int)shape.DashStyle;
                if (shape is FillShape fillShape)
                {
                    if (fillShape.Fill)
                        cbFillShape.SelectedIndex = 1;
                    else
                        cbFillShape.SelectedIndex = 0;
                }
                else
                    cbFillShape.SelectedIndex = 0;
                trbLineWidth.Value = shape.LineWidth;
                cbHatchStyle.SelectedItem = shape.HatchStyle.ToString();
            }
        }
        private void myPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (currentShape == CurrentShape.NoDrawing)
            {
                if (isControlKey)
                {
                    shapes.ForEach(shape =>
                    {
                        if (shape.IsHit(e.Location))
                        {
                            shape.IsSelected = true;
                            isMoving = true;
                            previousPoint = e.Location;
                            if(shape is Curve curve)
                            {
                                curve.BEGIN = curve.Begin;
                                curve.END = curve.End;
                            }
                            if (shape is Polygon polygon)
                            {
                                polygon.BEGIN = polygon.Begin;
                                polygon.END = polygon.End;
                            }
                        }
                    });
                    myPanel.Invalidate();
                }
                else
                {
                    shapes.ForEach(shape =>
                    {
                        shape.IsSelected = false;
                    });
                    shapes.ForEach(shape =>
                    {
                        if (shape.IsHit(e.Location))
                        {
                            shape.IsSelected = true;
                            UpdateInfo(shape);
                            isMoving = true;
                            previousPoint = e.Location;
                        }
                    });
                    myPanel.Invalidate();
                }
                shapes.ForEach(shape =>
                {
                    if (shape is Line || shape is Rectangle || shape is Ellipse ||shape is Curve||shape is Polygon )
                    {
                        if (IsHit(shape.Begin,e.Location) || IsHit(shape.End, e.Location))
                        {
                            isChangeShape = true;
                            isMoving = false;
                            shape.IsSelected = true;

                          
                            if (IsHit(shape.Begin, e.Location))
                            {
                                if (shape is Circle || shape is Square)
                                {
                                    PointF point = shape.Begin;
                                    shape.Begin = shape.End;
                                    shape.End = point;
                                    isChangeEnd = true;
                                }
                                else
                                    isChangeBegin = true;
                            }
                            if (IsHit(shape.End, e.Location))
                            {
                                isChangeEnd = true;
                            }
                            myPanel.Invalidate();
                        }
                    }
                    
                });
                if (e.Button == MouseButtons.Right)
                {
                    int count = 0;
                    for (int j = 0; j < shapes.Count; j++)
                    {
                        if (shapes[j].IsSelected)
                            count++;
                    }
                    if (count < 1)
                    {
                        menuDelete.Enabled = false;
                        menuGroup.Enabled = false;
                        menuUnGroup.Enabled = false;
                    }
                    else if (count == 1)
                    {
                        menuDelete.Enabled = true;
                        menuGroup.Enabled = false;
                        menuUnGroup.Enabled = false;
                        for (int i = 0; i < shapes.Count; i++)
                            if (shapes[i] is Group)
                                menuUnGroup.Enabled = true;
                    }
                    else
                    {
                        menuDelete.Enabled = true;
                        menuGroup.Enabled = true;
                        menuUnGroup.Enabled = false;
                        for (int i = 0; i < shapes.Count; i++)
                            if (shapes[i] is Group)
                                menuUnGroup.Enabled = true;
                    } 
                }
            }
            else
            {
                isMouseDown = true;
                if (currentShape == CurrentShape.Line)
                {
                    Line line = new Line()
                    {
                        Color = btnColor.BackColor,
                        DashStyle = (DashStyle)cbDashStyle.SelectedIndex,
                        LineWidth = (int)trbLineWidth.Value,
                        Begin = e.Location,
                        HatchStyle = HatchStyle,
                        IsSolid = IsSolid,
                        ForeColor = btnColor1.BackColor
                    };
                    shapes.Add(line);
                }
                if(currentShape==CurrentShape.Rectangle)
                {
                    Rectangle rectangle = new Rectangle()
                    {
                        Color = btnColor.BackColor,
                        DashStyle = (DashStyle)cbDashStyle.SelectedIndex,
                        LineWidth = (int)trbLineWidth.Value,
                        Begin=e.Location,
                        HatchStyle = HatchStyle,
                        IsSolid = IsSolid,
                        ForeColor = btnColor1.BackColor
                    };
                    if (cbFillShape.SelectedIndex == 1)
                        rectangle.Fill = true;
                    shapes.Add(rectangle);
                }
                if(currentShape==CurrentShape.Square)
                {
                    Square square = new Square()
                    {
                        Color = btnColor.BackColor,
                        DashStyle = (DashStyle)cbDashStyle.SelectedIndex,
                        LineWidth = (int)trbLineWidth.Value,
                        Begin = e.Location,
                        HatchStyle = HatchStyle,
                        IsSolid = IsSolid,
                        ForeColor = btnColor1.BackColor
                    };
                    if (cbFillShape.SelectedIndex == 1)
                        square.Fill = true;
                    shapes.Add(square);
                }
                if(currentShape==CurrentShape.Ellipse)
                {
                    Ellipse ellipse = new Ellipse()
                    {
                        Color = btnColor.BackColor,
                        DashStyle = (DashStyle)cbDashStyle.SelectedIndex,
                        LineWidth = (int)trbLineWidth.Value,
                        Begin = e.Location,
                        HatchStyle = HatchStyle,
                        IsSolid = IsSolid,
                        ForeColor = btnColor1.BackColor
                    };
                    if (cbFillShape.SelectedIndex == 1)
                        ellipse.Fill = true;
                    shapes.Add(ellipse);
                }
                if(currentShape==CurrentShape.Circle)
                {
                    Circle circle = new Circle()
                    {
                        Color = btnColor.BackColor,
                        DashStyle = (DashStyle)cbDashStyle.SelectedIndex,
                        LineWidth = (int)trbLineWidth.Value,
                        Begin = e.Location,
                        HatchStyle = HatchStyle,
                        IsSolid = IsSolid,
                        ForeColor = btnColor1.BackColor
                    };
                    if (cbFillShape.SelectedIndex == 1)
                        circle.Fill = true;
                    shapes.Add(circle);
                }
                if(currentShape==CurrentShape.Curve)
                {
                    if(!isDrawCurve)
                    {
                        Curve curve = new Curve()
                        {
                            Color = btnColor.BackColor,
                            DashStyle = (DashStyle)cbDashStyle.SelectedIndex,
                            LineWidth = (int)trbLineWidth.Value,
                            HatchStyle = HatchStyle,
                            IsSolid = IsSolid,
                            ForeColor = btnColor1.BackColor
                        };
                        curve.Points.Add(e.Location);
                        curve.Points.Add(e.Location);
                        shapes.Add(curve);
                        isMouseDown = false;
                        isDrawCurve = true;
                    }
                    else
                    {
                        Shape shape = shapes[shapes.Count - 1];
                        if (shape is Curve curve)
                            curve.Points.Add(e.Location);
                    }
                }
                if(currentShape==CurrentShape.Polygon)
                {
                    if(!isDrawPolygon)
                    {
                        Polygon polygon = new Polygon()
                        {
                            Color = btnColor.BackColor,
                            DashStyle = (DashStyle)cbDashStyle.SelectedIndex,
                            LineWidth = (int)trbLineWidth.Value,
                            HatchStyle = HatchStyle,
                            IsSolid = IsSolid,
                            ForeColor = btnColor1.BackColor
                        };
                        polygon.Points.Add(e.Location);
                        polygon.Points.Add(e.Location);
                        if (cbFillShape.SelectedIndex == 1)
                            polygon.Fill = true;
                        shapes.Add(polygon);
                        isDrawPolygon = true;
                        isMouseDown = false;
                    }
                    else
                    {
                        Shape shape = shapes[shapes.Count - 1];
                        if(shape is Polygon polygon)
                        {
                            polygon.Points[polygon.Points.Count - 1] = e.Location;
                            polygon.Points.Add(e.Location);
                        }
                    }
                }
                
            }
        }


        float h = 0;
        float k = 0;
        private void myPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSelected)
            {
                if (shapes.Exists(shape => shape.IsHit(e.Location)))
                {
                    myPanel.Cursor = Cursors.SizeAll; 
                }
                else
                {
                    myPanel.Cursor = Cursors.Default;
                }
                shapes.ForEach(shape =>
                {
                    if (shape is Line || shape is Rectangle || shape is Ellipse||shape is Curve ||shape is Polygon)
                    {
                        if (IsHit(shape.Begin, e.Location))
                        {
                            myPanel.Cursor = Cursors.SizeNWSE;
                        }
                        if (IsHit(shape.End, e.Location))
                        {
                            myPanel.Cursor = Cursors.SizeNWSE;
                        }
                        //Khong them BEGIN END o day
                    }
                });
            }
            if (isChangeShape)
            {
                if (isChangeBegin)
                {
                    shapes.ForEach(shape =>
                    {
                        if (shape.IsSelected)
                        {
                            if(shape is Curve curve)
                            {
                                for (int i = 0; i < curve.Points.Count; i++)
                                {
                                    float x = curve.POINT[i].X;
                                    float y = curve.POINT[i].Y;

                                    h = (float)((curve.END.X - ((curve.END.X - x) * 1.0 / (curve.END.X - curve.BEGIN.X)) * (curve.END.X - e.Location.X)));
                                    k = (float)(curve.END.Y - ((curve.END.Y - y) * 1.0 / (curve.END.Y - curve.BEGIN.Y)) * (curve.END.Y - e.Location.Y));
                                    curve.Points[i] = new PointF(h, k);
                                }
                            }
                            if (shape is Polygon polygon)
                            {
                                for (int i = 0; i < polygon.Points.Count; i++)
                                {
                                    float x = polygon.POINT[i].X;
                                    float y = polygon.POINT[i].Y;

                                    h = (float)((polygon.END.X - ((polygon.END.X - x) * 1.0 / (polygon.END.X - polygon.BEGIN.X)) * (polygon.END.X - e.Location.X)));
                                    k = (float)(polygon.END.Y - ((polygon.END.Y - y) * 1.0 / (polygon.END.Y - polygon.BEGIN.Y)) * (polygon.END.Y - e.Location.Y));
                                    polygon.Points[i] = new PointF(h, k);
                                }
                            }
                            shape.Begin = e.Location;
                        }
                    });
                }
                if (isChangeEnd)
                {
                    shapes.ForEach(shape =>
                    {
                        if (shape.IsSelected)
                        {
                            if (shape is Curve curve)
                            {
                                for (int i = 0; i < curve.Points.Count; i++)
                                {
                                    float x = curve.POINT[i].X;
                                    float y = curve.POINT[i].Y;

                                    h = (float)((curve.BEGIN.X + ((x - curve.BEGIN.X) * 1.0 / (curve.END.X - curve.BEGIN.X)) * (e.Location.X - curve.BEGIN.X)));
                                    k = (float)(curve.BEGIN.Y + ((y - curve.BEGIN.Y) * 1.0 / (curve.END.Y - curve.BEGIN.Y)) * (e.Location.Y - curve.BEGIN.Y));
                                    curve.Points[i] = new PointF(h, k);
                                }
                            }
                            if (shape is Polygon polygon)
                            {
                                for (int i = 0; i < polygon.Points.Count; i++)
                                {
                                    float x = polygon.POINT[i].X;
                                    float y = polygon.POINT[i].Y;

                                    h = (float)((polygon.BEGIN.X + ((x - polygon.BEGIN.X) * 1.0 / (polygon.END.X - polygon.BEGIN.X)) * (e.Location.X - polygon.BEGIN.X)));
                                    k = (float)(polygon.BEGIN.Y + ((y - polygon.BEGIN.Y) * 1.0 / (polygon.END.Y - polygon.BEGIN.Y)) * (e.Location.Y - polygon.BEGIN.Y));
                                    polygon.Points[i] = new PointF(h, k);
                                }
                            }
                            shape.End = e.Location;
                        }
                    });
                }
                myPanel.Invalidate();
            }
            if (isMouseDown)
            {
                shapes[shapes.Count - 1].End = e.Location;
                myPanel.Invalidate();
            }
            else if(isMoving)
            {
                Point distance = new Point(e.Location.X-previousPoint.X, e.Location.Y-previousPoint.Y);
                shapes.ForEach(shape =>
                {
                    if(shape.IsSelected)
                        shape.Move(distance);
                });
                previousPoint = e.Location;
                myPanel.Invalidate();
            }
            if(isDrawCurve)
            {
                Shape shape = shapes[shapes.Count - 1];
                if(shape is Curve curve)
                {
                    curve.Points[curve.Points.Count - 1] = e.Location;
                }
                myPanel.Invalidate();
            }
            if(isDrawPolygon)
            {
                Shape shape = shapes[shapes.Count - 1];
                if (shape is Polygon polygon)
                {
                    polygon.Points[polygon.Points.Count - 1] = e.Location;
                }
                myPanel.Invalidate();
            }
        }

        private void myPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (isControlKey) isControlKey = false;
            if(isChangeShape)
            {
                isChangeShape = false;                
                currentShape = CurrentShape.NoDrawing;
                myPanel.Cursor = Cursors.Default;
                shapes.ForEach(shape =>
                {
                    if (shape.IsSelected)
                        if (!(shape is Group || shape is Curve || shape is Polygon || shape is Line))
                        {
                            FindMaxMinShape(shape);
                            if (isChangeEnd)
                            {
                                if (shape is Circle circle)
                                    shape.End = new PointF(shape.Begin.X + circle.Length, shape.Begin.Y + circle.Length);
                            }
                            if(isChangeBegin)
                            {
                                if (shape is Circle circle)
                                    shape.End = new PointF(shape.Begin.X + circle.Length, shape.Begin.Y + circle.Length);
                            }
                        }
                });
                isChangeBegin = false;
                isChangeEnd = false;
                Invalidate();
            }
            
            if(isSelected)
            {
                shapes.ForEach(shape =>
                {
                    if (shape.IsSelected)
                        if (shape is Curve curve)
                        {
                            curve.BEGIN = curve.Begin;
                            curve.END = curve.End;
                            for (int i = 0; i < curve.Points.Count; i++)
                                curve.POINT[i] = curve.Points[i];
                        }
                        else if(shape is Polygon polygon)
                        {
                            polygon.BEGIN = polygon.Begin;
                            polygon.END = polygon.End;
                            for (int i = 0; i < polygon.Points.Count; i++)
                                polygon.POINT[i] = polygon.Points[i];
                        }
                });
                isMoving = false;
            }
            if(isMouseDown)
            {
                Shape shape = shapes[shapes.Count - 1];
                if (!(shape is Group || shape is Curve || shape is Polygon||shape is Line))
                {
                    FindMaxMinShape(shape);
                }
                if (shape is Circle circle)
                    shape.End = new PointF(shape.Begin.X + circle.Length, shape.Begin.Y + circle.Length);
                if (shape is Square square)
                    shape.End = new PointF(shape.Begin.X + square.Length, shape.Begin.Y + square.Length);
                
            }
            isMouseDown = false;
        }

        private void myPanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (isDrawCurve)
            {
                Shape shape = shapes[shapes.Count - 1];
                if (shape is Curve curve)
                {
                    curve.Points.RemoveAt(curve.Points.Count - 1);
                    curve.Points.RemoveAt(curve.Points.Count - 1);
                    FindMaxMinCurve(curve);
                    curve.BEGIN = curve.Begin;
                    curve.END = curve.End;
                    curve.Points.ForEach(point =>
                    {
                        curve.POINT.Add(point);
                    });
                }
                
                isDrawCurve = false;
                myPanel.Invalidate();
            }
            if(isDrawPolygon)
            {
                Shape shape = shapes[shapes.Count - 1];
                if (shape is Polygon polygon)
                {
                    polygon.Points.RemoveAt(polygon.Points.Count - 1);
                    polygon.Points.RemoveAt(polygon.Points.Count - 1);
                    FindMaxMinPolygon(polygon);
                    polygon.BEGIN = polygon.Begin;
                    polygon.END = polygon.End;
                    polygon.Points.ForEach(point =>
                    {
                        polygon.POINT.Add(point);
                    });
                }
                isDrawPolygon = false;
                myPanel.Invalidate();
            }
        }

        private void FindMaxMinShape(Shape shape)
        {
            PointF Max = new PointF(int.MinValue, int.MinValue);
            PointF Min = new PointF(int.MaxValue, int.MaxValue);
            if (shape.Begin.X > Max.X)
                Max.X = shape.Begin.X;
            if (shape.Begin.X < Min.X)
                Min.X = shape.Begin.X;
            if (shape.Begin.Y > Max.Y)
                Max.Y = shape.Begin.Y;
            if (shape.Begin.Y < Min.Y)
                Min.Y = shape.Begin.Y;

            if (shape.End.X > Max.X)
                Max.X = shape.End.X;
            if (shape.End.X < Min.X)
                Min.X = shape.End.X;
            if (shape.End.Y > Max.Y)
                Max.Y = shape.End.Y;
            if (shape.End.Y < Min.Y)
                Min.Y = shape.End.Y;

            shape.Begin = Min;
            shape.End = Max;
        }
        private void FindMaxMinCurve(Curve curve)
        {
            PointF Max = new PointF(int.MinValue, int.MinValue);
            PointF Min = new PointF(int.MaxValue, int.MaxValue);
            for(int i=0; i<curve.Points.Count;i++)
            {
                if (curve.Points[i].X > Max.X) Max.X = curve.Points[i].X;
                if (curve.Points[i].X < Min.X) Min.X = curve.Points[i].X;
                if (curve.Points[i].Y > Max.Y) Max.Y = curve.Points[i].Y;
                if (curve.Points[i].Y < Min.Y) Min.Y = curve.Points[i].Y;
            }
            curve.Begin = Min;
            curve.End = Max;
        }

        private void FindMaxMinPolygon(Polygon polygon)
        {
            PointF Max = new PointF(int.MinValue, int.MinValue);
            PointF Min = new PointF(int.MaxValue, int.MaxValue);
            for (int i = 0; i < polygon.Points.Count; i++)
            {
                if (polygon.Points[i].X > Max.X) Max.X = polygon.Points[i].X;
                if (polygon.Points[i].X < Min.X) Min.X = polygon.Points[i].X;
                if (polygon.Points[i].Y > Max.Y) Max.Y = polygon.Points[i].Y;
                if (polygon.Points[i].Y < Min.Y) Min.Y = polygon.Points[i].Y;
            }
            polygon.Begin = Min;
            polygon.End = Max;
        }

        private void FindMaxMinGroup(Group group)
        {
            PointF Max = new Point(int.MinValue, int.MinValue);
            PointF Min = new Point(int.MaxValue, int.MaxValue);
            for(int i=0; i<group.Shapes.Count; i++)
            {
                if (group.Shapes[i].Begin.X > Max.X)
                    Max.X = group.Shapes[i].Begin.X;
                if(group.Shapes[i].Begin.X < Min.X)
                    Min.X= group.Shapes[i].Begin.X;
                if(group.Shapes[i].Begin.Y > Max.Y)
                    Max.Y = group.Shapes[i].Begin.Y;
                if (group.Shapes[i].Begin.Y < Min.Y)
                    Min.Y = group.Shapes[i].Begin.Y;
                if (group.Shapes[i].End.X > Max.X)
                    Max.X = group.Shapes[i].End.X;
                if (group.Shapes[i].End.X < Min.X)
                    Min.X = group.Shapes[i].End.X;
                if (group.Shapes[i].End.Y > Max.Y)
                    Max.Y = group.Shapes[i].End.Y;
                if (group.Shapes[i].End.Y < Min.Y)
                    Min.Y = group.Shapes[i].End.Y;
            }
            group.Begin = Min;
            group.End = Max;
        }
        private void btnSelect_Click(object sender, EventArgs e)
        {
            myPanel.Cursor = System.Windows.Forms.Cursors.Default;
            currentShape = CurrentShape.NoDrawing;
            isSelected = true;
            
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = btnColor.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
                btnColor.BackColor = colorDialog1.Color;
            shapes.ForEach(shape =>
            {
                if (shape.IsSelected)
                {
                    if (shape is Group group)
                    {
                        ChangeGroup(group, btnColor.BackColor,btnColor1.BackColor,IsSolid,HatchStyle, cbDashStyle.SelectedIndex, trbLineWidth.Value, cbFillShape.SelectedIndex==1);
                    }
                    else
                        shape.Color = btnColor.BackColor;
                }
                    
            });
            myPanel.Invalidate();
        }
        private void cbFillShape_SelectedIndexChanged(object sender, EventArgs e)
        {
            shapes.ForEach(shape =>
            {
                if (shape.IsSelected)
                {
                    if(shape is FillShape fillShape)
                    {
                        if (cbFillShape.SelectedIndex == 1)
                            fillShape.Fill = true;
                        else
                            fillShape.Fill = false;
                    }
                    if (shape is Group group)
                    {
                        ChangeGroup(group, btnColor.BackColor, btnColor1.BackColor, IsSolid, HatchStyle, cbDashStyle.SelectedIndex, trbLineWidth.Value, cbFillShape.SelectedIndex == 1);
                    }
                }    
            });
            myPanel.Invalidate();
        }
        private void cbDashStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            shapes.ForEach(shape =>
            {
                if(shape.IsSelected)
                {
                    if (shape is Group group)
                    {
                        ChangeGroup(group, btnColor.BackColor, btnColor1.BackColor, IsSolid, HatchStyle, cbDashStyle.SelectedIndex, trbLineWidth.Value, cbFillShape.SelectedIndex == 1);
                    }
                    else
                        shape.DashStyle = (DashStyle)cbDashStyle.SelectedIndex;
                }
                    
            });
            myPanel.Invalidate();
        }

        private void trbLineWidth_Scroll(object sender, EventArgs e)
        {
            shapes.ForEach(shape =>
            {
                if (shape.IsSelected)
                {
                    if(shape is Group group)
                        ChangeGroup(group, btnColor.BackColor, btnColor1.BackColor, IsSolid, HatchStyle, cbDashStyle.SelectedIndex, trbLineWidth.Value, cbFillShape.SelectedIndex == 1);
                    else
                        shape.LineWidth = trbLineWidth.Value;
                }
                    
            });
            myPanel.Invalidate();
        }

        private void btnLine_Click(object sender, EventArgs e)
        {
            myPanel.Cursor = System.Windows.Forms.Cursors.Cross;
            currentShape = CurrentShape.Line;
            isSelected = false;
            myPanel.Invalidate();
        }

        private void btnRectangle_Click(object sender, EventArgs e)
        {
            myPanel.Cursor = System.Windows.Forms.Cursors.Cross;
            currentShape = CurrentShape.Rectangle;
            isSelected = false;
            myPanel.Invalidate();
        }
        private void btnEllipse_Click(object sender, EventArgs e)
        {
            myPanel.Cursor = System.Windows.Forms.Cursors.Cross;
            currentShape = CurrentShape.Ellipse;
            isSelected = false;
            myPanel.Invalidate();
        }

        private void btnSquare_Click(object sender, EventArgs e)
        {
            myPanel.Cursor = System.Windows.Forms.Cursors.Cross;
            currentShape = CurrentShape.Square;
            isSelected = false;
            myPanel.Invalidate();
        }

        private void btnCircle_Click(object sender, EventArgs e)
        {
            myPanel.Cursor = System.Windows.Forms.Cursors.Cross;
            currentShape = CurrentShape.Circle;
            isSelected = false;
            myPanel.Invalidate();
        }

        private void btnCurve_Click(object sender, EventArgs e)
        {
            myPanel.Cursor = System.Windows.Forms.Cursors.Cross;
            currentShape = CurrentShape.Curve;
            isSelected = false;
            myPanel.Invalidate();
        }

        private void btnPolygon_Click(object sender, EventArgs e)
        {
            myPanel.Cursor = System.Windows.Forms.Cursors.Cross;
            currentShape = CurrentShape.Polygon;
            isSelected = false;
            myPanel.Invalidate();
        }
        
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            isControlKey = e.Control;
            
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            isControlKey = false;
        }

        private void ChangeGroup(Group groups,Color color,Color foreColor,bool solid,HatchStyle hatchStyle,int dashStyle,int lineWidth, bool fill)
        {
            for(int i=0; i<groups.Shapes.Count; i++)
            {
                if (groups.Shapes[i] is Group group)
                    ChangeGroup(group, color,foreColor,solid,hatchStyle, dashStyle, lineWidth, fill);
                else
                {
                    groups.Shapes[i].Color = color;
                    groups.Shapes[i].DashStyle = (DashStyle)dashStyle;
                    groups.Shapes[i].LineWidth = lineWidth;
                    groups.Shapes[i].ForeColor = foreColor;
                    groups.Shapes[i].IsSolid = solid;
                    groups.Shapes[i].HatchStyle = hatchStyle;
                    if(groups.Shapes[i] is FillShape fillShape)
                    {
                        fillShape.Fill = fill;
                    }
                }
            }
        }
        private void menuGroup_Click(object sender, EventArgs e)
        {
            if (shapes.Count(shape => shape.IsSelected) > 1)
            {
                Group group = new Group();

                for (int i = 0; i < shapes.Count; i++)
                {
                    if (shapes[i].IsSelected)
                    {
                        group.Add(shapes[i]);
                        shapes.RemoveAt(i);
                        i--;
                    }
                }
                FindMaxMinGroup(group);
                shapes.Add(group);
                group.IsSelected = true;
                myPanel.Invalidate();
            }   
        }

        private void menuDelete_Click(object sender, EventArgs e)
        {
            if (shapes.Count(shape => shape.IsSelected) >= 1)
            {
                for (int i = 0; i < shapes.Count; i++)
                {
                    if (shapes[i].IsSelected)
                    {
                        shapes.RemoveAt(i);
                        i--;
                    }
                }
                myPanel.Invalidate();
            }
        }

        private void menuUnGroup_Click(object sender, EventArgs e)
        {
            for(int i=0; i<shapes.Count; i++)
            {
                if(shapes[i].IsSelected && shapes[i] is Group group)
                {
                    for (int j = 0; j < group.Shapes.Count; j++)
                        shapes.Add(group.Shapes[j]);
                    shapes.RemoveAt(i);
                }
            }
            myPanel.Invalidate();
        }

        private void tsMenuFileNew_Click(object sender, EventArgs e)
        {
            while (shapes.Count > 0)
                shapes.RemoveAt(0);
            myPanel.Invalidate();
        }

        private void btnColor1_Click(object sender, EventArgs e)
        {
            colorDialog2.Color = btnColor1.BackColor;
            if (colorDialog2.ShowDialog() == DialogResult.OK)
                btnColor1.BackColor = colorDialog2.Color;
            shapes.ForEach(shape =>
            {
                if (shape.IsSelected)
                {
                    if (shape is Group group)
                        ChangeGroup(group, btnColor.BackColor, btnColor1.BackColor, IsSolid, HatchStyle, cbDashStyle.SelectedIndex, trbLineWidth.Value, cbFillShape.SelectedIndex == 1);
                    else
                        shape.ForeColor = btnColor1.BackColor;
                }
            });
            myPanel.Invalidate();
        }

        private void cbHatchStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbHatchStyle.SelectedIndex != -1)
            {
                switch (cbHatchStyle.SelectedItem.ToString())
                {
                    case "ForwardDiagonal":
                        HatchStyle = (HatchStyle)2;
                        break;
                    case "DiagonalCross":
                        HatchStyle = (HatchStyle)5;
                        break;
                    case "Percent05":
                        HatchStyle = (HatchStyle)6;
                        break;
                    case "LightDownwardDiagonal":
                        HatchStyle = (HatchStyle)18;
                        break;
                    case "DashedDownwardDiagonal":
                        HatchStyle = (HatchStyle)30;
                        break;
                    case "DashedUpwardDiagonal":
                        HatchStyle = (HatchStyle)31;
                        break;
                    case "DashedVertical":
                        HatchStyle = (HatchStyle)33;
                        break;
                    case "ZigZag":
                        HatchStyle = (HatchStyle)36;
                        break;
                    case "DiagonalBrick":
                        HatchStyle = (HatchStyle)38;
                        break;
                    case "Weave":
                        HatchStyle = (HatchStyle)40;
                        break;
                    case "Divot":
                        HatchStyle = (HatchStyle)42;
                        break;
                    case "DottedDiamond":
                        HatchStyle = (HatchStyle)44;
                        break;
                    case "OutlinedDiamond":
                        HatchStyle = (HatchStyle)51;
                        break;
                }
            }
            shapes.ForEach(shape =>
            {
                if (shape.IsSelected)
                {
                    if (shape is Group group)
                        ChangeGroup(group, btnColor.BackColor, btnColor1.BackColor, IsSolid, HatchStyle, cbDashStyle.SelectedIndex, trbLineWidth.Value, cbFillShape.SelectedIndex == 1);
                    else
                        shape.HatchStyle = HatchStyle;
                }
            });
            myPanel.Invalidate();
        }

        private void chbSolid_CheckedChanged(object sender, EventArgs e)
        {
            if (chbSolid.Checked)
                IsSolid = true;
            else
                IsSolid = false;
            shapes.ForEach(shape =>
            {
                if (shape.IsSelected)
                {
                    if (shape is Group group)
                        ChangeGroup(group, btnColor.BackColor, btnColor1.BackColor, IsSolid, HatchStyle, cbDashStyle.SelectedIndex, trbLineWidth.Value, cbFillShape.SelectedIndex == 1);
                    else
                        shape.IsSolid = IsSolid;
                }
            });
            myPanel.Invalidate();
        }
    }
    public class MyPanel : Panel
    {
        public MyPanel() : base()
        {
            this.DoubleBuffered = true;
        }
    }
}
