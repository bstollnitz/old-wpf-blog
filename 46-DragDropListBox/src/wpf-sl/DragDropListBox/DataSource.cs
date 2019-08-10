using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace DragDropListBox
{
	public class Album
	{
		public ObservableCollection<Picture> PicturesLeft { get; set; }
		public ObservableCollection<Picture> PicturesRight { get; set; }

		public Album()
		{
			this.PicturesLeft = new ObservableCollection<Picture>();
			this.PicturesLeft.Add(new Picture(new Uri("Images\\2moons_2.gif", UriKind.Relative), "Saturn", "Saturn is the most distant of the five planets known to ancient stargazers."));
			this.PicturesLeft.Add(new Picture(new Uri("Images\\earglobe.gif", UriKind.Relative), "Earth", "Earth, our home planet, is the only planet in our solar system known to harbor life."));
			this.PicturesLeft.Add(new Picture(new Uri("Images\\jupglobe.gif", UriKind.Relative), "Jupiter", "With its numerous moons and several rings, the Jupiter system is a \"mini-solar system.\""));
			this.PicturesLeft.Add(new Picture(new Uri("Images\\marglobe.gif", UriKind.Relative), "Mars", "The red planet Mars has inspired wild flights of imagination over the centuries."));
			this.PicturesLeft.Add(new Picture(new Uri("Images\\merglobe.gif", UriKind.Relative), "Mercury", "The small and rocky planet Mercury is the closest planet to the Sun."));
			this.PicturesLeft.Add(new Picture(new Uri("Images\\nepglobe.gif", UriKind.Relative), "Neptune", "Neptune was the first planet located through mathematical predictions."));
			this.PicturesLeft.Add(new Picture(new Uri("Images\\plutoch_2.gif", UriKind.Relative), "Pluto", "Long considered to be the smallest, coldest, and most distant planet from the Sun."));
			this.PicturesLeft.Add(new Picture(new Uri("Images\\uraglobe.gif", UriKind.Relative), "Uranus", "Uranus gets its blue-green color from methane gas above the deeper cloud layers."));
			this.PicturesLeft.Add(new Picture(new Uri("Images\\venglobe.gif", UriKind.Relative), "Venus", "At first glance, if Earth had a twin, it would be Venus."));

			this.PicturesRight = new ObservableCollection<Picture>();
		}
	}

	public class Picture
	{
		public Uri Location { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public Picture(Uri location, string name, string description)
		{
			this.Location = location;
			this.Name = name;
			this.Description = description;

			this.Pictures = new ObservableCollection<Picture>();
		}

		public ObservableCollection<Picture> Pictures { get; set; }
	}
}
