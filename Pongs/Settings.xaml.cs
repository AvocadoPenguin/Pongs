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
using System.Windows.Shapes;
using NLog;
using PongsSBK;

namespace Pongs
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>

    public partial class Settings : Window
    {
        public Settings(SliderInfo sliderINfo)
        {
            InitializeComponent();
            DataContext = sliderINfo;
        }

        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
             Logger log = LogManager.GetLogger("");
             log.Info("Settings Changed!");
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((SliderInfo)DataContext).Engine.GamePlayable = true;
        }
    }
}
