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


namespace MultipleFilters
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>

	public partial class Window1 : System.Windows.Window
	{
		CollectionViewSource cvs;
		public Window1()
		{
			InitializeComponent();
			cvs = (CollectionViewSource)(this.Resources["cvs"]);
		}

		private void AddAFilter(object sender, RoutedEventArgs e)
		{
			cvs.Filter += new FilterEventHandler(FilterOutA);
		}

		private void RemoveAFilter(object sender, RoutedEventArgs e)
		{
			cvs.Filter -= new FilterEventHandler(FilterOutA);
		}

		private void AddWhiteHairFilter(object sender, RoutedEventArgs e)
		{
			cvs.Filter += new FilterEventHandler(FilterOutWhiteHair);
		}

		private void RemoveWhiteHairFilter(object sender, RoutedEventArgs e)
		{
			cvs.Filter -= new FilterEventHandler(FilterOutWhiteHair);
		}

		private void FilterOutA(object sender, FilterEventArgs e)
		{
			AsterixCharacter character = e.Item as AsterixCharacter;
			if ((character == null) || character.Name.StartsWith("A"))
			{
				e.Accepted = false;
			}
		}

		private void FilterOutWhiteHair(object sender, FilterEventArgs e)
		{
			AsterixCharacter character = e.Item as AsterixCharacter;
			if ((character == null) || (character.Hair == HairColor.White))
			{
				e.Accepted = false;
			}
		}
	}

	public class AsterixCharacters : ObservableCollection<AsterixCharacter>
	{
		public AsterixCharacters()
		{
			this.Add(new AsterixCharacter("Asterix", HairColor.Blond));
			this.Add(new AsterixCharacter("Assurancetourix", HairColor.Blond));
			this.Add(new AsterixCharacter("Maestria", HairColor.Blond));
			this.Add(new AsterixCharacter("Idefix", HairColor.White));
			this.Add(new AsterixCharacter("Goudurix", HairColor.Blond));
			this.Add(new AsterixCharacter("Mme Agecanonix", HairColor.Red));
			this.Add(new AsterixCharacter("Gueuselambix", HairColor.Blond));
			this.Add(new AsterixCharacter("Panoramix", HairColor.White));
			this.Add(new AsterixCharacter("Pepe", HairColor.Black));
			this.Add(new AsterixCharacter("Obelix", HairColor.Red));
			this.Add(new AsterixCharacter("Abraracourcix", HairColor.Red));
		}
	}

	public class AsterixCharacter
	{
		private string name;

		public string Name
		{
			get { return name; }
		}

		private HairColor hair;

		public HairColor Hair
		{
			get { return hair; }
		}
	
		public AsterixCharacter(string name, HairColor hair)
		{
			this.name = name;
			this.hair = hair;
		}
	}

	public enum HairColor
	{
		Blond,
		White,
		Black,
		Red
	}
}