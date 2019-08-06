using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SortingHierarchy
{
	public class State : INotifyPropertyChanged
	{
		private string abbreviation;
		public string Abbreviation
		{
			get { return this.abbreviation; }
			set
			{
				if (this.abbreviation != value)
				{
					this.abbreviation = value;
					this.OnPropertyChanged("Abbreviation");
				}
			}
		}

		private string stateName;
		public string StateName
		{
			get { return this.stateName; }
			set
			{
				if (this.stateName != value)
				{
					this.stateName = value;
					this.OnPropertyChanged("StateName");
				}
			}
		}

		public ObservableCollection<County> Counties { get; private set; }

		public State()
		{
			this.Counties = new ObservableCollection<County>();
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
