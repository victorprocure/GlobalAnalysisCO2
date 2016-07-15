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
        private long ticks;

        public event EventHandler<double> CO2Reading;

        public bool Connect()
        {
            this.InitializeReadings();
            return true;
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;

            while (!worker.CancellationPending)
            {
                OnTimerElapsed();
                Thread.Sleep(TimeSpan.FromMilliseconds(500));
            }
        }

        private void InitializeReadings()
        {
            this.StartWorker();
        }

        private void OnTimerElapsed()
        {
            var handler = this.CO2Reading;

            double theta = Math.Sin(this.ticks);
            //theta /= 10;

            CO2Reading?.Invoke(this, theta);

            this.ticks++;
        }

        private void StartWorker()
        {
            BackgroundWorker worker = new BackgroundWorker
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