using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace LightningMarks
{
    /// <summary>
    /// Логика взаимодействия для StudentWindow.xaml
    /// </summary>
    public partial class StudentWindow : Window
    {
        public StudentWindow()
        {
            InitializeComponent();
            FillDataGrid();
        }
        private void FillDataGrid()
        {
            Manager.connection.Close(); // КОСТЫЛЬ!!! Без этой строки выдает ошибку, что подключение не закрыто,
                                        // при закртытии подключения на странице StartPage
            Manager.connection.Open();

            string EmpString = "SELECT dbo.Marks.Mark, dbo.Disciplines.Name_Discipline, dbo.Marks.Date, dbo.Employees.Surname, dbo.Employees.Name, dbo.Employees.Patronymic " +
                "FROM dbo.Marks INNER JOIN " +
                "dbo.Students ON dbo.Marks.Student_id = dbo.Students.Student_id INNER JOIN" +
                " dbo.Lessons ON dbo.Marks.Lesson_id = dbo.Lessons.Lesson_id INNER JOIN" +
                " dbo.Disciplines ON dbo.Lessons.Discipline_id = dbo.Disciplines.Discipline_id INNER JOIN " +
                "dbo.Employees ON dbo.Lessons.Employee_id = dbo.Employees.Employee_id WHERE dbo.Students.Student_id = @myID";
            SqlCommand emp = new SqlCommand(EmpString, Manager.connection);
            emp.Parameters.Add("@myID", SqlDbType.Int);
            emp.Parameters["@myID"].Value = Manager.my_id;
            SqlDataAdapter emp_sda = new SqlDataAdapter(emp);


            DataTable emp_dt = new DataTable("Marks");
            emp_sda.Fill(emp_dt);
            MarksDataGrid.ItemsSource = emp_dt.DefaultView;

            Manager.connection.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
