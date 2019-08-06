# How to expand items in a TreeView – Part III

This is the third of a three-part series about expanding TreeViewItems. In the <a href="..\47-ExpandTreeViewPart1">first post</a> I explained how to use an implicit Style to expand all TreeViewItems at load time. In the <a href="..\48-ExpandTreeViewPart2">second post</a> I showed how you can drive expansion and selection of items using an intermediate data source. In this post, I will explain how you can expand and select TreeViewItems using the dispatcher.

Expanding all TreeViewItems by setting the IsExpanded property on the items directly is not as simple as doing a tree walk and marking this property as you go. The problem is that after expanding a TreeViewItem, you need to return control to WPF or Silverlight so that the children TreeViewItems can be instantiated, before it's their turn to be expanded. Fortunately, the Dispatcher can be used on both of these technologies to ensure the instantiation of the TreeViewItems.

## WPF

Those of you who have experience with previous Microsoft technologies may have used the "DoEvents" method in the past, which performs a non-blocking wait. WPF doesn't ship an equivalent method, but it's easy to implement similar behavior using the Dispatcher. I like using a version of this method that takes a DispatcherPriority, so that I have more control over when to resume execution of my code. You can take a look at the code I use below:

	internal static void WaitForPriority(DispatcherPriority priority)
	{
		DispatcherFrame frame = new DispatcherFrame();
		DispatcherOperation dispatcherOperation = Dispatcher.CurrentDispatcher.BeginInvoke(priority, new DispatcherOperationCallback(ExitFrameOperation), frame);
		Dispatcher.PushFrame(frame);
		if (dispatcherOperation.Status != DispatcherOperationStatus.Completed)
		{
			dispatcherOperation.Abort();
		}
	}
	
	private static object ExitFrameOperation(object obj)
	{
		((DispatcherFrame)obj).Continue = false;
		return null;
	}

In the code above, you can see that I create a new DispatcherFrame and only exit from that frame once dispatcher operations of the specified priority are reached. This way, I will give WPF a chance to execute anything with a priority higher than the one passed as a parameter before continuing executing the next line of code. For example, in the following code I ensure that all jobs with priority higher than Background have been executed by the time I call MyMethod2.

	MyMethod1();
	WaitForPriority(DispatcherPriority.Background);
	MyMethod2();

If there is other work for the dispatcher to do at the priority specified, it is not guaranteed that that work will happen before starting execution of MyMethod2. Since the "ExitFrameOperation" method is BeginInvoked at Background priority in this case, it is possible that other tasks at the same priority will be executed after this one. For this reason, if you want to make sure all operations at Background priority have been executed, you should pass priority ContextIdle instead (the next priority level). 

Now that you understand this very useful WaitForPriority method, we can look at how we can use it to expand all items in a TreeView. Expanding the first level of TreeViewItems is easy, but in order to expand the second level of items, we need to make sure that the TreeViewitems are fully instantiated. This can only be achieved by returning control back to WPF just long enough for those items to be instantiated, and then continue execution. This is the perfect job for WaitForPriority.

In the code below, I do a full non-recursive depth-first tree traversal, and for each item I encounter, I set IsExpanded to true and wait in a non-blocking way for the child TreeViewItems to be instantiated. 

	private void ExpandAll(object sender, RoutedEventArgs e)
	{
		ApplyActionToAllTreeViewItems(itemsControl =>
		{
			itemsControl.IsExpanded = true;
			DispatcherHelper.WaitForPriority(DispatcherPriority.ContextIdle);
		}, 
		treeView);
	}
	
	private void ApplyActionToAllTreeViewItems(Action<TreeViewItem> itemAction, ItemsControl itemsControl)
	{
		Stack<ItemsControl> itemsControlStack = new Stack<ItemsControl>();
		itemsControlStack.Push(itemsControl);
	
		while (itemsControlStack.Count != 0)
		{
			ItemsControl currentItem = itemsControlStack.Pop() as ItemsControl;
			TreeViewItem currentTreeViewItem = currentItem as TreeViewItem;
			if (currentTreeViewItem != null)
			{
				itemAction(currentTreeViewItem);
			}
			if (currentItem != null) // this handles the scenario where some TreeViewItems are already collapsed
			{
				foreach (object dataItem in currentItem.Items)
				{
					ItemsControl childElement = (ItemsControl)currentItem.ItemContainerGenerator.ContainerFromItem(dataItem);
					itemsControlStack.Push(childElement);
				}
			}
		}
	}

My explanation about collapsing TreeViewItems in the previous post is applicable to this scenario too. There are basically two ways you can collapse the item: you can either collapse just the top level items (in which case expanding them again will keep the previous expansion state) or you can collapse every single item in the tree (in which case the previous expansion state is lost).

	private void CollapseTopLevel(object sender, RoutedEventArgs e)
	{
		foreach (Taxonomy item in treeView.Items)
		{
			TreeViewItem tvi = treeView.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
			tvi.IsExpanded = false;
		}
	}
	
	private void CollapseAll(object sender, RoutedEventArgs e)
	{
		ApplyActionToAllTreeViewItems(itemsControl => itemsControl.IsExpanded = false, treeView);
	}

Expanding and selecting one tree node is a bit more complex using the dispatcher than it was using the intermediate data source. In the previous post, I was able to use recursion to look for the item, and as I returned from each level of the recursion, I expanded the item. So I started by expanding the bottom level node, and worked my way to the top. When using the intermediate data source solution this order didn't matter because all items were updated in the same layout pass. 

However, when interacting with the TreeViewItems directly, I always need to start expanding the nodes from the top and wait for the next level of nodes to be instantiated before proceeding. So the simple algorithm from the previous post won't help me here. The solution is to do this in two parts: first I navigate the whole tree using recursion, find the item to expand, and as I return from each level of recursion, I create a collection with the parent hierarchy. Once I have that information, I can now start from the top of the tree and expand each TreeViewItem that corresponds to a data item in the collection. 

	private void SelectOne(object sender, RoutedEventArgs e)
	{
		ArrayList treeOfLifeCollection = (ArrayList)this.Resources["treeOfLife"];
		Taxonomy elementToExpand = ((Taxonomy)treeOfLifeCollection[2]).Subclasses[3].Subclasses[0].Subclasses[0].Subclasses[0];
	
		foreach (Taxonomy firstLevelDataItem in treeView.Items)
		{
			Collection<Taxonomy> superclasses = GetSuperclasses(firstLevelDataItem, elementToExpand);
			if (superclasses != null)
			{
				// Expand superclasses
				TreeViewItem parentTreeViewItem = (TreeViewItem)treeView.ItemContainerGenerator.ContainerFromItem(firstLevelDataItem);
				parentTreeViewItem.IsExpanded = true;
				DispatcherHelper.WaitForPriority(DispatcherPriority.Background);
	
				foreach (Taxonomy superclassToExpand in superclasses.Skip(1))
				{
					TreeViewItem treeViewItemToExpand = (TreeViewItem)parentTreeViewItem.ItemContainerGenerator.ContainerFromItem(superclassToExpand);
					treeViewItemToExpand.IsExpanded = true;
					DispatcherHelper.WaitForPriority(DispatcherPriority.Background);
					parentTreeViewItem = treeViewItemToExpand;
				}
	
				// Select node
				TreeViewItem treeViewItemToSelect = (TreeViewItem)parentTreeViewItem.ItemContainerGenerator.ContainerFromItem(elementToExpand);
				treeViewItemToSelect.IsSelected = true;
			}
		}
	}
	
	private Collection<Taxonomy> GetSuperclasses(Taxonomy currentItem, Taxonomy itemToLookFor)
	{
		if (itemToLookFor == currentItem)
		{
			Collection<Taxonomy> results = new Collection<Taxonomy>();
			return results;
		}
		else
		{
			foreach (Taxonomy subclass in currentItem.Subclasses)
			{
				Collection<Taxonomy> results = GetSuperclasses(subclass, itemToLookFor);
				if (results != null)
				{
					results.Insert(0, currentItem);
					return results;
				}
			}
			return null;
		}
	}

If you were able to change your data source, a simpler alternative to this algorithm would be to add parent pointers to each data item. If you had parent pointers, instead of using recursion in the GetSuperclasses method, you could find the parent hierarchy with a simple for loop. If you were not able to change the data source but your data source used an ObservableCollection&lt;T&gt; to store the children of each data item, yet another option would be to add an intermediate data source that listens to collection changes in the original data, and adds parent pointers to the intermediate data when items are added. However, I didn't want to provide a solution that assumes you can change your data source, because often you can't. And I didn't want to assume your source uses ObservableCollection&lt;T&gt; because often it doesn't. I am certain that if you have the luxury of parent pointers or collection change notifications, you will be able to write the corresponding code easily.

I'd provide a link to the running xbap here, but that's not possible for this example because DispatcherFrame can't be used in the partial-trust security model of an xbap.  Instead, you'll have to build the WPF example from the source code provided at the end of this post.

## Silverlight

The Silverlight version of the Dispatcher solution is quite a bit different from the WPF one. Silverlight does not have DispatcherFrame or DispatcherPriority, so there is no way to write a helper method similar to DoEvents. Fortunately, the Silverlight Dispatcher has a BeginInvoke method that I can use to return control to Silverlight, and allow it to instantiate the next level of TreeViewItems before continuing. Take a look at the code below. By calling MyMethod2 asynchronously, I am ensuring that control is returned to Silverlight before MyMethod2 is executed. This is the technique I will use to allow Silverlight to instantiate the next level of TreeViewItems before I can expand them.

	MyMethod1();
	myElement.Dispatcher.BeginInvoke(MyMethod2);

You can see this technique being using to expand all TreeViewItems:

	private void ExpandAll(object sender, RoutedEventArgs e)
	{
		for (int i = 0; i < treeView.Items.Count; i++)
		{
			ExpandAllTreeViewItems((TreeViewItem)treeView.ItemContainerGenerator.ContainerFromIndex(i));
		}
	}
	
	private void ExpandAllTreeViewItems(TreeViewItem currentTreeViewItem)
	{
		if (!currentTreeViewItem.IsExpanded)
		{
			currentTreeViewItem.IsExpanded = true;
			currentTreeViewItem.Dispatcher.BeginInvoke(() => ExpandAllTreeViewItems(currentTreeViewItem));
		}
		else
		{
			for (int i = 0; i < currentTreeViewItem.Items.Count; i++)
			{
				TreeViewItem child = (TreeViewItem)currentTreeViewItem.ItemContainerGenerator.ContainerFromIndex(i);
				ExpandAllTreeViewItems(child);
			}
		}
	}

Similarly to the previous solutions, I show both how you can collapse all items or just the top level. Since we don't have to wait for TreeViewItems to be instantiated when collapsing all items, it is not necessary to use BeginInvoke. Any tree walking algorithm would work.

	private void CollapseAll(object sender, RoutedEventArgs e)
	{
		for (int i = 0; i < treeView.Items.Count; i++)
		{
			CollapseAllTreeViewItems((TreeViewItem)treeView.ItemContainerGenerator.ContainerFromIndex(i));
		}
	}
	
	private void CollapseAllTreeViewItems(TreeViewItem rootTreeViewItem)
	{
		Stack<TreeViewItem> treeViewItemsStack = new Stack<TreeViewItem>();
		treeViewItemsStack.Push(rootTreeViewItem);
		while (treeViewItemsStack.Count != 0)
		{
			TreeViewItem current = treeViewItemsStack.Pop();
			current.IsExpanded = false;
	
			for (int i = 0; i < current.Items.Count; i++)
			{
				treeViewItemsStack.Push(current.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem);
			}
		}
	}
	
	private void CollapseTopLevel(object sender, RoutedEventArgs e)
	{
		// This iterates through the three top-level items only.
		foreach (Taxonomy item in treeView.Items)
		{
			TreeViewItem tvi = treeView.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
			tvi.IsExpanded = false;
		}
	}

The code for expanding and selecting one item is also quite a bit different, and a bit more complex. In WPF, once we had all superclasses, we could navigate down the TreeView hierarchy with a for loop, as long as we remembered to return control to WPF after expanding each TreeViewItem. In Silverlight, I had to introduce a new ExpandPathAndSelectLast method that calls itself using BeginInvoke, giving an opportunity for Silverlight to create the next level of TreeViewItems between method calls. I don't show the GetSuperclasses method again here, since it's the same as the WPF version.

	private void SelectOne(object sender, RoutedEventArgs e)
	{
		ObjectCollection treeOfLifeCollection = (ObjectCollection)this.Resources["treeOfLife"];
		Taxonomy elementToExpand = ((Taxonomy)treeOfLifeCollection[2]).Subclasses[3].Subclasses[0].Subclasses[0].Subclasses[0];
	
		foreach (Taxonomy firstLevelDataItem in treeView.Items)
		{
			Collection<Taxonomy> superclasses = GetSuperclasses(firstLevelDataItem, elementToExpand);
			if (superclasses != null)
			{
				TreeViewItem parentTreeViewItem = (TreeViewItem)treeView.ItemContainerGenerator.ContainerFromItem(firstLevelDataItem);
				ExpandPathAndSelectLast(parentTreeViewItem, superclasses.Skip(1).GetEnumerator(), elementToExpand);
			}
		}
	}
	
	private void ExpandPathAndSelectLast(TreeViewItem currentTreeViewItem, IEnumerator enumerator, object itemToSelect)
	{
		if (!currentTreeViewItem.IsExpanded)
		{
			currentTreeViewItem.IsExpanded = true;
			currentTreeViewItem.Dispatcher.BeginInvoke(() => ExpandPathAndSelectLast(currentTreeViewItem, enumerator, itemToSelect));
		}
		else if (enumerator.MoveNext())
		{
			object dataItem = enumerator.Current;
			TreeViewItem nextContainer = (TreeViewItem)currentTreeViewItem.ItemContainerGenerator.ContainerFromItem(dataItem);
			ExpandPathAndSelectLast(nextContainer, enumerator, itemToSelect);
		}
		else
		{
			TreeViewItem treeViewItemToSelect = (TreeViewItem)currentTreeViewItem.ItemContainerGenerator.ContainerFromItem(itemToSelect);
			treeViewItemToSelect.IsSelected = true;
		}
	}

This is all the code you need to expand and collapse TreeViewItems.

## Which TreeView expansion solution should I use?

Now that you know of three ways to expand, collapse and select items in a TreeView, you're faced with the decision of which one you should use in your project. Below I show a quick bullet-point list of the pros and cons of each solution. Hopefully this will help you make the right decision.

### Solution 1 - Expand all TreeViewItems using an implicit style

Pros:
- Really really simple to implement.

Cons:
- It is only useful to expand all TreeViewItems at app startup. 

### Solution 2 - Expand, collapse and select TreeViewItems using an intermediate data source

Pros:
- This is the fastest way to expand items, since they all get expanded in one layout pass. If you are binding your TreeView to a large amount of data, this may be your best option.

Cons:
- If your data source is complex, it may be very hard to write an intermediate data source.

### Solution 3 - Expand, collapse and select TreeViewItems using the Dispatcher

Pros:
- It's independent of the data source, so you can reuse the code in TreeViews bound to different data sources (in fact, you can even add these as extension methods on TreeView).
- If you don't have a lot of data, the visual effect of expanding one hierarchy level per layout pass can be fun.

Cons:
- If your data has a deep hierarchy, it will be very slow to expand the items. It will take as many layout passes as levels in the hierarchy.
