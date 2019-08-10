using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using System.Windows.Controls.DataVisualization.Charting;

namespace CustomControls
{
    /// <summary>
    /// A PieChart that is capable of displaying labels for its wedges.
    /// It derives from Control (instead of Chart) because Chart is sealed. Instead, it contains a Chart
    /// in its template.
    /// </summary>
    [TemplatePart(Name = "Chart_PART", Type = typeof(Chart))]
    [TemplatePart(Name = "PieSeries_PART", Type = typeof(DataPointSeries))]
    public class LabeledPieChart : Control
    {
        private DataPointSeries series;

        public event SelectionChangedEventHandler SelectionChanged;

        static LabeledPieChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LabeledPieChart), new FrameworkPropertyMetadata(typeof(LabeledPieChart)));
        }

        /// <summary>
        /// Title DP - Chart's Title is template bound to this property.
        /// </summary>
        public object Title
        {
            get { return (object)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = Chart.TitleProperty.AddOwner(typeof(LabeledPieChart), null);

        /// <summary>
        /// ItemsSource DP - PieSeries's ItemsSource is template bound to this property.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = PieSeries.ItemsSourceProperty.AddOwner(typeof(LabeledPieChart), null);

        /// <summary>
        /// IsSelectionEnabled DP - PieSerie's IsSelectionEnabled is template bound to this property.
        /// </summary>
        public bool IsSelectionEnabled
        {
            get { return (bool)this.GetValue(IsSelectionEnabledProperty); }
            set { this.SetValue(IsSelectionEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsSelectionEnabledProperty = PieSeries.IsSelectionEnabledProperty.AddOwner(typeof(LabeledPieChart), null);

        /// <summary>
        /// SelectedItem DP - PieSerie's SelectedItem is two-way data bound to this property.
        /// </summary>
        public object SelectedItem
        {
            get { return (object)this.GetValue(SelectedItemProperty); }
            set { this.SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty = PieSeries.SelectedItemProperty.AddOwner(typeof(LabeledPieChart), null);

        /// <summary>
        /// IndependentValuePath DP - PieSeries's IndependentValuePath is not template bound to this property because PieSeries's
        /// IndependentValuePath is not a DP. Instead, we propagate any changes made to this property to the corresponding
        /// property in PieSeries.
        /// </summary>
        public string IndependentValuePath
        {
            get { return (string)this.GetValue(IndependentValuePathProperty); }
            set { this.SetValue(IndependentValuePathProperty, value); }
        }

        public static readonly DependencyProperty IndependentValuePathProperty =
            DependencyProperty.Register("IndependentValuePath", typeof(string), typeof(LabeledPieChart), new PropertyMetadata(IndependentValuePathPropertyChanged));

        private static void IndependentValuePathPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((LabeledPieChart)obj).UpdateValuePaths();
        }

        /// <summary>
        /// DependentValuePath DP - PieSeries's DependentValuePath is not template bound to this propety because PieSeries's
        /// DependentValuePath is not a DP. Instead, we propagate any changes made to this property to the corresponding
        /// property in PieSeries.
        /// </summary>
        public string DependentValuePath
        {
            get { return (string)this.GetValue(DependentValuePathProperty); }
            set { this.SetValue(DependentValuePathProperty, value); }
        }

        public static readonly DependencyProperty DependentValuePathProperty =
            DependencyProperty.Register("DependentValuePath", typeof(string), typeof(LabeledPieChart), new PropertyMetadata(DependentValuePathPropertyChanged));

        private static void DependentValuePathPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((LabeledPieChart)obj).UpdateValuePaths();
        }

        /// <summary>
        /// PieChartLabelStyle DP - The style to be applied to each PieChartLabel.
        /// </summary>
        public Style PieChartLabelStyle
        {
            get { return (Style)this.GetValue(PieChartLabelStyleProperty); }
            set { this.SetValue(PieChartLabelStyleProperty, value); }
        }

        public static readonly DependencyProperty PieChartLabelStyleProperty =
            DependencyProperty.Register("PieChartLabelStyle", typeof(Style), typeof(LabeledPieChart));

        /// <summary>
        /// PieChartLabelItemTemplate DP - The DataTemplate to be applied to each PieChartLabel.
        /// </summary>
        public DataTemplate PieChartLabelItemTemplate
        {
            get { return (DataTemplate)this.GetValue(PieChartLabelItemTemplateProperty); }
            set { this.SetValue(PieChartLabelItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty PieChartLabelItemTemplateProperty =
            DependencyProperty.Register("PieChartLabelItemTemplate", typeof(DataTemplate), typeof(LabeledPieChart));

        /// <summary>
        /// LabelDisplayMode DP - Controls the mode in which the labels should appear.
        /// </summary>
        public DisplayMode LabelDisplayMode
        {
            get { return (DisplayMode)this.GetValue(LabelDisplayModeProperty); }
            set { this.SetValue(LabelDisplayModeProperty, value); }
        }

        public static readonly DependencyProperty LabelDisplayModeProperty =
            DependencyProperty.Register("LabelDisplayMode", typeof(DisplayMode), typeof(LabeledPieChart), new PropertyMetadata(DisplayMode.AutoMixed));

        /// <summary>
        /// When template is applied, the valuse of IndependentValuePath and DependentValuePath are propagated to the 
        /// corresponding properties in PieSeries.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.UpdateValuePaths();

            series = this.GetTemplateChild("PieSeries_PART") as DataPointSeries;
            if (series != null)
            {
                series.SelectionChanged += new SelectionChangedEventHandler(series_SelectionChanged);
            }
        }

        /// <summary>
        /// When the PieSeries' SelectionChanged event is fired, the LabeledPieChart's SelectionChanged
        /// event is also fired.
        /// </summary>
        private void series_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, e);
            }
        }

        /// <summary>
        /// Propagates the values of IndependentValuePath and DependentValuePath to the corresponding properties
        /// in PieSeries.
        /// </summary>
        private void UpdateValuePaths()
        {
            PieSeries pieSeries = this.GetTemplateChild("PieSeries_PART") as PieSeries;
            if (pieSeries != null)
            {
                pieSeries.IndependentValuePath = this.IndependentValuePath;
                pieSeries.DependentValuePath = this.DependentValuePath;
            }
        }
    }
}
