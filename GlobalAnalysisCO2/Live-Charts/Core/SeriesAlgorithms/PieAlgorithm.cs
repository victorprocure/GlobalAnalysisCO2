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

using System.Linq;
using LiveCharts.Definitions.Charts;
using LiveCharts.Definitions.Points;
using LiveCharts.Definitions.Series;

namespace LiveCharts.SeriesAlgorithms
{
    public class PieAlgorithm : SeriesAlgorithm, IPieSeries
    {
        public PieAlgorithm(ISeriesView view) : base(view)
        {
            PreferredSelectionMode= TooltipSelectionMode.SharedXValues;
        }

        public override void Update()
        {
            var pieChart = (IPieChart) View.Model.Chart.View;

            var maxPushOut = View.Model.Chart.View.ActualSeries
                .OfType<IPieSeriesView>()
                .Select(x => x.PushOut)
                .DefaultIfEmpty(0).Max();

            var minDimension = Chart.DrawMargin.Width < Chart.DrawMargin.Height
                ? Chart.DrawMargin.Width
                : Chart.DrawMargin.Height;
            minDimension -= 10 + maxPushOut;
            minDimension = minDimension < 10 ? 10 : minDimension;
            
            var inner = pieChart.InnerRadius;

            var startAt = pieChart.StartingRotationAngle > 360
                ? 360
                : (pieChart.StartingRotationAngle < 0
                    ? 0
                    : pieChart.StartingRotationAngle);

            foreach (var chartPoint in View.ActualValues.Points)
            {
                chartPoint.View = View.GetPointView(chartPoint.View, chartPoint,
                    View.DataLabels
                        ? (chartPoint.Participation > 0.05
                            ? View.GetLabelPointFormatter()(chartPoint)
                            : string.Empty)
                        : null);

                var pieSlice = (IPieSlicePointView) chartPoint.View;

                var space = pieChart.InnerRadius +
                            ((minDimension/2) - pieChart.InnerRadius)*((chartPoint.X + 1)/(View.Values.Limit1.Max + 1));

                chartPoint.SeriesView = View;

                pieSlice.Radius = space;
                pieSlice.InnerRadius = inner;
                pieSlice.Rotation = startAt + (chartPoint.StackedParticipation - chartPoint.Participation)*360;
                pieSlice.Wedge = chartPoint.Participation*360 > 0 ? chartPoint.Participation*360 : 0;

                chartPoint.View.DrawOrMove(null, chartPoint, 0, Chart);

                inner = pieSlice.Radius;
            }
        }
    }
}
