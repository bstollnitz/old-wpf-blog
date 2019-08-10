using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;


namespace SelectedValueSample
{
    public partial class Window1 : Window
    {
        GreekGods items;
        public Window1()
        {
            InitializeComponent();
            items = new GreekGods();
            mainStackPanel.DataContext = items;
        }

        public void BtnClick(object sender, RoutedEventArgs args)
        {
            string messengerOfGods = (string)(listBox1.SelectedValue);
            GreekGod hermes = (GreekGod)(listBox1.SelectedItem);
        }
    }
}