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
//SOFTWARE.

using System;
using System.Linq;
using LiveCharts.Defaults;
using LiveCharts.Definitions.Points;
using LiveCharts.Definitions.Series;
using LiveCharts.Dtos;

namespace LiveCharts.SeriesAlgorithms
{
    public class RowAlgorithm : SeriesAlgorithm, ICartesianSeries
    {
        public RowAlgorithm(ISeriesView view) : base(view)
        {
            SeriesOrientation = SeriesOrientation.Vertical;
            PreferredSelectionMode = TooltipSelectionMode.SharedYInSeries;
        }

        public override void Update()
        {
            var castedSeries = (IRowSeriesView) View;

            var padding = castedSeries.RowPadding;
            
            var totalSpace = ChartFunctions.GetUnitWidth(AxisOrientation.Y, Chart, View.ScalesYAt) - padding;
            var typeSeries = Chart.View.ActualSeries.OfType<IRowSeriesView>().ToList();

            var singleRowHeight = totalSpace/typeSeries.Count;

            double exceed = 0;

            var seriesPosition = typeSeries.IndexOf((IRowSeriesView) View);

            if (singleRowHeight > castedSeries.MaxRowHeigth)
            {
                exceed = (singleRowHeight - castedSeries.MaxRowHeigth)*typeSeries.Count/2;
                singleRowHeight = castedSeries.MaxRowHeigth;
            }

            var relativeTop = padding + exceed + singleRowHeight * (seriesPosition);

            var startAt = CurrentXAxis.MinLimit >= 0 && CurrentXAxis.MaxLimit > 0   //both positive
                ? CurrentXAxis.MinLimit                                             //then use Min
                : (CurrentXAxis.MinLimit < 0 && CurrentXAxis.MaxLimit <= 0          //both negative
                    ? CurrentXAxis.MaxLimit                                         //then use Max
                    : 0);                                                           //if mixed then use 0

            var zero = ChartFunctions.ToDrawMargin(startAt, AxisOrientation.X, Chart, View.ScalesXAt);

            var correction = ChartFunctions.GetUnitWidth(AxisOrientation.Y, Chart, View.ScalesYAt);

            foreach (var chartPoint in View.ActualValues.Points)
            {
                var reference =
                    ChartFunctions.ToDrawMargin(chartPoint, View.ScalesXAt, View.ScalesYAt, Chart);

                chartPoint.View = View.GetPointView(chartPoint.View, chartPoint,
                    View.DataLabels ? View.GetLabelPointFormatter()(chartPoint) : null);

                chartPoint.SeriesView = View;

                var rectangleView = (IRectanglePointView) chartPoint.View;

                var w = Math.Abs(reference.X - zero);
                var l = reference.X < zero
                    ? reference.X
                    : zero;

                rectangleView.Data.Height = singleRowHeight - padding;
                rectangleView.Data.Top = reference.Y + relativeTop - correction;

                rectangleView.Data.Left = l;
                rectangleView.Data.Width = w;

                rectangleView.ZeroReference = zero;

                chartPoint.ChartLocation = new CorePoint(rectangleView.Data.Left + rectangleView.Data.Width,
                    rectangleView.Data.Top);

                chartPoint.View.DrawOrMove(null, chartPoint, 0, Chart);
            }
        }

        double ICartesianSeries.GetMinX(AxisCore axis)
        {
            var f = AxisLimits.SeparatorMin(axis);
            return CurrentYAxis.MinLimit >= 0 && CurrentYAxis.MaxLimit > 0
                ? (f >= 0 ? f : 0)
                : f;
        }

        double ICartesianSeries.GetMaxX(AxisCore axis)
        {
            var f = AxisLimits.SeparatorMax(axis);
            return CurrentYAxis.MinLimit < 0 && CurrentYAxis.MaxLimit <= 0
                ? (f >= 0 ? f : 0)
                : f;
        }

        double ICartesianSeries.GetMinY(AxisCore axis)
        {
            return AxisLimits.StretchMin(axis);
        }

        double ICartesianSeries.GetMaxY(AxisCore axis)
        {
            return AxisLimits.UnitRight(axis);
        }
    }
}
