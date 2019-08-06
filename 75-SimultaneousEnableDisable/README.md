# How to simultaneously enable/disable multiple controls

In a comment after our last blog post, a reader named Craig asked an interesting question. He wanted to know how he could enable and disable controls according to their individual needs, but also disable groups of controls simultaneously when his application is busy working on a long-running task. For example, in the text editor of a programming environment, the cut and paste buttons should be enabled or disabled depending on the current text selection and clipboard contents; however, while the programming environment is compiling code, all of these buttons must be disabled.

The question is interesting because there are many possible approaches, and it's not immediately obvious which is the best way. We'll explore some of the possibilities in this post, first for WPF (the platform Craig asked about) and then for Windows 8 XAML apps (because it's interesting to compare with the latest technology).

## WPF applications

### Styles

Craig mentioned one approach to the problem: creating a Style that can be shared by many controls, and using a Setter or DataTrigger within the Style to disable the controls whenever the app is busy. This idea is appealing because it separates concerns--the Style deals with disabling when busy, leaving each individual control to deal with disabling for its own particular reasons. Unfortunately, neither of the following styles actually does what we want:

	<Window.Resources>
		<Style x:Key="disabledWhenWorkingStyle1" TargetType="Button">
			<Setter Property="IsEnabled" Value="{Binding IsAvailable}"/>
		</Style>

		<Style x:Key="disabledWhenWorkingStyle2" TargetType="Button">
			<Style.Triggers>
				<DataTrigger Binding="{Binding IsAvailable}" Value="False">
					<Setter Property="IsEnabled" Value="False"/>
				</DataTrigger>
			</Style.Triggers>
		</Style>
	</Window.Resources>

	<Button Content="Cut"
		Style="{StaticResource disabledWhenWorkingStyle1}"
		IsEnabled="{Binding IsTextSelected}"/>
	<Button Content="Paste"
		Style="{StaticResource disabledWhenWorkingStyle2}"
		IsEnabled="{Binding DoesClipboardContainText}"/>

The problem is that the local value of the "IsEnabled" property (the binding to "IsTextSelected" or to "DoesClipboardContainText") always takes precedence over any value provided by a style, regardless of whether it's through a style setter or a data trigger setter.

### MultiBindings

Another approach is to set each control's "IsEnabled" property to a MultiBinding that binds to two properties simultaneously--the global "IsAvailable" property, and a local property specific to that particular control. While this approach works, it has the disadvantage of requiring a lot of markup for each control:

	<Window.Resources>
		<local:AndMultiValueConverter x:Key="andMultiValueConverter"/>
	</Window.Resources>

	<Button Content="Cut">
		<Button.IsEnabled>
			<MultiBinding
				Converter="{StaticResource andMultiValueConverter}">
				<Binding Path="IsAvailable"/>
				<Binding Path="IsTextSelected"/>
			</MultiBinding>
		</Button.IsEnabled>
	</Button>

	<Button Content="Paste">
		<Button.IsEnabled>
			<MultiBinding
				Converter="{StaticResource andMultiValueConverter}">
				<Binding Path="IsAvailable"/>
				<Binding Path="DoesClipboardContainText"/>
			</MultiBinding>
		</Button.IsEnabled>
	</Button>

### View model logic

A third approach is to extend the view model to include additional properties that combine the global "IsAvailable" property with individual properties. For example, we could bind the "IsEnabled" dependency property of the cut button to a new view model property called "CanCut", which checks the values of both "IsAvailable" and "IsTextSelected":

	public bool CanCut
	{
		get { return IsAvailable && IsTextSelected; }
	}

	public bool CanPaste
	{
		get { return IsAvailable && DoesClipboardContainText; }
	}

There are two drawbacks to this technique: first, the view model will be cluttered with many more properties; and second, we must be very diligent to raise property change notifications for each of these new properties whenever either of the original properties changes. In particular, the "IsAvailable" property will have to raise a long list of property change notifications whenever its value changes. In general, this approach is quite error prone and therefore we don't recommend it.

### Commands

Our fourth approach takes advantage of WPF's built-in support for commands. When a button's "Command" dependency property is set to an object that implements the ICommand interface, the button will automatically be enabled or disabled according to the CanExecute method of the command object. This may sound great at first, but of course, we have to notify the Cut command object to change the value it returns from its CanExecute method whenever "IsAvailable" or "IsTextSelected" changes; and likewise for the Paste command whenever "IsAvailable" or "DoesClipboardContainText" changes. The code complexity is similar to the view model logic case above.

Commands do have their place--they are perfect when data binding buttons to a view model. However, there are only a few controls that work with commands, so this approach doesn't help with TextBox, ComboBox, RadioButton, ListBox, and many other controls.

### Inheritance of "IsEnabled"

Finally, we arrive at our recommended solution. In WPF, the "IsEnabled" dependency property is supported on every UI element, including panels, not just elements that derive from Control. Furthermore, setting "IsEnabled" to false on an element disables that element and all its descendants through property inheritance. Therefore, by placing a group of controls within a single panel, we can disable all the controls simultaneously by binding the panel's "IsEnabled" dependency property to our "IsAvailable" property. This approach is simple and concise:

	<StackPanel IsEnabled="{Binding IsAvailable}">
		<Button Content="Cut"
			IsEnabled="{Binding IsTextSelected}"/>
		<Button Content="Paste"
			IsEnabled="{Binding DoesClipboardContainText}"/>
	</StackPanel>

## Windows 8 XAML applications

Now let's take a brief look at how each of the approaches suits a Windows 8 XAML application.

• As we discussed earlier, Styles don't solve the problem. Even if they did, Windows 8 XAML applications don't provide support for data triggers or for bindings in style setters.
• MultiBinding is not available in Windows 8 XAML applications.
• We can extend the view model logic, but at the cost of increased code complexity, as in WPF.
• We can use commands, again at the cost of increased complexity, and only for buttons, as in WPF.
• The "IsEnabled" dependency property is only offered by Control and its derived classes in Windows 8 XAML apps. As a result, we can't disable a group of controls with a binding on a panel. However, there's a simple work-around: place the panel in a ContentControl, and bind the ContentControl's "IsEnabled" property.

Here's how the XAML looks in a Windows 8 XAML application:

	<ContentControl IsEnabled="{Binding IsAvailable}">
		<StackPanel>
			<Button Content="Cut"
				IsEnabled="{Binding IsTextSelected}"/>
			<Button Content="Paste"
				IsEnabled="{Binding DoesClipboardContainText}"/>
		</StackPanel>
	</ContentControl>

