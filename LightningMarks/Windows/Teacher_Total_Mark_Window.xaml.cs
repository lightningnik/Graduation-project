using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace LightningMarks.Windows
{
    /// <summary>
    /// Логика взаимодействия для Teacher_Total_Mark_Window.xaml
    /// </summary>
    public partial class Teacher_Total_Mark_Window : Window
    {
        public Teacher_Total_Mark_Window()
        {
            InitializeComponent();
            FillComboBox();
        }

        private void FillComboBox()
        {
            Manager.connection.Close(); // КОСТЫЛЬ!!! Без этой строки выдает ошибку, что подключение не закрыто,
            // при закртытии подключения на странице StartPage
            Manager.connection.Open();

            string GroupsString = ("SELECT DISTINCT dbo.Groups.Name_Group " +
                "FROM dbo.Lessons INNER JOIN " +
                "dbo.Groups ON dbo.Lessons.Group_id = dbo.Groups.Group_id WHERE Employee_id = @my_id");
            SqlCommand grp = new SqlCommand(GroupsString, Manager.connection);
            grp.Parameters.Add("@my_id", SqlDbType.Int);
            grp.Parameters["@my_id"].Value = Manager.my_id;
            SqlDataAdapter grp_da = new SqlDataAdapter(grp);
            DataTable grp_dt = new DataTable();
            grp_da.Fill(grp_dt);
            for (int i = 0; i < grp_dt.Rows.Count; i++)
            {
                Name_Group_ComboBox.Items.Add(grp_dt.Rows[i]["Name_Group"].ToString());
            }

            //string DispString = ("SELECT DISTINCT dbo.Disciplines.Name_Discipline, dbo.Groups.Name_Group " +
            //        "FROM dbo.Lessons INNER JOIN " +
            //        "dbo.Disciplines ON dbo.Lessons.Discipline_id = dbo.Disciplines.Discipline_id INNER JOIN " +
            //        "dbo.Groups ON dbo.Lessons.Group_id = dbo.Groups.Group_id WHERE Employee_id = @my_id");
            //SqlCommand disp = new SqlCommand(DispString, Manager.connection);
            //disp.Parameters.Add("@my_id", SqlDbType.Int);
            //disp.Parameters["@my_id"].Value = Manager.my_id;
            //SqlDataAdapter disp_da = new SqlDataAdapter(disp);
            //DataTable disp_dt = new DataTable();
            //disp_da.Fill(disp_dt);
            //for (int i = 0; i < disp_dt.Rows.Count; i++)
            //{
            //    DisciplineCombobox.Items.Add(disp_dt.Rows[i]["Name_Discipline"].ToString());
            //}

            for (double i = 0; i < 1; i += 0.05)
            {
                Control_work.Items.Add(Math.Round(i, 2));
                Practice_Work.Items.Add(Math.Round(i, 2));
                Questions.Items.Add(Math.Round(i, 2));
                Test.Items.Add(Math.Round(i, 2));
            }

            Manager.connection.Close();

        }

        private void Average_Mark_button_Click(object sender, RoutedEventArgs e)
        {
            Manager.connection.Open();

            string ListString = "SELECT * FROM Average_Mark WHERE Name_Group = @NameGroup AND Name_Discipline = @DisciplineCombobox AND Employee_id = @myid";
            SqlCommand list = new SqlCommand(ListString, Manager.connection);
            list.Parameters.Add("@myid", SqlDbType.Int);
            list.Parameters["@myid"].Value = Manager.my_id;
            SqlParameter Name_Disp_param = new SqlParameter("@DisciplineCombobox", DisciplineCombobox.Text);
            list.Parameters.Add(Name_Disp_param);
            SqlParameter Name_Group_param = new SqlParameter("@NameGroup", Name_Group_ComboBox.Text);
            list.Parameters.Add(Name_Group_param);
            list.ExecuteNonQuery();
            SqlDataAdapter list_sda = new SqlDataAdapter(list);
            DataTable list_dt = new DataTable("Average_Mark");
            list_sda.Fill(list_dt);
            Total_Marks_DataGrid.ItemsSource = list_dt.DefaultView;
            Manager.connection.Close();
        }


        private void Name_Group_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string DispString = ("SELECT DISTINCT dbo.Disciplines.Name_Discipline, dbo.Groups.Name_Group " +
                    "FROM dbo.Lessons INNER JOIN " +
                    "dbo.Disciplines ON dbo.Lessons.Discipline_id = dbo.Disciplines.Discipline_id INNER JOIN " +
                    "dbo.Groups ON dbo.Lessons.Group_id = dbo.Groups.Group_id WHERE Employee_id = @my_id AND dbo.Groups.Name_Group = @NameGroup");
            SqlCommand disp = new SqlCommand(DispString, Manager.connection);
            disp.Parameters.Add("@my_id", SqlDbType.Int);
            disp.Parameters["@my_id"].Value = Manager.my_id;
            SqlParameter Disp_param = new SqlParameter("@NameGroup", Name_Group_ComboBox.Text);
            disp.Parameters.Add(Disp_param);
            SqlDataAdapter disp_da = new SqlDataAdapter(disp);
            DataTable disp_dt = new DataTable();
            DisciplineCombobox.Items.Clear();
            disp_da.Fill(disp_dt);
            for (int i = 0; i < disp_dt.Rows.Count; i++)
            {
                DisciplineCombobox.Items.Add(disp_dt.Rows[i]["Name_Discipline"].ToString());
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            TeacherWindow totalWindow = new TeacherWindow();
            totalWindow.Show();
            this.Close();
        }

        private void Total_Mark_Button_Click(object sender, RoutedEventArgs e)
        {
                //проверка отмеченных типов работ
                if (ControlCheckBox.IsChecked == true)
                {
                    if (PracticeCheckBox.IsChecked == true)
                    {
                        if (TestCheckBox.IsChecked == true)
                        {
                            if (QuestionCheckBox.IsChecked == true)
                            {
                                if ((Practice_Work.SelectedIndex == -1 || Control_work.SelectedIndex == -1 || Questions.SelectedIndex == -1 || Test.SelectedIndex == -1) 
                                || (Convert.ToDouble(Control_work.Text) + Convert.ToDouble(Practice_Work.Text) + Convert.ToDouble(Test.Text) + Convert.ToDouble(Questions.Text)) != 1)
                                {
                                    MessageBox.Show("Сумма коэффициентов должна быть равна единице");
                                }
                                else
                                {
                                    Manager.connection.Open();
                                    string ListString = "SELECT dbo.Students.Surname, dbo.Students.Name, dbo.Students.Patronymic, ROUND(((dbo.Practic_works.Sum_Marks * @Practics + dbo.Quetions.Sum_Marks * @Questions) + dbo.Test.Sum_Marks * @Test) + dbo.Control_Works.Sum_Marks * @Controls, 5)  " +
                                            "AS TotalMark FROM dbo.Practic_works INNER JOIN " +
                                            "dbo.Students ON dbo.Practic_works.Student_id = dbo.Students.Student_id INNER JOIN " +
                                            "dbo.Quetions ON dbo.Students.Student_id = dbo.Quetions.Student_id INNER JOIN " +
                                            "dbo.Test ON dbo.Students.Student_id = dbo.Test.Student_id INNER JOIN " +
                                            "dbo.Control_Works ON dbo.Students.Student_id = dbo.Control_Works.Student_id";
                                    SqlCommand list = new SqlCommand(ListString, Manager.connection);
                                    list.Parameters.Add("@myid", SqlDbType.Int);
                                    list.Parameters["@myid"].Value = Manager.my_id;
                                    SqlParameter Practics_param = new SqlParameter("@Practics", Convert.ToDouble(Practice_Work.Text));
                                    list.Parameters.Add(Practics_param);
                                    SqlParameter Questions_param = new SqlParameter("@Questions", Convert.ToDouble(Questions.Text));
                                    list.Parameters.Add(Questions_param);
                                    SqlParameter Controls_param = new SqlParameter("@Controls", Convert.ToDouble(Control_work.Text));
                                    list.Parameters.Add(Controls_param);
                                    SqlParameter Test_param = new SqlParameter("@Test", Convert.ToDouble(Test.Text));
                                    list.Parameters.Add(Test_param);
                                    list.ExecuteNonQuery();
                                    SqlDataAdapter list_sda = new SqlDataAdapter(list);
                                    DataTable list_dt = new DataTable("Average_Mark");
                                    list_sda.Fill(list_dt);
                                    Total_Marks_DataGrid.ItemsSource = list_dt.DefaultView;
                                    Manager.connection.Close();
                                }
                            }
                            else
                            {
                                if ((Practice_Work.SelectedIndex == -1 || Control_work.SelectedIndex == -1 || Test.SelectedIndex == -1)
                                || ((Convert.ToDouble(Control_work.Text) + Convert.ToDouble(Practice_Work.Text) + Convert.ToDouble(Test.Text)) != 1))
                                {
                                    MessageBox.Show("Сумма коэффициентов должна быть равна единице");
                                }
                                else
                                {
                                    Manager.connection.Open();
                                    string ListString = "SELECT dbo.Students.Surname, dbo.Students.Name, dbo.Students.Patronymic, ROUND((dbo.Practic_works.Sum_Marks * @Practics  + dbo.Test.Sum_Marks * @Test) + dbo.Control_Works.Sum_Marks * @Controls, 5)  " +
                                            "AS TotalMark FROM dbo.Practic_works INNER JOIN " +
                                            "dbo.Students ON dbo.Practic_works.Student_id = dbo.Students.Student_id INNER JOIN " +
                                            "dbo.Quetions ON dbo.Students.Student_id = dbo.Quetions.Student_id INNER JOIN " +
                                            "dbo.Test ON dbo.Students.Student_id = dbo.Test.Student_id INNER JOIN " +
                                            "dbo.Control_Works ON dbo.Students.Student_id = dbo.Control_Works.Student_id";
                                    SqlCommand list = new SqlCommand(ListString, Manager.connection);
                                    list.Parameters.Add("@myid", SqlDbType.Int);
                                    list.Parameters["@myid"].Value = Manager.my_id;
                                    SqlParameter Practics_param = new SqlParameter("@Practics", Convert.ToDouble(Practice_Work.Text));
                                    list.Parameters.Add(Practics_param);
                                    SqlParameter Controls_param = new SqlParameter("@Controls", Convert.ToDouble(Control_work.Text));
                                    list.Parameters.Add(Controls_param);
                                    SqlParameter Test_param = new SqlParameter("@Test", Convert.ToDouble(Test.Text));
                                    list.Parameters.Add(Test_param);
                                    list.ExecuteNonQuery();
                                    SqlDataAdapter list_sda = new SqlDataAdapter(list);
                                    DataTable list_dt = new DataTable("Average_Mark");
                                    list_sda.Fill(list_dt);
                                    Total_Marks_DataGrid.ItemsSource = list_dt.DefaultView;
                                    Manager.connection.Close();
                                }
                            }
                        }
                        else
                        {
                            if (QuestionCheckBox.IsChecked == true)
                            {
                                if ((Practice_Work.SelectedIndex == -1 || Control_work.SelectedIndex == -1 || Questions.SelectedIndex == -1)
                                 || ((Convert.ToDouble(Control_work.Text) + Convert.ToDouble(Practice_Work.Text) + Convert.ToDouble(Questions.Text)) != 1))
                                {
                                    MessageBox.Show("Сумма коэффициентов должна быть равна единице");
                                }
                                else
                                {
                                    Manager.connection.Open();
                                    string ListString = "SELECT dbo.Students.Surname, dbo.Students.Name, dbo.Students.Patronymic, ROUND((dbo.Practic_works.Sum_Marks * @Practics + dbo.Quetions.Sum_Marks * @Questions + dbo.Control_Works.Sum_Marks * @Controls, 5)  " +
                                            "AS TotalMark FROM dbo.Practic_works INNER JOIN " +
                                            "dbo.Students ON dbo.Practic_works.Student_id = dbo.Students.Student_id INNER JOIN " +
                                            "dbo.Quetions ON dbo.Students.Student_id = dbo.Quetions.Student_id INNER JOIN " +
                                            "dbo.Test ON dbo.Students.Student_id = dbo.Test.Student_id INNER JOIN " +
                                            "dbo.Control_Works ON dbo.Students.Student_id = dbo.Control_Works.Student_id";
                                    SqlCommand list = new SqlCommand(ListString, Manager.connection);
                                    list.Parameters.Add("@myid", SqlDbType.Int);
                                    list.Parameters["@myid"].Value = Manager.my_id;
                                    SqlParameter Practics_param = new SqlParameter("@Practics", Convert.ToDouble(Practice_Work.Text));
                                    list.Parameters.Add(Practics_param);
                                    SqlParameter Questions_param = new SqlParameter("@Questions", Convert.ToDouble(Questions.Text));
                                    list.Parameters.Add(Questions_param);
                                    SqlParameter Controls_param = new SqlParameter("@Controls", Convert.ToDouble(Control_work.Text));
                                    list.Parameters.Add(Controls_param);
                                    list.ExecuteNonQuery();
                                    SqlDataAdapter list_sda = new SqlDataAdapter(list);
                                    DataTable list_dt = new DataTable("Average_Mark");
                                    list_sda.Fill(list_dt);
                                    Total_Marks_DataGrid.ItemsSource = list_dt.DefaultView;
                                    Manager.connection.Close();
                                }
                            }
                            else
                            {
                                if ((Practice_Work.SelectedIndex == -1 || Control_work.SelectedIndex == -1) 
                                || ((Convert.ToDouble(Control_work.Text) + Convert.ToDouble(Practice_Work.Text)) != 1))
                                {
                                    MessageBox.Show("Сумма коэффициентов должна быть равна единице");
                                }
                                else
                                {
                                    Manager.connection.Open();
                                    string ListString = "SELECT dbo.Students.Surname, dbo.Students.Name, dbo.Students.Patronymic, ROUND(dbo.Practic_works.Sum_Marks * @Practics + dbo.Control_Works.Sum_Marks * @Controls, 5)  " +
                                            "AS TotalMark FROM dbo.Practic_works INNER JOIN " +
                                            "dbo.Students ON dbo.Practic_works.Student_id = dbo.Students.Student_id INNER JOIN " +
                                            "dbo.Quetions ON dbo.Students.Student_id = dbo.Quetions.Student_id INNER JOIN " +
                                            "dbo.Test ON dbo.Students.Student_id = dbo.Test.Student_id INNER JOIN " +
                                            "dbo.Control_Works ON dbo.Students.Student_id = dbo.Control_Works.Student_id";
                                    SqlCommand list = new SqlCommand(ListString, Manager.connection);
                                    list.Parameters.Add("@myid", SqlDbType.Int);
                                    list.Parameters["@myid"].Value = Manager.my_id;
                                    SqlParameter Practics_param = new SqlParameter("@Practics", Convert.ToDouble(Practice_Work.Text));
                                    list.Parameters.Add(Practics_param);
                                    SqlParameter Controls_param = new SqlParameter("@Controls", Convert.ToDouble(Control_work.Text));
                                    list.Parameters.Add(Controls_param);
                                    list.ExecuteNonQuery();
                                    SqlDataAdapter list_sda = new SqlDataAdapter(list);
                                    DataTable list_dt = new DataTable("Average_Mark");
                                    list_sda.Fill(list_dt);
                                    Total_Marks_DataGrid.ItemsSource = list_dt.DefaultView;
                                    Manager.connection.Close();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (TestCheckBox.IsChecked == true)
                        {
                            if (QuestionCheckBox.IsChecked == true)
                            {
                                if ((Control_work.SelectedIndex == -1 || Questions.SelectedIndex == -1 || Test.SelectedIndex == -1) 
                                || ((Convert.ToDouble(Control_work.Text) + Convert.ToDouble(Test.Text) + Convert.ToDouble(Questions.Text)) != 1))
                                {
                                    MessageBox.Show("Сумма коэффициентов должна быть равна единице");
                                }
                                else
                                {
                                    Manager.connection.Open();
                                    string ListString = "SELECT dbo.Students.Surname, dbo.Students.Name, dbo.Students.Patronymic, ROUND((dbo.Quetions.Sum_Marks * @Questions + dbo.Test.Sum_Marks * @Test) + dbo.Control_Works.Sum_Marks * @Controls, 5)  " +
                                            "AS TotalMark FROM dbo.Practic_works INNER JOIN " +
                                            "dbo.Students ON dbo.Practic_works.Student_id = dbo.Students.Student_id INNER JOIN " +
                                            "dbo.Quetions ON dbo.Students.Student_id = dbo.Quetions.Student_id INNER JOIN " +
                                            "dbo.Test ON dbo.Students.Student_id = dbo.Test.Student_id INNER JOIN " +
                                            "dbo.Control_Works ON dbo.Students.Student_id = dbo.Control_Works.Student_id";
                                    SqlCommand list = new SqlCommand(ListString, Manager.connection);
                                    list.Parameters.Add("@myid", SqlDbType.Int);
                                    list.Parameters["@myid"].Value = Manager.my_id;
                                    SqlParameter Questions_param = new SqlParameter("@Questions", Convert.ToDouble(Questions.Text));
                                    list.Parameters.Add(Questions_param);
                                    SqlParameter Controls_param = new SqlParameter("@Controls", Convert.ToDouble(Control_work.Text));
                                    list.Parameters.Add(Controls_param);
                                    SqlParameter Test_param = new SqlParameter("@Test", Convert.ToDouble(Test.Text));
                                    list.Parameters.Add(Test_param);
                                    list.ExecuteNonQuery();
                                    SqlDataAdapter list_sda = new SqlDataAdapter(list);
                                    DataTable list_dt = new DataTable("Average_Mark");
                                    list_sda.Fill(list_dt);
                                    Total_Marks_DataGrid.ItemsSource = list_dt.DefaultView;
                                    Manager.connection.Close();
                                }
                            }
                            else
                            {
                                if ((Control_work.SelectedIndex == -1 || Test.SelectedIndex == -1)
                                || ((Convert.ToDouble(Control_work.Text) + Convert.ToDouble(Test.Text)) != 1))
                                {
                                    MessageBox.Show("Сумма коэффициентов должна быть равна единице");
                                }
                                else
                                {
                                    Manager.connection.Open();
                                    string ListString = "SELECT dbo.Students.Surname, dbo.Students.Name, dbo.Students.Patronymic, ROUND(dbo.Test.Sum_Marks * @Test + dbo.Control_Works.Sum_Marks * @Controls, 5)  " +
                                            "AS TotalMark FROM dbo.Practic_works INNER JOIN " +
                                            "dbo.Students ON dbo.Practic_works.Student_id = dbo.Students.Student_id INNER JOIN " +
                                            "dbo.Quetions ON dbo.Students.Student_id = dbo.Quetions.Student_id INNER JOIN " +
                                            "dbo.Test ON dbo.Students.Student_id = dbo.Test.Student_id INNER JOIN " +
                                            "dbo.Control_Works ON dbo.Students.Student_id = dbo.Control_Works.Student_id";
                                    SqlCommand list = new SqlCommand(ListString, Manager.connection);
                                    list.Parameters.Add("@myid", SqlDbType.Int);
                                    list.Parameters["@myid"].Value = Manager.my_id;
                                    SqlParameter Controls_param = new SqlParameter("@Controls", Convert.ToDouble(Control_work.Text));
                                    list.Parameters.Add(Controls_param);
                                    SqlParameter Test_param = new SqlParameter("@Test", Convert.ToDouble(Test.Text));
                                    list.Parameters.Add(Test_param);
                                    list.ExecuteNonQuery();
                                    SqlDataAdapter list_sda = new SqlDataAdapter(list);
                                    DataTable list_dt = new DataTable("Average_Mark");
                                    list_sda.Fill(list_dt);
                                    Total_Marks_DataGrid.ItemsSource = list_dt.DefaultView;
                                    Manager.connection.Close();
                                }
                            }
                        }
                        else
                        {
                            if (QuestionCheckBox.IsChecked == true)
                            {
                                if ((Control_work.SelectedIndex == -1 || Questions.SelectedIndex == -1 )  
                                || ((Convert.ToDouble(Control_work.Text) + Convert.ToDouble(Questions.Text)) != 1))
                                {
                                    MessageBox.Show("Сумма коэффициентов должна быть равна единице");
                                }
                                else
                                {
                                    Manager.connection.Open();
                                    string ListString = "SELECT dbo.Students.Surname, dbo.Students.Name, dbo.Students.Patronymic, ROUND(dbo.Quetions.Sum_Marks * @Questions + dbo.Control_Works.Sum_Marks * @Controls, 5)  " +
                                            "AS TotalMark FROM dbo.Practic_works INNER JOIN " +
                                            "dbo.Students ON dbo.Practic_works.Student_id = dbo.Students.Student_id INNER JOIN " +
                                            "dbo.Quetions ON dbo.Students.Student_id = dbo.Quetions.Student_id INNER JOIN " +
                                            "dbo.Test ON dbo.Students.Student_id = dbo.Test.Student_id INNER JOIN " +
                                            "dbo.Control_Works ON dbo.Students.Student_id = dbo.Control_Works.Student_id";
                                    SqlCommand list = new SqlCommand(ListString, Manager.connection);
                                    list.Parameters.Add("@myid", SqlDbType.Int);
                                    list.Parameters["@myid"].Value = Manager.my_id;
                                    SqlParameter Practics_param = new SqlParameter("@Practics", Convert.ToDouble(Practice_Work.Text));
                                    list.Parameters.Add(Practics_param);
                                    SqlParameter Questions_param = new SqlParameter("@Questions", Convert.ToDouble(Questions.Text));
                                    list.Parameters.Add(Questions_param);
                                    SqlParameter Controls_param = new SqlParameter("@Controls", Convert.ToDouble(Control_work.Text));
                                    list.Parameters.Add(Controls_param);
                                    SqlParameter Test_param = new SqlParameter("@Test", Convert.ToDouble(Test.Text));
                                    list.Parameters.Add(Test_param);
                                    list.ExecuteNonQuery();
                                    SqlDataAdapter list_sda = new SqlDataAdapter(list);
                                    DataTable list_dt = new DataTable("Average_Mark");
                                    list_sda.Fill(list_dt);
                                    Total_Marks_DataGrid.ItemsSource = list_dt.DefaultView;
                                    Manager.connection.Close();
                                }
                            }
                            else
                            {
                                Manager.connection.Open();
                                string ListString = "SELECT dbo.Students.Surname, dbo.Students.Name, dbo.Students.Patronymic, ROUND(dbo.Control_Works.Sum_Marks, 5)  " +
                                        "AS TotalMark FROM dbo.Practic_works INNER JOIN " +
                                        "dbo.Students ON dbo.Practic_works.Student_id = dbo.Students.Student_id INNER JOIN " +
                                        "dbo.Quetions ON dbo.Students.Student_id = dbo.Quetions.Student_id INNER JOIN " +
                                        "dbo.Test ON dbo.Students.Student_id = dbo.Test.Student_id INNER JOIN " +
                                        "dbo.Control_Works ON dbo.Students.Student_id = dbo.Control_Works.Student_id";
                                SqlCommand list = new SqlCommand(ListString, Manager.connection);
                                list.Parameters.Add("@myid", SqlDbType.Int);
                                list.Parameters["@myid"].Value = Manager.my_id;
                                list.ExecuteNonQuery();
                                SqlDataAdapter list_sda = new SqlDataAdapter(list);
                                DataTable list_dt = new DataTable("Average_Mark");
                                list_sda.Fill(list_dt);
                                Total_Marks_DataGrid.ItemsSource = list_dt.DefaultView;
                                Manager.connection.Close();
                            }
                        }
                    }
                }
                else
                {
                    if (PracticeCheckBox.IsChecked == true)
                    {
                        if (TestCheckBox.IsChecked == true)
                        {
                            if (QuestionCheckBox.IsChecked == true)
                            {
                                if ((Practice_Work.SelectedIndex == -1 || Questions.SelectedIndex == -1 || Test.SelectedIndex == -1)  
                                || ((Convert.ToDouble(Practice_Work.Text) + Convert.ToDouble(Test.Text) + Convert.ToDouble(Questions.Text)) != 1))
                                {
                                    MessageBox.Show("Сумма коэффициентов должна быть равна единице");
                                }
                                else
                                {
                                    Manager.connection.Open();
                                    string ListString = "SELECT dbo.Students.Surname, dbo.Students.Name, dbo.Students.Patronymic, ROUND((dbo.Practic_works.Sum_Marks * @Practics + dbo.Quetions.Sum_Marks * @Questions) + dbo.Test.Sum_Marks * @Test, 5)  " +
                                            "AS TotalMark FROM dbo.Practic_works INNER JOIN " +
                                            "dbo.Students ON dbo.Practic_works.Student_id = dbo.Students.Student_id INNER JOIN " +
                                            "dbo.Quetions ON dbo.Students.Student_id = dbo.Quetions.Student_id INNER JOIN " +
                                            "dbo.Test ON dbo.Students.Student_id = dbo.Test.Student_id INNER JOIN " +
                                            "dbo.Control_Works ON dbo.Students.Student_id = dbo.Control_Works.Student_id";
                                    SqlCommand list = new SqlCommand(ListString, Manager.connection);
                                    list.Parameters.Add("@myid", SqlDbType.Int);
                                    list.Parameters["@myid"].Value = Manager.my_id;
                                    SqlParameter Practics_param = new SqlParameter("@Practics", Convert.ToDouble(Practice_Work.Text));
                                    list.Parameters.Add(Practics_param);
                                    SqlParameter Questions_param = new SqlParameter("@Questions", Convert.ToDouble(Questions.Text));
                                    list.Parameters.Add(Questions_param);
                                    SqlParameter Test_param = new SqlParameter("@Test", Convert.ToDouble(Test.Text));
                                    list.Parameters.Add(Test_param);
                                    list.ExecuteNonQuery();
                                    SqlDataAdapter list_sda = new SqlDataAdapter(list);
                                    DataTable list_dt = new DataTable("Average_Mark");
                                    list_sda.Fill(list_dt);
                                    Total_Marks_DataGrid.ItemsSource = list_dt.DefaultView;
                                    Manager.connection.Close();
                                }
                            }
                            else
                            {
                                if ((Practice_Work.SelectedIndex == -1 || Test.SelectedIndex == -1)  
                                || ((Convert.ToDouble(Practice_Work.Text) + Convert.ToDouble(Test.Text)) != 1))
                                {
                                    MessageBox.Show("Сумма коэффициентов должна быть равна единице");
                                }
                                else
                                {
                                    Manager.connection.Open();
                                    string ListString = "SELECT dbo.Students.Surname, dbo.Students.Name, dbo.Students.Patronymic, ROUND(dbo.Practic_works.Sum_Marks * @Practics + dbo.Test.Sum_Marks * @Test, 5)  " +
                                            "AS TotalMark FROM dbo.Practic_works INNER JOIN " +
                                            "dbo.Students ON dbo.Practic_works.Student_id = dbo.Students.Student_id INNER JOIN " +
                                            "dbo.Quetions ON dbo.Students.Student_id = dbo.Quetions.Student_id INNER JOIN " +
                                            "dbo.Test ON dbo.Students.Student_id = dbo.Test.Student_id INNER JOIN " +
                                            "dbo.Control_Works ON dbo.Students.Student_id = dbo.Control_Works.Student_id";
                                    SqlCommand list = new SqlCommand(ListString, Manager.connection);
                                    list.Parameters.Add("@myid", SqlDbType.Int);
                                    list.Parameters["@myid"].Value = Manager.my_id;
                                    SqlParameter Practics_param = new SqlParameter("@Practics", Convert.ToDouble(Practice_Work.Text));
                                    list.Parameters.Add(Practics_param);
                                    SqlParameter Test_param = new SqlParameter("@Test", Convert.ToDouble(Test.Text));
                                    list.Parameters.Add(Test_param);
                                    list.ExecuteNonQuery();
                                    SqlDataAdapter list_sda = new SqlDataAdapter(list);
                                    DataTable list_dt = new DataTable("Average_Mark");
                                    list_sda.Fill(list_dt);
                                    Total_Marks_DataGrid.ItemsSource = list_dt.DefaultView;
                                    Manager.connection.Close();
                                }
                            }
                        }
                        else
                        {
                            if (QuestionCheckBox.IsChecked == true)
                            {
                                if ((Practice_Work.SelectedIndex == -1 || Questions.SelectedIndex == -1) 
                                || ((Convert.ToDouble(Practice_Work.Text) + Convert.ToDouble(Questions.Text)) != 1))
                                {
                                    MessageBox.Show("Сумма коэффициентов должна быть равна единице");
                                }
                                else
                                {
                                    Manager.connection.Open();
                                    string ListString = "SELECT dbo.Students.Surname, dbo.Students.Name, dbo.Students.Patronymic, ROUND(dbo.Practic_works.Sum_Marks * @Practics + dbo.Quetions.Sum_Marks * @Questions, 5)  " +
                                            "AS TotalMark FROM dbo.Practic_works INNER JOIN " +
                                            "dbo.Students ON dbo.Practic_works.Student_id = dbo.Students.Student_id INNER JOIN " +
                                            "dbo.Quetions ON dbo.Students.Student_id = dbo.Quetions.Student_id INNER JOIN " +
                                            "dbo.Test ON dbo.Students.Student_id = dbo.Test.Student_id INNER JOIN " +
                                            "dbo.Control_Works ON dbo.Students.Student_id = dbo.Control_Works.Student_id";
                                    SqlCommand list = new SqlCommand(ListString, Manager.connection);
                                    list.Parameters.Add("@myid", SqlDbType.Int);
                                    list.Parameters["@myid"].Value = Manager.my_id;
                                    SqlParameter Practics_param = new SqlParameter("@Practics", Convert.ToDouble(Practice_Work.Text));
                                    list.Parameters.Add(Practics_param);
                                    SqlParameter Questions_param = new SqlParameter("@Questions", Convert.ToDouble(Questions.Text));
                                    list.Parameters.Add(Questions_param);
                                    list.ExecuteNonQuery();
                                    SqlDataAdapter list_sda = new SqlDataAdapter(list);
                                    DataTable list_dt = new DataTable("Average_Mark");
                                    list_sda.Fill(list_dt);
                                    Total_Marks_DataGrid.ItemsSource = list_dt.DefaultView;
                                    Manager.connection.Close();
                                }
                            }
                            else
                            {
                            Manager.connection.Open();
                            string ListString = "SELECT dbo.Students.Surname, dbo.Students.Name, dbo.Students.Patronymic, ROUND(dbo.Practic_works.Sum_Marks, 5)  " +
                                    "AS TotalMark FROM dbo.Practic_works INNER JOIN " +
                                    "dbo.Students ON dbo.Practic_works.Student_id = dbo.Students.Student_id INNER JOIN " +
                                    "dbo.Quetions ON dbo.Students.Student_id = dbo.Quetions.Student_id INNER JOIN " +
                                    "dbo.Test ON dbo.Students.Student_id = dbo.Test.Student_id INNER JOIN " +
                                    "dbo.Control_Works ON dbo.Students.Student_id = dbo.Control_Works.Student_id";
                            SqlCommand list = new SqlCommand(ListString, Manager.connection);
                            list.Parameters.Add("@myid", SqlDbType.Int);
                            list.Parameters["@myid"].Value = Manager.my_id;
                            list.ExecuteNonQuery();
                            SqlDataAdapter list_sda = new SqlDataAdapter(list);
                            DataTable list_dt = new DataTable("Average_Mark");
                            list_sda.Fill(list_dt);
                            Total_Marks_DataGrid.ItemsSource = list_dt.DefaultView;
                            Manager.connection.Close();
                        }
                        }
                    }
                    else
                    {
                        if (TestCheckBox.IsChecked == true)
                        {
                            if (QuestionCheckBox.IsChecked == true)
                            {
                                if ((Questions.SelectedIndex == -1 || Test.SelectedIndex == -1)  
                                || (Convert.ToDouble(Test.Text) + Convert.ToDouble(Questions.Text) != 1))
                                {
                                    MessageBox.Show("Сумма коэффициентов должна быть равна единице");
                                }
                                else
                                {
                                    Manager.connection.Open();
                                    string ListString = "SELECT dbo.Students.Surname, dbo.Students.Name, dbo.Students.Patronymic, ROUND(dbo.Quetions.Sum_Marks * @Questions + dbo.Test.Sum_Marks * @Test, 5)  " +
                                            "AS TotalMark FROM dbo.Practic_works INNER JOIN " +
                                            "dbo.Students ON dbo.Practic_works.Student_id = dbo.Students.Student_id INNER JOIN " +
                                            "dbo.Quetions ON dbo.Students.Student_id = dbo.Quetions.Student_id INNER JOIN " +
                                            "dbo.Test ON dbo.Students.Student_id = dbo.Test.Student_id INNER JOIN " +
                                            "dbo.Control_Works ON dbo.Students.Student_id = dbo.Control_Works.Student_id";
                                    SqlCommand list = new SqlCommand(ListString, Manager.connection);
                                    list.Parameters.Add("@myid", SqlDbType.Int);
                                    list.Parameters["@myid"].Value = Manager.my_id;
                                    SqlParameter Questions_param = new SqlParameter("@Questions", Convert.ToDouble(Questions.Text));
                                    list.Parameters.Add(Questions_param);
                                    SqlParameter Test_param = new SqlParameter("@Test", Convert.ToDouble(Test.Text));
                                    list.Parameters.Add(Test_param);
                                    list.ExecuteNonQuery();
                                    SqlDataAdapter list_sda = new SqlDataAdapter(list);
                                    DataTable list_dt = new DataTable("Average_Mark");
                                    list_sda.Fill(list_dt);
                                    Total_Marks_DataGrid.ItemsSource = list_dt.DefaultView;
                                    Manager.connection.Close();
                                }
                            }
                            else
                            {
                            Manager.connection.Open();
                            string ListString = "SELECT dbo.Students.Surname, dbo.Students.Name, dbo.Students.Patronymic, ROUND(dbo.Test.Sum_Marks, 5)  " +
                                    "AS TotalMark FROM dbo.Practic_works INNER JOIN " +
                                    "dbo.Students ON dbo.Practic_works.Student_id = dbo.Students.Student_id INNER JOIN " +
                                    "dbo.Quetions ON dbo.Students.Student_id = dbo.Quetions.Student_id INNER JOIN " +
                                    "dbo.Test ON dbo.Students.Student_id = dbo.Test.Student_id INNER JOIN " +
                                    "dbo.Control_Works ON dbo.Students.Student_id = dbo.Control_Works.Student_id";
                            SqlCommand list = new SqlCommand(ListString, Manager.connection);
                            list.Parameters.Add("@myid", SqlDbType.Int);
                            list.Parameters["@myid"].Value = Manager.my_id;
                            list.ExecuteNonQuery();
                            SqlDataAdapter list_sda = new SqlDataAdapter(list);
                            DataTable list_dt = new DataTable("Average_Mark");
                            list_sda.Fill(list_dt);
                            Total_Marks_DataGrid.ItemsSource = list_dt.DefaultView;
                            Manager.connection.Close();
                        }
                        }
                        else
                        {
                            if (QuestionCheckBox.IsChecked == true)
                            {
                            Manager.connection.Open();
                            string ListString = "SELECT dbo.Students.Surname, dbo.Students.Name, dbo.Students.Patronymic, ROUND(dbo.Quetions.Sum_Marks, 5)  " +
                                    "AS TotalMark FROM dbo.Practic_works INNER JOIN " +
                                    "dbo.Students ON dbo.Practic_works.Student_id = dbo.Students.Student_id INNER JOIN " +
                                    "dbo.Quetions ON dbo.Students.Student_id = dbo.Quetions.Student_id INNER JOIN " +
                                    "dbo.Test ON dbo.Students.Student_id = dbo.Test.Student_id INNER JOIN " +
                                    "dbo.Control_Works ON dbo.Students.Student_id = dbo.Control_Works.Student_id";
                            SqlCommand list = new SqlCommand(ListString, Manager.connection);
                            list.Parameters.Add("@myid", SqlDbType.Int);
                            list.Parameters["@myid"].Value = Manager.my_id;
                            list.ExecuteNonQuery();
                            SqlDataAdapter list_sda = new SqlDataAdapter(list);
                            DataTable list_dt = new DataTable("Average_Mark");
                            list_sda.Fill(list_dt);
                            Total_Marks_DataGrid.ItemsSource = list_dt.DefaultView;
                            Manager.connection.Close();
                        }
                            else
                            {
                                MessageBox.Show("Для подсчета итоговой кумулятивной оценки выберите как минимум один тип работ");
                            }
                        }
                    }
                }
            }
        }
    }