using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalAnalysisCO2.Laser
{
    internal interface ILaserController : IDisposable
    {
        event EventHandler<double> CO2Reading;

        bool Connect();

        void Disconnect();
    }
}