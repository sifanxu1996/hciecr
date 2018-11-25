﻿using System;
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

    // TimeSlot class
    public class TimeSlot {
        public int[] days;
        public int startTime;
        public int duration;

        public TimeSlot(int[] days, int startTime, int duration) {
            this.days = days;
            this.startTime = startTime;
            this.duration = duration;
        }

    }

    // ClassData class
    public class ClassData {

        public string name, title, description, professor, times;
        public TimeSlot[] timeSlots;
        public bool hasTutorial;

        public ClassData(string name, string title, string description, string professor, string times, TimeSlot[] timeSlots, bool hasTutorial) {
            this.name = name;
            this.title = title;
            this.description = description;
            this.professor = professor;
            this.times = times;
            this.timeSlots = timeSlots;
            this.hasTutorial = hasTutorial;
        }

        public static int CompareClassData(ClassData cd1, ClassData cd2) {
            return cd1.name.CompareTo(cd2.name);
        }

        public override string ToString() {
            return String.Format("{0}\n\n{1}\n\nProfessor: {2}\n\nTimes: {3}", new string[] { title, description, professor, times });
        }

        internal void OnGridPlace()
        {
            throw new NotImplementedException();
        }
    }

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

        // returns the coordinates
        private bool IsHoveringCell(Border b, Point p) {
            bool inX = b.Margin.Left <= p.X && b.Margin.Left + b.ActualWidth >= p.X;
            bool inY = b.Margin.Top <= p.Y && b.Margin.Top + b.ActualHeight >= p.Y;
            return inX && inY;
        }

        // returns true if course section available at that particular row, col
        private bool CellMatches(ClassSection released, int col, int row) {
            TimeSlot[] lectureSlots = released.data.timeSlots;

            for (int i = 0; i < lectureSlots.Length; i++) {
                int[] daySlots = lectureSlots[i].days;
                int startTime = lectureSlots[i].startTime;
                int duration = lectureSlots[i].duration;

                if (daySlots.Contains(col) && startTime == row) {
                    return true;
                }
            }

            return false;
        }

        // dropping a course onto the schedule
        private void ScheduleGrid_MouseUp(object sender, MouseButtonEventArgs e) {
            if (released != null) {
                foreach (UIElement ui in ScheduleGrid.Children) {
                    Border b = ui as Border;
                    if (b != null) {
                        int col = Grid.GetColumn(b);
                        int row = Grid.GetRow(b);
                        Point p = Mouse.GetPosition(b);
                        if (IsHoveringCell(b, p) && CellMatches(released, col, row)) {
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

        // generate list of available courses
        private List<ClassData> GetData() {

            List<ClassData> data = new List<ClassData>();
            data.Add(new ClassData("CPSC 231", "Introduction to Computer Science for Computer Science Majors I", "Introduction to problem solving, the analysis and design of small-scale computational systems, and implementation using a procedural programming language. For computer science majors.", "Nathaly Verwaal", "10AM-10:50AM MWF, 11:00AM-12:15PM TR", new TimeSlot[] {new TimeSlot(new int[] {1,3,5}, 3, 4), new TimeSlot(new int[] {2,4}, 4, 5)}, true));
            /*data.Add(new ClassData("CPSC 413", "Design and Analysis of Algorithms I", "Techniques for the analysis of algorithms, including counting, summation, recurrences, and asymptotic relations; techniques for the design of efficient algorithms, including greedy methods, divide and conquer, and dynamic programming; examples of their application; an introduction to tractable and intractable problems.", "Peter Hoyer", "9:30AM-10:45AM TR", true));
            data.Add(new ClassData("CPSC 481", "Human-Computer Interaction I", "Fundamental theory and practice of the design, implementation, and evaluation of human-computer interfaces. Topics include: principles of design; methods for evaluating interfaces with or without user involvement; techniques for prototyping and implementing graphical user interfaces.", "Ehud Sharlin", "10AM-10:50AM MWF", true));

            data.Add(new ClassData("MATH 211", "Linear Methods I", "Systems of equations and matrices, vectors, matrix representations and determinants. Complex numbers, polar form, eigenvalues, eigenvectors. Applications.", "Thi Dinh", "1PM-1:50PM MWF, 3:00PM-4:15PM TR", true));

            data.Add(new ClassData("LING 201", "Introduction to Linguistics I", "Introduction to the scientific study of language, including the analysis of word, sentence, and sound structure, and the exploration of language as a human, biological, social, and historical phenomenon.", "Stephen Winters", "2:00PM-2:50PM MWF", false));

            data.Add(new ClassData("PHIL 314", "Information Technology Ethics", "A critical and analytical examination of ethical and legal problems arising in and about information technology. May include hacking, online privacy, intellectual property rights, artificial intelligence, globalization and regulation issues, cheating in online games, and others.", "Reid Buchanan", "2:00PM-3:15PM TR", false));
            */
            data.Sort(ClassData.CompareClassData);
            return data;
        }

        // searchbox entries
        private void SearchBox_KeyUp(object sender, KeyEventArgs e) {
            bool found = false;
            Border border = (ResultStack.Parent as ScrollViewer).Parent as Border;
            List<ClassData> data = GetData();

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
            foreach (ClassData obj in data) {
                if (obj.name.ToLower().StartsWith(query)) {
                    // The word starts with this... Autocomplete must work   
                    AddItem(obj);
                    found = true;
                }
            }

            if (!found) {
                ResultStack.Children.Add(new TextBlock() { Text = "No results found.", Margin = new Thickness(2, 1, 0, 1) });
            }
        }

        // generating random brushes for courses
        private Brush GetRandomBrush() {
            byte r = (byte) rand.Next(0, 256);
            byte g = (byte) rand.Next(0, 256);
            byte b = (byte) rand.Next(0, 256);
            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        // Generate the Course selectors for drag-and-drop onto the schedule
        private void AddItem(ClassData data) {
            Brush brush = GetRandomBrush();
            SearchItem item = new SearchItem(data.name, data.ToString());
            ClassSection lecture = new ClassSection(this, "Lecture", item.Sections, data.name, brush, data);
            item.Sections.Children.Add(lecture);
            if (data.hasTutorial) {
                ClassSection tutorial = new ClassSection(this, "Tutorial", item.Sections, data.name, brush, data);
                item.Sections.Children.Add(tutorial);
            }
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

        // confirmation window
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConfirmationWin win = new ConfirmationWin(this);
            win.Owner = this;
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.ShowDialog();
            
        }
    }
}
