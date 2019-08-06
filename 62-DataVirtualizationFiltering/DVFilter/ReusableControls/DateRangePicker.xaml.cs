using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReusableControls
{
	public partial class DateRangePicker : UserControl
	{
		public event EventHandler FromDateChanged;
		public event EventHandler ToDateChanged;

		public Nullable<DateTime> DateFrom
		{
			get { return (Nullable<DateTime>)this.GetValue(DateFromProperty); }
			set { this.SetValue(DateFromProperty, value); }
		}

		public static readonly DependencyProperty DateFromProperty =
			DependencyProperty.Register("DateFrom", typeof(Nullable<DateTime>), typeof(DateRangePicker), new PropertyMetadata(DateFrom_PropertyChanged));


		public Nullable<DateTime> DateTo
		{
			get { return (Nullable<DateTime>)this.GetValue(DateToProperty); }
			set { this.SetValue(DateToProperty, value); }
		}

		public static readonly DependencyProperty DateToProperty =
			DependencyProperty.Register("DateTo", typeof(Nullable<DateTime>), typeof(DateRangePicker), new PropertyMetadata(DateTo_PropertyChanged));

		public DateRangePicker()
		{
			this.InitializeComponent();

			this.DatePickerFrom.BlackoutDates.Add(new CalendarDateRange(DateTime.Today.AddDays(1), DateTime.MaxValue));
			this.DatePickerTo.BlackoutDates.Add(new CalendarDateRange(DateTime.Today.AddDays(1), DateTime.MaxValue));

			this.DatePickerFrom.SetBinding(DatePicker.SelectedDateProperty, new Binding("DateFrom") { Source = this, Mode = BindingMode.TwoWay });
			this.DatePickerTo.SetBinding(DatePicker.SelectedDateProperty, new Binding("DateTo") { Source = this, Mode = BindingMode.TwoWay });
		}

		private static void DateFrom_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			DateRangePicker dateRangePicker = (DateRangePicker)obj;
			dateRangePicker.DateFromChanged();
		}

		private void DateFromChanged()
		{
			// This updates the blackout dates for DatePickerTo.
			this.DatePickerTo.BlackoutDates.Clear();
			if (this.DateFrom.HasValue)
			{
				DateTime dateFrom = this.DateFrom.Value;
				if (this.DateTo.HasValue)
				{
					DateTime dateTo = this.DateTo.Value;
					if (dateTo <= dateFrom)
					{
						this.DateTo = null;
					}
				}

				this.DatePickerTo.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, dateFrom));
			}
			this.DatePickerTo.BlackoutDates.Add(new CalendarDateRange(DateTime.Today.AddDays(1), DateTime.MaxValue));

			this.OnDateChanged(FromDateChanged);
		}

		private static void DateTo_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			DateRangePicker dateRangePicker = (DateRangePicker)obj;
			dateRangePicker.OnDateChanged(dateRangePicker.ToDateChanged);
		}

		private void OnDateChanged(EventHandler handler)
		{
			if (handler != null)
			{
				handler(this, EventArgs.Empty);
			}
		}
	}
}
