using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace PlanetsListBox
{
	public class SolarSystem
	{
		private ObservableCollection<SolarSystemObject> solarSystemObjects;

		public ObservableCollection<SolarSystemObject> SolarSystemObjects
		{
			get { return solarSystemObjects; }
		}

		public SolarSystem()
		{
			this.solarSystemObjects = new ObservableCollection<SolarSystemObject>();
			this.solarSystemObjects.Add(new SolarSystemObject("Sun", 0, 1380000, "Images/sun.jpg", "The yellow dwarf star in the center of our solar system."));
			this.solarSystemObjects.Add(new SolarSystemObject("Mercury", 0.38, 4880, "Images/merglobe.gif", "The small and rocky planet Mercury is the closest planet to the Sun."));
			this.solarSystemObjects.Add(new SolarSystemObject("Venus", 0.72, 12103.6, "Images/venglobe.gif", "At first glance, if Earth had a twin, it would be Venus."));
			this.solarSystemObjects.Add(new SolarSystemObject("Earth", 1, 12756.3, "Images/earglobe.gif", "Earth, our home planet, is the only planet in our solar system known to harbor life."));
			this.solarSystemObjects.Add(new SolarSystemObject("Mars", 1.52, 6794, "Images/marglobe.gif", "The red planet Mars has inspired wild flights of imagination over the centuries."));
			this.solarSystemObjects.Add(new SolarSystemObject("Jupiter", 5.20, 142984, "Images/jupglobe.gif", "With its numerous moons and several rings, the Jupiter system is a \"mini-solar system.\""));
			this.solarSystemObjects.Add(new SolarSystemObject("Saturn", 9.54, 120536, "Images/2moons_2.gif", "Saturn is the most distant of the five planets known to ancient stargazers."));
			this.solarSystemObjects.Add(new SolarSystemObject("Uranus", 19.218, 51118, "Images/uraglobe.gif", "Uranus gets its blue-green color from methane gas above the deeper cloud layers."));
			this.solarSystemObjects.Add(new SolarSystemObject("Neptune", 30.06, 49532, "Images/nepglobe.gif", "Neptune was the first planet located through mathematical predictions."));
		}
	}
}
