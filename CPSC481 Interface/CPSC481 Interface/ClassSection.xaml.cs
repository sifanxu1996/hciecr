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
    /// Interaction logic for ClassSection.xaml
    /// </summary>
    public partial class ClassSection : UserControl {

        private Point offset;
        private Thickness startPosition;

        public ClassSection() {
            InitializeComponent();

            offset = new Point();
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e) {
            offset = Mouse.GetPosition(this);
            startPosition = this.Margin;
            this.CaptureMouse();
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e) {
            this.ReleaseMouseCapture();
            this.Margin = startPosition;
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e) {
            if (this.IsMouseCaptured) {
                Point delta = Mouse.GetPosition(this);
                delta.Offset(-offset.X, -offset.Y);

                Thickness margin = this.Margin;
                margin.Left += delta.X;
                margin.Top += delta.Y;
                margin.Right -= delta.X;
                margin.Bottom -= delta.Y;
                this.Margin = margin;
            }
        }
    }
}
