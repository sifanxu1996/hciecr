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

namespace CPSC481_Interface {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ConfirmationWin : Window {
        private MainWindow window;
        public ConfirmationWin(MainWindow Window) {
            InitializeComponent();
            window = Window;
        }

        private void btnYes_Click(object sender, RoutedEventArgs e) {
            // enable EnrolStatus on all ClassSection, GridSection elements on Grid
            foreach (UIElement ui in window.ScheduleGrid.Children)
            {
                ClassSection cs = ui as ClassSection;
                GridSection gs = ui as GridSection;
                if (gs != null)
                {
                    gs.EnrolBorder.Visibility = System.Windows.Visibility.Visible;
                    gs.EnrolStatus.Visibility = System.Windows.Visibility.Visible;
                }
                else if (cs != null)
                {
                    cs.EnrolBorder.Visibility = System.Windows.Visibility.Visible;
                    cs.EnrolStatus.Visibility = System.Windows.Visibility.Visible;
                }
            }
            Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e) {
            //
            Close();
        }
    }
}
