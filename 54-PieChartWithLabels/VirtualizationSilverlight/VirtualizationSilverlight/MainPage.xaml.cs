using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Data;

namespace VirtualizationSilverlight
{
    public partial class MainPage : UserControl, INotifyPropertyChanged
    {
        private int pageSize;
        public int PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                pageSize = value;
                OnPropertyChanged("PageSize");
            }
        }

        private int pageTimeout;
        public int PageTimeout
        {
            get
            {
                return pageTimeout;
            }
            set
            {
                pageTimeout = value;
                OnPropertyChanged("PageTimeout");
            }
        }

        private int numItems;
        public int NumItems
        {
            get
            {
                return numItems;
            }
            set
            {
                numItems = value;
                OnPropertyChanged("NumItems");
            }
        }

        private int fetchDelay;
        public int FetchDelay
        {
            get
            {
                return fetchDelay;
            }
            set
            {
                fetchDelay = value;
                OnPropertyChanged("FetchDelay");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public MainPage()
        {
            InitializeComponent();

            this.PageSize = 100; // 100 items per page
            this.PageTimeout = 100000; // 100 seconds before it times out
            this.NumItems = 100000; // loads 100,000 items
            this.FetchDelay = 1000; // 1 second delay per "database" query

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            tbMemory.Text = string.Format("{0:0.00} MB", GC.GetTotalMemory(true) / 1024.0 / 1024.0);
        }

        private void LoadListBoxData1(object sender, RoutedEventArgs e)
        { 
            DemoCustomerProvider customerProvider = new DemoCustomerProvider(this.NumItems, this.FetchDelay);
            sample1.DataContext = new AsyncVirtualizingCollection<PaulsCustomer>(customerProvider, this.PageSize, this.PageTimeout);
        }

        private void ClearListBoxData1(object sender, RoutedEventArgs e)
        {
            sample1.DataContext = null;
        }

    }
}
