using PlanFlowApp.Views.Pages;
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

namespace PlanFlowApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new PlanPage());
        }
        private void AssignmentsBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AssignmentsPage());
        }

        private void PlanBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PlanPage());
        }

        private void WorkersBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new WorkersPage());
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SettingsPage());
        }
    }
}
