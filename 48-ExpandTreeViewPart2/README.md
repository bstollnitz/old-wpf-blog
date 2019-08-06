# How to expand items in a TreeView – Part II

For those of you in Europe, I will be giving a talk at the Oredev conference in Malmo, Sweden, on Thursday the 20th of November. I hope I'll get to meet some of you there.
	
<img style="DISPLAY: block; MARGIN: 0px auto 10px; TEXT-ALIGN: center" alt="" src="Images/Oredev.jpg" border="0" />
	
In my <a href="..\47-ExpandTreeViewPart1">last post</a>, I showed how you can expand all items in a TreeView at load time. In WPF, this can be done by simply adding an implicit style to the resources, and in Silverlight we need a little help from ImplicitStyleManager to achieve the same behavior.

However, applications typically allow more complex interaction with a TreeView. In particular, they often permit users to expand all nodes, collapse all nodes, and expand the tree to reveal a particular node. I will show you one way to accomplish these tasks in this post, and a different way in my next post.

For the first approach, I will show you how to add an intermediate data layer to your application that adds UI-specific functionality on top of the data source. The idea of having an intermediate data layer isn't new, and is explained in great detail by <a href="http://blogs.msdn.com/johngossman/archive/2005/10/08/478683.aspx">John Gossman</a> in the context of WPF and <a href="http://www.nikhilk.net/Silverlight-ViewModel-Pattern.aspx">Nikhil Kothari</a> in the context of Silverlight.

## WPF

I used the same "Taxonomy" data source in this post that you may already be familiar with from my previous post or from the Silverlight Toolkit's sample pages. 

For this sample, I decided that I wanted to allow users to perform three operations on the TreeView: expand all items, collapse all items, and expand just enough items to  select a particular data item. To make these operations possible, I need a data source that has IsExpanded and IsSelected properties in each data item, so that I can data bind the corresponding properties of each TreeViewItem. Since my original data source does not contain these properties, I introduced an intermediate data source (sometimes called a view model) that has the properties I need.

There are a couple of ways to create an intermediate data source: you can either wrap the original data (containment) or you can derive from it (inheritance). Deriving may not be an option, if the data source is sealed or already has subclasses, so I chose the more general solution of containment for this sample. Here's what my intermediate data source looks like at this point:

	public abstract class TaxonomyViewModel : INotifyPropertyChanged
	{
		public Taxonomy Taxonomy { get; private set; }
	
		private bool isExpanded;
		public bool IsExpanded
		{
			get { return isExpanded; }
			set
			{
				isExpanded = value;
				OnPropertyChanged("IsExpanded");
			}
		}
	
		private bool isSelected;
		public bool IsSelected
		{
			get { return isSelected; }
			set
			{
				isSelected = value;
				OnPropertyChanged("IsSelected");
			}
		}
		...
	}

Although wrapping is more flexible, it requires forwarding of properties and methods to the actual data items. I exposed the Classification and Rank properties of Taxonomy in the TaxonomyViewModel class, but the Subclasses collection presented a problem: a TaxonomyViewModel has as its "Subclasses" other TaxonomyViewModels, but a Taxonomy has "Subclasses" of type Taxonomy. For this reason, I had to create a parallel collection of the right type, and I had to make sure that changes to the collection in TaxonomyViewModel are propagated to Taxonomy.

	private TaxonomyViewModelCollection subclasses;
	public Collection<TaxonomyViewModel> Subclasses { get { return subclasses; } }
	
	private class TaxonomyViewModelCollection : Collection<TaxonomyViewModel>
	{
		private Collection<Taxonomy> originalCollection;
	
		public TaxonomyViewModelCollection(Collection<Taxonomy> originalCollection)
		{
			this.originalCollection = originalCollection;
		}
	
		protected override void InsertItem(int index, TaxonomyViewModel item)
		{
			base.InsertItem(index, item);
			originalCollection.Insert(index, item.Taxonomy);
		}
		...
	}

My next step was to add a method that expands all nodes. This method needs to traverse the whole hierarchy of data and set IsExpanded to true on all nodes. Any tree traversal algorithm would work - I chose to use a Stack to do a non-recursive depth-first traversal.

	public void ExpandAll()
	{
		ApplyActionToAllItems(item => item.IsExpanded = true);
	}
	
	private void ApplyActionToAllItems(Action<TaxonomyViewModel> itemAction)
	{
		Stack<TaxonomyViewModel> dataItemStack = new Stack<TaxonomyViewModel>();
		dataItemStack.Push(this);
	
		while (dataItemStack.Count != 0)
		{
			TaxonomyViewModel currentItem = dataItemStack.Pop();
			itemAction(currentItem);
			foreach (TaxonomyViewModel childItem in currentItem.Subclasses)
			{
				dataItemStack.Push(childItem);
			}
		}
	}

Now that the code for traversing the tree is already in place, collapsing all TreeViewItems can be done in one line. The result of setting IsExpanded to false in all nodes will have effect in the same layout pass (since I don't return control to Silverlight during the tree traversal), so the order in which the IsExpanded properties are set does not matter. All items in the TreeView will collapse at the same time, in one layout pass. 

	public void CollapseAll()
	{
		ApplyActionToAllItems(item => item.IsExpanded = false);
	}

And finally, I added a method that, given a data item, expands all the items in its ancestor chain. This method uses recursion to search for the data item passed as a parameter. Once the item is found, its  ancestor chain gets expanded as the recursive call stack unwinds.

	public bool ExpandSuperclasses(TaxonomyViewModel itemToLookFor)
	{
		return ApplyActionToSuperclasses(itemToLookFor, superclass => superclass.IsExpanded = true);
	}

	private bool ApplyActionToSuperclasses(TaxonomyViewModel itemToLookFor, Action<TaxonomyViewModel> itemAction)
	{
		if (itemToLookFor == this)
		{
			return true;
		}
		else
		{
			foreach (TaxonomyViewModel subclass in this.Subclasses)
			{
				bool foundItem = subclass.ApplyActionToSuperclasses(itemToLookFor, itemAction);
				if (foundItem)
				{
					itemAction(this);
					return true;
				}
			}
			return false;
		}
	}

And that's all for the intermediate data source. Now we need to hook this up to the UI. I started by adding three buttons that call the methods I just wrote. Because a TreeView can actually contain multiple trees, my button event handlers iterate over all the root items, calling the appropriate method.

	<collections:ArrayList x:Key="treeOfLife">
		<local:DomainViewModel Classification="Bacteria">
			...
		</local:DomainViewModel>
		<local:DomainViewModel Classification="Archaea">
			...
		</local:DomainViewModel>
		<local:DomainViewModel Classification="Eukarya">
			...
		</local:DomainViewModel>
	</collections:ArrayList>
	
	private void ExpandAll(object sender, RoutedEventArgs e)
	{
		foreach (TaxonomyViewModel item in treeView.Items)
		{
			item.ExpandAll();
		}
	}
	
	private void SelectOne(object sender, RoutedEventArgs e)
	{
		ArrayList treeOfLifeCollection = (ArrayList)this.Resources["treeOfLife"];
		TaxonomyViewModel elementToExpand = (TaxonomyViewModel)((TaxonomyViewModel)treeOfLifeCollection[2]).Subclasses[3].Subclasses[0].Subclasses[0].Subclasses[0];
	
		foreach (TaxonomyViewModel item in treeView.Items)
		{
			if (item.ExpandSuperclasses(elementToExpand))
			{
				elementToExpand.IsSelected = true;
				break; 
			}
		}
	}

The CollapseAll scenario is a bit of a special case. There are really two options for collapsing all items:

- You can set IsExpanded to false on every TreeViewItem. The CollapseAll method I showed earlier can be used in this case. If you pick this option, any previous item expansion is forgotten once you collapse all items. This means that if you expand a few items, collapse all, and expand one top level item, the previous item expansion will not be restored.

	private void CollapseAll(object sender, RoutedEventArgs e)
	{
		foreach (TaxonomyViewModel item in treeView.Items)
		{
			item.CollapseAll();
		}
	}

- The other option is to collapse only the top level items. If you pick this option, previous expansions will be remembered and restored after collapsing all items. 

	private void CollapseTopLevel(object sender, RoutedEventArgs e)
	{
		foreach (TaxonomyViewModel item in treeView.Items)
		{
			TreeViewItem tvi = treeView.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
			tvi.IsExpanded = false;
		}
	}

And last, I need to bind the TreeViewItem's IsExpanded and IsSelected properties to the corresponding properties in my intermediate data source. At first sight, it may seem that the following XAML would work well:

	<Style TargetType="TreeViewItem">
		<Setter Property="IsExpanded" Value="{Binding Path=IsExpanded}" />
		<Setter Property="IsSelected" Value="{Binding Path=IsSelected}" />
	</Style>

If you try this, however, you will notice that once you collapse a TreeViewItem manually by clicking on it,  the Expand All button will no longer affect that item. That's because when interacting directly with the UI, the IsExpanded property is set explicit, overwriting the binding. The solution is to make the Bindings two-way. As you would expect, two-way Bindings are not lost when the target value is set, they simply propagate the value back to the source.

	<Style TargetType="TreeViewItem">
		<Setter Property="IsExpanded" Value="{Binding Path=IsExpanded, Mode=TwoWay}" />
		<Setter Property="IsSelected" Value="{Binding Path=IsSelected, Mode=TwoWay}" />
	</Style>

## Silverlight

Most of the code and XAML I showed for WPF works in Silverlight too, with the exception of the Binding in the Setter's Value, since Silverlight currently doesn't support that feature. In order to work around this limitation, I created custom TreeView and TreeViewItem classes that derive from the Toolkit classes and override GetContainerForItemOverride. This method is called to create each TreeViewItem container, so I was able to include the Bindings through code at the moment these containers are created.

	public class MyTreeView : TreeView
	{
		protected override DependencyObject GetContainerForItemOverride()
		{
			MyTreeViewItem tvi = new MyTreeViewItem();
			Binding expandedBinding = new Binding("IsExpanded");
			expandedBinding.Mode = BindingMode.TwoWay;
			tvi.SetBinding(MyTreeViewItem.IsExpandedProperty, expandedBinding);
			Binding selectedBinding = new Binding("IsSelected");
			selectedBinding.Mode = BindingMode.TwoWay;
			tvi.SetBinding(MyTreeViewItem.IsSelectedProperty, selectedBinding);
			return tvi;
		}
	}

Also, unfortunately we have a bug in the Toolkit TreeView  that occasionally causes more than one item to appear selected. Hopefully we'll get that fixed for the next release.

And that's all for today. In my next post, I will discuss a third way of expanding, collapsing  and selecting TreeViewItems.
