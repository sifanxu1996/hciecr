using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class MainWindow : Window {

        public ClassSection released;

        public MainWindow() {
            InitializeComponent();

            ClassSection section1 = new ClassSection(this, "Lecture");
            Grid.SetRow(section1, 1);
            Grid.SetColumn(section1, 1);
            ScheduleGrid.Children.Add(section1);

            ClassSection section2 = new ClassSection(this, "Tutorial");
            Grid.SetRow(section2, 4);
            Grid.SetColumn(section2, 6);
            ScheduleGrid.Children.Add(section2);

            released = null;
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e) {
            if (SearchBox.Text.Equals("Search Course")) {
                SearchBox.Text = "";
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e) {
            if (SearchBox.Text.Equals("")) {
                SearchBox.Text = "Search Course";
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e) {
            if (released != null) {
                released.ResetPosition();
                released = null;
            }
        }

        private bool IsHovering(Border b, Point p) {
            bool inX = b.Margin.Left <= p.X && b.Margin.Left + b.ActualWidth >= p.X;
            bool inY = b.Margin.Top <= p.Y && b.Margin.Top + b.ActualHeight >= p.Y;
            return inX && inY;
        }

        private void ScheduleGrid_MouseUp(object sender, MouseButtonEventArgs e) {
            if (released != null) {
                foreach (UIElement ui in ScheduleGrid.Children) {
                    Border b = ui as Border;
                    if (b != null) {
                        int col = Grid.GetColumn(b);
                        int row = Grid.GetRow(b);
                        Point p = Mouse.GetPosition(b);
                        if (IsHovering(b, p)) {
                            Grid.SetColumn(released, col);
                            Grid.SetRow(released, row);
                            released.Margin = new Thickness(0);
                            released = null;
                            break;
                        }
                    }
                }
            }
        }
    }
}
