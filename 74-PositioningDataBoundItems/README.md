# How to position data bound items

A long time ago in a galaxy far, far away, we wrote about the <a href="http://www.zagstudio.com/blog/470">power of styles and templates in WPF</a>, showing how ordinary-looking list box items could be transformed into a visualization of the planets in our solar system. Recently, we noticed that some of the techniques described there don't work in Windows Store applications for Windows 8.  In this post, we'll revive the planets list box and describe the changes required to make it to work in a Win8 app.

As in the WPF version, the first step is to create a data source and use it as the source for a data bound list box:

	<Page.Resources>
		<local:SolarSystem x:Key="solarSystem"/>
	</Page.Resources>
    
	<ListBox ItemsSource="{Binding Source={StaticResource solarSystem}, Path=SolarSystemObjects}"/>

Of course, this creates a rather uninteresting user interface.

If we attempt to use the styles and templates from our earlier WPF example, we quickly discover that WPF XAML is incompatible with Win8 XAML in a few places (mostly the same places as Silverlight):
- DataTemplate doesn't have a DataType property.  Instead, we have to refer to DataTemplates explicitly by x:Key.
- ControlTemplate doesn't have a Triggers collection.  Instead, we have to use VisualStateManager to control the appearance of selected and unselected items through visual states.
- Canvas doesn't support the Canvas.Bottom or Canvas.Right attached properties.  Instead, we must use Canvas.Left and Canvas.Top to position child elements.
- Various other properties have been moved, simplified, or eliminated.

But the main problem with the style we used for the WPF ListBoxItem is that it doesn't correctly position the planets in Win8.  Even when we replace Canvas.Bottom with Canvas.Top, the following setters have no effect:

	<Style TargetType="ListBoxItem">
		<Setter Property="Canvas.Left"
			Value="{Binding Path=Orbit,
				Converter={StaticResource convertOrbit},
				ConverterParameter=0.707}"/>
		<Setter Property="Canvas.Top"
			Value="{Binding Path=Orbit,
				Converter={StaticResource convertOrbit},
				ConverterParameter=0.707}"/>
		…
	</Style>

The planets end up piled on top of each other, rather than distributed over the list box.  This is because (unfortunately) Win8 apps don't support bindings as the values of setters.  They compile and run without any complaint, but they don't do anything.  Silverlight had the same limitation in its first 4 versions, though Silverlight 5 added support for bindings in setters.

If you search the web, you'll find a variety of solutions to this lack of bindings in setters, but the one we like best is <a href="http://blogs.msdn.com/b/delay/archive/2009/11/02/as-the-platform-evolves-so-do-the-workarounds-better-settervaluebindinghelper-makes-silverlight-setters-better-er.aspx">David Anson's SetterValueBindingHelper class</a>.  His code is for Silverlight, but the same approach ought to work for Win8 apps.

In our case, however, there's a simple workaround.  Rather than setting Canvas.Left and Canvas.Top on the ListBoxItem, we can position the content within the list box item using a RenderTransform:

	<Style TargetType="ListBoxItem">
		<Setter Property="Template">
			<Setter.Value>
				<ControlTemplate TargetType="ListBoxItem">
					<Grid>
						<Grid.RenderTransform>
							<TranslateTransform
								X="{Binding Path=Orbit,
									Converter={StaticResource convertOrbit},
									ConverterParameter=0.707}"
								Y="{Binding Path=Orbit,
									Converter={StaticResource convertOrbit},
									ConverterParameter=0.707}"/>
						</Grid.RenderTransform>
						...

This simple change puts all the planets in the right place:

<img src="Images/PlanetsListBox.png" class="postImage" />
