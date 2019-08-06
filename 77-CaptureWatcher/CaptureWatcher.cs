using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace ZagStudio.Helpers
{
	public class CaptureWatcher
    {
        #region --- constants ---

        private const double margin = 2;
        private const double borderThickness = 2;
        private const double fontSize = 13;
        private static readonly Thickness padding = new Thickness(10, 6, 10, 4);
        private static readonly Color backgroundColor = Colors.White;
        private static readonly Color borderColor = Colors.Black;
        private static readonly Color foregroundColor = Colors.Black;
        private static readonly TimeSpan updateInterval = TimeSpan.FromSeconds(0.25);

        #endregion

        #region --- fields ---

        private static CaptureWatcher instance;

        private TextBlock textBlock;
        private Popup popup;
        private DispatcherTimer timer;

        #endregion

        #region --- constructor ---

        private CaptureWatcher()
		{
			this.textBlock = new TextBlock()
			{
				FontSize = fontSize,
				FontWeight = FontWeights.SemiLight,
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

			this.popup = new Popup
			{
				Child = border,
				HorizontalOffset = margin,
				VerticalOffset = margin,
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
            instance = new CaptureWatcher();
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
			StringBuilder builder = new StringBuilder();
			foreach (UIElement element in Window.Current.Content.DescendantsAndSelf<UIElement>())
			{
				IReadOnlyList<Pointer> pointerCaptures = element.PointerCaptures;
				if (pointerCaptures != null && pointerCaptures.Count > 0)
				{
					builder.Append(element.GetType().Name);
					builder.Append(GetName(element));
					builder.AppendLine(GetContent(element));

					foreach (Pointer pointer in pointerCaptures)
					{
						builder.Append("   ");
						builder.Append(pointer.PointerDeviceType.ToString());
						builder.Append(" (");
						builder.Append(pointer.PointerId);
						builder.AppendLine(pointer.IsInContact ? ") in contact" :
							(pointer.IsInRange ? ") in range" : ")"));
					}
				}
			}
			if (builder.Length > 0)
			{
				this.textBlock.Text = builder.ToString().Trim();
			}
			else
			{
				this.textBlock.Text = "no captures";
			}
		}

		private static string GetName(object focusedElement)
		{
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

        #endregion --- private methods ---
    }
}
