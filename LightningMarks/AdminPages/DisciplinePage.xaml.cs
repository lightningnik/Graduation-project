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
    /// Логика взаимодействия для AdminDisplinesPage.xaml
    /// </summary>
    public partial class DisciplinePage : Page
    {
        public DisciplinePage()
        {
            InitializeComponent();
            FillDataGrid();
        }

        private void FillDataGrid()
        {
            Manager.connection.Close(); // КОСТЫЛЬ!!! Без этой строки выдает ошибку, что подключение не закрыто,
                                        // при закртытии подключения на странице StartPage
            Manager.connection.Open();

            string EmpString = "SELECT Discipline_id, Name_Discipline FROM Disciplines";
            SqlCommand emp = new SqlCommand(EmpString, Manager.connection);
            SqlDataAdapter emp_sda = new SqlDataAdapter(emp);
            DataTable emp_dt = new DataTable("Disciplines");
            emp_sda.Fill(emp_dt);
            DiscplineDataGrid.ItemsSource = emp_dt.DefaultView;
            Manager.connection.Close();
        }

        private void DiscplineDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                ID.Text = row_selected["Discipline_id"].ToString();
                NameDisp.Text = row_selected["Name_Discipline"] as string;
            }
        }

        private void Add_Discipline_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Manager.connection.Open();
                string addEmp = "INSERT INTO dbo.Disciplines VALUES (@Name_Discipline)";
                SqlCommand cmd = new SqlCommand(addEmp, Manager.connection);
                SqlParameter Name_Discipline_param = new SqlParameter("@Name_Discipline", NameDisp.Text);
                cmd.Parameters.Add(Name_Discipline_param);
                cmd.ExecuteNonQuery();
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.Number + " " + er.Message);
            }
            FillDataGrid();
            Manager.connection.Close();
        }

        private void Upd_Discipline_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Manager.connection.Open();
                string addEmp = "UPDATE dbo.Disciplines SET Name_Discipline = @Name_Discipline WHERE (Discipline_id = @Discipline_id)";
                SqlCommand cmd = new SqlCommand(addEmp, Manager.connection);
                SqlParameter Discipline_id_param = new SqlParameter("@Discipline_id", ID.Text);
                cmd.Parameters.Add(Discipline_id_param);
                SqlParameter Name_Discipline_param = new SqlParameter("@Name_Discipline", NameDisp.Text);
                cmd.Parameters.Add(Name_Discipline_param);
                cmd.ExecuteNonQuery();
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.Number + " " + er.Message);
            }
            FillDataGrid();
            Manager.connection.Close();
        }

        private void Del_Discipline_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Manager.connection.Open();
                string addEmp = "DELETE FROM dbo.Disciplines WHERE (Discipline_id = @Discipline_id)";
                SqlCommand cmd = new SqlCommand(addEmp, Manager.connection);
                SqlParameter Discipline_id_param = new SqlParameter("@Discipline_id", ID.Text);
                cmd.Parameters.Add(Discipline_id_param);
                SqlParameter Name_Discipline_param = new SqlParameter("@Name_Discipline", NameDisp.Text);
                cmd.Parameters.Add(Name_Discipline_param);
                cmd.ExecuteNonQuery();
            }
            catch (SqlException er)
            {
                MessageBox.Show(er.Number + " " + er.Message);
            }
            FillDataGrid();
            Manager.connection.Close();
        }
        private void Name_GotFocus(object sender, RoutedEventArgs e)
        {
            if (NameDisp.Text == "Название дисциплины")
            {
                NameDisp.Text = "";
            }
        }
        private void ID_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ID.Text == "ID")
            {
                ID.Text = "";
            }
        }

        private void ID_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ID.Text == "")
            {
                ID.Text = "ID";
            }
        }

        private void Name_LostFocus(object sender, RoutedEventArgs e)
        {
            if (NameDisp.Text == "ID")
            {
                NameDisp.Text = "Название дисциплины";
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            NameDisp.Text = "Название дисциплины";
            ID.Text = "";
        }
    }
}
