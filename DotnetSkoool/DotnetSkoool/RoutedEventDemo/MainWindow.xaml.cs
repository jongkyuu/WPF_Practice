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

namespace RoutedEventDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OuterButton_Click(object sender, RoutedEventArgs e)  // 버블링 이벤트
        {
            MessageBox.Show("Outer Button Click!");
        }

        private void outerEllipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("Green Ellipse Click!");
            this.Title = "Green Ellipse has Changed the Title";

        }

        private void InnerButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Innter Button Click!");
        }

        private void OutterButton_MouseMove(object sender, MouseEventArgs e)
        {
            MessageBox.Show("Outter Button Mouse Move!");

        }
    }
}
