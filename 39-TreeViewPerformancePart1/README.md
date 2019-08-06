# How to improve TreeView's performance – Part I

*Update: This post is partially out of date. With .NET 3.5 SP1, TreeView now provides opt-in UI virtualization. You can see this feature working by setting VirtualizingStackPanel.IsVirtualizing="True" on the TreeView itself. TreeView also supports container recycling, which you can control by setting the VirtualizingStackPanel.VirtualizationMode property. With .NET 3.5 SP1, there is still no support for data virtualization, so the data virtualization portions of this blog post series still apply.


This is the first of three blog posts where I will discuss TreeView performance. This post will cover the problems with our current implementation of TreeView; the next post will show a solution that will enable binding to a large data set; and the third post will discuss a simple idea that adds UI virtualization to a hierarchical data representation. 

There are currently three aspects of TreeView's implementation that affect performance when binding to large data sets:

- UI elements stay in memory after collapsed.
- There is no UI virtualization.
- There is no data virtualization.

I will discuss each of these problems after describing a typical scenario.

Suppose I want to display my registry keys in a TreeView. To do so, I first need to create a hierarchical data structure that is able to hold the registry key data in memory. I defined a "RegistryKeyHolder1" class containing a property called "Key" that holds the actual RegistryKey, and a property called "SubKeys" of type ObservableCollection. This class also has a "PopulateSubKeys" method that populates the SubKeys property:

	public class RegistryKeyHolder1
	{
		...
		
		public void PopulateSubKeys()
		{
			try
			{
				string[] subKeyNames = this.key.GetSubKeyNames();
				for (int i = 0; i < subKeyNames.Length; i++)
				{
					this.subKeys.Add(new RegistryKeyHolder1(this.key.OpenSubKey(subKeyNames[i])));
				}
			}
			catch (SecurityException se)
			{
				System.Console.WriteLine(se.Message);
			}
		}
	}

I also defined a "RegistryData1" class that contains a "RootKeys" property, and a "PopulateSubKeys" method. To populate the entire data structure, I start with a collection of RootKeys and use the following recursive method:

	public class RegistryData1
	{
		...
	
		private void PopulateSubKeys(ObservableCollection<RegistryKeyHolder1> keys)
		{
			foreach (RegistryKeyHolder1 keyHolder in keys)
			{
				keyHolder.PopulateSubKeys();
				this.dataItemsCount += keyHolder.SubKeys.Count;
				// It will take forever if I get all registry keys
				if (this.dataItemsCount >= 5000)
				{
					return;
				}
				PopulateSubKeys(keyHolder.SubKeys);
			}
		}
	}

With this data structure in place, I am now able to bind the TreeView to the RootKeys property directly.

	public Window1()
	{
		InitializeComponent();
		this.grid1.DataContext = new RegistryData1();
		...
	}
	
	<TreeView ItemsSource="{Binding Path=RootKeys}" Name="treeView1" ... />
		<TreeView.Resources>
			<HierarchicalDataTemplate DataType="{x:Type local:RegistryKeyHolder1}" ItemsSource="{Binding Path=SubKeys}">
				<TextBlock Text="{Binding Path=ShortName}" />
			</HierarchicalDataTemplate>
		</TreeView.Resources>
	</TreeView>

This data structure makes it really easy to provide a default look for all levels of the hierarchy by using a HierarchicalDataTemplate. Notice the TextBlock in the template, bound to the ShortName property. Since the Name property of RegistryKey returns the whole path of that key, I added a ShortName property to RegistryKeyHolder1 that returns only the interesting part of the key. I use the ItemsSource property of the HierarchicalDataTemplate to specify the items that should be displayed in the next level of the hierarchy - in this case, the subkeys.

## UI elements stay in memory after collapsed

If you run this post's project, you will see that the initial number of visuals of this TreeView is 49. You can use <a href="http://www.blois.us/Snoop/">Snoop</a> to see exactly which elements are in the visual tree (although Snoop includes GridColumns in its count and I don't, so you may see some differences in the results). Now expand the first node and collapse it again. This time the number of visuals is 169, even after you collapsed the node. In our current implementation of TreeView, we let the children items stay around in memory, even after they are collapsed. If your TreeView has a usage pattern where the same few nodes are often expanded and collapsed, then you will get better perf with our current design because the children items don't have to be garbage collected and re-created every time. However, we realize that in most situations, customers would rather have their items be garbage collected when a node is collapsed. So, overall, I consider this a limitation of our current design of TreeView.

![](Images/39TreeViewPerformance11.png)

## There is no UI virtualization

There are two WPF controls that support UI virtualization currently: ListBox and ListView. This happens because their default panel is a VirtualizingStackPanel, which behaves like a StackPanel but provides UI virtualization. 

By UI virtualization, I mean that when you bind a ListBox to a large collection of data, we only create UI containers for the items that are visually displayed (plus a few more before and after, to improve the speed of scrolling). When you scroll, we create new containers for the items that are newly visible, and dispose of the ones that are no longer visible. This greatly improves the performance of binding a ListBox to a large collection.

What about the other Controls in WPF? ComboBox has StackPanel as its default panel, but a user can easily change that to be a VirtualizingStackPanel. TreeView, however, can not use VirtualizingStackPanel to display its items because this panel doesn't know how to display a hierarchy. The same applies to a ListBox with grouping enabled - a ListBox without grouping has UI virtualization by default, but once you group the data, the UI virtualization is lost.

If you run this post's code sample, you can easily tell that TreeView does not support UI virtualization. If you expand several nodes so that the scroll bar on the right is enabled, you will see that the number of visuals keeps increasing. Even though many of the items that you expanded are outside of the visible portion of the scroll viewer, WPF still creates all of the visuals necessary to display them.

![](Images/39TreeViewPerformance12.png)

## There is no data virtualization

None of the WPF controls supports data virtualization at the moment. Providing a generic data virtualization solution is a hard problem that everyone on the team is eager to solve, but it hasn't been our highest priority.

If we had data virtualization for ListBox, that would mean that only the data items displayed are kept in memory. As the user scrolls through the ListBox, new items are brought into memory, and the old ones are discarded. For TreeView, in addition to swapping items in and out of memory due to scrolling, we would also want to load and unload items from memory when their parent item is expanded and collapsed.

So, what is the difference between UI and data virtualization? With UI virtualization we keep in memory only the UI elements (e.g. ListBoxItems) that are displayed visually, but we may still have the whole data structure in memory. Data virtualization goes one step further: we only keep in memory the data items that are being displayed on the screen.

By running this post's code sample, you can easily tell that there is no data virtualization. You will see, even right after you run it the first time, that the number of data items in memory is over 5000. Since initially only two items are displayed, it's easy to see that we're keeping in memory many more items than the ones we need to build up that UI.

![](Images/39TreeViewPerformance13.png)

That's all for today's post. I've explained some of the current limitations of TreeView, and in the next two posts I will provide some solutions to these problems. So stay tuned.
