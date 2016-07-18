﻿//The MIT License(MIT)

//copyright(c) 2016 Alberto Rodriguez

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
//SOFTWARE.5

using System.Collections.Generic;
using System.Linq;
using LiveCharts.Defaults;
using LiveCharts.Definitions.Points;
using LiveCharts.Definitions.Series;
using LiveCharts.Dtos;
using LiveCharts.Helpers;

namespace LiveCharts.SeriesAlgorithms
{
    public class HeatAlgorithm : SeriesAlgorithm, ICartesianSeries
    {
        public HeatAlgorithm(ISeriesView view) : base(view)
        {
            PreferredSelectionMode = TooltipSelectionMode.OnlySender;
        }

        public override void Update()
        {
            var heatSeries = (IHeatSeriesView)View;

            var uw = new CorePoint(
                0 * ChartFunctions.GetUnitWidth(AxisOrientation.X, Chart, View.ScalesXAt) / 2,
                -ChartFunctions.GetUnitWidth(AxisOrientation.Y, Chart, View.ScalesYAt));

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            var wd = CurrentXAxis.MaxLimit - CurrentXAxis.MinLimit == 0
                ? double.MaxValue
                : CurrentXAxis.MaxLimit - CurrentXAxis.MinLimit;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            var hd = CurrentYAxis.MaxLimit - CurrentYAxis.MinLimit == 0
                ? double.MaxValue
                : CurrentYAxis.MaxLimit - CurrentYAxis.MinLimit;
            var w = Chart.DrawMargin.Width / wd;
            var h = Chart.DrawMargin.Height / hd;

            //lets force the gradients to always have an 'interpol-able' model

            if (!heatSeries.Stops.Any())
                throw new LiveChartsException("There is no a valid gradient to create a heat series.");

            var correctedGradients = heatSeries.Stops.Select(x => new CoreGradientStop
            {
                Color = x.Color,
                Offset = x.Offset < 0 ? 0 : (x.Offset > 1 ? 1 : x.Offset)
            }).ToList();
            var min = correctedGradients[0];
            min.Offset = 0;
            correctedGradients.Insert(0, min);
            var max = correctedGradients[correctedGradients.Count - 1];
            max.Offset = 1;
            correctedGradients.Add(max);

            foreach (var chartPoint in View.ActualValues.Points)
            {
                chartPoint.ChartLocation = ChartFunctions.ToDrawMargin(
                    chartPoint, View.ScalesXAt, View.ScalesYAt, Chart) + uw;

                chartPoint.SeriesView = View;

                chartPoint.View = View.GetPointView(chartPoint.View, chartPoint,
                    View.DataLabels ? View.GetLabelPointFormatter()(chartPoint) : null);

                var heatView = (IHeatPointView)chartPoint.View;

                heatView.ColorComponents = ColorInterpolation(correctedGradients,
                    chartPoint.Weight / Chart.Value3CoreLimit.Max);

                heatView.Width = w;
                heatView.Height = h;

                chartPoint.View.DrawOrMove(null, chartPoint, 0, Chart);
            }
        }

        public double GetMinX(AxisCore axis)
        {
            return AxisLimits.StretchMin(axis);
        }

        public double GetMaxX(AxisCore axis)
        {
            return AxisLimits.StretchMax(axis) + 1;
        }

        public double GetMinY(AxisCore axis)
        {
            return AxisLimits.StretchMin(axis);
        }

        public double GetMaxY(AxisCore axis)
        {
            return AxisLimits.StretchMax(axis) + 1;
        }

        private static CoreColor ColorInterpolation(IList<CoreGradientStop> gradients, double weight)
        {
            CoreColor from = new CoreColor(0, 0, 0, 0), to = new CoreColor(0, 0, 0, 0);
            double fromOffset = 0, toOffset = 0;

            for (var i = 0; i < gradients.Count; i++)
            {
                // ReSharper disable once InvertIf
                if (gradients[i].Offset <= weight && gradients[i + 1].Offset >= weight)
                {
                    from = gradients[i].Color;
                    to = gradients[i + 1].Color;

                    fromOffset = gradients[i].Offset;
                    toOffset = gradients[i + 1].Offset;

                    break;
                }
            }

            return new CoreColor(
                InterpolateColorComponent(from.A, to.A, fromOffset, toOffset, weight),
                InterpolateColorComponent(from.R, from.R, fromOffset, toOffset, weight),
                InterpolateColorComponent(from.G, to.G, fromOffset, toOffset, weight),
                InterpolateColorComponent(from.B, to.B, fromOffset, toOffset, weight));
        }

        private static byte InterpolateColorComponent(byte fromComponent, byte toComponent,
            double fromOffset, double toOffset, double value)
        {
            var p1 = new CorePoint(fromOffset, fromComponent);
            var p2 = new CorePoint(toOffset, toComponent);

            var deltaX = p2.X - p1.X;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            var m = (p2.Y - p1.Y) / (deltaX == 0 ? double.MinValue : deltaX);

            return (byte)(m * (value - p1.X) + p1.Y);
        }
    }
}
