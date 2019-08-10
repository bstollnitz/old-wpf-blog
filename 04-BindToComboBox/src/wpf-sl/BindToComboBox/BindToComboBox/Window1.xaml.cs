using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;


namespace BindToComboBox
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            GreekGod greekGod = (GreekGod)(comboBox.Items[0]);
            comboBox.IsDropDownOpen = true;
            ComboBoxItem cbi1 = (ComboBoxItem)(comboBox.ItemContainerGenerator.ContainerFromIndex(0));
            ComboBoxItem cbi2 = (ComboBoxItem)(comboBox.ItemContainerGenerator.ContainerFromItem(comboBox.Items.CurrentItem));
            comboBox.IsDropDownOpen = false;
        }
    }
}