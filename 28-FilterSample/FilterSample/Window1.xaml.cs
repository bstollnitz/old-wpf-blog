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


namespace FilterSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>

	public partial class Window1 : System.Windows.Window
	{

		public Window1()
		{
			InitializeComponent();

			object src1 = this.Resources["src1"];
			ICollectionView collectionView = CollectionViewSource.GetDefaultView(src1);
			collectionView.Filter = new Predicate<object>(FilterOutA);
		}

		public bool FilterOutA(object item)
		{
			GreekGod gg = item as GreekGod;
			if ((gg == null) || gg.RomanName.StartsWith("A"))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		private void FilterOutB(object sender, FilterEventArgs e)
		{
			GreekHero gh = e.Item as GreekHero;
			if ((gh == null) || gh.HeroName.StartsWith("B"))
			{
				e.Accepted = false;
			}
			else
			{
				e.Accepted = true;
			}
		}
	}

	public class GreekGod
	{
		private string name;

		public string Name
		{
			get { return name; }
		}

		private string description;

		public string Description
		{
			get { return description; }
		}

		private string romanName;

		public string RomanName
		{
			get { return romanName; }
		}

		public GreekGod(string name, string description, string romanName)
		{
			this.name = name;
			this.description = description;
			this.romanName = romanName;
		}
	}

	public class GreekGods : ObservableCollection<GreekGod>
	{
		public GreekGods()
		{
			this.Add(new GreekGod("Aphrodite", "Goddess of love, beauty and fertility", "Venus"));
			this.Add(new GreekGod("Apollo", "God of prophesy, music and healing", "Apollo"));
			this.Add(new GreekGod("Ares", "God of war", "Mars"));
			this.Add(new GreekGod("Artemis", "Virgin goddess of the hunt", "Diana"));
			this.Add(new GreekGod("Athena", "Goddess of crafts and the domestic arts", "Athena"));
			this.Add(new GreekGod("Demeter", "Goddess of agriculture", "Ceres"));
			this.Add(new GreekGod("Dionysus", "God of wine", "Bacchus"));
			this.Add(new GreekGod("Hephaestus", "God of fire and crafts", "Vulcan"));
			this.Add(new GreekGod("Hera", "Goddess of marriage", "Juno"));
			this.Add(new GreekGod("Hermes", "Messenger of the Gods", "Mercury"));
			this.Add(new GreekGod("Poseidon", "God of the sea, earthquakes and horses", "Neptune"));
			this.Add(new GreekGod("Zeus", "Supreme God of the Olympians", "Jupiter"));
		}
	}

	public class GreekHero
	{
		private string heroName;

		public string HeroName
		{
			get { return heroName; }
		}

		public GreekHero(string heroName)
		{
			this.heroName = heroName;
		}
	}

	public class GreekHeroes : ObservableCollection<GreekHero>
	{
		public GreekHeroes()
		{
			this.Add(new GreekHero("Bellerophon"));
			this.Add(new GreekHero("Hercules"));
			this.Add(new GreekHero("Jason"));
			this.Add(new GreekHero("Odysseus"));
			this.Add(new GreekHero("Perseus"));
			this.Add(new GreekHero("Theseus"));
		}
	}
}