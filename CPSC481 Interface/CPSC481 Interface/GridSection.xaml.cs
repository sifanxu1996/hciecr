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
    /// Interaction logic for GridSection.xaml
    /// </summary>
    public partial class GridSection : UserControl {

        private Brush normal, hover;
        public GridSection[] connected;
        public bool stayOnGrid;
        public ClassSection parentClass;

        public GridSection(ClassSection ParentClass, string Name, string Type, Brush HoverColor, string location, string startTime) {
            InitializeComponent();

            parentClass = ParentClass;
            hover = HoverColor;

            Color c = ((SolidColorBrush) HoverColor).Color;
            normal = new SolidColorBrush(Color.FromArgb(100, c.R, c.G, c.B));
            BG.Fill = normal;

            SectionType.Content = Name + " " + Type + "\n" + location + "\n" + startTime;
            stayOnGrid = false;
        }

        public void SetStay(bool val) {
            foreach (GridSection g in connected) {
                g.stayOnGrid = val;
            }
        }

        public void SetConnected(GridSection[] newConnected) {
            connected = newConnected;
        }

        public void ShowConnected() {
            foreach (GridSection g in connected) {
                g.Visibility = Visibility.Visible;
            }
        }

        public void HideConnected() {
            if (!stayOnGrid) {
                foreach (GridSection g in connected) {
                    g.Visibility = Visibility.Hidden;
                }
            }
        }

        public void HighlightConnected() {
            foreach (GridSection g in connected) {
                g.BG.Fill = hover;
            }
        }

        public void ShadowConnected() {
            if (!stayOnGrid) {
                foreach (GridSection g in connected) {
                    g.BG.Fill = normal;
                }
            }
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e) {
            parentClass.UserControl_MouseDown(sender, e);
        }

        public void SetEnrollment(bool enrolled) {
            foreach (GridSection gs in connected) {
                if (enrolled) {
                    gs.Bevel.Visibility = Visibility.Visible;
                    gs.EnrollStatus.Visibility = Visibility.Visible;
                } else {
                    gs.Bevel.Visibility = Visibility.Hidden;
                    gs.EnrollStatus.Visibility = Visibility.Hidden;
                }
            }
        }
    }
}
