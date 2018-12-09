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

        private Point radius;
        public Thickness startPosition, originalMargin;
        private MainWindow window;
        public Panel originalParent;
        private string type, time;
        public string name;
        public Brush color;
        public ClassData data;
        public bool isTutorial, onGrid, placedOnce;
        private GridSection[][] sections;
        private double originalHeight;
        public SearchItem searchParent;
        public ClassSection other;
        public GridSection linked;
        public bool enrolled;

        public ClassSection(MainWindow Window, bool IsTutorial, Panel OriginalParent, ClassData Data, Brush Color, SearchItem SearchParent) {
            InitializeComponent();

            window = Window;
            searchParent = SearchParent;

            originalParent = OriginalParent;
            originalMargin = new Thickness(5, 0, 5, 0);
            originalHeight = Height;

            data = Data;
            isTutorial = IsTutorial;
            name = data.name;
            type = (IsTutorial) ? "Tutorial" : "Lecture";
            SectionType.Content = type;
            radius = new Point(BG.RadiusX, BG.RadiusY);
            color = Color;
            BG.Fill = Color;
            placedOnce = false;
            onGrid = false;
            other = null;

            TimeSlot[] slots = (IsTutorial) ? Data.tutorialSlots : Data.timeSlots;

            sections = new GridSection[slots.Length][];
            for (int j = 0; j < slots.Length; j++) {
                TimeSlot t = slots[j];
                time = getTime(t.startTime, t.duration);
                GridSection[] gs = new GridSection[t.days.Length];
                for (int i = 0; i < t.days.Length; i++) {
                    GridSection g = new GridSection(this, name, type, color, t.location, time, t);
                    Grid.SetRow(g, (int) Math.Floor(t.startTime));
                    Grid.SetColumn(g, t.days[i]);
                    Grid.SetRowSpan(g, (int) Math.Ceiling(t.duration));
                    g.VerticalAlignment = VerticalAlignment.Top;
                    double defaultHeight = 116.5;
                    double height = t.duration * defaultHeight;
                    g.Height = height;
                    Thickness thickness = new Thickness(0);
                    thickness.Top = (t.startTime - Math.Floor(t.startTime)) * defaultHeight;
                    g.Margin = thickness;
                    gs[i] = g;
                    window.ScheduleGrid.Children.Add(g);
                }
                foreach (GridSection g in gs) {
                    g.SetConnected(gs);
                }
                if (gs.Length > 0) {
                    gs[0].HideConnected();
                }
                sections[j] = gs;
            }
        }

        private string getTime(float startTime, float duration) {
            string period, sTime, eTime, e2Time;
            float realStartTime = startTime + 7;
            float realEndTime;

            if (duration == 1 || duration == 2) {
                e2Time = ":50";
                realEndTime = realStartTime + duration - 1;

            } else if (duration == 1.25) {
                e2Time = ":15";
                realEndTime = realStartTime + duration;
            } else {
                e2Time = ":45";
                realEndTime = realStartTime + duration - 1;
            }

            if (startTime <= 4) {
                period = "AM";
            } else {
                period = "PM";
            }

            if (Math.Floor(realStartTime) == 12) {
                sTime = "12";
            } else {
                sTime = "" + (Math.Floor(realStartTime) % 12);
            }

            if (Math.Floor(realEndTime) == 12) {
                eTime = "12" + e2Time;
            } else {
                eTime = "" + (Math.Floor(realEndTime) % 12) + e2Time;
            }

            if (Math.Floor(realStartTime) != realStartTime) {
                return sTime + ":30" + period + "-" + eTime + period;
            } else {
                return sTime + ":00" + period + "-" + eTime + period;
            }
        }

        public void UserControl_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                if (placedOnce) {
                    Height = 75;
                }

                window.Garbage.Visibility = Visibility.Visible;
                Grid.SetZIndex(this, 1);
                window.TryExpandSearchItem(searchParent);

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

                foreach (GridSection[] gs in sections) {
                    if (gs.Length > 0) {
                        gs[0].ShowConnected();
                        gs[0].SetStay(false);
                        gs[0].ShadowConnected();
                    }
                }
            }
        }

        public void HideConnected() {
            foreach (GridSection[] gs in sections) {
                if (gs.Length > 0) {
                    gs[0].SetStay(false);
                    gs[0].HideConnected();
                }
            }
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Released && e.ChangedButton == MouseButton.Left) {
                window.Garbage.Visibility = Visibility.Hidden;
                this.ReleaseMouseCapture();
                window.released = this;
            }
        }

        // generates class sections on calendar when mouse hovers over ClassSection
        private void UserControl_MouseEnter(object sender, MouseEventArgs e) {
            if (!onGrid) {
                foreach (GridSection[] gs in sections) {
                    if (gs.Length > 0) {
                        gs[0].ShowConnected();
                    }
                }
            }
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e) {
            foreach (GridSection[] gs in sections) {
                if (gs.Length > 0) {
                    gs[0].HideConnected();
                }
            }
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed && this.IsMouseCaptured) {
                Point offset = new Point(ActualWidth / 2, ActualHeight / 2);

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

        public void ResetPosition(bool showHelp = true) {
            if (window.ScheduleGrid.Children.Contains(this)) {
                window.ScheduleGrid.Children.Remove(this);
            }
            if (!originalParent.Children.Contains(this)) {
                originalParent.Children.Add(this);
            }
            Margin = originalMargin;
            Height = originalHeight;
            SectionType.Content = type;
            BG.RadiusX = radius.X;
            BG.RadiusY = radius.Y;
            onGrid = false;
            if (showHelp) {
                searchParent.Dragging_Info.Visibility = Visibility.Visible;
            }

            for (int i = 0; i < sections.Length; i++) {
                for (int j = 0; j < sections[i].Length; j++) {
                    GridSection g = sections[i][j];
                    g.SetStay(false);
                    g.ShadowConnected();
                }
            }
        }

        // Make rectangle
        public void OnGridPlace(bool SetHeight = false) {
            BG.RadiusX = 0;
            BG.RadiusY = 0;
            SectionType.Content = name + " " + type;
            placedOnce = true;
            onGrid = true;

            for (int i = 0; i < sections.Length; i++) {
                for (int j = 0; j < sections[i].Length; j++) {
                    GridSection g = sections[i][j];
                    if (Grid.GetRow(g) == Grid.GetRow(this) && Grid.GetColumn(g) == Grid.GetColumn(this)) {
                        Grid.SetRowSpan(this, Grid.GetRowSpan(g));
                        g.SetStay(true);
                        g.HighlightConnected();
                        g.ShowConnected();
                        if (SetHeight) {
                            Height = g.Height;
                        }
                        break;
                    } else {
                        g.SetStay(false);
                        g.ShadowConnected();
                        g.HideConnected();
                    }
                }
            }

            if (originalParent.Children.Count == 0) {
                searchParent.Dragging_Info.Visibility = Visibility.Collapsed;
                bool alreadyPlaced = false;
                foreach (UIElement ui in window.ListOfCourses.Children) {
                    CourseListItem cli = ui as CourseListItem;
                    if (cli != null && cli.name == name) {
                        alreadyPlaced = true;
                        break;
                    }
                }

                if (!alreadyPlaced) {
                    window.ListOfCourses.Children.Add(new CourseListItem(this, other, window));
                }
            }
        }

        public void SetEnrollment(bool enrolled) {
            this.enrolled = enrolled;
            if (enrolled) {
                Bevel.Visibility = Visibility.Visible;
                EnrollStatus.Visibility = Visibility.Visible;
            } else {
                Bevel.Visibility = Visibility.Hidden;
                EnrollStatus.Visibility = Visibility.Hidden;
            }
            foreach (GridSection[] gs in sections) {
                gs[0].SetEnrollment(enrolled);
            }
        }
    }
}
