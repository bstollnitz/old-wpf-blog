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

namespace CustomControls
{
	[TemplatePart(Name = "LabelArea_PART", Type = typeof(PieChartLabelArea))]
	public class LabeledPieChart : Chart
	{
		public LabeledPieChart()
		{
			this.DefaultStyleKey = typeof(LabeledPieChart);
		}
	}
}
