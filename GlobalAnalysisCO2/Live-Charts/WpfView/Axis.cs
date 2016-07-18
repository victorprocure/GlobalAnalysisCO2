﻿//copyright(c) 2016 Alberto Rodriguez

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using LiveCharts.Charts;
using LiveCharts.Definitions.Charts;
using LiveCharts.Dtos;
using LiveCharts.Wpf.Components;
using LiveCharts.Wpf.Converters;

namespace LiveCharts.Wpf
{
    /// <summary>
    /// An Axis of a chart
    /// </summary>
    public class Axis : FrameworkElement, IAxisView
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of Axis class
        /// </summary>
        public Axis()
        {
            TitleBlock = BindATextBlock();
            SetCurrentValue(SeparatorProperty, new Separator());
            SetCurrentValue(ShowLabelsProperty, true);
            SetCurrentValue(SectionsProperty, new SectionsCollection());

            TitleBlock.SetBinding(TextBlock.TextProperty,
                new Binding {Path = new PropertyPath(TitleProperty), Source = this});
        }
        #endregion

        #region properties

        private TextBlock TitleBlock { get; set; }

        /// <summary>
        /// Gets the Model of the axis, the model is used a DTO to communicate with the core of the library.
        /// </summary>
        public AxisCore Model { get; set; }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty LabelsProperty = DependencyProperty.Register(
            "Labels", typeof (IList<string>), typeof (Axis), 
            new PropertyMetadata(default(IList<string>), UpdateChart()));
        /// <summary>
        /// Gets or sets axis labels, labels property stores the array to map for each index and value, for example if axis value is 0 then label will be labels[0], when value 1 then labels[1], value 2 then labels[2], ..., value n labels[n], use this property instead of a formatter when there is no conversion between value and label for example names, if you are plotting sales vs salesman name.
        /// </summary>
        [TypeConverter(typeof(StringCollectionConverter))]
        public IList<string> Labels
        {
            get { return (IList<string>) GetValue(LabelsProperty); }
            set { SetValue(LabelsProperty, value); }
        }

        public static readonly DependencyProperty SectionsProperty = DependencyProperty.Register(
            "Sections", typeof (SectionsCollection), typeof (Axis), new PropertyMetadata(default(SectionsCollection)));
        /// <summary>
        /// Gets or sets the axis sectionsCollection, a section is useful to highlight ranges or values in a chart.
        /// </summary>
        public SectionsCollection Sections
        {
            get { return (SectionsCollection) GetValue(SectionsProperty); }
            set { SetValue(SectionsProperty, value); }
        }

        public static readonly DependencyProperty LabelFormatterProperty = DependencyProperty.Register(
            "LabelFormatter", typeof (Func<double, string>), typeof (Axis),
            new PropertyMetadata(default(Func<double, string>), UpdateChart()));
        /// <summary>
        /// Gets or sets the function to convert a value to label, for example when you need to display your chart as currency ($1.00) or as degrees (10°), if Labels property is not null then formatter is ignored, and label will be pulled from Labels prop.
        /// </summary>
        public Func<double, string> LabelFormatter
        {
            get { return (Func<double, string>) GetValue(LabelFormatterProperty); }
            set { SetValue(LabelFormatterProperty, value); }
        }

        public static readonly DependencyProperty SeparatorProperty = DependencyProperty.Register(
            "Separator", typeof (ISeparatorView), typeof (Axis),
            new PropertyMetadata(default(ISeparatorView), UpdateChart()));
        /// <summary>
        /// Get or sets configuration for parallel lines to axis.
        /// </summary>
        public ISeparatorView Separator
        {
            get { return (ISeparatorView) GetValue(SeparatorProperty); }
            set { SetValue(SeparatorProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
            "Stroke", typeof (Brush), typeof (Axis), new PropertyMetadata(default(Brush)));
        /// <summary>
        /// Gets or sets axis color, axis means only the zero value, if you need to highlight where zero is. to change separators color, see Axis.Separator
        /// </summary>
        public Brush Stroke
        {
            get { return (Brush) GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            "StrokeThickness", typeof (double), typeof (Axis), 
            new PropertyMetadata(default(double), UpdateChart()));
        /// <summary>
        /// Gets or sets axis thickness.
        /// </summary>
        public double StrokeThickness
        {
            get { return (double) GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty ShowLabelsProperty = DependencyProperty.Register(
            "ShowLabels", typeof (bool), typeof (Axis), 
            new PropertyMetadata(default(bool), LabelsVisibilityChanged));

        /// <summary>
        /// Gets or sets if labels are shown in the axis.
        /// </summary>
        public bool ShowLabels
        {
            get { return (bool) GetValue(ShowLabelsProperty); }
            set { SetValue(ShowLabelsProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue", typeof (double?), typeof (Axis), 
            new PropertyMetadata(default(double?), UpdateChart()));
        /// <summary>
        /// Gets or sets axis max value, set it to null to make this property Auto, default value is null
        /// </summary>
        public double? MaxValue
        {
            get { return (double?) GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            "MinValue", typeof (double?), typeof (Axis),
            new PropertyMetadata(null, UpdateChart()));
        /// <summary>
        /// Gets or sets axis min value, set it to null to make this property Auto, default value is null
        /// </summary>
        public double? MinValue
        {
            get { return (double?)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(Axis), 
            new PropertyMetadata(null, UpdateChart()));
        /// <summary>
        /// Gets or sets axis title, the title will be displayed only if this property is not null, default is null.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            "Position", typeof (AxisPosition), typeof (Axis), 
            new PropertyMetadata(default(AxisPosition), UpdateChart()));
        /// <summary>
        /// Gets or sets the axis position, default is Axis.Position.LeftBottom, when the axis is at Y and Position is LeftBottom, then axis will be placed at left, RightTop position will place it at Right, when the axis is at X and position LeftBottom, the axis will be placed at bottom, if position is RightTop then it will be placed at top.
        /// </summary>
        public AxisPosition Position
        {
            get { return (AxisPosition) GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }


        public static readonly DependencyProperty IsMergedProperty = DependencyProperty.Register(
            "IsMerged", typeof (bool), typeof (Axis), 
            new PropertyMetadata(default(bool), UpdateChart()));
        /// <summary>
        /// Gets or sets if the axis labels should me placed inside the chart, this is useful to save some space.
        /// </summary>
        public bool IsMerged
        {
            get { return (bool) GetValue(IsMergedProperty); }
            set { SetValue(IsMergedProperty, value); }
        }


        public static readonly DependencyProperty DisableAnimationsProperty = DependencyProperty.Register(
            "DisableAnimations", typeof (bool), typeof (Axis), new PropertyMetadata(default(bool), UpdateChart(true)));
        /// <summary>
        /// Gets or sets if the axis is animated.
        /// </summary>
        public bool DisableAnimations
        {
            get { return (bool) GetValue(DisableAnimationsProperty); }
            set { SetValue(DisableAnimationsProperty, value); }
        }

        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(Axis),
                new PropertyMetadata(new FontFamily("Calibri")));

        /// <summary>
        /// Gets or sets labels font family, font to use for any label in this axis
        /// </summary>
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(Axis), new PropertyMetadata(11.0));
        /// <summary>
        /// Gets or sets labels font size
        /// </summary>
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static readonly DependencyProperty FontWeightProperty =
            DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(Axis),
                new PropertyMetadata(FontWeights.Normal));
        /// <summary>
        /// Gets or sets labels font weight
        /// </summary>
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        public static readonly DependencyProperty FontStyleProperty =
            DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(Axis),
                new PropertyMetadata(FontStyles.Normal));

        /// <summary>
        /// Gets or sets labels font style
        /// </summary>
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        public static readonly DependencyProperty FontStretchProperty =
            DependencyProperty.Register("FontStretch", typeof(FontStretch), typeof(Axis),
                new PropertyMetadata(FontStretches.Normal));

        /// <summary>
        /// Gets or sets labels font stretch
        /// </summary>
        public FontStretch FontStretch
        {
            get { return (FontStretch)GetValue(FontStretchProperty); }
            set { SetValue(FontStretchProperty, value); }
        }

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(Axis),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(150, 150, 150))));

        /// <summary>
        /// Gets or sets labels text color.
        /// </summary>
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public static readonly DependencyProperty LabelsRotationProperty = DependencyProperty.Register(
            "LabelsRotation", typeof (double), typeof (Axis), new PropertyMetadata(default(double), UpdateChart()));
        /// <summary>
        /// Gets or sets the labels rotation in the axis, the angle starts as a horizontal line, you can use any angle in degrees, even negatives.
        /// </summary>
        public double LabelsRotation
        {
            get { return (double) GetValue(LabelsRotationProperty); }
            set { SetValue(LabelsRotationProperty, value); }
        }

        #endregion

        #region Public Methods
        public void Clean()
        {
            Model.ClearSeparators();
            Model.Chart.View.RemoveFromView(TitleBlock);
            Sections.Clear();
            TitleBlock = null;
        }

        public void RenderSeparator(SeparatorElementCore model, ChartCore chart)
        {
            AxisSeparatorElement ase;

            if (model.View == null)
            {
                ase = new AxisSeparatorElement(model)
                {
                    Line = BindALine(),
                    TextBlock = BindATextBlock()
                };

                model.View = ase;
                chart.View.AddToView(ase.Line);
                chart.View.AddToView(ase.TextBlock);
                Panel.SetZIndex(ase.Line, -1);
            }
            else
            {
                ase = (AxisSeparatorElement) model.View;
            }

            if (!Separator.IsEnabled)
                ase.Line.Visibility = Visibility.Collapsed;
            if (!ShowLabels)
                ase.TextBlock.Visibility = Visibility.Collapsed;
        }

        public CoreSize UpdateTitle(ChartCore chart, double rotationAngle = 0)
        {
            if (TitleBlock.Parent == null)
            {
                if (Math.Abs(rotationAngle) > 1)
                    TitleBlock.RenderTransform = new RotateTransform(rotationAngle);

                chart.View.AddToView(TitleBlock);
            }

            TitleBlock.UpdateLayout();
            return string.IsNullOrWhiteSpace(Title)
                ? new CoreSize()
                : new CoreSize(TitleBlock.RenderSize.Width, TitleBlock.RenderSize.Height);
        }

        public void SetTitleTop(double value)
        {
            Canvas.SetTop(TitleBlock, value);
        }

        public void SetTitleLeft(double value)
        {
            Canvas.SetLeft(TitleBlock, value);
        }

        public double GetTitleLeft()
        {
            return Canvas.GetLeft(TitleBlock);
        }

        public double GetTileTop()
        {
            return Canvas.GetTop(TitleBlock);
        }

        public CoreSize GetLabelSize()
        {
            return new CoreSize(TitleBlock.RenderSize.Width, TitleBlock.RenderSize.Height);
        }

        public AxisCore AsCoreElement(ChartCore chart, AxisOrientation source)
        {
            if (Model == null) Model = new AxisCore(this);

            Model.ShowLabels = ShowLabels;
            Model.Chart = chart;
            Model.IsMerged = IsMerged;
            Model.Labels = Labels;
            Model.LabelFormatter = LabelFormatter;
            Model.MaxValue = MaxValue;
            Model.MinValue = MinValue;
            Model.Title = Title;
            Model.Position = Position;
            Model.Separator = Separator.AsCoreElement(Model);
            Model.DisableAnimations = DisableAnimations;
            Model.Sections = Sections.Select(x => x.AsCoreElement(Model, source)).ToList();

            return Model;
        }

        #endregion

        internal TextBlock BindATextBlock()
        {
            var tb = new TextBlock();

            tb.SetBinding(TextBlock.FontFamilyProperty,
                new Binding { Path = new PropertyPath(FontFamilyProperty), Source = this });
            tb.SetBinding(FontSizeProperty,
                new Binding { Path = new PropertyPath(FontSizeProperty), Source = this });
            tb.SetBinding(TextBlock.FontStretchProperty,
                new Binding { Path = new PropertyPath(FontStretchProperty), Source = this });
            tb.SetBinding(TextBlock.FontStyleProperty,
                new Binding { Path = new PropertyPath(FontStyleProperty), Source = this });
            tb.SetBinding(TextBlock.FontWeightProperty,
                new Binding { Path = new PropertyPath(FontWeightProperty), Source = this });
            tb.SetBinding(TextBlock.ForegroundProperty,
                 new Binding { Path = new PropertyPath(ForegroundProperty), Source = this });

            return tb;
        }

        internal Line BindALine()
        {
            var l = new Line();

            var s = Separator as Separator;
            if (s == null) return l;

            l.SetBinding(Shape.StrokeProperty,
                new Binding {Path = new PropertyPath(Wpf.Separator.StrokeProperty), Source = s});
            l.SetBinding(Shape.StrokeDashArrayProperty,
                new Binding {Path = new PropertyPath(Wpf.Separator.StrokeDashArrayProperty), Source = s});
            l.SetBinding(Shape.StrokeThicknessProperty,
                new Binding {Path = new PropertyPath(Wpf.Separator.StrokeThicknessProperty), Source = s});
            l.SetBinding(VisibilityProperty,
                new Binding {Path = new PropertyPath(VisibilityProperty), Source = s});

            return l;
        }

        private static PropertyChangedCallback UpdateChart(bool animate = false)
        {
            return (o, args) =>
            {
                var wpfAxis = o as Axis;
                if (wpfAxis == null) return;

                if (wpfAxis.Model != null)
                    wpfAxis.Model.Chart.Updater.Run(animate);
            };
        }

        private static void LabelsVisibilityChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var axis = (Axis) dependencyObject;
            if (axis.Model == null) return;
            
            foreach (var separator in axis.Model.CurrentSeparators)
            {
                var s = (AxisSeparatorElement) separator.View;
                s.TextBlock.Visibility = axis.ShowLabels
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }

            UpdateChart()(dependencyObject, dependencyPropertyChangedEventArgs);
        }
    }
}