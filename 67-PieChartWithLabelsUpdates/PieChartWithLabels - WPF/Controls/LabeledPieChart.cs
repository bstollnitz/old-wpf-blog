using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows;

namespace Controls
{
	[TemplatePart(Name = "LabelArea_PART", Type = typeof(PieChartLabelArea))]
	public class LabeledPieChart : Chart
	{
		static LabeledPieChart()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(LabeledPieChart), new FrameworkPropertyMetadata(typeof(LabeledPieChart)));
		}
	}
}
