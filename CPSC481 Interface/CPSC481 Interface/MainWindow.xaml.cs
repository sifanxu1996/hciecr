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
        private Random rand;

        public MainWindow() {
            InitializeComponent();

            released = null;
            rand = new Random();
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e) {
            if (released != null) {
                released.ResetPosition();
                released = null;
            }
        }

        private bool IsHoveringCell(Border b, Point p) {
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
                        if (IsHoveringCell(b, p)) {
                            Grid.SetColumn(released, col);
                            Grid.SetRow(released, row);
                            released.OnGridPlace();
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
            data.Sort();
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

        private Brush GetRandomBrush() {
            byte r = (byte) rand.Next(0, 256);
            byte g = (byte) rand.Next(0, 256);
            byte b = (byte) rand.Next(0, 256);
            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        private void AddItem(string text) {
            SearchItem item = new SearchItem(text, "Class Description goes here\nProfessors here\nTimes here");
            ClassSection s1 = new ClassSection(this, "Lecture", item.Sections, text, GetRandomBrush());
            item.Sections.Children.Add(s1);
            item.ClassName.MouseLeftButtonDown += (sender, e) => {
                foreach (UIElement ui in ResultStack.Children) {
                    SearchItem si = ui as SearchItem;
                    if (si != null) {
                        si.SetExpanded(false);
                    }
                }
                item.SetExpanded(true);
            };
            ResultStack.Children.Add(item);
        }
    }
}
