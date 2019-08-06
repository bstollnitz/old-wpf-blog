using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace CaptureWatcherSample
{
    public sealed partial class MainPage : Page
    {
        private const int size = 100;

        private Color[] colors =
        {
            Colors.Red,
            Colors.Orange,
            Colors.Yellow,
            Colors.Green,
            Colors.Aqua,
            Colors.Blue,
            Colors.Violet
        };
        private Dictionary<uint, Ellipse> ellipses =
            new Dictionary<uint,Ellipse>();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (this.canvas.CapturePointer(e.Pointer))
            {
                uint id = e.Pointer.PointerId;
                Ellipse ellipse = this.GetEllipse(id);
                if (ellipse == null)
                {
                    ellipse = new Ellipse
                    {
                        Width = size,
                        Height = size,
                        Fill = new SolidColorBrush(this.colors[id % this.colors.Length])
                    };
                    this.ellipses[id] = ellipse;
                }

                Point position = e.GetCurrentPoint(this.canvas).Position;
                Canvas.SetLeft(ellipse, position.X - size / 2);
                Canvas.SetTop(ellipse, position.Y - size / 2);
                this.canvas.Children.Add(ellipse);
            }
        }

        private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            uint id = e.Pointer.PointerId;
            Ellipse ellipse = this.GetEllipse(id);
            if (ellipse != null)
            {
                Point position = e.GetCurrentPoint(this.canvas).Position;
                Canvas.SetLeft(ellipse, position.X - size / 2);
                Canvas.SetTop(ellipse, position.Y - size / 2);
            }
        }

        private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            this.canvas.ReleasePointerCapture(e.Pointer);
        }

        private void Canvas_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            uint id = e.Pointer.PointerId;
            Ellipse ellipse = this.GetEllipse(id);
            if (ellipse != null)
            {
                this.canvas.Children.Remove(ellipse);
                this.ellipses.Remove(id);
            }
        }

        private Ellipse GetEllipse(uint pointerId)
        {
            Ellipse ellipse = null;
            this.ellipses.TryGetValue(pointerId, out ellipse);
            return ellipse;
        }
    }
}
