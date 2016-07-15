//-----------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="Brookfield Residential Properties">
//     Copyright (c) Brookfield Residential Properties. All rights reserved.
// </copyright>
// <author>Victor Procure</author>
//-----------------------------------------------------------------------
namespace GlobalAnalysisCO2.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Laser;
    using LiveCharts;
    using LiveCharts.Defaults;
    using LiveCharts.Wpf;
    using Ninject;

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private ChartValues<ObservableValue> chartValues = new ChartValues<ObservableValue>();
        private ILaserController laserController;

        public MainWindowViewModel()
        {
            this.InitializeLaser();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ChartValues<ObservableValue> ChartValues
        {
            get
            {
                return this.chartValues;
            }

            set
            {
                this.chartValues = value;
                NotifyPropertyChanged(nameof(this.ChartValues));
            }
        }

        private void InitializeLaser()
        {
            var kernel = new StandardKernel(new LaserBindings());
            this.laserController = kernel.Get<ILaserController>();
            this.laserController.CO2Reading += OnLaserRead;

            this.laserController.Connect();
        }

        private void NotifyPropertyChanged(string control)
        {
            var handler = this.PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(control));
        }

        private void OnLaserRead(object sender, double e)
        {
            this.chartValues.Add(new ObservableValue(e));
        }
    }
}