# How to update an explicit binding within a template

![](Images/38mix.bmp)

Mix07 in Vegas was awesome! I always get pumped up when I meet new customers that are using WPF and loving it! It gives me fuel to come back to rainy Seattle with a smile and extra energized to do my absolute best for WPF!

In today's sample, I have a ListBox with editable data. When I change the data of one of the items, I want that to be reflected in the source, as usual. However, this time I set my Binding's UpdateSourceTrigger to Explicit, which means I need to make that update by calling the UpdateSource() method on BindingExpression through code. Unfortunately, getting a handle to the BindingExpression in a ListBox scenario is a little tricky with the current bits of WPF, which is why I decided to write this post.

I will start with a quick explanation of two basic concepts, which are core to the understanding of this problem: Binding Mode and UpdateSourceTrigger. If this is too basic for you, you can skip the next couple of sections and safely jump to "Updating Explicit Bindings."

## Binding Mode

WPF Data Binding supports five binding modes (which you can set by using the Mode property of Binding):

- One way - The data flows from the source to the target only. If you add a Binding to a TextBlock's Text property and don't specify the Mode, it will be one way by default.

- Two way - The data flows from the source data to the target UI, and the other way around. A Binding on a TextBox's Text property without the Mode specified is two way by default: changes in the source are reflected in the TextBox, and changes typed into the TextBox are also propagated back to the source.

- One time - Like one way, but the UI doesn't listen to change notifications in the source. You may want to consider using this mode if your source doesn't support property change notifications. If you don't care about changes in the source, setting your binding to one time will make it a little more performant.

- One way to source - The opposite of one way: the data flows only from the target UI to the source. I have yet to see a good use of this binding mode - the scenarios that require it are quite rare.

- Default - This is the same as not setting the Mode property at all. The Binding engine will look at the default mode specified at the time the DependencyProperty was registered, and will use that. Therefore, setting the Mode to Default will not mean the same thing for all DependencyProperties. For example, as I mentioned before, TextBlock's Text has a default mode of one way, while the TextBox's Text has a default of two way.

But how do we know the default Binding Mode of a DependencyProperty? How can we set a default Binding Mode when we define a new DependencyProperty?

<a href="http://www.red-gate.com/products/reflector/">.NET Reflector</a> is your friend. With reflector, search for TextBox and look at the source for the static constructor (.cctor()). Here, you will be able to find the code used for registering the TextProperty DP:

	TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TextBox), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(TextBox.OnTextPropertyChanged), new CoerceValueCallback(TextBox.CoerceText), true, UpdateSourceTrigger.LostFocus));

Notice that a parameter is passed to the Register method indicating the default Binding Mode: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault. If you use reflector to look at the registration for TextBlock's Text DP, you will see that no such value is passed, in which case we assume the binding is one way by default.

It is not possible to make the binding one way to source or one time by default (we couldn't think of any compelling user scenarios that would justify increasing the complixity of the API). However, it is possible to say that you don't want the DP to support data binding at all - to do this, simply pass FrameworkPropertyMetadataOptions.NotDataBindable as a parameter.

## Binding UpdateSourceTrigger

In the case of a binding that propagates values from the target to the source (Mode = TwoWay or OneWayToSource), we allow you to specify how you want that update to be triggered. There are three ways to cause the changes to propagate to the source (which you can set by using the UpdateSourceTrigger property of Binding):

- LostFocus - The value will be updated when the element loses focus. This is the default behavior for TextBox (notice that UpdateSourceTrigger.LostFocus is specified when the Text dependency property was registered). When you type something in a TextBox, that value will be updated to the source when you change focus to some other element.

- PropertyChanged - The value is updated every time it changes. In the TextBox scenario, the value will be updated every time you type a new character.

- Explicit - The target value if not updated until you explicitly call "UpdateSource()" on the BindingExpression.

## Updating Explicit Bindings

In today's sample, I started by defining a source object (MySource) with a property Employees of type ObservableCollection&lt;Employee&gt;. Employee is a class that contains two properties: Name and Title, both of type string. The code for defining this source is straight forward, so I won't show it here.

Then I added a ListBox to my XAML file that is bound to the Employees collection, and I added a Style for its items:

	<Window.Resources>
		<DataTemplate x:Key="nonEditableEmployee">
			<StackPanel Margin="2">
				<TextBlock FontWeight="Bold" Text="{Binding Path=Name}"/>
				<TextBlock Text="{Binding Path=Title}" />
			</StackPanel>
		</DataTemplate>
	
		<DataTemplate x:Key="editableEmployee">
			<StackPanel Margin="2" >
				<TextBlock FontWeight="Bold" Text="{Binding Path=Name}" />
				<TextBox Text="{Binding Path=Title, UpdateSourceTrigger=Explicit}" Width="130" x:Name="tb"/>
			</StackPanel>
		</DataTemplate>
	
		<Style TargetType="ListBoxItem" x:Key="lbiStyle">
			<Setter Property="Height" Value="40" />
			<Setter Property="ContentTemplate" Value="{StaticResource nonEditableEmployee}" />
			<Style.Triggers>
				<Trigger Property="IsSelected" Value="True">
					<Setter Property="ContentTemplate" Value="{StaticResource editableEmployee}" />
				</Trigger>
			</Style.Triggers>
		</Style>
	</Window.Resources>
	
	<ListBox ItemsSource="{Binding Path=Employees}" ItemContainerStyle="{StaticResource lbiStyle}" IsSynchronizedWithCurrentItem="True" (...) Name="lb"/>
	<Button Content="Submit" Click="Submit" (...) />

In the Style for ListBoxItems, I am specifying that I want the DataTemplate applied to each of the items to be "nonEditableEmployee" except when the item is selected, in which case I want to use "editableEmployee". Both DataTemplates display the Name and Title of the Employee, the only difference is that "editableEmployee" displays the Title using a TextBox instead of a TextBlock. So, when the user selects an employee in the ListBox, that employee's Title becomes editable. 

Below the ListBox I have a Submit Button. If I click on this Button after editing some data in the ListBox, I want that data to be submitted to the source. However, if I change the data in the ListBox and don't click on the Button, I don't want the source to be modified. Notice that I set the UpdateSourceTrigger of editable Title's Binding to Explicit.

To achieve this behavior, I need to call the UpdateSource() on the BindingExpression's instance. Getting this instance is a little tricky, so let's think about it for a minute. The best way to think about this is by starting from the end, and walking backwards. Utimately, this is the code I want to write (assuming "be" is the BindingExpression instance, and "tb" is the TextBox's instance):

	BindingExpression be = tb.GetBindingExpression(TextBox.TextProperty);
	be.UpdateSource();

As you can see, in order to get the BindingExpression for the Binding in the TextBox's Text property, I need to have a handle to that TextBox. Traversing the DataTemplate to get to the TextBox won't give me the actual instance, it will only give me the FrameworkElementFactory used to create the TextBox instance. Fortunately, DataTemplate has a FindName method that, given the instance of the object the template is applied to, will give you a particular named part within its visual tree. So, assuming "cp" is the object the DataTemplate is applied to, this is the code I would like to write to get the actual TextBox:

	DataTemplate dt = (DataTemplate)(this.Resources["editableEmployee"]);
	TextBox tb = (TextBox)(dt.FindName("tb", cp));

We're getting close. The next step is to figure out how to get to the element the DataTemplate is applied to. A DataTemplate is always applied to a ContentPresenter, which is an element present in the visual tree of every ContentControl. Since every item in the ListBox is wrapped with a ListBoxItem (which is a ContentControl), I simply have to get to the ContentPresenter instance in the ControlTemplate for the currently selected ListBoxItem. Here is what I did:

	Employee currentEmployee = (Employee)(lb.Items.CurrentItem);
	ListBoxItem lbi = (ListBoxItem)(lb.ItemContainerGenerator.ContainerFromItem(currentEmployee));
	ContentPresenter cp = GetObjectOfTypeInVisualTree<ContentPresenter>(lbi);

"GetObjectOfTypeInVisualTree" is a simple recursive method that walks the visual tree of an object (in this case, the current ListBoxItem) and returns the first element matching the type specified by the generic parameter (in this case ContentPresenter). Here is the complete code:

	private void Submit(object sender, RoutedEventArgs e)
	{
		Employee currentEmployee = (Employee)(lb.Items.CurrentItem);
		ListBoxItem lbi = (ListBoxItem)(lb.ItemContainerGenerator.ContainerFromItem(currentEmployee));
		ContentPresenter cp = GetObjectOfTypeInVisualTree<ContentPresenter>(lbi);
	
		DataTemplate dt = (DataTemplate)(this.Resources["editableEmployee"]);
		TextBox tb = (TextBox)(dt.FindName("tb", cp));
	
		BindingExpression be = tb.GetBindingExpression(TextBox.TextProperty);
		be.UpdateSource();
	}
	
	private T GetObjectOfTypeInVisualTree<T>(DependencyObject dpob) where T : DependencyObject
	{
		int count = VisualTreeHelper.GetChildrenCount(dpob);
		for (int i = 0; i < count; i++)
		{
			DependencyObject child = VisualTreeHelper.GetChild(dpob, i);
			T childAsT = child as T;
			if (childAsT != null)
			{
				return childAsT;
			}
			childAsT =  GetObjectOfTypeInVisualTree<T>(child);
			if(childAsT != null)
			{
				return childAsT;
			}
		}
		return null;
	}

This code is not only useful for submitting explicit bindings; it is useful for any scenario where you need to get to the instance of some element in your DataTemplate. It should be pretty easy to tweak this sample to suit your needs.

We do realize that this scenario is quite hard to implement, and we know it's not that uncommon. We would like to make it easier, but unfortunately we will not be able to make that happen in the next release. At least for now you have a sample solution that should help you solve most scenarios. Hopefully we will have a chance to fix this in the platform in the near future.

Below is a screenshot of the final application. The ListBox on the right is bound to the same source. I added it to this sample so you can see when the source changes.

![](Images/38UpdateExplicit.png)
