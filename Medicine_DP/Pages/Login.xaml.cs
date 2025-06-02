using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using Medicine_DP.Config;
using Medicine_DP.Models;
using Medicine_DP.Windows;

namespace Medicine_DP.Pages
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        
        Models.employees employees;
        private readonly DataContext dataContext = new DataContext();
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string loginUser = UsernameTextBox.Text;
            string passwordBox = Password.Password.ToString();

            try
            {
                // Сначала проверяем сотрудников
                var empl = dataContext.employees.FirstOrDefault(x => x.login == loginUser && x.password_hash == passwordBox);
                if (empl != null)
                {
                    MainWindow.init.OpenPages(new Pages.Main(loginUser));
                    return;
                }

                // Затем проверяем пациентов
                var patient = dataContext.patients.FirstOrDefault(x => x.login == loginUser && x.password_hash == passwordBox);
                if (patient != null)
                {
                    MainWindow.init.OpenPages(new Pages.Main(loginUser));
                    return;
                }

                MessageBox.Show("Неверный логин или пароль!", "Ошибка");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при входе: {ex.Message}", "Ошибка");
            }
        }

        private void Reg_Click(object sender, RoutedEventArgs e)
        {
            Patients_Edit_Window patients_Edit_ = new Patients_Edit_Window();
            patients_Edit_.Show();
        }

        private void Forgot_Password_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
