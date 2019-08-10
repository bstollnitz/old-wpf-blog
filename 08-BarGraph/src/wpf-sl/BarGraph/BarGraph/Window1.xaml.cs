using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BarGraph
{
    public partial class Window1 : Window
    {
        private Random random;

        public Window1()
        {
            InitializeComponent();
            random = new Random();
        }

        private void ChangeData(object sender, RoutedEventArgs e) 
        {
            DataSource source = (DataSource)this.Resources["source"];
            for (int i = 0; i < source.ValueCollection.Count; i++)
            {
                source.ValueCollection[i] = random.Next(10,130);
            }
        }
    }
}