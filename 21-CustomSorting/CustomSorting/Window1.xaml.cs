using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections;


namespace CustomSorting
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

		private void Sort(object sender, RoutedEventArgs args)
		{
			BlogPosts posts = (BlogPosts)(this.Resources["posts"]);
			ListCollectionView lcv = (ListCollectionView)(CollectionViewSource.GetDefaultView(posts));
			lcv.CustomSort = new SortPosts();
		}
	}

	public class SortPosts : IComparer
	{
		public int Compare(object x, object y)
		{
			string str1 = x.ToString();
			string str2 = y.ToString();

			int int1 = GetNumberAtBeginning(str1);
			int int2 = GetNumberAtBeginning(str2);

			// strings start with same number or don't start with numbers
			if (int1 == int2)
			{
				return str1.CompareTo(str2); // compare the strings
			}
			// strings start with different numbers
			else if ((int1 != -1) && (int2 != -1))
			{
				return int1.CompareTo(int2); // compare the numbers
			}
			// first string does not start with number
			else if (int1 == -1)
			{
				return 1; // second string goes first
			}
			// second string does not start with number
			else 
			{
				return -1; // first string goes first
			}
		}

		private int GetNumberAtBeginning(string str)
		{
			// see if the string begins with a number
			if (string.IsNullOrEmpty(str) || !char.IsDigit(str[0]))
			{
				return -1;
			}

			// parse the number at the beginning of the string
			int result = 0;
			foreach (char c in str)
			{
				if (char.IsDigit(c))
				{
					result = result * 10 + c - '0';
				}
				else
				{
					break;
				}
			}
			return result;
		}
	}

	public class BlogPosts : ObservableCollection<string>
	{
		public BlogPosts()
		{
			this.Add("14SortingGroups");
			this.Add("21CustomSorting");
			this.Add("2EmptyBinding");
			this.Add("3GetListBoxItem");
			this.Add("12DataBoundDialogBox");
			this.Add("4BindToComboBox");
			this.Add("16GroupByType");
			this.Add("9CollectionViewSourceSample");
			this.Add("19ObjectDataProviderSample");
			this.Add("20InsertingSeparators");
			this.Add("5DisplayMemberPath");
			this.Add("7ChangePanelItemsControl");
			this.Add("8BarGraph");
			this.Add("10MasterDetail");
			this.Add("1DataContextSample");
			this.Add("6SelectedValue");
			this.Add("13TemplatingItems");
			this.Add("18ThreeLevelMasterDetailADO");
			this.Add("15GroupingTreeView");
			this.Add("11MasterDetailThreeLevels");
			this.Add("17BoundListView");
		}
	}
}