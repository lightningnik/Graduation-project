using System.Windows;

namespace LightningMarks.Windows
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            Manager.MainFrame = MainFrame;
        }

        private void Update_Students_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new StudentPage());
        }

        private void Update_Discplines_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new DisciplinePage());
        }

        private void Update_Employees_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new EmployeesPage());
        }

        private void Update_Marks_Click(object sender, RoutedEventArgs e)
        {
           // Manager.MainFrame.Navigate(new MarksPage());
        }


        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
