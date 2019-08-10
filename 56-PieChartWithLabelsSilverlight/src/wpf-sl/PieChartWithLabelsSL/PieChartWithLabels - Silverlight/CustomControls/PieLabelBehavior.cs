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
using System.Windows.Data;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Interactivity;

namespace CustomControls
{
    /// <summary>
    /// The intended use of this behavior is to attach LabelStyle DP to a PieDataPoint. LabelStyle
    /// should specify the Style and DataTemplate of each label.
    /// When this DP is set, we walk up the visual tree to find the "LabelArea_PART" canvas and add the label there.
    /// We're using a behavior (instead of deriving from PieDataPoint) because PieDataPoint is sealed at this
    /// point.
    /// </summary>
    public static class PieLabelBehavior
    {
        /// <summary>
        /// IsLabeled - When set to true, attaches a behavior that adds labels to the label area of a pie chart.
        /// </summary>        
        public static bool GetIsLabeled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsLabeledProperty);
        }

        public static void SetIsLabeled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsLabeledProperty, value);
        }

        public static readonly DependencyProperty IsLabeledProperty =
            DependencyProperty.RegisterAttached("IsLabeled", typeof(bool), typeof(PieLabelBehavior), new PropertyMetadata(IsLabeledPropertyChanged));

        private static void IsLabeledPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            bool isLabeled = (bool)e.NewValue;
            if (isLabeled == true)
            {
                PieDataPoint pieDataPoint = obj as PieDataPoint;
                if (pieDataPoint != null)
                {
                    Chart chart = TreeHelper.FindAncestor<Chart>(pieDataPoint.Parent as DependencyObject);
                    if (chart != null)
                    {
                        //Panel labelArea = chart.Template.FindName("LabelArea_PART", chart) as Panel;
                        Panel labelArea = TreeHelper.FindDescendent(chart, "LabelArea_PART") as Panel;
                        if (labelArea != null)
                        {
                            AddLabel(pieDataPoint, labelArea);
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
        private static void AddLabel(PieDataPoint pieDataPoint, Panel labelArea)
        {
            PieChartLabel label = new PieChartLabel();

            Style pieChartLabelStyle;
            DataTemplate pieChartLabelItemTemplate;
            LabeledPieChart chart = TreeHelper.FindAncestor<LabeledPieChart>(pieDataPoint);
            if (chart != null)
            {
                pieChartLabelStyle = chart.PieChartLabelStyle;
                if (pieChartLabelStyle != null)
                {
                    label.Style = pieChartLabelStyle;
                }
                pieChartLabelItemTemplate = chart.PieChartLabelItemTemplate;
                if (pieChartLabelItemTemplate != null)
                {
                    label.ContentTemplate = pieChartLabelItemTemplate;
                }
            }

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

            Binding displayModeBinding = new Binding("LabelDisplayMode") { Source = chart };
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

            //pieDataPoint.Unloaded += delegate
            //{
            //    labelArea.Children.Remove(label);
            //};

        }
    }
}
