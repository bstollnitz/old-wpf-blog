using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.ComponentModel;
using System.Windows.Input;

namespace Presenter
{
	public partial class Window1
	{
        private Presentation presentation;

		public Window1()
		{
			this.InitializeComponent();
            presentation = (Presentation)this.FindResource("presentation");

            int indexLastWritten = 0;
            DateTime latestDateTimeWritten = DateTime.MinValue;
            for(int i=0; i<presentation.Slides.Length; i++)
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
        }
	}
}