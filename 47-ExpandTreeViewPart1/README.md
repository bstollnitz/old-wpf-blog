# How to expand items in a TreeView – Part I

Exciting!

Yesterday, the new Silverlight Toolkit (the team I now work for) was made available in CodePlex. This was announced by <a href="http://weblogs.asp.net/scottgu/">Scott Guthrie</a> in his <a href="http://www.microsoftpdc.com/">PDC</a> keynote and by my manager <a href="http://blogs.msdn.com/sburke/">Shawn Burke in his blog</a>.

We created the Silverlight Toolkit to provide developers with a set of reusable components that  maximize productivity and help take Silverlight to the next level. The toolkit is available under the <a href="http://www.microsoft.com/opensource/licenses.mspx#Ms-PL">Microsoft Public License</a>, so we provide the full source code (as well as tests and samples), and encourage you to reuse our code in your applications.

If you want to know more about it, I recommend that you check out our <a href="http://www.codeplex.com/Silverlight">web site</a>, <a href="http://www.codeplex.com/Silverlight/Release/ProjectReleases.aspx?ReleaseId=18804">download the toolkit</a>, and post questions in the <a href="http://silverlight.net/forums/35.aspx">Silverlight controls forum</a>.  My team just went through a resource-constrained, short, crazy release, where everyone on the team went beyond at so many levels, so it feels amazing to get these bits out to customers. 


## Expanding items in a TreeView

Among many other controls, we shipped a Silverlight TreeView, nearly identical to the WPF one. This is the first of three blog posts in which I am going to discuss different ways of expanding the items in a TreeView. These blog posts will cover both WPF and Silverlight, and point out the differences between the two. I'm hoping this will be especially useful to those of you who are familiar with one technology and interested in learning the other.

I have encountered customers in the past who had implemented complex solutions to expand all items of a TreeView at load time (which is a pretty common scenario). This scenario is actually really easy to accomplish - you can simply add an implicit Style for TreeViewItem that sets IsExpanded to true. 

## WPF

In WPF, this can be done by adding an implicit style to the resources of your page:

	<Page.Resources>
		<collections:ArrayList x:Key="treeOfLife">
			<local:Domain Classification="Bacteria">
				<local:Kingdom Classification="Eubacteria" />
			</local:Domain>
			...
		</collections:ArrayList>
		...
		<Style TargetType="TreeViewItem">
			<Setter Property="IsExpanded" Value="True" />
		</Style>
	</Page.Resources>

        <TreeView ItemsSource="{StaticResource treeOfLife}" ItemTemplate="{StaticResource treeOfLifeTemplate}"/>

Couldn't be any easier. 

## Silverlight

Now let's look at the differences between the WPF solution and the corresponding Silverlight one.

You may have noticed that I used an ArrayList to store my data source in the WPF sample.  Usually I would prefer to use a strongly typed generic collection, but for this example I chose ArrayList because the WPF XAML parser does not yet support generic types. In Silverlight, the ArrayList type is not available for customer use (it's internal), so I used an ObjectCollection instead. ObjectCollection is a new collection type we ship in the Silverlight toolkit exactly for the purpose of defining collections in XAML. Here is its source code:

	public partial class ObjectCollection : Collection<object>
	{
		public ObjectCollection()
		{
		}
	
		public ObjectCollection(IEnumerable collection)
		{
			foreach (object obj in collection)
			{
				Add(obj);
			}
		}
	}

In this case I don't need collection change notifications, so deriving from Collection&lt;object&gt; works well for me, without adding the overhead of ObservableCollection&lt;T&gt;. If you need collection change notifications, you can use the same technique and simply replace the base class with ObservableCollection&lt;T&gt;.

The Style for the TreeViewItem looks really similar to the WPF one. However, if you try this in Silverlight, you will notice that the style doesn't get applied. 

	<UserControl.Resources>
		...
		<Style TargetType="controls:TreeViewItem">
			<Setter Property="IsExpanded" Value="True" />
		</Style>
	</UserControl.Resources>

This is because Silverlight doesn't have support for implicit styles. When using the Silverlight runtime, you need to give each style a key and refer to it explicitly using StaticResource. Because the TreeViewItems are generated automatically by the TreeView, it would be pretty hard to specify an explicit style for each one.

Fortunately, the Silverlight Toolkit contains a solution to this problem that provides a behavior similar to WPF's implicit styles. You can use the "ImplicitStyleManager" class, and in particular its ApplyMode attached property:

	<controls:TreeView ItemsSource="{StaticResource treeOfLife}" ItemTemplate="{StaticResource treeOfLifeTemplate}" theming:ImplicitStyleManager.ApplyMode="Auto"/>

And now the implicit style is applied and the TreeView expands all items on load! 

## ImplicitStyleManager

ImplicitStyleManager only affects elements in the subtree rooted at the element where ApplyMode is set. In the sample above, implicit styles are only applied to the TreeView  itself and its descendants in the visual tree (TreeViewItems, etc.). However, when searching for styles to use, ImplicitStyleManager looks within resource dictionaries in the entire name scope of the element where ApplyMode is set, as well as in the Application's resources (just like in WPF). In this sample, it looks for implicit styles anywhere within the current UserControl (each UserControl defines a name scope), and in the Application's resources.

You've already seen how to use ImplicitStyleManager in a simple scenario above, but this class provides some additional functionality. ISM (as we lovingly call it) has three different modes, which can be specified through its ApplyMode property:

### Auto

ISM reapplies implicit styles every time layout is updated. So, if elements are added later to the visual tree (like the TreeViewItems in this sample), they will be implicitly styled. 

Keep in mind that the LayoutUpdated event fires quite frequently (not just when elements are added to the visual tree), and walking the whole visual tree may be an expensive operation if you have a large tree. Unfortunately there is no "ItemAddedToTree" event that we could listen to, so we had to compromise on performance  to offer the convenience of this mode.

In this post's sample, the visual tree is small and very dynamic, so it makes sense to use this mode. But if your tree is relatively static or very large, you should consider using the OneTime mode instead.

### OneTime

In this mode, ISM applies the implicit styles to the elements in the visual tree at load time. If new elements get added later, they will not be styled. In this blog's sample, the generated TreeViewItems are not all instantiated at load time, so using ApplyMode=OneTime wouldn't work.

Sometimes you may have a scenario where new items are added to the tree occasionally, but your tree is large enough that you would rather not use the Auto mode. For these situations, you can set ApplyMode to OneTime, and call ISM's "Apply" method through code every time you want to reapply the styles. This provides extra flexibility while avoiding the performance cost of the Auto mode.

### None

This mode is exactly the same as not attaching the ApplyMode property. It does not prevent styles from being propagated to the subtree where this property is set, as some people may expect. Since WPF doesn't permit you to disable implicit styling within a subtree, ImplicitStyleManager doesn't either.

There is more to ImplicitStyleManager than what I've explained here, so stay tuned for future posts.
