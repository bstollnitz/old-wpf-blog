# How to port the WPF labeled pie chart to Silverlight

*Update April 4 2010: The LabeledPieChart code in this post has been updated to the latest WPF and Silverlight toolkits. You can find more details in <a href="http://www.zagstudio.com/blog/507">this blog post</a>.


<a href="http://www.zagstudio.com/blog/502">Two posts ago</a> I showed a possible solution to add labels to a WPF pie chart. <a href="http://www.zagstudio.com/blog/503">In my last post</a>, I explained some implementation details of that solution. In this blog post, I will show the steps I took to port the labeled pie chart to Silverlight.

The Silverlight and WPF teams do their best to ensure that porting a Silverlight application to WPF is a smooth experience. This is expected, since Silverlight is (for the most part) a subset of WPF. Porting WPF applications to Silverlight, on the other hand, can be pretty tricky. The Silverlight and WPF teams are not specifically supporting this scenario, but the reality is that in the real world, many people need to do just that. I decided to port my WPF labeled pie chart to Silverlight to see how many issues I would come across and to document the workarounds. 

## MeasureOverride is sealed in Silverlight

The first issue I encountered was the fact that Canvas' MeasureOverride is sealed in Silverlight, so I couldn't override it. I was overriding it in the PieChartLabelArea, where I had to know whether any of the child labels are associated with small arcs to help with the Auto display mode.

My workaround was to derive from Panel instead, since Panel's MeasureOverride is not sealed. Since Panel doesn't do automatic arranging, I had to also implement ArrangeOverride:

	protected override Size ArrangeOverride(Size finalSize)
	{
		foreach (UIElement child in this.Children)
		{
			child.Arrange(new Rect(new Point(0, 0), child.DesiredSize));
		}
	
		return finalSize;
	}

In this scenario it was OK to replace the Canvas with a Panel because the WPF implementation wasn't using any of Canvas' functionality other than the automatic arranging. The contents of the labels are positioned inside a different Canvas that's part of the ControlTemplate for PieChartLabel. When PieChartLabels are added to the PieChartLabelArea, the canvases in their templates are all placed at the origin of the label area, and each label is positioned correctly within its own canvas.

## OverrideMetadata doesn't exist in Silverlight

In WPF I was using the following code in the LabeledPieChart's static constructor to set its default style key:

	static LabeledPieChart()
	{
		DefaultStyleKeyProperty.OverrideMetadata(typeof(LabeledPieChart), new FrameworkPropertyMetadata(typeof(LabeledPieChart)));
	}

Since OverrideMetadata doesn't exist in Silverlight, I used the instance constructor instead to set the DefaultStyleKey property:

	public LabeledPieChart()
	{
		this.DefaultStyleKey = typeof(LabeledPieChart);
	}

## AddOwner doesn't exist in Silverlight

In WPF, I was registering some DPs with the convenient AddOwner method. For example, the following line of code indicates that the Title property of Chart can be applied to LabeledPieChart as well:

	public static readonly DependencyProperty TitleProperty = Chart.TitleProperty.AddOwner(typeof(LabeledPieChart), null);

The workaround is to register a new DP with the same name in LabeledPieChart. The syntax is not much longer:

	public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(object), typeof(LabeledPieChart), new PropertyMetadata(String.Empty));

## Vector doesn't exist in Silverlight

The WPF version of this code makes use of the Vector class in several places. We rely on Vector in PieChartHelper to calculate the midpoint of a wedge's arc, and in PieChartLabel to determine the three points needed in Connected mode. 

As a workaround, I decided to bring a version of WPF's Vector class into the Silverlight project. I simply copied this code from Reflector and tweaked it a bit.

## FrameworkPropertyMetadataOptions.AffectsArrange doesn't exist in Silverlight

In WPF, I specified that the DisplayMode property should invalidate arrange when set, which in turn was causing the label to be repositioned. 

	public static readonly DependencyProperty DisplayModeProperty = DependencyProperty.Register("DisplayMode", typeof(DisplayMode), typeof(PieChartLabel), new FrameworkPropertyMetadata(DisplayMode.ArcMidpoint, FrameworkPropertyMetadataOptions.AffectsArrange));

FrameworkPropertyMetadataOptions.AffectsArrange doesn't exist in Silverlight, but we can work around it by invalidating arrange in the change handler for the DP. Here's the corresponding code in Silverlight:

	public static readonly DependencyProperty DisplayModeProperty = DependencyProperty.Register("DisplayMode", typeof(DisplayMode), typeof(PieChartLabel), new PropertyMetadata(DisplayMode.ArcMidpoint, DisplayModePropertyChanged));
	
	private static void DisplayModePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
	{
		PieChartLabel label = obj as PieChartLabel;
		if (label != null)
		{
			label.InvalidateArrange();
		}            
	}

## FrameworkPropertyMetadataOptions.AffectsParentMeasure doesn't exist in Silverlight

In WPF, I specified that changing the Geometry property of PieChartLabel should affect the parent's measure when I registered the DP:

	public static readonly DependencyProperty GeometryProperty = PieDataPoint.GeometryProperty.AddOwner(typeof(PieChartLabel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, GeometryPropertyChanged));

The equivalent behavior in Silverlight can be done by adding a change handler for this DP that invalidates measure on the element's parent:

	public static readonly DependencyProperty GeometryProperty = DependencyProperty.Register("Geometry", typeof(Geometry), typeof(PieChartLabel), new PropertyMetadata(null, GeometryPropertyChanged));
	
	private static void GeometryPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
	{
		PieChartLabel label = obj as PieChartLabel;
		if (label != null)
		{
			...
			FrameworkElement fe = label.Parent as FrameworkElement;
			if (fe != null)
			{
				fe.InvalidateMeasure();
			}
		}
	}

## Template.FindName doesn't exist in Silverlight

WPF has a useful method that allows us to find an element with a specified name within a template:

	PieChartLabelArea labelArea = chart.Template.FindName("LabelArea_PART", chart) as PieChartLabelArea;

Silverlight doesn't include the FindName method. As a workaround, I wrote a helper method that searches the visual tree until it finds an element with the specified name. The following line of code returns the label area in Silverlight:

	PieChartLabelArea labelArea = TreeHelper.FindDescendent(chart, "LabelArea_PART") as PieChartLabelArea;

## FrameworkElement.Unloaded doesn't exist in Silverlight

In the WPF version of this project, I ensure that the labels are removed when the PieDataPoints are unloaded with the following code:

	pieDataPoint.Unloaded += delegate
	{
		labelArea.Children.Remove(label);
	};

However, in Silverlight, FrameworkElement does not have an Unloaded event. As a workaround, I check whether the PieDataPoint is still in the tree every time layout is updated. When I discover that the data point is no longer in the tree, I remove the corresponding label from the tree.

	pieDataPoint.LayoutUpdated += delegate
	{
		if (!pieDataPoint.IsInTree())
		{
			labelArea.Children.Remove(label);
		}
	};

And here's how I implemented the IsInTree helper method:

	public static bool IsInTree(this FrameworkElement element)
	{
		var rootElement = Application.Current.RootVisual as FrameworkElement;
		while (element != null)
		{
			if (element == rootElement)
			{
				return true;
			}
			element = VisualTreeHelper.GetParent(element) as FrameworkElement;
		}
		return false;
	}

## SnapsToDevicePixels doesn't exist in Silverlight

I set the inherited property SnapsToDevicePixels to true on the label area in WPF to make sure all the labels have crisp borders - I don't want antialiasing to blur any edges that fall between pixel boundaries. There's no SnapsToDevicePixels property in Silverlight, but the workaround is easy: just leave it out! Silverlight introduces a new inherited property, UseLayoutRounding, which is true by default. The difference between the two properties is subtle (UseLayoutRounding affects layout sizes, while SnapsToDevicePixels doesn't), but the effect is the same: both keep single-pixel borders sharp. Note that UseLayoutRounding was recently added to version 4 of WPF, but it's false by default to maintain backward compatibility. 

## {x:Type ...} syntax is not supported in Silverlight

The following syntax is supported only in WPF:

	<Style TargetType="{x:Type local:PieChartLabel}">

The syntax below is equivalent and is supported both in WPF and Silverlight:

	<Style TargetType="local:PieChartLabel">

## Binding to AncestorType is not supported in Silverlight

Below you can see the control and data templates for the PieChartLabel in WPF:

	<Style TargetType="{x:Type local:PieChartLabel}">
		<Setter Property="Template">
			<Setter.Value>
				<ControlTemplate TargetType="{x:Type local:PieChartLabel}">
					<Canvas Name="Canvas_PART">
						<ContentPresenter Name="Content_PART"/>
					</Canvas>
				</ControlTemplate>
			</Setter.Value>
		</Setter>
		<Setter Property="ContentTemplate">
			<Setter.Value>
				<DataTemplate>
					<StackPanel Background="LightGray">
						<TextBlock Text="{Binding RelativeSource={RelativeSource AncestorType={x:Type local:PieChartLabel}}, Path=FormattedRatio}" VerticalAlignment="Center" Margin="5,0,5,0" />
					</StackPanel>
				</DataTemplate>
			</Setter.Value>
		</Setter>
	</Style>

Within the DataTemplate, I use an AncestorType binding to display the FormattedRatio property defined on PieChartLabel. I couldn't use a TemplateBinding because that would refer to properties of the ContentPresenter.

Silverlight doesn't support AncestorType bindings. One possible solution to work around this issue could have been to use a binding to Self with a Converter, and within the converter walk up the visual tree until the PieChartLabel is found. However, the binding is evaluated before the PieChartLabel is added to the tree, so this approach doesn't really help. 

As a workaround, I merged the data template into the control template so that I could use a TemplateBinding to access the FormattedRatio. 

	<Style TargetType="local:PieChartLabel">
		<Setter Property="Template">
			<Setter.Value>
				<ControlTemplate TargetType="local:PieChartLabel">
					<Canvas Name="Canvas_PART">
						<Polyline Name="Polyline_PART" StrokeThickness="{TemplateBinding LineStrokeThickness}" Stroke="{TemplateBinding LineStroke}" StrokeLineJoin="Round" />
						<StackPanel Background="LightGray" Name="Content_PART">
							<TextBlock Text="{TemplateBinding FormattedRatio}" VerticalAlignment="Center" Margin="5,0,5,0" />
						</StackPanel>
					</Canvas>
				</ControlTemplate>
			</Setter.Value>
		</Setter>
	</Style>

## WPF's Polyline rendering bug is not present in Silverlight

To end on a high note, I was able to remove the workaround for the Polyline rendering bug I struggled with in the WPF version of this code. As I explained in my <a href="http://www.zagstudio.com/blog/503">previous post</a>, if I placed a Polyline in the label's template and modified its points whenever the label position changed, WPF would occasionally render the Polyline incorrectly. To work around the issue, I had to create a new Polyline every time the label was repositioned, which is not as efficient.

I was glad to see that this bug is not present in Silverlight. So I added the Polyline to the PieChartLabel's template and simply changed its points in code. You can see the XAML containing the Polyline in the Silverlight Style in the previous section. The code that adds the points to the Polyline instead of creating a new Polyline can be found in the PositionConnected() method of PieChartLabel. This is a straightforward change, so I won't show it here.

