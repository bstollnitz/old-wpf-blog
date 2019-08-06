using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Threading;

namespace WPFPresenter
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private Presentation presentation;
        private DispatcherTimer timer;

        public Window1()
        {
            this.InitializeComponent();
            presentation = (Presentation)this.FindResource("presentation");

            // When restarting this app, it will start the slide show in the last edited XAML page.
            int indexLastWritten = 0;
            DateTime latestDateTimeWritten = DateTime.MinValue;
            for (int i = 0; i < presentation.Slides.Length; i++)
            {
                string slide = presentation.Slides[i];
                DateTime dateLastWritten = File.GetLastWriteTime(@"..\..\" + slide);
                if (dateLastWritten.CompareTo(latestDateTimeWritten) > 0)
                {
                    latestDateTimeWritten = dateLastWritten;
                    indexLastWritten = i;
                }
            }
            presentation.CurrentIndex = indexLastWritten;

            // Initializes timer that causes the mouse cursor to disappear after 5 seconds.
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();        
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.Cursor = Cursors.None;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            presentation.GoBack();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            presentation.GoNext();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                presentation.GoBack();
                e.Handled = true;
            }
            else if (e.Key == Key.Right)
            {
                presentation.GoNext();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                Application.Current.Shutdown();
                e.Handled = true;
            }
        }

        private void ShowCursor()
        {
            timer.Start();
            this.ClearValue(FrameworkElement.CursorProperty);
        }

        private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ShowCursor();
            Point currentPoint = e.GetPosition(this);
            TranslateTransform transform = new TranslateTransform(currentPoint.X, currentPoint.Y);
            this.clickCanvas.RenderTransform = transform;
        }

        private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            ShowCursor();
        }
    }
}
