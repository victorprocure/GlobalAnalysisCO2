//The MIT License(MIT)

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
using System.Collections.Generic;
using LiveCharts.Dtos;

namespace LiveCharts.Configurations
{
    /// <summary>
    /// Mapper to configure financial points
    /// </summary>
    /// <typeparam name="T">type to configure</typeparam>
    public class FinancialMapper<T> : IPointEvaluator<T>
    {
        private Func<T, int, double> _x = (v, i) => i;
        private Func<T, int, double> _y = (v, i) => 0;
        private Func<T, int, double> _open;
        private Func<T, int, double> _high;
        private Func<T, int, double> _low;
        private Func<T, int, double> _close;

        /// <summary>
        /// Sets values for a specific point
        /// </summary>
        /// <param name="valuePair">Key and value</param>
        /// <param name="point">Point to set</param>
        public void SetAll(KeyValuePair<int, T> valuePair, ChartPoint point)
        {
            point.X = _x(valuePair.Value, valuePair.Key);
            point.Y = _y(valuePair.Value, valuePair.Key);
            point.Open = _open(valuePair.Value, valuePair.Key);
            point.High = _high(valuePair.Value, valuePair.Key);
            point.Close = _close(valuePair.Value, valuePair.Key);
            point.Low = _low(valuePair.Value, valuePair.Key);
        }

        /// <summary>
        /// Evaluates a point with a given value and key
        /// </summary>
        /// <param name="valuePair">Value and Key</param>
        /// <returns>point evaluation</returns>
        public Xyw[] GetEvaluation(KeyValuePair<int, T> valuePair)
        {
            var x = _x(valuePair.Value, valuePair.Key);
            return new[]
            {
                new Xyw(x, _low(valuePair.Value, valuePair.Key), 0),
                new Xyw(x, _high(valuePair.Value, valuePair.Key), 0)
            };
        }

        /// <summary>
        /// Maps X value
        /// </summary>
        /// <param name="predicate">function that pulls X coordinate</param>
        /// <returns>current mapper instance</returns>
        public FinancialMapper<T> X(Func<T, double> predicate)
        {
            return X((t, i) => predicate(t));
        }
        /// <summary>
        /// Maps X value
        /// </summary>
        /// <param name="predicate">function that pulls X coordinate, with value and index as parameters</param>
        /// <returns>current mapper instance</returns>
        public FinancialMapper<T> X(Func<T, int, double> predicate)
        {
            _x = predicate;
            return this;
        }

        /// <summary>
        /// Maps Y value
        /// </summary>
        /// <param name="predicate">function that pulls Y coordinate</param>
        /// <returns>current mapper instance</returns>
        public FinancialMapper<T> Y(Func<T, double> predicate)
        {
            return Y((t, i) => predicate(t));
        }
        /// <summary>
        /// Maps Y value
        /// </summary>
        /// <param name="predicate">function that pulls Y coordinate, with value and index as parameters</param>
        /// <returns>current mapper instance</returns>
        public FinancialMapper<T> Y(Func<T, int, double> predicate)
        {
            _y = predicate;
            return this;
        }

        /// <summary>
        /// Maps Open value
        /// </summary>
        /// <param name="predicate">function that pulls open value</param>
        /// <returns>current mapper instance</returns>
        public FinancialMapper<T> Open(Func<T, double> predicate)
        {
            return Open((t, i) => predicate(t));
        }
        /// <summary>
        /// Maps Open value
        /// </summary>
        /// <param name="predicate">function that pulls open value, value and index as parameters</param>
        /// <returns>current mapper instance</returns>
        public FinancialMapper<T> Open(Func<T, int, double> predicate)
        {
            _open = predicate;
            return this;
        }

        /// <summary>
        /// Maps High value
        /// </summary>
        /// <param name="predicate">function that pulls High value</param>
        /// <returns>current mapper instance</returns>
        public FinancialMapper<T> High(Func<T, double> predicate)
        {
            return High((t, i) => predicate(t));
        }
        /// <summary>
        /// Maps High value
        /// </summary>
        /// <param name="predicate">function that pulls High value</param>
        /// <returns>current mapper instance</returns>
        public FinancialMapper<T> High(Func<T, int, double> predicate)
        {
            _high = predicate;
            return this;
        }

        /// <summary>
        /// Maps Close value
        /// </summary>
        /// <param name="predicate">function that pulls close value</param>
        /// <returns>current mapper instance</returns>
        public FinancialMapper<T> Close(Func<T, double> predicate)
        {
            return Close((t, i) => predicate(t));
        }
        /// <summary>
        /// Maps Close value
        /// </summary>
        /// <param name="predicate">function that pulls close value, value and index as parameters</param>
        /// <returns>current mapper instance</returns>
        public FinancialMapper<T> Close(Func<T, int, double> predicate)
        {
            _close = predicate;
            return this;
        }

        /// <summary>
        /// Maps Low value
        /// </summary>
        /// <param name="predicate">function that pulls low value</param>
        /// <returns>current mapper instance</returns>
        public FinancialMapper<T> Low(Func<T, double> predicate)
        {
            return Low((t, i) => predicate(t));
        }
        /// <summary>
        /// Maps Low value
        /// </summary>
        /// <param name="predicate">function that pulls low value, index and value as parameters</param>
        /// <returns>current mapper instance</returns>
        public FinancialMapper<T> Low(Func<T, int, double> predicate)
        {
            _low = predicate;
            return this;
        }
    }
}
