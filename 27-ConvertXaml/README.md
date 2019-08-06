# How to encode XAML using a Value Converter

This blog post is especially useful for those of you that have your own blog. Several people have asked me about my solution for posting XAML markup, given that the &lt; and &gt; characters have to be encoded. With the amount of XAML I post, I certainly don't replace them all by hand. As you probably guessed, I came up with a geeky solution that uses Avalon and Data Binding. 

The UI of my solution is as simple as it could be. It contains two TextBoxes, one where I paste the text and XAML I am writing, and another one that shows the encoded version of this text.

	<Window.Resources>
		<local:XamlConverter x:Key="xamlConverter"/>
	</Window.Resources>
	    
	<TextBox Name="source" AcceptsReturn="true" AcceptsTab="true"/>
	<TextBox Text="{Binding ElementName=source, Path=Text, Converter={StaticResource xamlConverter}, Mode=OneWay}" Grid.Row="1" Name="target"/>

I only need one Binding in this sample. The TextBox where I input the text is the source of the binding, and the TextBox with the encoded text is the target. The Converter is where I add all the interesting logic. I am not going to show that code here because it does all sorts of fancy things that are not Avalon related, but feel free to download and reuse it. You may want to modify the CSS styles to suit your blog, or you will end up with the same fonts and colors I use.

Next I would like to discuss best practices on writing the ConvertBack method of the Converter, when the Binding's mode is OneWay. In this case, you want to add your logic to the Convert method, which gets called when the data is transfered from the source to the target. The ConvertBack method, normally used when transfering data from target to source, should not be called. There are 3 very common ways to deal with this scenario:

## Throw

If you let Visual Studio generate a basic implementation for your interface (by hovering over the interface name), this is what you will get:

	public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{
		throw new Exception("The method or operation is not implemented.");
	}

I don't recommend keeping this code. Throwing an exception is a fine solution, but it's good practice to be as specific as possible in the type of Exception we pick. I typically use a NotImplementedException, and pass a nice error message:

	public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{
		throw new NotImplementedException("The ConvertBack method is not implemented because this Converter should only be used in a one-way Binding.");
	}

## Return Binding.DoNothing

If you return Binding.DoNothing, you are telling the Data Binding engine to ignore the current data transfer. In this case, if you happen to use the Converter in a two-way (or one-way-to-source) binding, the binding will fail silently.

I don't particularly like this solution. As a rule of thumb, I avoid approaches that involve failing silently.

So why did we add Binding.DoNothing to the API? We added it to handle scenarios where you want to cancel the binding based on the value being transfered. For example, you may have a behavior requirement in your application where you don't want changes to be propagated to the source when the user enters certain data in the UI. In this case, you would inspect the data entered by the user in the Convert method, and if it has a certain value, you return Binding.DoNothing. The application is not "failing" in this scenario, since it's part of the business logic to cancel the binding, so it's OK to do that silently.

	public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{
		return Binding.DoNothing;
	}

## Return null

It's tempting to return null from ConvertBack when you don't expect it to ever be called. I don't recommend this approach because you may inadvertently end up setting your source property to null. One of two things can happen: if null is a valid value for your source property, it will update the source when you didn't intend it to; if null is not a valid value for your source property, a debug message will be printed in the Output Window of Visual Studio. Data Binding typically informs the developer of errors through the Output Window, so it's good to keep an eye on it.

	public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{
		return null;
	}

In summary, my favorite solution is throwing NotImplementedException from the ConvertBack method of a one-way binding. If the ConvertBack method is called by mistake, your application will crash, and you will find the problem right away.
