# UI Virtualization

Today's post is motivated by a scenario that's common in business applications: displaying and interacting with a large data set. We'll quickly run into performance problems if we use the naïve approach of loading the entire data set into memory and creating UI elements for each data item. Fortunately, there are some things we can do to make sure our applications perform well, even with extremely large data sets.

The first approach is called "UI virtualization." A control that supports UI virtualization is smart enough to create only the UI elements needed to display the data items that are actually visible on screen. For example, suppose we have a scrolling ListBox that is data bound to 1,000,000 items but only 100 are visible at any time. Without UI virtualization, the ListBox would create a million ListBoxItems - a slow process - and include them in the UI, even though only a hundred of them are visible. With UI virtualization, on the other hand, the ListBox would only create 100 ListBoxItems (or a few more, to improve scrolling performance).

The second approach, called "data virtualization," goes one step further. A control that uses data virtualization doesn't load all the data items into memory. Instead, it only loads the ones it needs to display. In our ListBox example above, a solution using data virtualization would only keep about 100 data items in memory at any given time.

In this post, I will talk about the current level of support for UI virtualization in Silverlight and WPF. In my next post, I will discuss data virtualization.

## UI virtualization in Silverlight

Silverlight 3 just shipped! I am very pleased to say that with this release, Silverlight's ListBox now supports UI virtualization! This feature was not part of the beta release - it's brand new in the final release. I have to admit it's my favorite new feature of Silverlight 3.

It was possible to work around the lack of UI virtualization before (in fact, I wrote a virtualized ListBox in Silverlight a year ago), but it wasn't straightforward. I'm very glad that this feature is now part of Silverlight 3!

If you're familiar with the UI virtualization provided by WPF's controls, you're probably curious about the level of support for virtualization in Silverlight. Just like WPF, Silverlight supports container recycling, but there is no support for deferred scrolling or for UI virtualization with hierarchical data. I will expand on these concepts below while discussing UI virtualization in WPF.

## UI virtualization in WPF

WPF has supported UI virtualization for a long time. The ListBox and ListView controls use VirtualizingStackPanel as their default panel, and VSP knows to create UI containers (ListBoxItems or ListViewItems) when new items are about to be shown in the UI, and to discard those containers when items are scrolled out of view. 

If you're using another ItemsControl (such as ComboBox) that doesn't use VirtualizingStackPanel by default, you can change the panel used by the control in a very simple way:

	<ComboBox ItemsSource="{Binding}">
		<ComboBox.ItemsPanel>
			<ItemsPanelTemplate>
				<VirtualizingStackPanel />
			</ItemsPanelTemplate>
		</ComboBox.ItemsPanel>
	</ComboBox>

The UI virtualization support in .NET 3.5 was already pretty solid, but the WPF team decided to further improve UI virtualization in .NET 3.5 SP1. With that release, the following new features were introduced:

### Container recycling

.NET 3.5 SP1 supports the reuse of UI containers already in memory. For example, imagine that when a ListBox is loaded, 30 ListBoxItems are created to display the visible data. When the user scrolls the ListBox, instead of discarding ListBoxItems that scroll out of view and creating new ones for the data items that scroll into view, WPF reuses the existing ListBoxItems. This results in significant performance improvements compared to previous versions because it decreases the time spent initializing ListBoxItems. And since garbage collection is not instantaneous, it also reduces the number of ListBoxItems in memory at one time. 

You can enable container recycling by setting the attached property "VirtualizingStackPanel.VirtualizationMode" to "Recycling"  on your control: 

	<ListBox VirtualizingStackPanel.VirtualizationMode="Recycling" ... />

To maintain backwards compatibility with the behavior of earlier versions, container recycling is disabled by default (the default VirtualizationMode is "Standard"). As a rule of thumb, I suggest setting this property every time you create a control that requires scrolling to display data items. 

Silverlight 3 also supports container recycling, but it's enabled by default for ListBox, so there is no need to set the "VirtualizationMode" to "Recycling" explicitly. This is a slight incompatibility between the two frameworks. Because I frequently switch back and forth between Silverlight and WPF, I'd rather be explicit about it every time so that I don't forget it when I need it.

### Deferred scrolling

"Deferred scrolling" is a feature that allows the user to drag the scroll bar thumb around without changing the displayed items until the scroll bar thumb is released. This improves the application's perceived responsiveness to scrolling when the items are displayed using complex templates, though of course, the user can't see the items they're scrolling through.

 With .NET 3.5 SP1, it is possible to enable deferred scrolling by setting an attached property on the control:

	<ListBox ScrollViewer.IsDeferredScrollingEnabled="True" ... />

Again, for backwards compatibility reasons, this feature is disabled by default. Deferred scrolling is not supported in Silverlight 3.
 
### Hierarchical data

In .NET 3.5 SP1, the WPF team extended UI virtualization to TreeView by adding support for hierarchical data to VirtualizingStackPanel. As a consequence, the container recycling and deferred scrolling features discussed above also apply to hierarchical data. UI virtualization is disabled by default in TreeView - here's how you enable it:

	<TreeView VirtualizingStackPanel.IsVirtualizing="True" ... />

This property is useful not just for TreeView, but for any control that uses VirtualizingStackPanel and that doesn't set IsVirtualizing to true (ItemsControl, for example). ListBox already sets IsVirtualizing to True by default, so there is no need to set it explicitly. 

Silverlight 3 doesn't support UI virtualization for hierarchical data. It also doesn't allow you to set the "IsVirtualizing" property. If your Silverlight control scrolls and uses a VirtualizingStackPanel to display non-hierarchical data, UI virtualization is enabled automatically.

### Limitations

.NET 3.5 SP1 fixed many previous limitations on UI virtualization, but a couple still remain:

- ScrollViewer currently allows two scrolling modes: smooth pixel-by-pixel scrolling (CanContentScroll = false) or discrete item-by-item scrolling (CanContentScroll = true). Currently WPF  supports UI virtualization only when scrolling by item. Pixel-based scrolling is also called "physical scrolling" and item-based scrolling is also called "logical scrolling". 
- When using data binding's "Grouping" feature, there is no support for UI virtualization.

These are really the same limitation. If you look at the default style for ListBox, ListView and ComboBox, you will find the following trigger:

	<Trigger Property="IsGrouping" Value="true">
		<Setter Property="ScrollViewer.CanContentScroll" Value="false"/>
	</Trigger>

The implementation of Grouping assumes that each group is a separate item in the ItemsControl that contains it. Since each group can (and typically does) have many sub-items, scrolling by item would result in a really bad user experience - scrolling down a bit would cause a big jump to the top of the next group. That's why the team decided to switch to pixel based scrolling when displaying grouped data. The unfortunate consequence is that no UI virtualization is supported when grouping.

I'm often asked if there is a way to work around this limitation. Well, anything is possible, but there is no *easy* workaround. You would have to re-implement significant portions of the current virtualization logic to combine pixel-based scrolling with UI virtualization. You would also have to solve some interesting problems that come with it. For example, how do you calculate the size of the thumb when the item containers have different heights? (Remember that you don't know the height of the virtualized containers - you only know the height of the containers that are currently displayed.) You could assume an average based on the heights you do know, or you could keep a list with the item heights as items are brought into memory (which would increase accuracy of the thumb size as the user interacts with the control). You could also decide that pixel-based scrolling only works with items that are of the same height - this would simplify the solution. So, yes, you could come up with a solution to work around this limitation, but it's not trivial.

And this brings me to another change introduced in Silverlight 3. Silverlight 2's ListBox used to support only pixel-based scrolling, but with the introduction of UI virtualization in Silverlight 3, the default scrolling mode for ListBox is now item-based. Unlike WPF, Silverlight's ScrollViewer doesn't have a "CanContentScroll" property. In Silverlight 3, if your ListBox uses VSP it will scroll by item and have virtualization enabled, and if you change it to use StackPanel instead, it will scroll by pixel and have virtualization disabled.


