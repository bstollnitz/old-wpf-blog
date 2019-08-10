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
using System.Diagnostics;
using System.ComponentModel;


namespace UpdateExplicit
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>

	public partial class Window1 : System.Windows.Window
	{

		public Window1()
		{
			InitializeComponent();
			root.DataContext = new MySource();
		}

		private void Submit(object sender, RoutedEventArgs e)
		{
			Employee currentEmployee = (Employee)(lb.Items.CurrentItem);
			ListBoxItem lbi = (ListBoxItem)(lb.ItemContainerGenerator.ContainerFromItem(currentEmployee));
			ContentPresenter cp = GetObjectOfTypeInVisualTree<ContentPresenter>(lbi);
			DataTemplate dt = (DataTemplate)(this.Resources["editableEmployee"]);
			TextBox tb = (TextBox)(dt.FindName("tb", cp));
			BindingExpression be = tb.GetBindingExpression(TextBox.TextProperty);
			be.UpdateSource();
		}

		private T GetObjectOfTypeInVisualTree<T>(DependencyObject dpob) where T : DependencyObject
		{
			int count = VisualTreeHelper.GetChildrenCount(dpob);
			for (int i = 0; i < count; i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(dpob, i);
				T childAsT = child as T;
				if (childAsT != null)
				{
					return childAsT;
				}
                childAsT = GetObjectOfTypeInVisualTree<T>(child);
                if (childAsT != null)
                {
                    return childAsT;
                }
            }
			return null;
		}
	}

	public class MySource
	{
		private ObservableCollection<Employee> employees;

		public ObservableCollection<Employee> Employees
		{
			get { return employees; }
		}

		public MySource()
		{
			employees = new ObservableCollection<Employee>();
			employees.Add(new Employee("Matt", "Program Manager"));
			employees.Add(new Employee("Joan", "Developer"));
			employees.Add(new Employee("Mark", "Programming Writer"));
			employees.Add(new Employee("Mary", "Test Lead"));
			employees.Add(new Employee("Karen", "Developer"));
			employees.Add(new Employee("George", "Programming Writer"));
			employees.Add(new Employee("Peter", "Program Manager"));
		}
	}

	public class Employee : INotifyPropertyChanged
	{
		private string name;

		public string Name
		{
			get { return name; }
			set 
			{ 
				name = value; 
				OnPropertyChanged("Name");
			}
		}

		private string title;

		public string Title
		{
			get { return title; }
			set 
			{ 
				title = value; 
				OnPropertyChanged("Title");
			}
		}
	

		public Employee(string name, string title)
		{
			this.name = name;
			this.title = title;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}