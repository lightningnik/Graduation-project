using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace LightningMarks
{
    /// <summary>
    /// Логика взаимодействия для AdminEmployeesoage.xaml
    /// </summary>
    public partial class EmployeesPage : Page
    {

        public EmployeesPage()
        {
            InitializeComponent();
            FillDataGrid();
        }

        private void FillDataGrid()
        {
            Manager.connection.Close(); // КОСТЫЛЬ!!! Без этой строки выдает ошибку, что подключение не закрыто,
                                        // при закртытии подключения на странице StartPage
            Manager.connection.Open();

            string EmpString = "SELECT DISTINCT Employee_id, Surname, Name, Patronymic, Role_Employee, Date_Of_Birth, Phone_number, Mail, Password " +
            "FROM Employees";
            SqlCommand emp = new SqlCommand(EmpString, Manager.connection);
            SqlDataAdapter emp_sda = new SqlDataAdapter(emp);
            DataTable emp_dt = new DataTable("Employees");
            emp_sda.Fill(emp_dt);
            EmpDataGrid.ItemsSource = emp_dt.DefaultView;

            string LessonsString = "SELECT DISTINCT " +
                "dbo.Groups.Group_id, dbo.Groups.Name_Group, dbo.Disciplines.Discipline_id, dbo.Disciplines.Name_Discipline, dbo.Employees.Surname, dbo.Employees.Name, dbo.Employees.Patronymic, dbo.Lessons.Lesson_id, " +
                "dbo.Employees.Employee_id " +
                "FROM            dbo.Employees INNER JOIN " +
                "dbo.Lessons ON dbo.Employees.Employee_id = dbo.Lessons.Employee_id INNER JOIN " +
                "dbo.Disciplines ON dbo.Lessons.Discipline_id = dbo.Disciplines.Discipline_id INNER JOIN " +
                "dbo.Groups ON dbo.Lessons.Group_id = dbo.Groups.Group_id";
            SqlCommand lesson = new SqlCommand(LessonsString, Manager.connection);
            SqlDataAdapter lesson_sda = new SqlDataAdapter(lesson);
            DataTable lesson_dt = new DataTable("Lessons");
            lesson_sda.Fill(lesson_dt);
            LessonsDataGrid.ItemsSource = lesson_dt.DefaultView;

            string DisciplineString = "SELECT DISTINCT Discipline_id, Name_Discipline FROM Disciplines";
            SqlCommand disp = new SqlCommand(DisciplineString, Manager.connection);
            SqlDataAdapter disp_sda = new SqlDataAdapter(disp);
            DataTable disp_dt = new DataTable("Disciplines");
            disp_sda.Fill(disp_dt);
            DisciplineDataGrid.ItemsSource = disp_dt.DefaultView;

            string GroupString = "SELECT DISTINCT Group_id, Name_Group FROM Groups";
            SqlCommand grp = new SqlCommand(GroupString, Manager.connection);
            SqlDataAdapter grp_sda = new SqlDataAdapter(grp);
            DataTable grp_dt = new DataTable("Groups");
            grp_sda.Fill(grp_dt);
            GroupDataGrid.ItemsSource = grp_dt.DefaultView;

            Manager.connection.Close();
        }

        private void Clear()
        {
            Surname_textBox.Text = "Фамилия";
            Name_textBox.Text = "Имя";
            Patronymic_textBox.Text = "Отчество";
            Mail_textBox.Text = "Почтовый адрес";
            Phone_textBox.Text = "Номер телефона";
            Password_textBox.Text = "Пароль";
            ID_TextBox.Text = "ID сотрудника";
            Employee_ID_Textbox.Text = "ID сотрудника";
            Group_id.Text = "ID группы";
            ID_Discipline.Text = "ID дисциплины";
            ID.Text = "ID записи";
        }

        private void EmpDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                Role_ComboBox.Text = (string)row_selected["Role_Employee"];
                ID_TextBox.Text = row_selected["Employee_id"].ToString();
                Employee_ID_Textbox.Text = row_selected["Employee_id"].ToString();
            }
        }

        private void LessonsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                Employee_ID_Textbox.Text = row_selected["Employee_id"].ToString();
                ID.Text = row_selected["Lesson_id"].ToString();
                Group_id.Text = row_selected["Group_id"].ToString();
                ID_Discipline.Text = row_selected["Discipline_id"].ToString();
            }
        }

        private void GroupDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                Group_id.Text = row_selected["Group_id"].ToString();
            }
        }

        private void DisciplineDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                ID_Discipline.Text = row_selected["Discipline_id"].ToString();
            }
        }

        private void Add_Employee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Manager.connection.Open();
                string authorization = ("SELECT * FROM [dbo].[Employees] WHERE Surname = @Surname AND Name = @Name AND Patronymic = @Patronymic AND Role_Employee = @Role AND Date_Of_Birth = @DOB");
                SqlCommand cmd = new SqlCommand(authorization, Manager.connection);
                SqlParameter Surname_check_param = new SqlParameter("@Surname", Surname_textBox.Text);
                cmd.Parameters.Add(Surname_check_param);
                SqlParameter Name_check_param = new SqlParameter("@Name", Name_textBox.Text);
                cmd.Parameters.Add(Name_check_param);
                SqlParameter Patronymic_check_param = new SqlParameter("@Patronymic", Patronymic_textBox.Text);
                cmd.Parameters.Add(Patronymic_check_param);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    MessageBox.Show("Данная запись существует");
                }
                else
                {
                    reader.Close();
                    string addEmp = "INSERT INTO dbo.Employees VALUES (@Surname, @Name, @Patronymic, @Role,@DOB, @Phone, @Mail, @Password)";
                    SqlCommand command = new SqlCommand(addEmp, Manager.connection);
                    SqlParameter Surname_param = new SqlParameter("@Surname", Surname_textBox.Text);
                    command.Parameters.Add(Surname_param);
                    SqlParameter Name_param = new SqlParameter("@Name", Name_textBox.Text);
                    command.Parameters.Add(Name_param);
                    SqlParameter Patronymic_param = new SqlParameter("@Patronymic", Patronymic_textBox.Text);
                    command.Parameters.Add(Patronymic_param);
                    SqlParameter Role_param = new SqlParameter("@Role", Role_ComboBox.Text);
                    command.Parameters.Add(Role_param);
                    SqlParameter DOB_param = new SqlParameter("@DOB", DOB.SelectedDate); //DOB - Date of birth (Дата рождения)
                    command.Parameters.Add(DOB_param);
                    SqlParameter Phone_param = new SqlParameter("@Phone", Phone_textBox.Text);
                    command.Parameters.Add(Phone_param);
                    SqlParameter Mail_param = new SqlParameter("@Mail", Mail_textBox.Text);
                    command.Parameters.Add(Mail_param);
                    SqlParameter Password_param = new SqlParameter("@Password", Password_textBox.Text);
                    command.Parameters.Add(Password_param);
                    command.ExecuteNonQuery();
                    Clear();
                }
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.Number + " " + er.Message);
            }
            FillDataGrid();
            Manager.connection.Close();
        }

        private void Upd_Employee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Manager.connection.Open();
                string authorization = ("SELECT * FROM [dbo].[Employees] WHERE Surname = @Surname AND Name = @Name AND Patronymic = @Patronymic AND Role_Employee = @Role AND Date_Of_Birth = @DOB");
                SqlCommand cmd = new SqlCommand(authorization, Manager.connection);
                SqlParameter Surname_check_param = new SqlParameter("@Surname", Surname_textBox.Text);
                cmd.Parameters.Add(Surname_check_param);
                SqlParameter Name_check_param = new SqlParameter("@Name", Name_textBox.Text);
                cmd.Parameters.Add(Name_check_param);
                SqlParameter Patronymic_check_param = new SqlParameter("@Patronymic", Patronymic_textBox.Text);
                cmd.Parameters.Add(Patronymic_check_param);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    MessageBox.Show("Данная запись существует");
                }
                else
                {
                    reader.Close();
                    string addEmp = "UPDATE dbo.Employees SET Surname = @Surname, Name = @Name, Patronymic = @Patronymic, Role_Employee = @Role," +
                   "Date_Of_Birth = @DOB, Phone_number = @Phone, Mail = @Mail, Password = @Password WHERE (Employee_id = @ID_value)";
                    SqlCommand command = new SqlCommand(addEmp, Manager.connection);
                    SqlParameter ID_param = new SqlParameter("@ID_value", ID_TextBox.Text);
                    command.Parameters.Add(ID_param);
                    SqlParameter Surname_param = new SqlParameter("@Surname", Surname_textBox.Text);
                    command.Parameters.Add(Surname_param);
                    SqlParameter Name_param = new SqlParameter("@Name", Name_textBox.Text);
                    command.Parameters.Add(Name_param);
                    SqlParameter Patronymic_param = new SqlParameter("@Patronymic", Patronymic_textBox.Text);
                    command.Parameters.Add(Patronymic_param);
                    SqlParameter Role_param = new SqlParameter("@Role", Role_ComboBox.Text);
                    command.Parameters.Add(Role_param);
                    SqlParameter DOB_param = new SqlParameter("@DOB", DOB.SelectedDate); //DOB - Date of birth (Дата рождения)
                    command.Parameters.Add(DOB_param);
                    SqlParameter Phone_param = new SqlParameter("@Phone", Phone_textBox.Text);
                    command.Parameters.Add(Phone_param);
                    SqlParameter Mail_param = new SqlParameter("@Mail", Mail_textBox.Text);
                    command.Parameters.Add(Mail_param);
                    SqlParameter Password_param = new SqlParameter("@Password", Password_textBox.Text);
                    command.Parameters.Add(Password_param);
                    command.ExecuteNonQuery();
                    Clear();
                }
                Manager.connection.Open();
               
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.Number + " " + er.Message);
            }
            FillDataGrid();
            Manager.connection.Close();
        }

        private void Del_Employee_Click(object sender, RoutedEventArgs e)
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
                    string Delete = "DELETE FROM Employees WHERE Employee_id = (@Employee_id)";
                    SqlCommand cmd = new SqlCommand(Delete, Manager.connection);
                    SqlParameter Delete_param = new SqlParameter("@Employee_id", ID_TextBox.Text);
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

        private void Set_Discipline_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Manager.connection.Open();
                string authorization = ("SELECT * FROM [dbo].[Lessons] WHERE Group_id = @Group_id_param AND Employee_id = @Emp_id_param AND Discipline_id = @Disp_id_param");
                SqlCommand cmd = new SqlCommand(authorization, Manager.connection);
                SqlParameter Emp_id_param = new SqlParameter("@Emp_id_param", Employee_ID_Textbox.Text);
                cmd.Parameters.Add(Emp_id_param);
                SqlParameter Disp_id_param = new SqlParameter("@Disp_id_param", ID_Discipline.Text);
                cmd.Parameters.Add(Disp_id_param);
                SqlParameter Group_id_param = new SqlParameter("@Group_id_param", Group_id.Text);
                cmd.Parameters.Add(Group_id_param);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    MessageBox.Show("Данная запись существует");
                }
                else
                {
                    reader.Close();
                    string addLesson = "INSERT INTO dbo.Lessons VALUES (@Group_id_param, @Emp_id_param, @Disp_id_param)";
                    SqlCommand command = new SqlCommand(addLesson, Manager.connection);
                    SqlParameter Emp_id_insert_param = new SqlParameter("@Emp_id_param", Employee_ID_Textbox.Text);
                    command.Parameters.Add(Emp_id_insert_param);
                    SqlParameter Disp_id_insert_param = new SqlParameter("@Disp_id_param", ID_Discipline.Text);
                    command.Parameters.Add(Disp_id_insert_param);
                    SqlParameter Group_id_insert_param = new SqlParameter("@Group_id_param", Group_id.Text);
                    command.Parameters.Add(Group_id_insert_param);
                    command.ExecuteNonQuery();
                    Clear();
                }
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.Number + " " + er.Message);
            }
            FillDataGrid();
            Manager.connection.Close();
        }

        private void Upd_Discipline_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Manager.connection.Open();
                string authorization = ("SELECT * FROM [dbo].[Lessons] WHERE Group_id = @Group_id_param AND Employee_id = @Emp_id_param AND Discipline_id = @Disp_id_param");
                SqlCommand cmd = new SqlCommand(authorization, Manager.connection);
                SqlParameter Emp_id_Check_param = new SqlParameter("@Emp_id_param", Employee_ID_Textbox.Text);
                cmd.Parameters.Add(Emp_id_Check_param);
                SqlParameter Disp_id_Check_param = new SqlParameter("@Disp_id_param", ID_Discipline.Text);
                cmd.Parameters.Add(Disp_id_Check_param);
                SqlParameter Group_id_Check_param = new SqlParameter("@Group_id_param", Group_id.Text);
                cmd.Parameters.Add(Group_id_Check_param);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    MessageBox.Show("Данная запись существует");
                }
                else
                {
                    reader.Close();
                    string addEmp = "UPDATE dbo.Lessons SET Employee_id = @Emp_id_param, Discipline_id = @Disp_id_param, Group_id = @Group_id_param WHERE (Lesson_id = @Lesson_id)";
                    SqlCommand command = new SqlCommand(addEmp, Manager.connection);
                    SqlParameter ID_param = new SqlParameter("@Lesson_id", ID.Text);
                    cmd.Parameters.Add(ID_param);
                    SqlParameter Emp_id_param = new SqlParameter("@Emp_id_param", Employee_ID_Textbox.Text);
                    cmd.Parameters.Add(Emp_id_param);
                    SqlParameter Disp_id_param = new SqlParameter("@Disp_id_param", ID_Discipline.Text);
                    cmd.Parameters.Add(Disp_id_param);
                    SqlParameter Group_id_param = new SqlParameter("@Group_id_param", Group_id.Text);
                    cmd.Parameters.Add(Group_id_param);
                    cmd.ExecuteNonQuery();
                    Clear();
                }
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.Number + " " + er.Message);
            }
            FillDataGrid();
            Employee_ID_Textbox.Text = "ID сотрудника";
            Group_id.Text = "ID группы";
            ID_Discipline.Text = "ID дисциплины";
            Manager.connection.Close();
        }

        private void Del_Discipline_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ID.Text == "")
                {
                    MessageBox.Show("Пожалуйста, заполните поле для удаления!!!");
                }
                else
                {
                    Manager.connection.Open();
                    string Delete = "DELETE FROM Lessons WHERE Lesson_id = (@ID)";
                    SqlCommand cmd = new SqlCommand(Delete, Manager.connection);
                    SqlParameter Delete_param = new SqlParameter("@ID", ID.Text);
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

        private void Clear_Text1_Click(object sender, RoutedEventArgs e)
        {
            Surname_textBox.Text = "Фамилия";
            Name_textBox.Text = "Имя";
            Patronymic_textBox.Text = "Отчество";
            Mail_textBox.Text = "Почтовый адрес";
            Phone_textBox.Text = "Номер телефона";
            Password_textBox.Text = "Пароль";
            ID_TextBox.Text = "ID сотрудника";
            Role_ComboBox.Text = null;
        }

        private void Clear_Text2_Click(object sender, RoutedEventArgs e)
        {
            ID.Text = "ID записи";
            Employee_ID_Textbox.Text = "ID сотрудника";
            Group_id.Text = "ID группы";
            ID_Discipline.Text = "ID Дисциплины";
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            if (Role_ComboBox.Text != null && Role_ComboBox.SelectedIndex != -1)
            {
                Manager.connection.Open();
                string SearchString = "SELECT * FROM Employees WHERE Role_Employee = @Role";
                SqlCommand cmd = new SqlCommand(SearchString, Manager.connection);
                SqlParameter Role_param = new SqlParameter("@Role", Role_ComboBox.Text);
                cmd.Parameters.Add(Role_param);
                cmd.ExecuteNonQuery();

                SqlDataAdapter search_sda = new SqlDataAdapter(cmd);
                DataTable search_dt = new DataTable("Employees");
                search_sda.Fill(search_dt);
                EmpDataGrid.ItemsSource = search_dt.DefaultView;
                Manager.connection.Close();
            }
            else
            {
                MessageBox.Show("Заполните поле для поиска");
            }
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

        private void Employee_ID_Textbox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Employee_ID_Textbox.Text == "ID сотрудника")
            {
                Employee_ID_Textbox.Text = "";
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
    }
}
