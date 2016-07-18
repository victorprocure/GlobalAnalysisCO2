using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GlobalAnalysisCO2.ViewModels;

namespace GlobalAnalysisCO2
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.HomeButton_MouseLeftButtonDown(this, null);
        }

        private BaseViewModel currentVM;
        private HomeViewModel homeVM = new HomeViewModel();
        private ConfigurationViewModel configurationViewModel = new ConfigurationViewModel();

        private void HomeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.SwitchViewModel(this.homeVM);
        }

        private void GearButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.SwitchViewModel(this.configurationViewModel);
        }

        private void SwitchViewModel(BaseViewModel viewModel)
        {
            if (this.currentVM != null)
            {
                this.currentVM.SuspendView();
            }

            this.DataContext = viewModel;
            this.currentVM = viewModel;
            this.currentVM.StartView();
        }
    }
}