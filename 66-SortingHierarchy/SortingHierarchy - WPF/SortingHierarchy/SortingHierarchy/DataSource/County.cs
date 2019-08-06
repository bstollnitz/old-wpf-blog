using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace SortingHierarchy
{
	public class County : INotifyPropertyChanged
	{
		private string countyName;
		public string CountyName
		{
			get { return this.countyName; }
			set
			{
				if (this.countyName != value)
				{
					this.countyName = value;
					this.OnPropertyChanged("CountyName");
				}
			}
		}

		public ObservableCollection<City> Cities { get; private set; }

		public County()
		{
			this.Cities = new ObservableCollection<City>();
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
