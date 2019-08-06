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

namespace DataVirtualization
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private DemoCustomerProvider customerProvider;
        private DispatcherTimer timer;

        public Window1()
        {
            InitializeComponent();
        }

        private void GetDataHandler(object sender, RoutedEventArgs e)
        {
            customerProvider = new DemoCustomerProvider(1000 /*number of items*/, 2000 /*delay*/);
            AsyncVirtualizingCollection<Customer> customerList = new AsyncVirtualizingCollection<Customer>(customerProvider, 30 /*page size*/, 3000 /*timeout*/);
            this.DataContext = customerList;
        }
    }
}
