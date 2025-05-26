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
using System.Windows.Shapes;
using Medicine_DP.Config;
using Medicine_DP.Models;
using Medicine_DP.Pages;

namespace Medicine_DP.Windows
{
    /// <summary>
    /// Логика взаимодействия для Emploeess_Edit_Window.xaml
    /// </summary>
    public partial class Emploeess_Edit_Window : Window
    {
        private readonly DataContext _context = Main._main._context;
        public employees _employee;
        private bool _isNewEmployee;

        public string WindowTitle => _isNewEmployee ? "Новый сотрудник" : "Редактирование сотрудника";
        public Visibility ShowEmployeeId => _isNewEmployee ? Visibility.Collapsed : Visibility.Visible;
        public Emploeess_Edit_Window(employees employee = null)
        {
            InitializeComponent();

            DataContext = this;

            _employee = employee;
            _isNewEmployee = _employee == null;

            if (!_isNewEmployee)
            {
                LoadEmployeeData();
            }
            else
            {
                // Установка значений по умолчанию для нового сотрудника
                dpHireDate.SelectedDate = DateTime.Today;
                chkIsActive.IsChecked = true;
            }
        }

        private void LoadEmployeeData()
        {
            txtEmployeeId.Text = _employee.employee_id.ToString();
            txtLastName.Text = _employee.last_name;
            txtFirstName.Text = _employee.first_name;
            txtMiddleName.Text = _employee.middle_name;
            txtPosition.Text = _employee.position;
            txtSpecialization.Text = _employee.specialization;
            dpBirthDate.SelectedDate = _employee.birth_date;

            // Установка пола
            foreach (ComboBoxItem item in cbGender.Items)
            {
                if (item.Tag.ToString() == _employee.gender.ToString())
                {
                    cbGender.SelectedItem = item;
                    break;
                }
            }

            txtPhoneNumber.Text = _employee.phone_number;
            txtEmail.Text = _employee.email;
            dpHireDate.SelectedDate = _employee.hire_date;
            txtAddress.Text = _employee.address;
            txtLogin.Text = _employee.login;
            chkIsActive.IsChecked = _employee.is_active == 1;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                //if (_isNewEmployee)
                //{
                //    _employee = new employees
                //    {
                //        password_hash = HashPassword("temp123") // Генерация временного пароля
                //    };
                //    _context.employees.Add(_employee);
                //}

                // Обновление данных
                _employee.last_name = txtLastName.Text;
                _employee.first_name = txtFirstName.Text;
                _employee.middle_name = txtMiddleName.Text;
                _employee.position = txtPosition.Text;
                _employee.specialization = txtSpecialization.Text;
                _employee.birth_date = dpBirthDate.SelectedDate.Value;
                _employee.gender = ((ComboBoxItem)cbGender.SelectedItem).Tag.ToString()[0];
                _employee.phone_number = txtPhoneNumber.Text;
                _employee.email = txtEmail.Text;
                _employee.hire_date = dpHireDate.SelectedDate.Value;
                _employee.address = txtAddress.Text;
                _employee.login = txtLogin.Text;
                _employee.is_active = chkIsActive.IsChecked == true ? 1 : 0;

                _context.Update(_employee);
                _context.SaveChanges();

                MessageBox.Show("Данные сотрудника успешно сохранены", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                dpBirthDate.SelectedDate == null ||
                cbGender.SelectedItem == null ||
                dpHireDate.SelectedDate == null)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (dpBirthDate.SelectedDate > DateTime.Today.AddYears(-18))
            {
                MessageBox.Show("Сотрудник должен быть старше 18 лет", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        //private string HashPassword(string password)
        //{
        //    // Реализация хеширования пароля (например, BCrypt)
        //    return BCrypt.Net.BCrypt.HashPassword(password);
        //}

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

