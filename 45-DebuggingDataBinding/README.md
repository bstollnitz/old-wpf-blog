# How to debug WPF bindings

Data Binding can be tricky to debug. In this post, I will share the techniques I use to debug WPF bindings, including the new debugging improvements we implemented in the latest 3.5 release.  I will discuss the following four techniques:

- Scanning Visual Studio's Output window for errors.
- Using Trace Sources for maximum control.
- Using the new property introduced in WPF 3.5 - PresentationTraceSources.TraceLevel.
- Adding a breakpoint to a Converter.

The DataContext of this app is set to a new instance of Star, which is a class that contains two properties: Picture and Caption. Picture is of type BitmapImage and contains an image of the sun, which I included in the project. Caption is a string property that takes a while to be initialized (more details about this later).


## Output Window

In the XAML of this application, I have an Image whose Source is data bound to an incorrect property name. This is a simple scenario that causes a Binding to fail.

	<Image Source="{Binding Path=PictureWrong}" Width="300" Height="300" Grid.Row="0"/>

Every time a Binding fails, the binding engine prints an informative message in the Output window of Visual Studio. In this case, I get the following message:

	System.Windows.Data Error: 35 : BindingExpression path error: 'PictureWrong' property not found on 'object' ''Star' (HashCode=49535530)'. BindingExpression:Path=PictureWrong; DataItem='Star' (HashCode=49535530); target element is 'Image' (Name=''); target property is 'Source' (type 'ImageSource')

This message should give you enough information to quickly figure out the mistake in the property name. 

Advantage of this technique:
- It is very easy to look at the Output window, and in most cases it's sufficient. It should be the first approach you consider when you have a problem with your bindings.

Disadvantage of this technique:
- Most real world applications print so much information to the Output window that it may be hard to find the error you’re looking for.


## Trace Sources

The Trace Sources solution was already around in WPF 3.0. It adds a lot more flexibility to the previous solution by allowing you to control the level of detail you care about, where you want that messages to be printed and the WPF feature you want to debug. 

The Trace Sources solution relies on a config file for the configuration of its behavior - the App.config file.  Here is a portion of the contents of that file:

	<configuration>
		<system.diagnostics>
			<sources>
				<source name="System.Windows.Data" switchName="SourceSwitch" >
					<listeners>
						<add name="textListener" />
					</listeners>
				</source>
				...
			</sources>
		    
			<switches>
				...
				<add name="SourceSwitch" value="All" />
			</switches>
		
			<sharedListeners>
				...
				<add name="textListener"
				type="System.Diagnostics.TextWriterTraceListener"
				initializeData="DebugTrace.txt" />
				...
			</sharedListeners>
		
			<trace autoflush="true" indentsize="4"></trace>
		
		</system.diagnostics>
	</configuration>

In this file, I am specifying that:
- I want only messages generated in the Data subarea to be printed. If you're trying to debug, for example, animations, you would instead add the area System.Windows.Media.Animation.
- I want as much information as possible about data binding. This was done by setting the switch to All. Other possible values are Off, Error, Warning. For a complete list, look up SourceLevels in .NET Reflector.
- I want the messages to be printed to an external file called DebugTrace.txt, instead of the Output Window. This file will be created in the bin\debug folder for the application. If you run this application twice, the messages generated the second time will be appended to the existing messages in this file. If you don't want this behavior, remember to delete the file before running the app. 
Other pre-defined listeners allow printing to the Console (ConsoleTraceListener), or to an external file in XML format (XmlWriterTraceListener).

If you run the application with the settings above, you should find a DebugTrace.txt file in the bin\debug directory. If you open it, you will see the data binding error we saw previously in the Output Window, plus four "Information" messages. These lower-priority messages are printed because I specified in the switch that I want all the information available about the bindings. 

If you want to learn more about this topic, I recommend <a href="http://blogs.msdn.com/mikehillberg/archive/2006/09/14/WpfTraceSources.aspx">Mike Hillberg's blog</a>. He wrote the best article I've read about Trace Sources, which I use frequently as a reference.

Advantages of this technique:
- It separates the debug messages you care about from the rest of the information printed in the Output window.
- This solution may help you debug other areas in WPF, not just binding.
- You can get lower-priority messages (such as information or warnings) that are not typically printed to the Output window.

Disadvantages of this technique:
- The text file (or whatever form of output you choose) will contain debug messages about all the bindings in your application. Although this is not as much clutter as the Output window, it may still require some digging for you to find exactly the information you need.
- It won't help you in scenarios where your Binding actually succeeds, but you still don't see what you expect in the UI. The first and second techniques I show here only help in scenarios where the Binding fails.

Before you move on, make sure you correct the Path in the Image's Source so that these errors won't interfere with the ones I show next.


## Trace Level - new in 3.5

In order to understand this feature, let's start by uncommenting the first TextBlock in the XAML of this application:

	<TextBlock Text="{Binding Path=Caption}" ... />

This TextBlock attempts to bind to Caption, a property whose value is slow to be initialized. In this case, I am simulating a slow data source by adding a Dispatcher timer to the constructor, but in reality this delay could have many different causes. Notice also that I am *not* raising a property changed notification when Caption changes value. 

	public string Caption
	{
		private set
		{
			this.caption= value;
			//OnPropertyChanged("Caption");
		}
		get
		{
			return caption;
		}
	}
	
	public Star()
	{
		this.Picture = new BitmapImage(new Uri("Images\\sun.jpg", UriKind.Relative));
		this.Caption = String.Empty;
		DispatcherTimer timer = new DispatcherTimer();
		timer.Interval = new TimeSpan(0, 0, 3);
		timer.Tick += new EventHandler(Timer_Tick);
		timer.Start();
	}
	
	private void Timer_Tick(object sender, EventArgs e)
	{
		this.Caption = "Sun";
	}

In this scenario, the Binding will succeed (it will bind to the initial value of empty string), but you don't see what you expect to see in the UI. The unwanted behavior is caused by the fact that the events happen in an order different from what you expect: the Binding will be evaluated before Caption gets its real value.  In this case, there are no errors in the Output window or DebugTrace.txt because the Binding succeeds. 

You can use the new debugging feature in 3.5 to debug this scenario by adding the attached property PresentationTraceSources.TraceLevel to the Binding, which can be set to None, Low, Medium and High. Since PresentationTraceSource is not in the default namespace mappings for WPF, you will have to write the following XAML:

	<Window ...
	xmlns:diagnostics="clr-namespace:System.Diagnostics;assembly=WindowsBase"
	/>
	
	<TextBlock Text="{Binding Path=Caption, diagnostics:PresentationTraceSources.TraceLevel=High}" ... />

If you look at the Output window, you will notice that the binding engine generated debug information for every task that may help users find problems with this particular binding:

	System.Windows.Data Warning: 47 : Created BindingExpression (hash=25209742) for Binding (hash=3888474)
	...
	System.Windows.Data Warning: 91 : BindingExpression (hash=25209742): GetValue at level 0 from Star (hash=31609076) using RuntimePropertyInfo(Caption): ' '
	System.Windows.Data Warning: 71 : BindingExpression (hash=25209742): TransferValue - got raw value ' '
	System.Windows.Data Warning: 78 : BindingExpression (hash=25209742): TransferValue - using final value ' '

In this case, we can scan the debug messages quickly to see that there were no errors, and look at the last few messages to understand that the value found at the Source was the empty string. This information will help you come to the conclusion that there is some timing issue with the scenario.

Advantages of this technique:
- This feature is particularly useful when you know exactly which binding you want to find out more about, which is the most common scenario. 
- It allows you to know more about bindings that succeed, which many times helps you find the mistake in your logic.

Disadvantages of this technique:
- If you forget to remove the property after you found the problem, it adds to the clutter of the Output Window (which adds to the time it takes to start your application in debug mode). 


## Converter

The last solution is extremely simple: you can simply add a no-op Converter to your binding and put a breakpoint in its Convert method. This is what this solution looks like:

	<TextBlock Text="{Binding Path=Caption, Converter={StaticResource converter}}" .../>

	public class DebuggingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value; // Add the breakpoint here!!
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException("This method should never be called");
		}
	}

If you run this code and set a breakpoint inside the Convert method, you can simply hover over the "value" parameter to see what is being passed from the source to the target. In this case, you will see that "value" contains an empty string. This may help you realize that the source of your binding does not have the value you thought it had.

Advantages of this technique:
- It is really easy to implement. It relies on a concept that most data binding users are familiar with.
- It does not depend on the Output window.
- It helps you find out more about scenarios where the binding doesn't fail.

Disadvantages to this technique:
- It doesn't provide as much information as the TraceLevel technique. 
- If the Binding fails early, the Converter may not be called.


## Conclusion

I mentioned here techniques to debug WPF bindings using Visual Studio, however there are other tools that can help you with this process. One other tool I use frequently is <a href="http://blois.us/blog/2006/08/long-time-since-my-last-post-but-dont_21.html">Snoop</a>, not only to debug bindings but many other aspects of my application. <a href="http://karlshifflett.wordpress.com/mole-for-visual-studio/">Mole</a> is also useful, and has great documentation. 

What technique or tool you use to debug your bindings comes down to a combination of personal preference and your specific scenario.

