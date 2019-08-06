using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Collections.Specialized;


namespace BoundListView
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

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			((INotifyCollectionChanged)lv.Items).CollectionChanged += new NotifyCollectionChangedEventHandler(Restyle);
		}

		private void AddItem(object sender, RoutedEventArgs args)
		{
			XmlDataProvider xdp = (XmlDataProvider)(this.Resources["planets"]);
			XmlDocument doc = xdp.Document;
			XmlElement xe = doc.CreateElement("Planet");
			xe.SetAttribute("Name", "Planet X");
			XmlElement orbit = doc.CreateElement("Orbit");
			orbit.InnerText = "97,240,000 km (0.65 AU)";
			xe.AppendChild(orbit);
			XmlElement diameter = doc.CreateElement("Diameter");
			diameter.InnerText = "8,445.6 km";
			xe.AppendChild(diameter);
			XmlElement mass = doc.CreateElement("Mass");
			mass.InnerText = "8.653e24 kg";
			xe.AppendChild(mass);
			XmlElement image = doc.CreateElement("Image");
			image.InnerText = "planetx.jpg";
			xe.AppendChild(image);
			XmlElement details = doc.CreateElement("Details");
			details.InnerText = "This planet inhabited by aliens is cloaked so that humans can't see it.";
			xe.AppendChild(details);
			doc.ChildNodes[0].InsertAfter(xe, doc.ChildNodes[0].FirstChild);
		}

		private void Restyle(object sender, NotifyCollectionChangedEventArgs args)
		{
			StyleSelector selector = lv.ItemContainerStyleSelector;
			lv.ItemContainerStyleSelector = null;
			lv.ItemContainerStyleSelector = selector;
		}
	}

	public class ListViewItemStyleSelector : StyleSelector
	{
		private int i = 0;
		public override Style SelectStyle(object item, DependencyObject container)
		{
			// makes sure the first item always gets the first style, even when restyling
			ItemsControl ic = ItemsControl.ItemsControlFromItemContainer(container);
			if (item == ic.Items[0])
			{
				i = 0;
			}
			string styleKey;
			if (i % 2 == 0)
			{
				styleKey = "ListViewItemStyle1";
			}
			else
			{
				styleKey = "ListViewItemStyle2";
			}
			i++;
			return (Style)(ic.FindResource(styleKey));
		}
	}

	public class StringToImageSourceConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string strValue = value as string;
			if (strValue != null)
			{
				return new BitmapImage(new Uri(strValue, UriKind.Relative));
			}
			throw new InvalidOperationException("Unexpected value in converter");
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new InvalidOperationException("The method or operation is not implemented.");
		}
	}

}