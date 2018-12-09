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
    /// Interaction logic for CourseList.xaml
    /// </summary>
    public partial class CourseListItem : UserControl {
        private MainWindow window;
        private Brush color;
        public string name;
        private ClassSection section, other;
        public bool isChecked;

        public CourseListItem(ClassSection Section, ClassSection Other, MainWindow Window) {
            InitializeComponent();
            section = Section;
            other = Other;
            window = Window;
            name = Section.name;
            color = Section.color;

            SectionType.Content = name;
            BG.Fill = color;
            isChecked = true;
        }

        private void CourseListItem_Checked(object sender, RoutedEventArgs e) {
            isChecked = true;
            if (section != null) {
                section.Visibility = Visibility.Visible;
                if (section.linked != null) {
                    section.linked.SetStay(true);
                    section.linked.ShowConnected();
                    section.linked.HighlightConnected();
                }
                if (other != null) {
                    other.Visibility = Visibility.Visible;
                    if (other.linked != null) {
                        other.linked.SetStay(true);
                        other.linked.ShowConnected();
                        other.linked.HighlightConnected();
                    }
                }
            }
        }

        private void CourseListitem_Unchecked(object sender, RoutedEventArgs e) {
            isChecked = false;
            section.Visibility = Visibility.Hidden;
            if (section.linked != null) {
                section.linked.SetStay(false);
                section.linked.HideConnected();
            }
            if (other != null) {
                other.Visibility = Visibility.Hidden;
                if (other.linked != null) {
                    other.linked.SetStay(false);
                    other.linked.HideConnected();
                }
            }
        }

        public void SetEnrollment(bool enrolled) {
            if (enrolled) {
                Bevel.Visibility = Visibility.Visible;
                EnrollStatus.Visibility = Visibility.Visible;
            } else {
                Bevel.Visibility = Visibility.Hidden;
                EnrollStatus.Visibility = Visibility.Hidden;
            }
            section.SetEnrollment(enrolled);
            if (other != null) {
                other.SetEnrollment(enrolled);
            }
        }
    }
}
