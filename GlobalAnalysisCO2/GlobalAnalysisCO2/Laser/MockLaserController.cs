//-----------------------------------------------------------------------
// <copyright file="MockLaserController.cs" company="Brookfield Residential Properties">
//     Copyright (c) Brookfield Residential Properties. All rights reserved.
// </copyright>
// <author>Victor Procure</author>
//-----------------------------------------------------------------------
namespace GlobalAnalysisCO2.Laser
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class MockLaserController : ILaserController
    {
        private double ticks;
        private BackgroundWorker worker;

        public event EventHandler<int> CO2Reading;

        public bool Connect()
        {
            this.InitializeReadings();
            return true;
        }

        public void Disconnect()
        {
            //is.worker.CancelAsync();
        }

        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;

            while (!this.worker.CancellationPending)
            {
                OnTimerElapsed();
                Thread.Sleep(TimeSpan.FromMilliseconds(500)); // For the time being this has to remain quite high to avoid a bug in LiveCharts
            }
        }

        private void InitializeReadings()
        {
            this.StartWorker();
        }

        private void OnTimerElapsed()
        {
            var handler = this.CO2Reading;

            double theta = this.ticks * 2 * Math.PI; // Calculate circle point for sine wave
            theta /= 20; //For our mock this will be the total number of points on our graph

            theta = Math.Sin(theta);

            CO2Reading?.Invoke(this, (int)Math.Round(theta * 1000, 0)); // Because we cannot invoke double precision ( the Controller is int), we round

            this.ticks += 1;
        }

        private void StartWorker()
        {
            this.worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };

            worker.DoWork += BackgroundWorkerOnDoWork;
            worker.RunWorkerAsync();
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above. GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free
        //       unmanaged resources. ~MockLaserController() { // Do not change this code. Put
        // cleanup code in Dispose(bool disposing) above. Dispose(false); }

        #endregion IDisposable Support
    }
}