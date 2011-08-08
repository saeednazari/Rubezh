﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlanEditor.Basic;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PlanEditor
{
    class PELine : PEShape
    {
        Point from;
        Point to;
        Line line = null;
        Thumb thumb1;
        Thumb thumb2;
        int hashcode;
        List<Thumb> thumbList = null;

        public double Left
        {
            get
            {
                return line.Margin.Left;
            }
        }
        public double Top
        {
            get
            {
                return line.Margin.Top;
            }
        }
        public double Width
        {
            get
            {
                //return line.Width;
                return Math.Abs(X2 - X1);
            }
        }
        public double Height
        {
            get
            {
                //return line.Height;
                return Math.Abs(Y2 - Y1);
            }
        }
        public double X1
        {
            get
            {
                return line.X1;
            }
            set
            {
                line.X1 = value;
            }
        }
        public double X2
        {
            get
            {
                return line.X2;
            }
            set
            {
                line.X2 = value;
            }
        }
        public double Y1
        {
            get
            {
                return line.Y1;
            }
            set
            {
                line.Y1 = value;
            }
        }
        public double Y2
        {
            get
            {
                return line.Y2;
            }
            set
            {
                line.Y2 = value;
            }
        }

        //public void SetActive(Canvas canvas, PECanvas pe)
        public void SetActive(Canvas canvas)
        {
            active = true;
            double dLeft = Canvas.GetLeft(this.line);
            double dTop = Canvas.GetTop(this.line);
            foreach (Thumb thumb in thumbList)
            {
                thumb.Height = 10;
                thumb.Width = 10;
                thumb.Background = Brushes.Blue;
                dLeft = Canvas.GetLeft(this.line);
                dTop = Canvas.GetTop(this.line);
                if (thumb.Name == "thumb1")
                {
                    Canvas.SetLeft(thumb, line.X1 - 5);
                    Canvas.SetTop(thumb, line.Y1 - 5);
                    if (canvas.Children.IndexOf(thumb) == -1) canvas.Children.Add(thumb);
                }
                else
                {
                    Canvas.SetLeft(thumb, line.X2 - 5);
                    Canvas.SetTop(thumb, line.Y2 - 5);
                    if (canvas.Children.IndexOf(thumb) == -1) canvas.Children.Add(thumb);
                }
            }
        }
        public void SetDeactive(Canvas canvas)
        {
            active = false;
            /*
            if (canvas.Children.IndexOf(thumb1) != -1) canvas.Children.Remove(thumb1);
            if (canvas.Children.IndexOf(thumb2) != -1) canvas.Children.Remove(thumb2);
             * */
            foreach (Thumb thumb in thumbList)
            {
                if (canvas.Children.IndexOf(thumb) != -1) canvas.Children.Remove(thumb);
            }

        }

        public void Move(Canvas canvas)
        {
            foreach (Thumb thumb in thumbList)
            {
                if (canvas.Children.IndexOf(thumb) != -1) canvas.Children.Remove(thumb);
            }
            /*
            if (canvas.Children.IndexOf(thumb1) != -1) canvas.Children.Remove(thumb1);
            if (canvas.Children.IndexOf(thumb2) != -1) canvas.Children.Remove(thumb2);
             */
        }

        public void MoveFinished(Canvas canvas, double top, double left)
        {
            line.Y1 = line.Y1 + top;
            line.Y2 = line.Y2 + top;
            line.X1 = line.X1 + left;
            line.X2 = line.X2 + left;
            SetActive(canvas);
        }

        public PELine()
        {
            this.line = new Line();
            this.line.Stroke = Brushes.Red;
            this.line.StrokeThickness = 3;
        }

        public PELine(Line line)
        {
            this.line = line;
        }

        double dLeft;
        double dTop;

        public void onDragStarted(object sender, DragStartedEventArgs e)
        {
            Thumb thumb = (Thumb)e.Source;
            Thumb fromThumb = null;
            thumb.Background = Brushes.Green;
            _originalElement = this.line as UIElement;
            _overlayElementLine = new LineAdorner(_originalElement);
            _overlayElementLine.SetCanvas((Canvas)line.Parent);
            ((CustomPanel)line.Parent).SetResize(null);

            Line l = this.line;
            if (thumb.Name.Equals("thumb2"))
            {
                fromThumb = thumbList[0];
                _overlayElementLine.SetPointFrom(new Point(X1,Y1));
            }

            if (thumb.Name.Equals("thumb1"))
            {
                fromThumb = thumbList[1];
                _overlayElementLine.SetPointFrom(new Point(X2, Y2));
            }

            _overlayElementLine.SetOperationMove(false);
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(_originalElement);
            layer.Add(_overlayElementLine);

        }

        public void onDragCompleted(object sender, DragCompletedEventArgs e)
        {
            Thumb thumb = (Thumb)e.Source;
            thumb.Background = Brushes.Blue;
            AdornerLayer.GetAdornerLayer(_overlayElementLine.AdornedElement).Remove(_overlayElementLine);
            Point CurrentPosition = System.Windows.Input.Mouse.GetPosition((Canvas)line.Parent);
            if (thumb.Name.Equals("thumb2"))
            {
                X2 = CurrentPosition.X;
                Y2 = CurrentPosition.Y;
            }

            if (thumb.Name.Equals("thumb1"))
            {
                line.X1 = CurrentPosition.X;
                line.Y1 = CurrentPosition.Y;
            }

            //SetActive((Canvas)line.Parent);
        }

        #region Для пробы ресайза через thumb
        private UIElement _originalElement;
        private LineAdorner _overlayElementLine;
        private Point _startPoint;
        double fromX = 0, fromY = 0;
        #endregion

        void onDragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = (Thumb)e.Source;
            thumb.Background = Brushes.Red;
            Canvas.SetLeft(thumb, Canvas.GetLeft(thumb) +
                        e.HorizontalChange);
            Canvas.SetTop(thumb, Canvas.GetTop(thumb) +
                                    e.VerticalChange);
            Point CurrentPosition = System.Windows.Input.Mouse.GetPosition(thumb);
            _overlayElementLine.LeftOffset = CurrentPosition.X;
            _overlayElementLine.TopOffset = CurrentPosition.Y;
        }

        public PELine(Point _from, Point _to)
            : base()
        {
            this.from = _from;
            this.to = _to;
            this.line = new Line();
            this.hashcode = this.line.GetHashCode();
            this.X1 = _from.X;
            this.X2 = _to.X;
            this.Y1 = _from.Y;
            this.Y2 = _to.Y;
            this.line.Stroke = Brushes.Red;
            this.line.StrokeThickness = 3;
            thumbList = new List<Thumb>();
            for (int i = 0; i < 2; i++)
            {
                Thumb thumbTmp = new Thumb();
                thumbTmp.DragStarted += new DragStartedEventHandler(onDragStarted);
                thumbTmp.DragCompleted += new DragCompletedEventHandler(onDragCompleted);
                thumbTmp.DragDelta += new DragDeltaEventHandler(onDragDelta);
                //new System.Windows.Input.MouseButtonEventHandler(MyMouseLeftButtonDown);
                int count = thumbList.Count + 1;
                string s = "thumb" + (i + 1).ToString();
                try
                {
                    thumbTmp.Name = s;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                thumbList.Add(thumbTmp);
            }
            thumb1 = new Thumb();
            thumb2 = new Thumb();
            thumb1.Name = "thumb1";
            thumb2.Name = "thumb2";
            active = true;
        }

        public List<Thumb> GetListThumb()
        {
            return thumbList;
        }


        public Thumb GetMovingThumb(UIElement element)
        {
            Thumb res = null;
            foreach (Thumb thumb in thumbList)
            {
                if (thumb.Equals((Thumb)element))
                {
                    res = thumb;
                    break;
                }
            }
            return res;
        }

        public Canvas GetCanvas(ref Canvas canvas)
        {
            return canvas;
        }



        public override UIElement GetShape() { return line; }
    }
}
