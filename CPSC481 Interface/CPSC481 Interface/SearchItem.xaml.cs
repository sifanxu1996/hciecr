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
    /// Interaction logic for SearchItem.xaml
    /// </summary>
    public partial class SearchItem : UserControl {

        private bool expanded;
        private Brush highlight, normal;

        public SearchItem(string Name, string DescriptionText) {
            InitializeComponent();

            ClassName.Content = Name;
            Description.Text = DescriptionText;

            expanded = false;
            normal = Brushes.Transparent;
            highlight = new SolidColorBrush(Color.FromRgb(170, 170, 50));
        }

        public void SetExpanded(bool visible) {
            if (visible) {
                Background = highlight;
                Sections.Visibility = Visibility.Visible;
                Description.Visibility = Visibility.Visible;
            } else {
                Background = normal;
                Sections.Visibility = Visibility.Collapsed;
                Description.Visibility = Visibility.Collapsed;
            }
            expanded = visible;
        }

        private void SetBackground(Brush b) {
            if (!expanded) {
                Background = b;
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e) {
            SetBackground(highlight);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e) {
            SetBackground(normal);
        }
    }
}
