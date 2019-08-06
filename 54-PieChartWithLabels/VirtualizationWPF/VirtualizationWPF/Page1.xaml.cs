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
using System.Windows.Threading;
using System.Threading;
using System.ComponentModel;
using WPF.Themes;

namespace VirtualizationWPF
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page, INotifyPropertyChanged
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

        // The following fields are used in Vincent's sorting solution.
        private GridViewColumnHeader m_LastHeaderClicked;
        private ListSortDirection m_LastDirection;

        public Page1()
        {
            Application.Current.ApplyTheme("ExpressionDark");

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
            DemoCustomerProvider customerProvider = new DemoCustomerProvider(numItems, fetchDelay);
            sample1.DataContext = new AsyncVirtualizingCollection<PaulsCustomer>(customerProvider, pageSize, pageTimeout);
        }

        private void ClearListBoxData1(object sender, RoutedEventArgs e)
        {
            sample1.DataContext = null;
        }

        private void LoadListBoxData2(object sender, RoutedEventArgs e)
        {
            sample2.DataContext = new VirtualList<VincentsCustomer>(Load, 6 /* # of pages in memory at the same time */, pageSize);
        }

        private void ClearListBoxData2(object sender, RoutedEventArgs e)
        {
            sample2.DataContext = null;
        }

        // The customers array is already allocated and our job is to populate all of its elements.
        // This method should return the total number of data items (not the number of items we're fetching).
        private int Load(SortDescriptionCollection sortDescriptions, Predicate<object> filter, VincentsCustomer[] customers, int startIndex)
        {
            Thread.Sleep(fetchDelay);

            // Sorting
            bool isDescending = sortDescriptions != null && sortDescriptions.Count > 0 && sortDescriptions[0].Direction == ListSortDirection.Descending;
            int customerIndex;

            for (int i = 0; startIndex < numItems && i < customers.Length; ++i, ++startIndex)
            {
                customerIndex = isDescending ? numItems - startIndex - 1 : startIndex;
                customers[i] = new VincentsCustomer { Id = customerIndex, Name = "Customer " + customerIndex };
            }

            return numItems;
        }

        private void TheView_HeaderClick(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null && headerClicked.Role != GridViewColumnHeaderRole.Padding)
            {
                if (m_LastHeaderClicked == null || (headerClicked == m_LastHeaderClicked && m_LastDirection == ListSortDirection.Ascending))
                    direction = ListSortDirection.Descending;
                else
                    direction = ListSortDirection.Ascending;

                // rely on the fact that the property to be sorted is the PropertyPath of the display member binding.
                string propertyName = ((Binding)headerClicked.Column.DisplayMemberBinding).Path.Path;
                ICollectionView view = CollectionViewSource.GetDefaultView(lv2.ItemsSource);

                using (view.DeferRefresh())
                {
                    view.SortDescriptions.Clear();
                    view.SortDescriptions.Add(new SortDescription(propertyName, direction));
                }

                m_LastHeaderClicked = headerClicked;
                m_LastDirection = direction;
            }
        }
    }
}
