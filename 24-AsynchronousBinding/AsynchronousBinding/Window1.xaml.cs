using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Net;


namespace AsynchronousBinding
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

		private void SlowDataSourceSync(object sender, RoutedEventArgs args)
		{
			((Button)sender).IsEnabled = false;
			ObjectDataProvider odp = new ObjectDataProvider();
			//odp.IsAsynchronous = false; - this is the default
			odp.ObjectType = typeof(SlowDataSource);
			Binding b = new Binding();
			b.Source = odp;
			b.Path = new PropertyPath("Property");
			tbSlowDSSync.SetBinding(TextBlock.TextProperty, b);
		}

		private void SlowDataSourceAsync(object sender, RoutedEventArgs args)
		{
			((Button)sender).IsEnabled = false;
			ObjectDataProvider odp = new ObjectDataProvider();
			odp.IsAsynchronous = true;
			odp.ObjectType = typeof(SlowDataSource);
			Binding b = new Binding();
			b.Source = odp;
			b.Path = new PropertyPath("Property");
			tbSlowDSAsync.SetBinding(TextBlock.TextProperty, b);
		}

		private void SlowPropertySync(object sender, RoutedEventArgs args)
		{
			((Button)sender).IsEnabled = false;
			DataSource source = new DataSource();
			Binding b = new Binding();
			b.Source = source;
			//b.IsAsync = false; - this is the default
			b.Path = new PropertyPath("SlowProperty");
			tbSlowPropSync.SetBinding(TextBlock.TextProperty, b);
		}

		private void SlowPropertyAsync(object sender, RoutedEventArgs args)
		{
			((Button)sender).IsEnabled = false;
			DataSource source = new DataSource();
			Binding b = new Binding();
			b.Source = source;
			b.IsAsync = true;
			b.Path = new PropertyPath("SlowProperty");
			tbSlowPropAsync.SetBinding(TextBlock.TextProperty, b);
		}
	}

	public class SlowDataSource
	{
		private string property;

		public string Property
		{
			get { return property; }
			set { property = value; }
		}
	
		public SlowDataSource()
		{
			Thread.Sleep(3000);
			this.property = "Slow data source";
		}
	}

	public class DataSource
	{
		private string slowProperty;

		public string SlowProperty
		{
			get 
			{
				Thread.Sleep(3000);
				return slowProperty; 
			}
			set { slowProperty = value; }
		}

		public DataSource()
		{
			this.slowProperty = "Slow property";
		}
	}
}