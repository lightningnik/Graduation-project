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
            FillComboBox();
        }

        private void FillComboBox()
        {
            Manager.connection.Close(); // КОСТЫЛЬ!!! Без этой строки выдает ошибку, что подключение не закрыто,
            // при закртытии подключения на странице StartPage
            Manager.connection.Open();
            string DispString = ("SELECT     DISTINCT   dbo.Disciplines.Name_Discipline " +
                "FROM dbo.Lessons INNER JOIN dbo.Group_List ON dbo.Lessons.Group_id = dbo.Group_List.Group_id INNER JOIN " +
                "dbo.Students ON dbo.Group_List.Student_id = dbo.Students.Student_id INNER JOIN " +
                "dbo.Disciplines ON dbo.Lessons.Discipline_id = dbo.Disciplines.Discipline_id " +
                "WHERE  dbo.Students.Student_id = @my_id");
            SqlCommand disp = new SqlCommand(DispString, Manager.connection);
            disp.Parameters.Add("@my_id", SqlDbType.Int);
            disp.Parameters["@my_id"].Value = Manager.my_id;
            SqlDataAdapter disp_da = new SqlDataAdapter(disp);
            DataTable disp_dt = new DataTable();
            disp_da.Fill(disp_dt);
            for (int i = 0; i < disp_dt.Rows.Count; i++)
            {
                Name_Discipline.Items.Add(disp_dt.Rows[i]["Name_Discipline"].ToString());
            }
            Manager.connection.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Name_Discipline_DropDownClosed(object sender, System.EventArgs e)
        {
            Manager.connection.Open();

            string EmpString = "SELECT        dbo.Marks.Mark, dbo.Marks.Date, dbo.Employees.Surname, dbo.Employees.Name, dbo.Employees.Patronymic, dbo.Work_Types.Work_Type " +
                "FROM dbo.Marks INNER JOIN " +
                "dbo.Students ON dbo.Marks.Student_id = dbo.Students.Student_id INNER JOIN " +
                "dbo.Lessons ON dbo.Marks.Lesson_id = dbo.Lessons.Lesson_id INNER JOIN " +
                "dbo.Disciplines ON dbo.Lessons.Discipline_id = dbo.Disciplines.Discipline_id INNER JOIN " +
                "dbo.Employees ON dbo.Lessons.Employee_id = dbo.Employees.Employee_id INNER JOIN " +
                "dbo.Work_Types ON dbo.Marks.Type_work_id = dbo.Work_Types.Type_id " +
                "WHERE dbo.Disciplines.Name_Discipline = @Name_Disp AND dbo.Students.Student_id = @myID";
            SqlCommand emp = new SqlCommand(EmpString, Manager.connection);
            emp.Parameters.Add("@myID", SqlDbType.Int);
            emp.Parameters["@myID"].Value = Manager.my_id;
            SqlDataAdapter emp_sda = new SqlDataAdapter(emp);
            SqlParameter Name_Disp_param = new SqlParameter("@Name_Disp", Name_Discipline.Text);
            emp.Parameters.Add(Name_Disp_param);

            DataTable emp_dt = new DataTable("Marks");
            emp_sda.Fill(emp_dt);
            MarksDataGrid.ItemsSource = emp_dt.DefaultView;
            Manager.connection.Close();
        }
    }
}
