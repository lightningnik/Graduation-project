using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Shapes;

namespace LightningMarks.Windows
{
    /// <summary>
    /// Логика взаимодействия для TeacherWindow.xaml
    /// </summary>
    public partial class TeacherWindow : Window
    {
        public TeacherWindow()
        {
            InitializeComponent();
            FillComboBox();
        }

        private void FillComboBox()
        {
            Manager.connection.Close(); // КОСТЫЛЬ!!! Без этой строки выдает ошибку, что подключение не закрыто,
            // при закртытии подключения на странице StartPage
            Manager.connection.Open();
            string ListString = ("SELECT DISTINCT dbo.Groups.Name_Group " +
                "FROM dbo.Lessons INNER JOIN " +
                "dbo.Groups ON dbo.Lessons.Group_id = dbo.Groups.Group_id WHERE Employee_id = @my_id");
            SqlCommand list = new SqlCommand(ListString, Manager.connection);
            list.Parameters.Add("@my_id", SqlDbType.Int);
            list.Parameters["@my_id"].Value = Manager.my_id;
            SqlDataAdapter da = new SqlDataAdapter(list);
            DataTable dt = new DataTable();
            da.Fill(dt);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Name_Group.Items.Add(dt.Rows[i]["Name_Group"].ToString());
            }

            //string DispString = ("SELECT DISTINCT dbo.Disciplines.Name_Discipline " +
            //    "FROM dbo.Lessons INNER JOIN " +
            //    "dbo.Disciplines ON dbo.Lessons.Discipline_id = dbo.Disciplines.Discipline_id WHERE Employee_id = @my_id");
            //SqlCommand disp = new SqlCommand(DispString, Manager.connection);
            //disp.Parameters.Add("@my_id", SqlDbType.Int);
            //disp.Parameters["@my_id"].Value = Manager.my_id;
            //SqlDataAdapter disp_da = new SqlDataAdapter(disp);
            //DataTable disp_dt = new DataTable();
            //disp_da.Fill(disp_dt);
            //for (int i = 0; i < disp_dt.Rows.Count; i++)
            //{
            //    Name_Discipline.Items.Add(disp_dt.Rows[i]["Name_Discipline"].ToString());
            //}
            Manager.connection.Close();
        }

        private void Set_Mark_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Manager.connection.Open();
                string addEmp = "INSERT INTO dbo.Marks VALUES (@Student_id, @Lesson_id, @Mark, @Date)";
                SqlCommand cmd = new SqlCommand(addEmp, Manager.connection);
                SqlParameter Student_id_param = new SqlParameter("@Student_id", ID_Student.Text);
                cmd.Parameters.Add(Student_id_param);
                SqlParameter Lesson_id_param = new SqlParameter("@Lesson_id", ID_Discipline.Text);
                cmd.Parameters.Add(Lesson_id_param);
                SqlParameter Mark_param = new SqlParameter("@Mark", MarkComboBox.Text);
                cmd.Parameters.Add(Mark_param);
                SqlParameter Date_param = new SqlParameter("@Date", Date.SelectedDate);
                cmd.Parameters.Add(Date_param);
                cmd.ExecuteNonQuery();
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.Number + " " + er.Message);
            }
            Manager.connection.Close();
        }

        private void Upd_Mark_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Manager.connection.Open();
                string addEmp = "UPDATE dbo.Marks SET dbo.Marks.Student_id = @Student_id, dbo.Marks.Lesson_id = @Lesson_id, dbo.Marks.Mark = @Mark, dbo.Marks.Date = @Date WHERE (dbo.Marks.ID = @ID)";
                SqlCommand cmd = new SqlCommand(addEmp, Manager.connection);
                SqlParameter ID_param = new SqlParameter("@ID", ID.Text);
                cmd.Parameters.Add(ID_param);
                SqlParameter Student_id_param = new SqlParameter("@Student_id", ID_Student.Text);
                cmd.Parameters.Add(Student_id_param);
                SqlParameter Lesson_id_param = new SqlParameter("@Lesson_id", ID_Discipline.Text);
                cmd.Parameters.Add(Lesson_id_param);
                SqlParameter Mark_param = new SqlParameter("@Mark", MarkComboBox.Text);
                cmd.Parameters.Add(Mark_param);
                SqlParameter Date_param = new SqlParameter("@Date", Date.SelectedDate);
                cmd.Parameters.Add(Date_param);
                cmd.ExecuteNonQuery();
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.Number + " " + er.Message);
            }
            Manager.connection.Close();
        }

        private void Del_Marks_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Manager.connection.Open();
                string addEmp = "DELETE FROM dbo.Marks WHERE ID = @ID";
                SqlCommand cmd = new SqlCommand(addEmp, Manager.connection);
                SqlParameter ID_param = new SqlParameter("@ID", ID.Text);
                cmd.Parameters.Add(ID_param);
                cmd.ExecuteNonQuery();
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.Number + " " + er.Message);
            }
            Manager.connection.Close();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void ID_Student_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ID_Student.Text == "ID студента")
            {
                ID_Student.Text = "";
            }
        }

        private void ID_Discipline_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ID_Discipline.Text == "ID дисциплины")
            {
                ID_Discipline.Text = "";
            }
        }

        private void ID_Student_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ID_Student.Text == "")
            {
                ID_Student.Text = "ID студента";
            }
        }

        private void ID_Discipline_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ID_Discipline.Text == "")
            {
                ID_Discipline.Text = "ID дисциплины";
            }
        }

        private void ID_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ID.Text == "ID записи")
            {
                ID.Text = "";
            }
        }

        private void ID_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ID.Text == "")
            {
                ID.Text = "ID записи";
            }
        }

        private void GroupListDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                ID_Student.Text = row_selected["Student_id"].ToString();
            }
        }

        private void DisciplineDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                ID_Discipline.Text = row_selected["Lesson_id"].ToString();
            }
        }

        private void MarksDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                ID_Student.Text = row_selected["Student_id"].ToString();
                ID_Discipline.Text = row_selected["Lesson_id"].ToString();
                ID.Text = row_selected["ID"].ToString();
                MarkComboBox.Text = row_selected["Mark"].ToString();
                Date.SelectedDate = (System.DateTime)row_selected["Date"];
            }
        }

        private void OutputButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.connection.Open();

            string ListString = "SELECT dbo.Students.Student_id, dbo.Students.Surname, dbo.Students.Name, dbo.Students.Patronymic, dbo.Groups.Name_Group " +
                "FROM dbo.Groups INNER JOIN " +
                "dbo.Group_List ON dbo.Groups.Group_id = dbo.Group_List.Group_id INNER JOIN " +
                "dbo.Students ON dbo.Group_List.Student_id = dbo.Students.Student_id WHERE dbo.Groups.Name_Group = @Name_Group";
            SqlCommand list = new SqlCommand(ListString, Manager.connection);
            list.Parameters.Add("@myid", SqlDbType.Int);
            list.Parameters["@myid"].Value = Manager.my_id;
            SqlParameter Name_Group_param = new SqlParameter("@Name_Group", Name_Group.Text);
            list.Parameters.Add(Name_Group_param);
            list.ExecuteNonQuery();
            SqlDataAdapter list_sda = new SqlDataAdapter(list);
            DataTable list_dt = new DataTable("Marks");
            list_sda.Fill(list_dt);
            GroupListDataGrid.ItemsSource = list_dt.DefaultView;


            string DispString = "SELECT dbo.Disciplines.Discipline_id, dbo.Disciplines.Name_Discipline, dbo.Groups.Name_Group, dbo.Lessons.Employee_id, dbo.Lessons.Lesson_id " +
                "FROM dbo.Lessons INNER JOIN " +
                "dbo.Disciplines ON dbo.Lessons.Discipline_id = dbo.Disciplines.Discipline_id INNER JOIN " +
                "dbo.Groups ON dbo.Lessons.Group_id = dbo.Groups.Group_id WHERE dbo.Lessons.Employee_id = @myid AND dbo.Groups.Name_Group = @Name_disp_Group";
            SqlCommand disp = new SqlCommand(DispString, Manager.connection);
            disp.Parameters.Add("@myid", SqlDbType.Int);
            disp.Parameters["@myid"].Value = Manager.my_id;
            SqlParameter Name_Group_disp_param = new SqlParameter("@Name_disp_Group", Name_Group.Text);
            disp.Parameters.Add(Name_Group_disp_param);
            disp.ExecuteNonQuery();
            SqlDataAdapter disp_sda = new SqlDataAdapter(disp);
            DataTable disp_dt = new DataTable("Lessons");
            disp_sda.Fill(disp_dt);
            DisciplineDataGrid.ItemsSource = disp_dt.DefaultView;


            string MrkString = "SELECT * " +
                "FROM dbo.Mark_Upd WHERE dbo.Mark_Upd.Name_Group = @Name_mrk_Group AND dbo.Mark_Upd.Name_Discipline = @Name_Discipline_mrk";
            SqlCommand mrk = new SqlCommand(MrkString, Manager.connection);
            mrk.Parameters.Add("@myid", SqlDbType.Int);
            mrk.Parameters["@myid"].Value = Manager.my_id;
            SqlParameter Name_Group_mrk_param = new SqlParameter("@Name_mrk_Group", Name_Group.Text);
            mrk.Parameters.Add(Name_Group_mrk_param);
            SqlParameter Name_Disicpiline_mrk_param = new SqlParameter("@Name_Discipline_mrk", Name_Discipline.Text);
            mrk.Parameters.Add(Name_Disicpiline_mrk_param);
            mrk.ExecuteNonQuery();
            SqlDataAdapter mrk_sda = new SqlDataAdapter(mrk);
            DataTable mrk_dt = new DataTable("Mark_Upd");
            mrk_sda.Fill(mrk_dt);
            MarkDataGrid.ItemsSource = mrk_dt.DefaultView;

            Manager.connection.Close();
        }

        private void TotalMark_Click(object sender, RoutedEventArgs e)
        {
            Teacher_Total_Mark_Window totalWindow = new Teacher_Total_Mark_Window();
            totalWindow.Show();
            this.Close();
        }

         private void Name_Group_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string DispString = ("SELECT DISTINCT dbo.Disciplines.Name_Discipline, dbo.Groups.Name_Group " +
               "FROM dbo.Lessons INNER JOIN " +
               "dbo.Disciplines ON dbo.Lessons.Discipline_id = dbo.Disciplines.Discipline_id INNER JOIN " +
               "dbo.Groups ON dbo.Lessons.Group_id = dbo.Groups.Group_id WHERE Employee_id = @my_id AND dbo.Groups.Name_Group = @NameGroup");
            SqlCommand disp = new SqlCommand(DispString, Manager.connection);
            disp.Parameters.Add("@my_id", SqlDbType.Int);
            disp.Parameters["@my_id"].Value = Manager.my_id;
            SqlParameter Name_Group_param = new SqlParameter("@NameGroup", Name_Group.Text);
            disp.Parameters.Add(Name_Group_param);
            SqlDataAdapter disp_da = new SqlDataAdapter(disp);
            DataTable disp_dt = new DataTable();
            disp_da.Fill(disp_dt);
            for (int i = 0; i < disp_dt.Rows.Count; i++)
            {
                Name_Discipline.Items.Add(disp_dt.Rows[i]["Name_Discipline"].ToString());
            }
        }
    }
}
