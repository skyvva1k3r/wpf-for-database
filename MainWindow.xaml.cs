using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        // Замените YourDatabase на имя вашей БД
        private const string ConnectionString =
            "Data Source=localhost;"
          + "Initial Catalog=ProjectDB;"
          + "Integrated Security=True;"
          + "TrustServerCertificate=True";

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using var con = new SqlConnection(ConnectionString);
                con.Open();

                var dt = new DataTable();

                // Group
                dt.Clear();
                new SqlDataAdapter("SELECT * FROM [Group]", con).Fill(dt);
                DataGridGroup.ItemsSource = dt.DefaultView;

                // Student
                dt = new DataTable();
                new SqlDataAdapter("SELECT * FROM Student", con).Fill(dt);
                DataGridStudent.ItemsSource = dt.DefaultView;

                // Project
                dt = new DataTable();
                new SqlDataAdapter("SELECT * FROM Project", con).Fill(dt);
                DataGridProject.ItemsSource = dt.DefaultView;

                // Assignment
                dt = new DataTable();
                new SqlDataAdapter("SELECT * FROM Assignment", con).Fill(dt);
                DataGridAssignment.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных:\n" + ex.Message,
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDistribute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using var con = new SqlConnection(ConnectionString);
                con.Open();
                using var cmd = new SqlCommand("Random", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.ExecuteNonQuery();

                MessageBox.Show("Темы распределены.", "Готово",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при распределении:\n" + ex.Message,
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using var con = new SqlConnection(ConnectionString);
                con.Open();
                using var cmd = new SqlCommand("ClearAssignments", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.ExecuteNonQuery();

                MessageBox.Show("Назначения очищены.", "Готово",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при очистке:\n" + ex.Message,
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnManualAssign_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int numberStudent   = int.Parse(TbManNumberStudent.Text);
                string nameGroup    = TbManGroupName.Text;
                int numberProject   = int.Parse(TbManNumberProject.Text);

                using var con = new SqlConnection(ConnectionString);
                con.Open();
                using var cmd = new SqlCommand("usp_ManualAssignment", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@numberStudent",  numberStudent);
                cmd.Parameters.AddWithValue("@nameGroup",      nameGroup);
                cmd.Parameters.AddWithValue("@numberProject",  numberProject);

                // optional: get return value
                var ret = new SqlParameter("@ReturnVal", SqlDbType.Int)
                {
                    Direction = ParameterDirection.ReturnValue
                };
                cmd.Parameters.Add(ret);

                cmd.ExecuteNonQuery();

                int rv = (int)(ret.Value ?? 0);
                MessageBox.Show($"Процедура выполнена (ReturnValue={rv}).",
                                "Готово", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при ручном назначении:\n" + ex.Message,
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
