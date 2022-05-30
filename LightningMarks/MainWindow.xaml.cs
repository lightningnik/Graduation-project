using LightningMarks.Windows;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"HOME-PC\SQLEXPRESS";
            builder.InitialCatalog = "Evaluations";
            builder.IntegratedSecurity = true;
            Manager.connection = new SqlConnection(builder.ConnectionString);

        }
        private void Authorize_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Status_Check.Text == "Студент")
            {
                Manager.connection.Open();
                string authorization = String.Format("SELECT Student_id, Phone_number, Mail, Password FROM [dbo].[Students] WHERE [Phone_number] = @login_value OR [Mail] = @login_value AND [Password] = @passwd_value");
                SqlCommand command = new SqlCommand(authorization, Manager.connection);
                SqlParameter login_param = new SqlParameter("@login_value", Login.Text);
                command.Parameters.Add(login_param);
                SqlParameter passwd_param = new SqlParameter("@passwd_value", Password.Password);
                command.Parameters.Add(passwd_param);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Manager.my_id = Convert.ToInt32(reader.GetValue(0));
                }
                if (reader.HasRows) // если такая запись существует       
                {
                    StudentWindow studentWindow = new StudentWindow();
                    studentWindow.Show();
                    this.Close();
                }
                else Notify.Content = "Сообщение: Проверьте правильность вводимых данных";
            }
            if (Status_Check.Text == "Сотрудник")
            {
                Manager.connection.Open();
                string authorization = String.Format("SELECT Employee_id, Role_Employee, Phone_number, Mail, Password FROM [dbo].[Employees] WHERE [Phone_number] = @login_value OR [Mail] = @login_value AND [Password] = @passwd_value");
                SqlCommand command = new SqlCommand(authorization, Manager.connection);
                SqlParameter login_param = new SqlParameter("@login_value", Login.Text);
                command.Parameters.Add(login_param);
                SqlParameter passwd_param = new SqlParameter("@passwd_value", Password.Password);
                command.Parameters.Add(passwd_param);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Manager.my_id = Convert.ToInt32(reader.GetValue(0));
                    Manager.myrole = Convert.ToString(reader.GetValue(1));

                }
                if (reader.HasRows)
                {
                    if (Manager.myrole == "Администратор")
                    {
                        AdminWindow adminWindow = new AdminWindow();
                        adminWindow.Show();
                        this.Close();
                        Manager.connection.Close();
                    }
                    else
                    {
                        TeacherWindow teacherWindow = new TeacherWindow();
                        teacherWindow.Show();
                        this.Close();
                        Manager.connection.Close();
                    }
                }
                else Notify.Content = "Сообщение: Проверьте правильность вводимых данных";
            }
            else Notify.Content = "Сообщение: Выберите статус";
            Manager.connection.Close();
        }

        private void Hint_Button_Click(object sender, RoutedEventArgs e)
        {
            Notify.Content = "Сообщение: Для авторизации введите логин (почтовый адрес или номер телефона) и пароль\nСообщение: Выберите статус (студент или сотрудник)";
        }
    }
}
