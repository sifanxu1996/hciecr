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

        private List<string> GetData() {
            List<string> data = new List<string>();
            data.Add("CPSC 231");
            data.Add("MATH 211");
            data.Add("CPSC 481");
            data.Add("LING 201");
            data.Add("CPSC 413");
            data.Add("PHIL 314");
            return data;
        }

        private void SearchBox_KeyUp(object sender, KeyEventArgs e) {
            bool found = false;
            Border border = (ResultStack.Parent as ScrollViewer).Parent as Border;
            List<string> data = GetData();

            string query = SearchBox.Text.ToLower();

            if (query.Length == 0) {
                // Hide   
                border.Visibility = System.Windows.Visibility.Collapsed;
            } else {
                border.Visibility = System.Windows.Visibility.Visible;
            }

            // Clear the list   
            ResultStack.Children.Clear();

            // Add the result   
            foreach (string obj in data) {
                if (obj.ToLower().StartsWith(query)) {
                    // The word starts with this... Autocomplete must work   
                    AddItem(obj);
                    found = true;
                }
            }

            if (!found) {
                ResultStack.Children.Add(new TextBlock() { Text = "No results found.", Margin = new Thickness(2, 1, 0, 1) });
            }
        }

        private void AddItem(string text) {
            TextBlock block = new TextBlock();
          
            // Add the text   
            block.Text = text;
            block.HorizontalAlignment = HorizontalAlignment.Center;
            block.FontSize = 20;

            // A little style...   
            block.Margin = new Thickness(2, 3, 2, 3);
            block.Cursor = Cursors.Hand;

            // Mouse events   
            block.MouseLeftButtonUp += (sender, e) => {
                SearchBox.Text = (sender as TextBlock).Text; // Dont know if we need this
            };

            block.MouseEnter += (sender, e) => {
                TextBlock b = sender as TextBlock;
                b.Background = new SolidColorBrush(Color.FromRgb(170, 170, 50));
            };

            block.MouseLeave += (sender, e) => {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.Transparent;
            };

            // Add to the panel   
            ResultStack.Children.Add(block);
        }
    }
}
