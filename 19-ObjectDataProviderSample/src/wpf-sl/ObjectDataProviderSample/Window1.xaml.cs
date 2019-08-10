using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;


namespace ObjectDataProviderSample
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

	public class MySource 
	{
	    private SolarSystemPlanets planets;
		private Planet planet;
		private Planet earth;

		public Planet Planet
		{
			get { return planet; }
		}

		public SolarSystemPlanets Planets
		{
			get { return planets; }
		}

	    public MySource(string planetName)
	    {
			planets = new SolarSystemPlanets();
			planet = planets.Find(planetName);
			earth = planets.Find("Earth");
		}

	    public double WeightOnPlanet(double weightOnEarth)
	    {
			double massRatio = earth.Mass / planet.Mass;
			double diameterRatio = earth.Diameter / planet.Diameter;
			double weightOnPlanet = weightOnEarth * diameterRatio * diameterRatio / massRatio;
			return weightOnPlanet;
		}
	}

	public class WeightValidationRule : ValidationRule
	{
		public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
		{
			// Value is not a string
			string strValue = value as string;
			if (strValue == null)
			{
				// not going to happen
				return new ValidationResult(false, "Invalid Weight - Value is not a string"); 
			}
			// Value can not be converted to double
			double result;
			bool converted = Double.TryParse(strValue, out result);
			if (!converted)
			{
				return new ValidationResult(false, "Invalid Weight - Please type a valid number");
			}
			// Value is not between 0 and 1000
			if ((result < 0) || (result > 1000))
			{
				return new ValidationResult(false, "Invalid Weight - You're either too light or too heavy");
			}
			return ValidationResult.ValidResult;
		}
	}

	public class DoubleToString : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value != null)
			{
				return value.ToString();
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string strValue = value as string;
			if (strValue != null)
			{
				double result;
				bool converted = Double.TryParse(strValue, out result);
				if (converted)
				{
					return result;
				}
			}
			return null;
		}
	}

	public class SolarSystemPlanets : ObservableCollection<Planet>
	{
		public SolarSystemPlanets()
		{
			this.Add(new Planet("Mercury", 57910000, 4880, 3.30e23));
			this.Add(new Planet("Venus", 108200000, 12103.6, 4.869e24));
			this.Add(new Planet("Earth", 149600000, 12756.3, 5.972e24));
			this.Add(new Planet("Mars", 227940000, 6794, 6.4219e23));
			this.Add(new Planet("Jupiter", 778330000, 142984, 1.900e27));
			this.Add(new Planet("Saturn", 1429400000, 120536, 5.68e26));
			this.Add(new Planet("Uranus", 2870990000, 51118, 8.683e25));
			this.Add(new Planet("Neptune", 4504000000, 49532, 1.0247e26));
			this.Add(new Planet("Pluto", 5913520000, 2274, 1.27e22));
		}

		public Planet Find(string planetName)
		{
			foreach (Planet p in this)
			{
				if (p.Name.Equals(planetName))
				{
					return p;
				}
			}
			return null;
		}
	}

	public class Planet
	{
		private string name;

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private double orbit;

		public double Orbit
		{
			get { return orbit; }
			set { orbit = value; }
		}

		private double diameter;

		public double Diameter
		{
			get { return diameter; }
			set { diameter = value; }
		}

		private double mass;

		public double Mass
		{
			get { return mass; }
			set { mass = value; }
		}

		public Planet(string name, double orbit, double diameter, double mass)
		{
			this.name = name;
			this.orbit = orbit;
			this.diameter = diameter;
			this.mass = mass;
		}
	}
}