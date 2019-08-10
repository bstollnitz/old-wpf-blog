using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Presenter
{
    class SolarSystem 
    {
        private ObservableCollection<SolarSystemObject> solarSystemObjects;

        public ObservableCollection<SolarSystemObject> SolarSystemObjects
        {
            get { return solarSystemObjects; }
        }
	

        public SolarSystem()
        {
            this.solarSystemObjects = new ObservableCollection<SolarSystemObject>();
            this.solarSystemObjects.Add(new SolarSystemObject("Sun", 0, 1380000, new Uri(@"Images\sun.jpg", UriKind.Relative), "The yellow dwarf star in the center of our solar system."));
            this.solarSystemObjects.Add(new SolarSystemObject("Mercury", 0.38, 4880, new Uri(@"Images\merglobe.gif", UriKind.Relative), "The small and rocky planet Mercury is the closest planet to the Sun."));
            this.solarSystemObjects.Add(new SolarSystemObject("Venus", 0.72, 12103.6, new Uri(@"Images\venglobe.gif", UriKind.Relative), "At first glance, if Earth had a twin, it would be Venus."));
            this.solarSystemObjects.Add(new SolarSystemObject("Earth", 1, 12756.3, new Uri(@"Images\earglobe.gif", UriKind.Relative), "Earth, our home planet, is the only planet in our solar system known to harbor life."));
            this.solarSystemObjects.Add(new SolarSystemObject("Mars", 1.52, 6794, new Uri(@"Images\marglobe.gif", UriKind.Relative), "The red planet Mars has inspired wild flights of imagination over the centuries."));
            this.solarSystemObjects.Add(new SolarSystemObject("Jupiter", 5.20, 142984, new Uri(@"Images\jupglobe.gif", UriKind.Relative), "With its numerous moons and several rings, the Jupiter system is a \"mini-solar system.\""));
            this.solarSystemObjects.Add(new SolarSystemObject("Saturn", 9.54, 120536, new Uri(@"Images\2moons_2.gif", UriKind.Relative), "Saturn is the most distant of the five planets known to ancient stargazers."));
            this.solarSystemObjects.Add(new SolarSystemObject("Uranus", 19.218, 51118, new Uri(@"Images\uraglobe.gif", UriKind.Relative), "Uranus gets its blue-green color from methane gas above the deeper cloud layers."));
            this.solarSystemObjects.Add(new SolarSystemObject("Neptune", 30.06, 49532, new Uri(@"Images\nepglobe.gif", UriKind.Relative), "Neptune was the first planet located through mathematical predictions."));
            this.solarSystemObjects.Add(new SolarSystemObject("Pluto", 39.5, 2274, new Uri(@"Images\plutoch_2.gif", UriKind.Relative), "Long considered to be the smallest, coldest, and most distant planet from the Sun."));
        }
    }
}
