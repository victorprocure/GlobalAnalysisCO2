using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using GlobalAnalysisCO2.Laser;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Ninject;

namespace GlobalAnalysisCO2.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private ChartValues<ObservableValue> chartValues = new ChartValues<ObservableValue>();

        private ILaserController laserController;

        private bool visible;

        public HomeViewModel()
        {
            base.ChangeTitle("Home");
        }

        public ChartValues<ObservableValue> ChartValues
        {
            get
            {
                return this.chartValues;
            }

            set
            {
                this.chartValues = value;
                base.NotifyPropertyChanged(nameof(this.ChartValues));
            }
        }

        public override void StartView()
        {
            this.visible = true;
            this.InitializeLaser();
        }

        public override void SuspendView()
        {
            this.laserController.Disconnect();
            this.visible = false;
            this.chartValues.Clear();
        }

        private void InitializeLaser()
        {
            var kernel = new StandardKernel(new LaserBindings());
            this.laserController = kernel.Get<ILaserController>();
            this.laserController.CO2Reading += OnLaserRead;

            this.laserController.Connect();
        }

        private void OnLaserRead(object sender, int e)
        {
            if (this.visible)
            {
                this.chartValues.Add(new ObservableValue(e));

                if (this.chartValues.Count > 200) this.chartValues.RemoveAt(0);
            }
        }
    }
}