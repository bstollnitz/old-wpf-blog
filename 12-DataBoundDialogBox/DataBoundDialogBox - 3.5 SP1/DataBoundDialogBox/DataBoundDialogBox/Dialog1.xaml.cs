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
using System.Windows.Shapes;

namespace DataBoundDialogBox
{
	/// <summary>
	/// Interaction logic for Dialog1.xaml
	/// </summary>
	public partial class Dialog1 : Window
	{
		public Dialog1()
		{
			InitializeComponent();
		}

		private void OKHandler(object sender, RoutedEventArgs args)
		{
			BindingExpression bindingExpressionName = BindingOperations.GetBindingExpression(Name, TextBox.TextProperty);
			bindingExpressionName.UpdateSource();
			BindingExpression bindingExpressionComment = BindingOperations.GetBindingExpression(Comment, TextBox.TextProperty);
			bindingExpressionComment.UpdateSource();
			this.DialogResult = true;
		}
	}
}
