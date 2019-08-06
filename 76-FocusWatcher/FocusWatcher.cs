using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace ZagStudio.Helpers
{
	public class FocusWatcher
    {
        #region --- constants ---

        private const double margin = 2;
        private const double borderThickness = 2;
        private const double highlightThickness = 6;
        private const double fontSize = 13;
        private static readonly Thickness padding = new Thickness(10, 6, 10, 4);
        private static readonly Color backgroundColor = Colors.White;
        private static readonly Color borderColor = Colors.Black;
        private static readonly Color foregroundColor = Colors.Black;
        private static readonly Color highlightColor = Color.FromArgb(128, 255, 0, 0);
        private static readonly TimeSpan updateInterval = TimeSpan.FromSeconds(0.25);

        #endregion

        #region --- fields ---

        private static FocusWatcher instance;

        private Run focusedTypeRun;
		private Run focusedNameRun;
		private Run focusedContentRun;
		private Rectangle highlightRectangle;
        private Popup popup;
        private DispatcherTimer timer;

        #endregion

        #region --- constructor ---

        private FocusWatcher()
		{
			this.focusedTypeRun = new Run
			{
				FontWeight = FontWeights.SemiBold
			};
			this.focusedNameRun = new Run
			{
				FontWeight = FontWeights.SemiLight
			};
			this.focusedContentRun = new Run
			{
				FontWeight = FontWeights.SemiLight
			};

			TextBlock textBlock = new TextBlock()
			{
				Inlines = { this.focusedTypeRun, this.focusedNameRun, this.focusedContentRun },
				FontSize = fontSize,
				Foreground = new SolidColorBrush(foregroundColor)
			};

			Border border = new Border
			{
				Child = textBlock,
				Background = new SolidColorBrush(backgroundColor),
                BorderBrush = new SolidColorBrush(borderColor),
				BorderThickness = new Thickness(borderThickness),
				Padding = padding
			};
			Canvas.SetLeft(border, margin);
			Canvas.SetTop(border, margin);

			this.highlightRectangle = new Rectangle
			{
				Stroke = new SolidColorBrush(highlightColor),
				StrokeThickness = highlightThickness,
				IsHitTestVisible = false
			};
			
			Canvas canvas = new Canvas();
			canvas.Children.Add(highlightRectangle);
			canvas.Children.Add(border);

			this.popup = new Popup
			{
				Child = canvas,
				IsOpen = true
			};

			this.timer = new DispatcherTimer
			{
				Interval = updateInterval
			};
            this.timer.Tick += this.Timer_Tick;
            this.timer.Start();
		}

        #endregion

        #region --- public methods ---

        public static void Start()
        {
            instance = new FocusWatcher();
        }

        public static void Stop()
        {
            if (instance != null)
            {
                instance.timer.Stop();
                instance.popup.IsOpen = false;
                instance = null;
            }
        }

        #endregion

        #region --- private methods ---

        private void Timer_Tick(object sender, object e)
		{
            // Update the type, name, and content shown in the popup's text.
			object focusedElement = FocusManager.GetFocusedElement();
			if (focusedElement == null)
			{
				this.focusedTypeRun.Text = "null";
				this.focusedNameRun.Text = String.Empty;
				this.focusedContentRun.Text = String.Empty;
			}
			else
			{
				this.focusedTypeRun.Text = focusedElement.GetType().Name;
				this.focusedNameRun.Text = GetName(focusedElement);
				this.focusedContentRun.Text = GetContent(focusedElement);
			}

            // Update the outline around the focused element.
			UIElement element = focusedElement as UIElement;
			if (element != null)
			{
				Rect bounds = new Rect(new Point(0, 0), element.RenderSize);
				bounds = element.TransformToVisual(Window.Current.Content).TransformBounds(bounds);
                double left = Math.Max(bounds.Left - highlightThickness, 0);
                double top = Math.Max(bounds.Top - highlightThickness, 0);
                double right = Math.Min(bounds.Right + highlightThickness, Window.Current.Bounds.Width);
                double bottom = Math.Min(bounds.Bottom + highlightThickness, Window.Current.Bounds.Height);
				Canvas.SetLeft(this.highlightRectangle, left);
				Canvas.SetTop(this.highlightRectangle, top);
                this.highlightRectangle.Width = Math.Max(0, right - left);
				this.highlightRectangle.Height = Math.Max(0, bottom - top);
			}

            // Close and re-open the popup to make sure it appears in front of all other popups.
            this.popup.IsOpen = false;
            this.popup.IsOpen = true;
		}

		private static string GetName(object focusedElement)
		{
            // See if the focused element has a name.
			string name = String.Empty;
			FrameworkElement frameworkElement = focusedElement as FrameworkElement;
			if (frameworkElement != null)
			{
				name = frameworkElement.Name;
			}

			return String.IsNullOrEmpty(name) ? String.Empty : " (" + name + ")";
		}

		private static string GetContent(object focusedElement)
		{
            // See if the focused element has content that we can present as a string.
			string content = String.Empty;
			ContentControl contentControl = focusedElement as ContentControl;
			if (contentControl != null && contentControl.Content != null)
			{
				TextBlock textBlock = contentControl.Content as TextBlock;
				if (textBlock != null)
				{
					content = textBlock.Text;
				}
				else
				{
					content = contentControl.Content.ToString();
				}
			}

			return String.IsNullOrEmpty(content) ? String.Empty : " [" + content + "]";
		}

        #endregion
    }
}
