using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using FontAwesome.WPF;

namespace GlobalAnalysisCO2
{
    /// <summary>
    ///     Interaction logic for NavigationButton.xaml
    /// </summary>
    public partial class NavigationButton : UserControl
    {
        // Using a DependencyProperty as the backing store for HoverColor. This enables animation,
        // styling, binding, etc...
        public static readonly DependencyProperty HoverColorProperty =
            DependencyProperty.Register("HoverColor", typeof(Brush), typeof(NavigationButton), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        // Using a DependencyProperty as the backing store for Icon. This enables animation, styling,
        // binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(FontAwesome.WPF.FontAwesomeIcon), typeof(NavigationButton), new PropertyMetadata(FontAwesomeIcon.Flag));

        // Using a DependencyProperty as the backing store for Label. This enables animation,
        // styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(NavigationButton), new PropertyMetadata("Label"));

        private int iconSize;

        private int labelSize;

        public NavigationButton()
        {
            InitializeComponent();

            this.labelSize = (int)Math.Round(this.Height / 1.30, 0);
            this.iconSize = (int)Math.Round(this.Height / 1.70, 0);

            this.Cursor = Cursors.Hand;

            this.DataContext = this;
        }

        public Brush HoverColor
        {
            get { return (Brush)GetValue(HoverColorProperty); }
            set { SetValue(HoverColorProperty, value); }
        }

        public FontAwesome.WPF.FontAwesomeIcon Icon
        {
            get { return (FontAwesome.WPF.FontAwesomeIcon)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public int IconSize
        {
            get
            {
                return (int)(this.ActualHeight * 0.60);
            }
        }

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public int LabelSize
        {
            get
            {
                return (int)(this.ActualHeight * 0.25);
            }
        }

        private void OnMouse(object sender, MouseEventArgs e)
        {
            StackPanel control = (StackPanel)sender;

            var currentHoverColor = this.HoverColor;

            this.HoverColor = this.Foreground;

            this.Foreground = currentHoverColor;
        }
    }
}