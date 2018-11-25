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

        private Point offset, radius;
        private Thickness startPosition, originalMargin;
        private MainWindow window;
        private Panel originalParent;
        private string name, type;
        public Brush color;
        public ClassData data;
        private bool placedOnce;

        public ClassSection(MainWindow Window, string Type, Panel OriginalParent, string ClassName, Brush Color, ClassData Data) {
            InitializeComponent();

            offset = new Point();
            window = Window;
            SectionType.Content = Type;

            originalParent = OriginalParent;
            originalMargin = Margin;

            name = ClassName;
            data = Data;
            type = Type;
            radius = new Point(BG.RadiusX, BG.RadiusY);
            color = Color;
            BG.Fill = Color;
            placedOnce = false;
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                offset = Mouse.GetPosition(this);
                startPosition = this.Margin;
                this.CaptureMouse();

                if (originalParent.Children.Contains(this)) {
                    originalParent.Children.Remove(this);
                }
                if (!window.ScheduleGrid.Children.Contains(this)) {
                    window.ScheduleGrid.Children.Add(this);
                }

                if (placedOnce) {
                    BG.RadiusX = radius.X;
                    BG.RadiusY = radius.Y;
                }
            }
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Released) {
                this.ReleaseMouseCapture();
                window.released = this;
            }
        }

        // generates class sections on calendar when mouse hovers over ClassSection
        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            TimeSlot[] lectureSlots = this.data.timeSlots;

            for (int i = 0; i < lectureSlots.Length; i++) {
                int[] daySlots = lectureSlots[i].days;
                int startTime = lectureSlots[i].startTime;
                int duration = lectureSlots[i].duration;

                for (int j = 0; j < daySlots.Length; j++) {
                    Grid.SetColumn(this, lectureSlots[i].days[j]);
                    Grid.SetRow(this, data.timeSlots[i].startTime);
                    this.OnGridPlace();
                }
            }
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed && this.IsMouseCaptured) {
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

        public void ResetPosition() {
            if (window.ScheduleGrid.Children.Contains(this)) {
                window.ScheduleGrid.Children.Remove(this);
            }
            if (!originalParent.Children.Contains(this)) {
                originalParent.Children.Add(this);
            }
            Margin = originalMargin;
            SectionType.Content = type;
            BG.RadiusX = radius.X;
            BG.RadiusY = radius.Y;
        }

        // Make rectangle
        public void OnGridPlace() {
            BG.RadiusX = 0;
            BG.RadiusY = 0;
            Margin = new Thickness(0);
            SectionType.Content = name + " " + type;
            placedOnce = true;
        }
    }
}
