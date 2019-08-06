# How to update LabeledPieChart to use the latest toolkit

A few months ago I blogged about a behavior that adds labels to a pie chart in Silverlight or WPF. I wrote a <a href="http://www.zagstudio.com/blog/502">post showing the usage </a> of that behavior, another one explaining the <a href="http://www.zagstudio.com/blog/503">implementation details of the WPF version</a>, and another one explaining <a href="http://www.zagstudio.com/blog/504">issues encountered when porting it to Silverlight</a>. Since those posts were written, a new version of the charting APIs came out with improvements that help simplify the labeled chart code. By popular request, this blog post explains how I updated the WPF and Silverlight pie chart label behavior to take advantage of the latest <a href="http://silverlight.codeplex.com/releases/view/36060">Silverlight Toolkit</a> and <a href="http://wpf.codeplex.com/releases/view/40535">WPF Toolkit</a>.

## The Chart class is no longer sealed any more

When the pie chart label behavior was originally written, the Chart class was sealed. As a workaround, I created a LabeledPieChart custom control that derived from Control and contained a Chart with a PieSeries in its template. This inner Chart also included in its template an extra panel where the labels were added. The big drawback of this approach was the fact that I had to re-expose all the interesting properties of Chart and PieSeries in the custom control, and make sure that the new properties were updated when the original ones changed. 

Now that Chart is no longer sealed, I am able to implement LabeledPieChart by deriving from Chart directly. This class is greatly simplified, since I no longer need to re-expose any properties. The only reason it exists is so I can give it a template containing the panel where the labels will be added.

	[TemplatePart(Name = "LabelArea_PART", Type = typeof(PieChartLabelArea))]
	public class LabeledPieChart : Chart
	{
		static LabeledPieChart()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(LabeledPieChart), new FrameworkPropertyMetadata(typeof(LabeledPieChart)));
		}
	}

## The PieSeries class is no longer sealed

In my earlier solution, the code that added the labels to the panel was implemented using a behavior - PieLabelBehavior - which was attached to each PieDataPoint. The XAML that attaches the behavior to its target was found in the LabeledPieDataPointStyle, in generic.xaml.

Now that PieSeries is no longer sealed, there is no need to use a behavior any more. Instead, the code that adds the labels is now located in a new class (LabeledPieSeries) that derives from PieSeries. The method below shows the logic that adds labels corresponding to each data point.

	protected override void OnAfterUpdateDataPoints()
	{
		LabeledPieChart chart = this.SeriesHost as LabeledPieChart;
		if (chart != null)
		{
			Canvas labelArea = chart.Template.FindName("LabelArea_PART", chart) as Canvas;
			if (labelArea != null && this.ItemsSource != null)
			{
				foreach (object dataItem in this.ItemsSource)
				{
					PieDataPoint pieDataPoint = this.GetDataPoint(dataItem) as PieDataPoint;
					if (pieDataPoint != null)
					{
						this.AddLabelPieDataPoint(pieDataPoint, labelArea);
					}
				}
			}
		}
	}

(The code for AddLabelPieDataPoint hasn't changed from my earlier code, so I don't show it here.)

In addition, PieSeries contains three properties that were previously in the LabeledPieChart custom control: PieChartLabelStyle, PieChartLabelItemTemplate and LabelDisplayMode. These are the only new properties needed by the code that adds labels. Since the ItemsSource property is in PieSeries, it makes sense to put these other properties in PieSeries, too.

The fact that I no longer need to attach a dependency property to each data point allowed me to remove the corresponding XAML in generic.xaml.

## Usage

Now that we have a LabeledPieChart class that derives from Chart and a LabeledPieSeries class that derives from PieSeries, the usage of the control has changed a bit. Here's some of the XAML from the project that I link to at the end of this post:

	<customControls:LabeledPieChart 
		x:Name="labeledPieChart"
		Title="Population of Puget Sound Cities"
		Height="500" Width="700"
		Grid.Row="3"
		BorderBrush="Gray"
		>			
		<customControls:LabeledPieChart.Series>
			<customControls:LabeledPieSeries 
				x:Name="labeledPieSeries"
				ItemsSource="{Binding}" 
				IndependentValuePath="Name" 
				DependentValuePath="Population" 
				IsSelectionEnabled="True" 
				PieChartLabelStyle="{StaticResource pieChartLabelStyle}"
				PieChartLabelItemTemplate="{StaticResource pieChartLabelDataTemplate}"
				LabelDisplayMode="Auto"
				/>
		</customControls:LabeledPieChart.Series>
	</customControls:LabeledPieChart>

As you can see, instead of setting a few attached properties (to add the behavior), now you use the LabeledPieChart and LabeledPieSeries classes directly. There is one big advantage to this technique: now you can use all properties available in Chart and PieSeries to customize your chart (not just the ones that were re-exposed in the LabeledPieChart in my previous implementation).

The fact that Chart and PieSeries are no longer sealed allows me to greatly simplify the source code (and XAML) of the labeled pie chart. In addition, it provides more flexibility for the developer to customize the chart and series.

