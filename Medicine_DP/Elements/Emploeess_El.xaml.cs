using System;
using System.Collections.Generic;
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
using Medicine_DP.Models;
using Medicine_DP.Windows;

namespace Medicine_DP.Elements
{
    /// <summary>
    /// Логика взаимодействия для Emploeess_El.xaml
    /// </summary>
    public partial class Emploeess_El : UserControl
    {
        employees _emploeess;
        public Emploeess_El(employees employee)
        {
            InitializeComponent();
            _emploeess = employee;
            // Основная информация
            lbEmployeeId.Content = employee.employee_id;
            lbLastName.Content = employee.last_name ?? "Не указано";
            lbFirstName.Content = employee.first_name ?? "Не указано";
            lbMiddleName.Content = employee.middle_name ?? "Не указано";
            lbPosition.Content = employee.position ?? "Не указано";
            lbSpecialization.Content = employee.specialization ?? "Не указано";

            // Даты
            lbBirthDate.Content = employee.birth_date.ToString("dd.MM.yyyy");
            lbHireDate.Content = employee.hire_date.ToString("dd.MM.yyyy");

            // Контактная информация
            lbGender.Content = GetGenderDisplay(employee.gender);
            lbPhoneNumber.Content = employee.phone_number ?? "Не указано";
            lbEmail.Content = employee.email ?? "Не указано";
            lbAddress.Content = employee.address ?? "Не указано";

            // Учетные данные
            lbLogin.Content = employee.login ?? "Не указано";
            lbIsActive.Content = employee.is_active == 1 ? "Активен" : "Неактивен";
        }
        private string GetGenderDisplay(char gender)
        {
            return gender switch
            {
                'M' => "Мужской",
                'F' => "Женский",
                _ => "Не указан"
            };
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Emploeess_Edit_Window emploeess_Edit_Window = new Emploeess_Edit_Window(_emploeess);
            emploeess_Edit_Window.Show();
        }
    }
}
