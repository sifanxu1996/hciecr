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
using Image = System.Windows.Controls.Image;

namespace CPSC481_Interface {

    // TimeSlot class
    public class TimeSlot {
        public int[] days;
        public float startTime, duration;
        public string location;

        public TimeSlot(int[] days, float startTime, int duration, string location) {
            this.days = days;
            this.startTime = startTime;
            this.duration = duration / 60f;
            this.location = location;
        }
    }

    // ClassData class
    public class ClassData {

        public string name, title, description, professor, times;
        public TimeSlot[] timeSlots, tutorialSlots;
        public bool hasTutorial;
        public Brush brush;

        public ClassData(string name, string title, string description, string professor, string times, TimeSlot[] timeSlots, bool hasTutorial, TimeSlot[] tutorialSlots) {
            this.name = name;
            this.title = title;
            this.description = description;
            this.professor = professor;
            this.times = times;
            this.timeSlots = timeSlots;
            this.hasTutorial = hasTutorial;
            this.tutorialSlots = tutorialSlots;
        }

        public static int CompareClassData(ClassData cd1, ClassData cd2) {
            return cd1.name.CompareTo(cd2.name);
        }

        public override string ToString() {
            return String.Format("{0}\n\nProfessor: {1}\n\nTimes: {2}", new string[] {description, professor, times });
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public ClassSection released;
        private Random rand;
        private Brush[] classColors;
        private SearchItem[] items;
        private List<ClassData> classes;
        public bool ConfirmResult;
        private GridSection conflicting;
        private List<CourseListItem> enrolled;

        public MainWindow() {
            InitializeComponent();

            enrolled = new List<CourseListItem>();

            released = null;
            rand = new Random();

            classColors = new Brush[6];
            classColors[0] = CreateBrush(75, 163, 255);  // Blue
            classColors[1] = CreateBrush(191, 0, 255);   // Yellow
            classColors[2] = CreateBrush(116, 195, 101); // Green
            classColors[3] = CreateBrush(253, 255, 0);   // Purple
            classColors[4] = CreateBrush(255, 36, 0);    // Red
            classColors[5] = CreateBrush(255, 159, 0);   // Orange

            classes = GetClasses();
            CreateSearchItems();

            Garbage.Visibility = Visibility.Hidden;
        }

        private Brush CreateBrush(byte r, byte g, byte b) {
            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e) {
            if (released != null) {
                if (IsHoveringGarbage(TrashEmpty) || IsHoveringGarbage(TrashFull)) {
                    if (released.onGrid) {
                        ConfirmationWin win = CreateWindow("Dropping", "Are you sure you wish to drop: " + released.data.name);
                        win.ShowDialog();

                        if (ConfirmResult) {
                            CourseList_Clear(released.name);
                            released.ResetPosition();
                            ClassSection other = released.other;
                            if (other != null) {
                                if (other.onGrid) {
                                    other.ResetPosition();
                                    other.HideConnected();
                                }
                            }
                        } else {
                            released.Margin = released.originalMargin;
                            released.OnGridPlace(true);
                        }
                    } else {
                        released.ResetPosition();
                    }
                } else {
                    if (released.onGrid) {
                        released.Margin = released.originalMargin;
                        released.OnGridPlace(true);
                    } else {
                        released.ResetPosition();
                    }
                }
                released = null;
            }
        }

        // returns the coordinates
        private bool IsHoveringCell(Border b, Point p) {
            bool inX = b.Margin.Left <= p.X && b.Margin.Left + b.ActualWidth >= p.X;
            bool inY = b.Margin.Top <= p.Y && b.Margin.Top + b.ActualHeight >= p.Y;
            return inX && inY;
        }

        private bool IsReleasedConflicting(GridSection section) {
            int count = 0;
            foreach (UIElement ui in ScheduleGrid.Children) {
                GridSection gs = ui as GridSection;
                if (gs != null && !section.connected.Contains(gs) && gs.Visibility == Visibility.Visible) {
                    Point p = Mouse.GetPosition(gs);
                    if (IsHoveringGridSection(gs, p)) {
                        count++;
                        conflicting = gs;
                        break;
                    }
                }
            }
            return count >= 1;
        }

        private void PlaceOnGrid(GridSection gs) {
            Grid.SetRow(released, Grid.GetRow(gs));
            Grid.SetColumn(released, Grid.GetColumn(gs));
            Grid.SetRowSpan(released, Grid.GetRowSpan(gs));
            Grid.SetZIndex(released, Grid.GetZIndex(gs) - 10);
            released.VerticalAlignment = VerticalAlignment.Top;
            released.Height = gs.Height;
            released.Margin = gs.Margin;
            released.OnGridPlace();
            released.linked = gs;
            released = null;
        }

        // dropping a course onto the schedule
        private void ScheduleGrid_MouseUp(object sender, MouseButtonEventArgs e) {
            if (released != null) {
                bool dropConflict = false;
                foreach (UIElement ui in ScheduleGrid.Children) {
                    GridSection gs = ui as GridSection;
                    if (gs != null) {
                        Point p = Mouse.GetPosition(gs);
                        if (IsHoveringGridSection(gs, p) && gs.parentClass == released) {
                            if (!IsReleasedConflicting(gs)) {
                                PlaceOnGrid(gs);
                                break;
                            } else {
                                if (!conflicting.parentClass.data.name.Equals(gs.parentClass.data.name)) {
                                    ConfirmationWin win = CreateWindow("Dropping", "Are you sure you wish to drop " + conflicting.parentClass.data.name + " to add " + released.data.name + "?");
                                    win.ShowDialog();

                                    if (ConfirmResult) {
                                        dropConflict = true;
                                        PlaceOnGrid(gs);
                                    }
                                }
                            }
                        }
                    }
                }
                if (dropConflict) {
                    conflicting.parentClass.ResetPosition();
                    conflicting.parentClass.HideConnected();
                    ClassSection other = conflicting.parentClass.other;
                    if (other != null) {
                        if (other.onGrid) {
                            other.ResetPosition();
                            other.HideConnected();
                        }
                    }
                    foreach (UIElement ui in ListOfCourses.Children) {
                        CourseListItem cli = ui as CourseListItem;
                        if (cli != null && conflicting.parentClass.data.name.Equals(cli.name)) {
                            cli.SetEnrollment(false);
                            ListOfCourses.Children.Remove(cli);
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

        // generate list of available courses
        private List<ClassData> GetClasses() {
            ClassData cpsc231 = new ClassData("CPSC 231", "Introduction to Computer Science for Computer Science Majors I", "Introduction to problem solving, the analysis and design of small-scale computational systems, and implementation using a procedural programming language. For computer science majors.", "Nathaly Verwaal", "10:00AM-10:50AM MWF, 11:00AM-12:15PM TR", new TimeSlot[] { new TimeSlot(new int[] { 1, 3, 5 }, 3, 60, "ST 140"), new TimeSlot(new int[] { 2, 4 }, 4, 75, "Admin 142") }, true, new TimeSlot[] { new TimeSlot(new int[] { 2, 4 }, 4, 60, "MS 176"), new TimeSlot(new int[] { 5 }, 1, 120, "MS 365") });
            ClassData cpsc413 = new ClassData("CPSC 413", "Design and Analysis of Algorithms I", "Techniques for the analysis of algorithms, including counting, summation, recurrences, and asymptotic relations; techniques for the design of efficient algorithms, including greedy methods, divide and conquer, and dynamic programming; examples of their application; an introduction to tractable and intractable problems.", "Peter Hoyer", "9:30AM-10:45AM TR", new TimeSlot[] { new TimeSlot(new int[] { 2, 4 }, 2.5f, 90, "ST 141") }, true, new TimeSlot[] { new TimeSlot(new int[] { 2, 4 }, 4, 60, "ST 070"), new TimeSlot(new int[] { 1, 3 }, 4, 60, "ST 068") });
            ClassData cpsc481 = new ClassData("CPSC 481", "Human-Computer Interaction I", "Fundamental theory and practice of the design, implementation, and evaluation of human-computer interfaces. Topics include: principles of design; methods for evaluating interfaces with or without user involvement; techniques for prototyping and implementing graphical user interfaces.", "Ehud Sharlin", "10:00AM-10:50AM MWF", new TimeSlot[] { new TimeSlot(new int[] { 1, 3, 5 }, 3, 60, "ICT 121") }, true, new TimeSlot[] { new TimeSlot(new int[] { 1 }, 5, 120, "MS 156"), new TimeSlot(new int[] { 5 }, 5, 120, "MS 121") });
            ClassData math211 = new ClassData("MATH 211", "Linear Methods I", "Systems of equations and matrices, vectors, matrix representations and determinants. Complex numbers, polar form, eigenvalues, eigenvectors. Applications.", "Thi Dinh", "1:00PM-1:50PM MWF, 2:00PM-3:15PM TR", new TimeSlot[] { new TimeSlot(new int[] { 1, 3, 5 }, 6, 60, "Science A 123"), new TimeSlot(new int[] { 2, 4 }, 7, 75, "Education 172") }, true, new TimeSlot[] { new TimeSlot(new int[] { 2, 4 }, 1, 60, "SB 020"), new TimeSlot(new int[] { 2, 4 }, 2, 60, "Kines 070") });
            ClassData ling201 = new ClassData("LING 201", "Introduction to Linguistics I", "Introduction to the scientific study of language, including the analysis of word, sentence, and sound structure, and the exploration of language as a human, biological, social, and historical phenomenon.", "Stephen Winters", "2:00PM-2:50PM MWF", new TimeSlot[] { new TimeSlot(new int[] { 1, 3, 5 }, 7, 60, "Murray Fraiser Hall 160") }, false, null);
            ClassData phil314 = new ClassData("PHIL 314", "Information Technology Ethics", "A critical and analytical examination of ethical and legal problems arising in and about information technology. May include hacking, online privacy, intellectual property rights, artificial intelligence, globalization and regulation issues, cheating in online games, and others.", "Reid Buchanan", "2:00PM-3:15PM TR", new TimeSlot[] { new TimeSlot(new int[] { 2, 4 }, 7, 75, "ST 148") }, false, null);

            List<ClassData> data = new List<ClassData>(new ClassData[] { cpsc231, cpsc413, cpsc481, math211, ling201, phil314 });
            data = Shuffle(data);

            for (int i = 0; i < data.Count; i++) {
                data[i].brush = classColors[i];
            }
            data.Sort(ClassData.CompareClassData);
            return data;
        }

        private List<ClassData> Shuffle(List<ClassData> cs) {
            List<ClassData> copy = new List<ClassData>(cs.Count);
            for (int i = 0; i < copy.Capacity; i++) {
                int idx = rand.Next(0, cs.Count);
                copy.Add(cs.ElementAt(idx));
                cs.RemoveAt(idx);
            }
            return copy;
        }

        // searchbox entries
        private void SearchBox_KeyUp(object sender, KeyEventArgs e) {
            bool found = false;
            Border border = (ResultStack.Parent as ScrollViewer).Parent as Border;

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
            foreach (SearchItem item in items) {
                if (item.ClassName.Content.ToString().ToLower().StartsWith(query)) {
                    // The word starts with this... Autocomplete must work   
                    ResultStack.Children.Add(item);
                    found = true;
                }
            }

            if (!found) {
                ResultStack.Children.Add(new TextBlock() { Text = "No results found", Margin = new Thickness(2, 1, 0, 1), FontSize = 30 });
            }
        }

        private void CreateSearchItems() {
            items = new SearchItem[classes.Count];
            for (int i = 0; i < items.Length; i++) {
                ClassData data = classes[i];
                SearchItem item = new SearchItem(data.name, data.ToString());
                item.Course_title.Text = data.title;
                ClassSection lecture = new ClassSection(this, false, item.Sections, data, data.brush, item);
                item.Sections.Children.Add(lecture);
                if (data.hasTutorial) {
                    ClassSection tutorial = new ClassSection(this, true, item.Sections, data, data.brush, item);
                    item.Sections.Children.Add(tutorial);
                    tutorial.other = lecture;
                    lecture.other = tutorial;
                    lecture.Margin = new Thickness(0, 0, 5, 0);
                    tutorial.Margin = new Thickness(5, 0, 0, 0);
                }
                items[i] = item;
            }
        }

        public void ExpandSearchItem(SearchItem s) {
            SearchBox.Text = s.ClassName.Content.ToString().Split(' ')[0];
            SearchBox_KeyUp(this, null);
            foreach (UIElement ui in ResultStack.Children) {
                SearchItem si = ui as SearchItem;
                if (si != null) {
                    si.SetExpanded(false);
                }
            }
            s.SetExpanded(true);
        }

        private ConfirmationWin CreateWindow(string title, string text) {
            ConfirmationWin win = new ConfirmationWin(this);
            win.Owner = this;
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.top_win.Text = title;
            win.question.Content = text;
            return win;
        }

        // confirmation window
        private void Enroll_Click(object sender, RoutedEventArgs e) {
            List<CourseListItem> addList = new List<CourseListItem>();
            List<CourseListItem> dropList = new List<CourseListItem>();
            foreach (UIElement ui in ListOfCourses.Children) {
                CourseListItem cli = ui as CourseListItem;
                if (cli != null) {
                    if (cli.isChecked) {
                        addList.Add(cli);
                    } else {
                        if (enrolled.Contains(cli)) {
                            dropList.Add(cli);
                        }
                    }
                }
            }

            string prompt = "Are you sure you want to enroll in the following courses?";
            foreach (CourseListItem cli in addList) {
                prompt += "\n\t" + cli.name;
            }

            if (dropList.Count > 0) {
                prompt += "\nAnd drop the following courses?";
                foreach (CourseListItem cli in dropList) {
                    prompt += "\n\t" + cli.name;
                }
            }

            ConfirmationWin win = CreateWindow("Confirming Enrollment", prompt);
            win.ShowDialog();

            if (ConfirmResult) {
                enrolled = addList;
                foreach (CourseListItem cli in addList) {
                    cli.SetEnrollment(true);
                }
                foreach (CourseListItem cli in dropList) {
                    cli.SetEnrollment(false);
                }
            }
        }

        private bool IsHoveringGridSection(GridSection gs, Point p) {
            bool inX = gs.Margin.Left <= p.X && gs.Margin.Left + gs.ActualWidth >= p.X;
            bool a = gs.Margin.Top <= p.Y;
            bool b = gs.ActualHeight >= p.Y;
            bool inY = a && b;
            return inX && inY;
        }

        private bool IsHoveringGarbage(Image i) {
            Point p = Mouse.GetPosition(i);
            bool inX = i.Margin.Left <= p.X && i.Margin.Left + i.ActualWidth >= p.X;
            bool inY = i.Margin.Top <= p.Y && i.Margin.Top + i.ActualHeight >= p.Y;
            return inX && inY;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e) {
            if (released == null) {
                foreach (UIElement ui in ScheduleGrid.Children) {
                    GridSection gs = ui as GridSection;
                    if (gs != null) {
                        bool anyHovering = false;
                        foreach (GridSection g in gs.connected) {
                            Point p = Mouse.GetPosition(g);
                            anyHovering = anyHovering || IsHoveringGridSection(g, p);
                        }
                        if (anyHovering) {
                            gs.HighlightConnected();
                        } else {
                            gs.ShadowConnected();
                        }
                    }
                }
                if (IsHoveringGarbage(TrashEmpty) || IsHoveringGarbage(TrashFull)) {
                    Garbage_MouseEnter();
                } else {
                    Garbage_MouseLeave();
                }
            }
        }

        private void Garbage_MouseEnter() {
            TrashEmpty.Visibility = Visibility.Collapsed;
            TrashFull.Visibility = Visibility.Visible;
        }

        private void Garbage_MouseLeave() {
            TrashFull.Visibility = Visibility.Collapsed;
            TrashEmpty.Visibility = Visibility.Visible;
        }

        private void CourseList_Clear(string name) {
            foreach (UIElement ui in ListOfCourses.Children) {
                CourseListItem cli = ui as CourseListItem;
                if (cli != null && cli.name == name) {
                    ListOfCourses.Children.Remove(cli);
                    break;
                }
            }
        }
    }
}
