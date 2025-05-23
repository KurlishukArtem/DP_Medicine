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

namespace Medicine_DP.Pages
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        
        Models.employees employees;
        DataContext dataContext = new DataContext();
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
                var empl = dataContext.employees.Where(x => x.login == loginUser && x.password_hash == passwordBox).First();
                if (empl != null)
                {
                    MainWindow.init.OpenPages(new Pages.Main(loginUser));
                }
                else
                {
                    MessageBox.Show("проверьте корректность ввода!", "Ошибка");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("проверьте корректность ввода!", "Ошибка");
                return;
            }
        }
    }
}
