using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;


namespace db_Test
{
    public partial class MainWindow : Window
    {
        public string name;
        public class CTest
        {
            public int id { get; set; }
            public string FIO { get; set; }
            public int Math { get; set; }
            public int Phis { get; set; }
        }

        SQLiteConnection m_dbConnection;
        CTest test;

        public void openDB()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.ShowDialog();
            string db_name = dlg.FileName;
            try
            {
                m_dbConnection = new SQLiteConnection("Data Source=" + db_name + ";Version=3;");
                m_dbConnection.Open();
                try
                {
                    dataUpdate();
                }
                catch
                {
                    MessageBox.Show("Невозможно открыть файл");
                }
            }
            catch
            { }           
        }

        public void dataUpdate()
        {
            string sql = "SELECT * FROM test_table ORDER BY FIO";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Items.Add(reader["id"] + " : " + reader["FIO"] + " : " + reader["Math"] + " : " + reader["Phis"]);

                var data = new CTest
                {
                    id = int.Parse(reader["id"].ToString()),
                    FIO = reader["FIO"].ToString(),
                    Phis = int.Parse(reader["Phis"].ToString()),
                    Math = int.Parse(reader["Math"].ToString())

                };
                DataGrid.Items.Add(data);
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            openDB();
        }

        //Открытие существующей базы данных
        public void open_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.Items.Clear();
            openDB();           
        }

        //Создание новой бд
        private void new_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.ShowDialog();
            string db_name = dlg.FileName;

            SQLiteConnection.CreateFile(db_name);
            
            string sql = "CREATE TABLE test_table (id INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, FIO TEXT, Phis INT, Math INT)";
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=" + db_name + ";Version=3;");
            m_dbConnection.Open();
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);

            command.ExecuteNonQuery();
            dataUpdate();
        }



        //Редактирование 
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            test = (CTest)DataGrid.SelectedItem;
            try
            {
                tb1.Text = test.FIO.ToString();
                tb2.Text = test.Phis.ToString();
                tb3.Text = test.Math.ToString();
            }
            catch { }
        }

        //добавление данных
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sql = "INSERT INTO test_table (FIO, Phis, Math) VALUES ('" + tb1.Text + "'," +  int.Parse(tb2.Text) + "," + int.Parse(tb3.Text) + ")";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
                DataGrid.Items.Clear();              
                dataUpdate();  
            }
            catch
            {
                MessageBox.Show("Заполните все поля корректными значениями");
            }           
        }

        //Изменить выбранную строку
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                string sql = "UPDATE test_table SET FIO = '" + tb1.Text + "', Phis = " + int.Parse(tb2.Text) + ", Math = " + int.Parse(tb3.Text) + " WHERE id = " + test.id;
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();

                DataGrid.Items.Clear();
                dataUpdate();
            }
            catch
            {
                MessageBox.Show("Заполните все поля корректными значениями");
            }
        }

        //Удалить выбранную строку
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                string sql = "DELETE FROM test_table WHERE id = " + test.id;
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();

                DataGrid.Items.Clear();
                dataUpdate();
            }
            catch
            {
                MessageBox.Show("Что-то пошло не так");
            }
        }
    }
}
