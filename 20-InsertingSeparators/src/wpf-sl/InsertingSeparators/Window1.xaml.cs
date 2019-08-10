using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;


namespace InsertingSeparators
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

	public class SeparatorStyleSelector : StyleSelector
	{
		public override Style SelectStyle(object item, DependencyObject container)
		{
			if (item is SeparatorData)
			{
				return (Style)((FrameworkElement)container).FindResource("separatorStyle");
			}
			return null;
		}
	}

	public class Animal
	{
		private string name;

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private Category category;

		public Category Category
		{
			get { return category; }
			set { category = value; }
		}

		public Animal(string name, Category category)
		{
			this.name = name;
			this.category = category;
		}
	}

	public enum Category
	{
		Amphibians,
		Bears,
		BigCats,
		Canines,
		Primates,
		Spiders,
	}

	public class SeparatorData
	{
	}

	public class Source
	{
		private ObservableCollection<object> animalCollection;

		public ObservableCollection<object> AnimalCollection
		{
			get { return animalCollection; }
		}

		public Source()
		{
			animalCollection = new ObservableCollection<object>();
			animalCollection.Add(new Animal("Golden Silk Spider", Category.Spiders));
			animalCollection.Add(new Animal("Black Widow Spider", Category.Spiders));
			animalCollection.Add(new Separator());
			animalCollection.Add(new Animal("Jaguar", Category.BigCats));
			animalCollection.Add(new Animal("African Wildcat", Category.BigCats));
			animalCollection.Add(new Animal("Cheetah", Category.BigCats));
			animalCollection.Add(new Separator());
			animalCollection.Add(new Animal("California Newt", Category.Amphibians));
			animalCollection.Add(new Animal("Tomato Frog", Category.Amphibians));
			animalCollection.Add(new Animal("Green Tree Frog", Category.Amphibians));
		}
	}
}