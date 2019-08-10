using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Globalization;

namespace GroupByType
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

	public class GodHeroTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			string templateKey;
			if (item is CollectionViewGroup)
			{
				templateKey = "GroupTemplate";
			}
			else if (item is GreekGod)
			{
				templateKey = "GreekGodTemplate";
			}
			else if (item is GreekHero)
			{
				templateKey = "GreekHeroTemplate";
			}
			else
			{
				return null;
			}
			return (DataTemplate)((FrameworkElement)container).FindResource(templateKey);
		}
	}

	public class GroupByTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is GreekGod)
			{
				return "Greek Gods";
			}
			else if (value is GreekHero)
			{
				return "Greek Heroes";
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class GreekGodsAndHeroes : ObservableCollection<object>
	{
		public GreekGodsAndHeroes()
		{
			Add(new GreekGod("Aphrodite", "Goddess of love, beauty and fertility"));
			Add(new GreekGod("Apollo", "God of prophesy, music and healing"));
			Add(new GreekGod("Ares", "God of war"));
			Add(new GreekGod("Artemis", "Virgin goddess of the hunt"));
			Add(new GreekGod("Athena", "Goddess of crafts and the domestic arts"));
			Add(new GreekHero("Bellerophon"));
			Add(new GreekGod("Demeter", "Goddess of agriculture"));
			Add(new GreekGod("Dionysus", "God of wine"));
			Add(new GreekGod("Hephaestus", "God of fire and crafts"));
			Add(new GreekGod("Hera", "Goddess of marriage"));
			Add(new GreekHero("Hercules"));
			Add(new GreekGod("Hermes", "Messenger of the gods and guide of dead souls to the Underworld"));
			Add(new GreekHero("Jason"));
			Add(new GreekHero("Odysseus"));
			Add(new GreekHero("Perseus"));
			Add(new GreekGod("Poseidon", "God of the sea, earthquakes and horses"));
			Add(new GreekHero("Theseus"));
			Add(new GreekGod("Zeus", "The supreme god of the Olympians"));
		}
	}

	public class GreekGod
	{
		private string godName;

		public string GodName
		{
			get { return godName; }
			set { godName = value; }
		}

		private string godDescription;

		public string GodDescription
		{
			get { return godDescription; }
			set { godDescription = value; }
		}

		public GreekGod(string godName, string godDescription)
		{
			this.GodName = godName;
			this.GodDescription = godDescription;
		}
	}

	public class GreekHero
	{
		private string heroName;

		public string HeroName
		{
			get { return heroName; }
			set { heroName = value; }
		}

		public GreekHero(string heroName)
		{
			this.HeroName = heroName;
		}
	}
}