using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;


namespace DataBoundDialogBox
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

        private void ShowDialog(object sender, RoutedEventArgs args)
        {
            Dialog1 dialog = new Dialog1();
            dialog.Owner = this;
            dialog.ShowDialog();
        }
    }
}