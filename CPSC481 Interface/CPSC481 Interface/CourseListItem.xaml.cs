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

namespace CPSC481_Interface
{
    /// <summary>
    /// Interaction logic for CourseList.xaml
    /// </summary>
    public partial class CourseListItem : UserControl
    {
        private MainWindow window;
        public Brush color;
        public string name;
        public ClassSection section;
        public ClassSection sectionOther;
        public GridSection linked;

        public CourseListItem(ClassSection section, ClassSection other, MainWindow window)
        {
            InitializeComponent();
            this.section = section;
            this.sectionOther = other;
            this.window = window;
            this.section.name = name;
            this.color = section.color;
            this.linked = section.linked;

            this.SectionType.Content = name;
            this.BG.Fill = color;
        }

        private void CourseListItem_Checked(object sender, RoutedEventArgs e)
        {
            if (linked != null)
            {
                linked.ShowConnected();
                sectionOther.linked.ShowConnected();
            }
        }

        private void CourseListitem_Unchecked(object sender, RoutedEventArgs e)
        {
            if (linked != null) {
                linked.HideConnected();
                sectionOther.linked.HideConnected();
            }
        }

    }
}
