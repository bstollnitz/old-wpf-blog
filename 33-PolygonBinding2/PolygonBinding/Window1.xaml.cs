using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PolygonBinding
{
	public partial class Window1 : System.Windows.Window
	{
		public Window1()
		{
			InitializeComponent();
		}

		private void ChangeSource(object sender, RoutedEventArgs e)
		{
			PolygonItem polygonItem = this.Resources["src"] as PolygonItem;
			polygonItem.AddPoint();

			// Unfortunately, the Polygon element won't update unless we call InvalidateMeasure and InvalidateVisual.
			this.polygonElement.InvalidateMeasure();
			this.polygonElement.InvalidateVisual();
        }
    }
}