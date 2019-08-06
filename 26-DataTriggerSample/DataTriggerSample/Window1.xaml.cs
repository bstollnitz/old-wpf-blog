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
using System.Globalization;

namespace DataTriggerSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>

	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
		}
	}

	public class IsInKeywords : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ReadOnlyCollection<string> keywords = value as ReadOnlyCollection<string>;
			return keywords.Contains(parameter.ToString());
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}

	public class TechEdPictures : ObservableCollection<Uri>
	{
		public TechEdPictures()
		{
			this.Add(new Uri(@"Pictures\TechEd 004.jpg", UriKind.Relative));
			this.Add(new Uri(@"Pictures\TechEd 022.jpg", UriKind.Relative));
			this.Add(new Uri(@"Pictures\TechEd 029.jpg", UriKind.Relative));
			this.Add(new Uri(@"Pictures\TechEd 030.jpg", UriKind.Relative));
			this.Add(new Uri(@"Pictures\TechEd 051.jpg", UriKind.Relative));
			this.Add(new Uri(@"Pictures\TechEd 061.jpg", UriKind.Relative));
			this.Add(new Uri(@"Pictures\TechEd 066.jpg", UriKind.Relative));
			this.Add(new Uri(@"Pictures\TechEd 074.jpg", UriKind.Relative));
			this.Add(new Uri(@"Pictures\TechEd 075.jpg", UriKind.Relative));
			this.Add(new Uri(@"Pictures\TechEd 077.jpg", UriKind.Relative));
			this.Add(new Uri(@"Pictures\TechEd 082.jpg", UriKind.Relative));
		}
	}
}