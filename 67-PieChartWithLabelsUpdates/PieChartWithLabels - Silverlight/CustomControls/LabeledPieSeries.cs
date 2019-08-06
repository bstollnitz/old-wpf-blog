using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;

namespace CustomControls
{
	public class LabeledPieSeries : PieSeries
	{
		/// <summary>
		/// PieChartLabelStyle DP - The style to be applied to each PieChartLabel.
		/// </summary>
		public Style PieChartLabelStyle
		{
			get { return (Style)this.GetValue(PieChartLabelStyleProperty); }
			set { this.SetValue(PieChartLabelStyleProperty, value); }
		}

		public static readonly DependencyProperty PieChartLabelStyleProperty =
			DependencyProperty.Register("PieChartLabelStyle", typeof(Style), typeof(LabeledPieSeries), new PropertyMetadata(null));

		/// <summary>
		/// LabelDisplayMode DP - Controls the mode in which the labels should appear.
		/// </summary>
		public DisplayMode LabelDisplayMode
		{
			get { return (DisplayMode)this.GetValue(LabelDisplayModeProperty); }
			set { this.SetValue(LabelDisplayModeProperty, value); }
		}

		public static readonly DependencyProperty LabelDisplayModeProperty =
			DependencyProperty.Register("LabelDisplayMode", typeof(DisplayMode), typeof(LabeledPieSeries), new PropertyMetadata(DisplayMode.AutoMixed));

		/// <summary>
		/// Adds the labels to each of its PieDataPoints.
		/// </summary>
		protected override void OnAfterUpdateDataPoints()
		{
			LabeledPieChart chart = this.SeriesHost as LabeledPieChart;
			if (chart != null)
			{
				Panel labelArea = TreeHelper.FindDescendent(chart, "LabelArea_PART") as Panel;
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

		/// <summary>
		/// Creates a new PieChartLabel control and adds it to the "LabelArea_PART" canvas.
		/// </summary>
		/// <param name="pieDataPoint">PieDataPoint that corresponds to PieChartLabel.</param>
		/// <param name="labelStyle">The style to be applied to the PieChartLabel.</param>
		/// <param name="labelArea">The canvas where PieChartLabel will be added.</param>
		private void AddLabelPieDataPoint(PieDataPoint pieDataPoint, Panel labelArea)
		{
			PieChartLabel label = new PieChartLabel();
			label.Style = this.PieChartLabelStyle;

			Binding contentBinding = new Binding("DataContext") { Source = pieDataPoint };
			label.SetBinding(ContentControl.ContentProperty, contentBinding);

			Binding dataContextBinding = new Binding("DataContext") { Source = pieDataPoint };
			label.SetBinding(ContentControl.DataContextProperty, contentBinding);

			Binding formattedRatioBinding = new Binding("FormattedRatio") { Source = pieDataPoint };
			label.SetBinding(PieChartLabel.FormattedRatioProperty, formattedRatioBinding);

			Binding visibilityBinding = new Binding("Ratio") { Source = pieDataPoint, Converter = new DoubleToVisibilityConverter() };
			label.SetBinding(PieChartLabel.VisibilityProperty, visibilityBinding);

			Binding geometryBinding = new Binding("Geometry") { Source = pieDataPoint };
			label.SetBinding(PieChartLabel.GeometryProperty, geometryBinding);

			Binding displayModeBinding = new Binding("LabelDisplayMode") { Source = this };
			label.SetBinding(PieChartLabel.DisplayModeProperty, displayModeBinding);

			labelArea.Children.Add(label);

			pieDataPoint.Loaded += delegate
			{
				if (label.Parent == null)
				{
					labelArea.Children.Add(label);
				}
			};

			pieDataPoint.LayoutUpdated += delegate
			{
				if (!pieDataPoint.IsInTree())
				{
					labelArea.Children.Remove(label);
				}
			};
		}
	}
}
