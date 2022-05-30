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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LightningMarks
{
    /// <summary>
    /// Логика взаимодействия для AdminStudentPage.xaml
    /// </summary>
    public partial class StudentPage : Page
    {
        public StudentPage()
        {
            InitializeComponent();
            FillDataGrid();
        }

        private void FillDataGrid()
        {
            Manager.connection.Close(); // КОСТЫЛЬ!!! Без этой строки выдает ошибку, что подключение не закрыто,
                                        // при закртытии подключения на странице StartPage
            Manager.connection.Open();

            string EmpString = "SELECT Student_id, Surname, Name, Patronymic, Date_Of_Birth, Phone_number, Mail, Password FROM Students";
            SqlCommand emp = new SqlCommand(EmpString, Manager.connection);
            SqlDataAdapter emp_sda = new SqlDataAdapter(emp);
            DataTable emp_dt = new DataTable("Students");
            emp_sda.Fill(emp_dt);
            StudentsDataGrid.ItemsSource = emp_dt.DefaultView;

            string LessonsString = "SELECT Group_id, Name_Group FROM Groups";
            SqlCommand lesson = new SqlCommand(LessonsString, Manager.connection);
            SqlDataAdapter lesson_sda = new SqlDataAdapter(lesson);
            DataTable lesson_dt = new DataTable("Groups");
            lesson_sda.Fill(lesson_dt);
            GroupsDataGrid.ItemsSource = lesson_dt.DefaultView;

            string GroupListString = "SELECT dbo.Groups.Name_Group, dbo.Students.Surname, dbo.Students.Name, dbo.Students.Patronymic, dbo.Students.Student_id " +
                "FROM dbo.Group_List INNER JOIN dbo.Groups ON dbo.Group_List.Group_id = dbo.Groups.Group_id INNER JOIN dbo.Students ON dbo.Group_List.Student_id = dbo.Students.Student_id";
            SqlCommand List = new SqlCommand(GroupListString, Manager.connection);
            SqlDataAdapter List_sda = new SqlDataAdapter(List);
            DataTable list_dt = new DataTable("Group_List");
            List_sda.Fill(list_dt);
            GroupListDataGrid.ItemsSource = list_dt.DefaultView;

            Manager.connection.Close();
        }

        private void StudentsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                Surname_textBox.Text = (string)row_selected["Surname"];
                Name_textBox.Text = (string)row_selected["Name"];
                Patronymic_textBox.Text = (string)row_selected["Patronymic"];
                Mail_textBox.Text = (string)row_selected["Mail"];
                DOB.SelectedDate = (System.DateTime)row_selected["Date_Of_Birth"];
                Phone_textBox.Text = (string)row_selected["Phone_number"];
                Password_textBox.Text = (string)row_selected["Password"];
                ID_TextBox.Text = row_selected["Student_id"].ToString();
                Student_ID_Text_Box.Text = row_selected["Student_id"].ToString();
            }
        }

        private void GroupsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                Group_ID_TextBox.Text = (string)row_selected["Group_id"].ToString();
                Name_Group_TextBox.Text = (string)row_selected["Name_Group"];
                Id_group_TextBox.Text = (string)row_selected["Group_id"].ToString();
            }
        }
        private void GroupListDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                Group_ID_TextBox.Text = (string)row_selected["Name_Group"];
                Student_ID_Text_Box.Text = (string)row_selected["Student_id"].ToString();
            }
        }

        private void Add_student_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Manager.connection.Open();
                string addEmp = "INSERT INTO dbo.Students VALUES (@Surname, @Name, @Patronymic,@DOB, @Phone, @Mail, @Password)";
                SqlCommand cmd = new SqlCommand(addEmp, Manager.connection);
                SqlParameter Surname_param = new SqlParameter("@Surname", Surname_textBox.Text);
                cmd.Parameters.Add(Surname_param);
                SqlParameter Name_param = new SqlParameter("@Name", Name_textBox.Text);
                cmd.Parameters.Add(Name_param);
                SqlParameter Patronymic_param = new SqlParameter("@Patronymic", Patronymic_textBox.Text);
                cmd.Parameters.Add(Patronymic_param);
                SqlParameter DOB_param = new SqlParameter("@DOB", DOB.SelectedDate); //DOB - Date of birth (Дата рождения)
                cmd.Parameters.Add(DOB_param);
                SqlParameter Phone_param = new SqlParameter("@Phone", Phone_textBox.Text);
                cmd.Parameters.Add(Phone_param);
                SqlParameter Mail_param = new SqlParameter("@Mail", Mail_textBox.Text);
                cmd.Parameters.Add(Mail_param);
                SqlParameter Password_param = new SqlParameter("@Password", Password_textBox.Text);
                cmd.Parameters.Add(Password_param);
                cmd.ExecuteNonQuery();
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.Number + " " + er.Message);
            }
            FillDataGrid();
            Manager.connection.Close();
        }

        private void Upd_student_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Manager.connection.Open();
                string addEmp = "UPDATE dbo.Students SET Surname = @Surname, Name = @Name, Patronymic = @Patronymic," +
                    "Date_Of_Birth = @DOB, Phone_number = @Phone, Mail = @Mail, Password = @Password WHERE (Student_id = @ID_value)";
                SqlCommand cmd = new SqlCommand(addEmp, Manager.connection);
                SqlParameter ID_param = new SqlParameter("@ID_value", ID_TextBox.Text);
                cmd.Parameters.Add(ID_param);
                SqlParameter Surname_param = new SqlParameter("@Surname", Surname_textBox.Text);
                cmd.Parameters.Add(Surname_param);
                SqlParameter Name_param = new SqlParameter("@Name", Name_textBox.Text);
                cmd.Parameters.Add(Name_param);
                SqlParameter Patronymic_param = new SqlParameter("@Patronymic", Patronymic_textBox.Text);
                cmd.Parameters.Add(Patronymic_param);
                SqlParameter DOB_param = new SqlParameter("@DOB", DOB.SelectedDate); //DOB - Date of birth (Дата рождения)
                cmd.Parameters.Add(DOB_param);
                SqlParameter Phone_param = new SqlParameter("@Phone", Phone_textBox.Text);
                cmd.Parameters.Add(Phone_param);
                SqlParameter Mail_param = new SqlParameter("@Mail", Mail_textBox.Text);
                cmd.Parameters.Add(Mail_param);
                SqlParameter Password_param = new SqlParameter("@Password", Password_textBox.Text);
                cmd.Parameters.Add(Password_param);
                cmd.ExecuteNonQuery();
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.Number + " " + er.Message);
            }
            FillDataGrid();
            Manager.connection.Close();
        }

        private void Del_student_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ID_TextBox.Text == "")
                {
                    MessageBox.Show("Пожалуйста, заполните поле для удаления!!!");
                }
                else
                {
                    Manager.connection.Open();
                    string Delete = "DELETE FROM Students WHERE Student_id = (@ID_value)";
                    SqlCommand cmd = new SqlCommand(Delete, Manager.connection);
                    SqlParameter Delete_param = new SqlParameter("@ID_value", ID_TextBox.Text);
                    cmd.Parameters.Add(Delete_param);
                    cmd.ExecuteNonQuery();
                    FillDataGrid();
                    MessageBox.Show("Запись удалена!!!");
                }
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.Number + " " + er.Message);
            }
            Manager.connection.Close();
        }

        private void Add_in_Group_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Manager.connection.Open();
                string addList = "INSERT INTO Group_List VALUES (@Student_id,@Group_id)";
                SqlCommand cmd = new SqlCommand(addList, Manager.connection);
                SqlParameter Student_id_param = new SqlParameter("@Student_id", Student_ID_Text_Box.Text);
                cmd.Parameters.Add(Student_id_param);
                SqlParameter Group_id_param = new SqlParameter("@Group_id", Group_ID_TextBox.Text);
                cmd.Parameters.Add(Group_id_param);
                cmd.ExecuteNonQuery();
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.Number + " " + er.Message);
            }
            FillDataGrid();
            Manager.connection.Close();
        }

        private void Upd_in_Group_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Manager.connection.Open();
                string updList = "UPDATE dbo.Group_List SET Student_id = @Student_id, Group_id = @Group_id";
                SqlCommand cmd = new SqlCommand(updList, Manager.connection);
                SqlParameter Student_id_param = new SqlParameter("@Student_id", Student_ID_Text_Box.Text);
                cmd.Parameters.Add(Student_id_param);
                SqlParameter Group_id_param = new SqlParameter("@Group_id", Group_ID_TextBox.Text);
                cmd.Parameters.Add(Group_id_param);
                cmd.ExecuteNonQuery();
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.Number + " " + er.Message);
            }
            FillDataGrid();
            Manager.connection.Close();
        }

        private void Del_in_Group_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Add_Group_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Manager.connection.Open();
                string addEmp = "INSERT INTO dbo.Groups VALUES (@Name_Group)";
                SqlCommand cmd = new SqlCommand(addEmp, Manager.connection);
                SqlParameter Name_Group_param = new SqlParameter("@Name_Group", Name_Group_TextBox.Text);
                cmd.Parameters.Add(Name_Group_param);
                cmd.ExecuteNonQuery();
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.Number + " " + er.Message);
            }
            FillDataGrid();
            Manager.connection.Close();
        }

        private void Upd_Group_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Manager.connection.Open();
                string addEmp = "UPDATE dbo.Groups SET Name_Group = @Name_Group WHERE (Group_id = @ID_value)";
                SqlCommand cmd = new SqlCommand(addEmp, Manager.connection);
                SqlParameter ID_param = new SqlParameter("@ID_value", Id_group_TextBox.Text);
                cmd.Parameters.Add(ID_param);
                SqlParameter Name_Group_param = new SqlParameter("@Name_Group", Name_Group_TextBox.Text);
                cmd.Parameters.Add(Name_Group_param);
                cmd.ExecuteNonQuery();
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.Number + " " + er.Message);
            }
            FillDataGrid();
            Manager.connection.Close();
        }

        private void Del_Group_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Manager.connection.Open();
                string addEmp = "DELETE FROM dbo.Groups WHERE (Group_id = @ID_value)";
                SqlCommand cmd = new SqlCommand(addEmp, Manager.connection);
                SqlParameter ID_param = new SqlParameter("@ID_value", Id_group_TextBox.Text);
                cmd.Parameters.Add(ID_param);
                SqlParameter Name_Group_param = new SqlParameter("@Name_Group", Name_Group_TextBox.Text);
                cmd.Parameters.Add(Name_Group_param);
                cmd.ExecuteNonQuery();
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.Number + " " + er.Message);
            }
            FillDataGrid();
            Manager.connection.Close();
        }

        private void Surname_textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Surname_textBox.Text == "Фамилия")
            {
                Surname_textBox.Text = "";
            }
        }

        private void Name_textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Name_textBox.Text == "Имя")
            {
                Name_textBox.Text = "";
            }
        }

        private void Patronymic_textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Patronymic_textBox.Text == "Отчество")
            {
                Patronymic_textBox.Text = "";
            }
        }

        private void Mail_textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Mail_textBox.Text == "Почтовый адрес")
            {
                Mail_textBox.Text = "";
            }
        }

        private void Phone_textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Phone_textBox.Text == "Номер телефона")
            {
                Phone_textBox.Text = "";
            }
        }

        private void Password_textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Password_textBox.Text == "Пароль")
            {
                Password_textBox.Text = "";
            }
        }

        private void ID_TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ID_TextBox.Text == "ID студента")
            {
                ID_TextBox.Text = "";
            }
        }

        private void Id_group_TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Id_group_TextBox.Text == "ID группы")
            {
                Id_group_TextBox.Text = "";
            }
        }

        private void Name_Group_TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Name_Group_TextBox.Text == "Название группы")
            {
                Name_Group_TextBox.Text = "";
            }
        }

        private void Student_ID_Text_Box_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Student_ID_Text_Box.Text == "ID студента")
            {
                Student_ID_Text_Box.Text = "";
            }
        }

        private void Name_in_Group_TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Group_ID_TextBox.Text == "Название группы")
            {
                Group_ID_TextBox.Text = "";
            }
        }

        private void Surname_textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Surname_textBox.Text == "")
            {
                Surname_textBox.Text = "Фамилия";
            }
        }
        private void Name_textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Name_textBox.Text == "")
            {
                Name_textBox.Text = "Имя";
            }
        }

        private void Patronymic_textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Patronymic_textBox.Text == "")
            {
                Patronymic_textBox.Text = "Отчество";
            }
        }

        private void Mail_textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Mail_textBox.Text == "")
            {
                Mail_textBox.Text = "Почтовый адрес";
            }
        }

        private void Phone_textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Phone_textBox.Text == "")
            {
                Phone_textBox.Text = "Номер телефона";
            }
        }

        private void Password_textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Password_textBox.Text == "")
            {
                Password_textBox.Text = "Пароль";
            }
        }

        private void ID_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ID_TextBox.Text == "")
            {
                ID_TextBox.Text = "ID студента";
            }
        }

        private void Id_group_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Id_group_TextBox.Text == "")
            {
                Id_group_TextBox.Text = "ID группы";
            }
        }

        private void Name_Group_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Name_Group_TextBox.Text == "")
            {
                Name_Group_TextBox.Text = "Название группы";
            }
        }

        private void Student_ID_Text_Box_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Student_ID_Text_Box.Text == "")
            {
                Student_ID_Text_Box.Text = "ID студента";
            }
        }

        private void Name_in_Group_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Group_ID_TextBox.Text == "")
            {
                Group_ID_TextBox.Text = "Название группы";
            }
        }

        private void Clear_Text1_Click(object sender, RoutedEventArgs e)
        {
            Surname_textBox.Text = "Фамилия";
            Name_textBox.Text = "Имя";
            Patronymic_textBox.Text = "Отчество";
            Mail_textBox.Text = "Почтовый адрес";
            Phone_textBox.Text = "Номер телефона";
            Password_textBox.Text = "Пароль";
            ID_TextBox.Text = "ID студента";
        }

        private void Clear_Text2_Click(object sender, RoutedEventArgs e)
        {
            Id_group_TextBox.Text = "ID группы";
            Name_Group_TextBox.Text = "Название группы";
        }

        private void Clear_Text3_Click(object sender, RoutedEventArgs e)
        {
            Student_ID_Text_Box.Text = "ID студента";
            Group_ID_TextBox.Text = "Название группы";
        }
    }
}
