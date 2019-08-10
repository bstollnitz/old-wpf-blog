using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SortingHierarchy
{
	public class City : INotifyPropertyChanged
	{
		private string cityName;
		public string CityName
		{
			get { return this.cityName; }
			set
			{
				if (this.cityName != value)
				{
					this.cityName = value;
					this.OnPropertyChanged("CityName");
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			System.Diagnostics.Debug.Assert(String.IsNullOrEmpty(propertyName) || this.GetType().GetProperty(propertyName) != null);
			PropertyChangedEventHandler handler = this.PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
