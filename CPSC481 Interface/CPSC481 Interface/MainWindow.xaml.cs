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
		public MainWindow() {
			InitializeComponent();
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

        // Data for courses to search from
        static public List<string> GetData() {
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
            var border = (resultStack.Parent as ScrollViewer).Parent as Border;
            var data = MainWindow.GetData();

            string query = (sender as TextBox).Text;

            if (query.Length == 0)
            {
                // Clear   
                resultStack.Children.Clear();
                border.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                border.Visibility = System.Windows.Visibility.Visible;
            }

            // Clear the list   
            resultStack.Children.Clear();

            // Add the result   
            foreach (var obj in data)
            {
                if (obj.ToLower().StartsWith(query.ToLower()))
                {
                    // The word starts with this... Autocomplete must work   
                    addItem(obj);
                    found = true;
                }
            }

            if (!found)
            {
                resultStack.Children.Add(new TextBlock() { Text = "No results found." });
            }
        }

        private void addItem(string text)
        {
            TextBlock block = new TextBlock();


            // Add the text   
            block.Text = text;
            


            // A little style...   
            block.Margin = new Thickness(2, 3, 2, 3);
            block.Cursor = Cursors.Hand;

            // Mouse events   
            block.MouseLeftButtonUp += (sender, e) =>
            {
                SearchBox.Text = (sender as TextBlock).Text;
            };

            block.MouseEnter += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.PeachPuff;
            };

            block.MouseLeave += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.Transparent;
            };

            // Add to the panel   
            resultStack.Children.Add(block);
        }

    }
}
