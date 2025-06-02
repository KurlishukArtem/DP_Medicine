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
using System.Windows.Shapes;
using Medicine_DP.Config;
using Medicine_DP.Models;
using Medicine_DP.Pages;
using Microsoft.EntityFrameworkCore;

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
                // Создаем нового сотрудника или используем существующего
                if (_isNewEmployee)
                {
                    _employee = new employees
                    {
                        // Установка значений по умолчанию для нового сотрудника
                        password_hash = "temp123", // Временный пароль (рекомендуется хеширование)
                        //created_at = DateTime.Now,
                        //registration_date = DateTime.Now
                    };
                    _context.employees.Add(_employee);
                }

                // Обновление данных (общее для нового и существующего сотрудника)
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
                _employee.password_hash = txtPassword.Text;
                _employee.is_active = chkIsActive.IsChecked == true ? 1 : 0;
                //_employee.rooms = int.TryParse(txtRoom.Text, out int room) ? room : 0;

                // Для нового сотрудника генерируем логин, если не заполнен
                if (_isNewEmployee && string.IsNullOrEmpty(_employee.login))
                {
                    _employee.login = GenerateDefaultLogin();
                }

                _context.SaveChanges();

                MessageBox.Show(_isNewEmployee ? "Новый сотрудник успешно добавлен" : "Данные сотрудника успешно обновлены",
                              "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (DbUpdateException dbEx)
            {
                MessageBox.Show($"Ошибка сохранения в базу данных: {dbEx.InnerException?.Message ?? dbEx.Message}",
                              "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Генерация логина по умолчанию (фамилия + инициалы)
        private string GenerateDefaultLogin()
        {
            string lastName = txtLastName.Text.Trim();
            string firstName = txtFirstName.Text.Trim();
            string middleName = txtMiddleName.Text.Trim();

            string initials = string.Empty;
            if (firstName.Length > 0) initials += firstName[0];
            if (middleName.Length > 0) initials += middleName[0];

            return $"{lastName}{initials}".ToLower();
        }

        // Обновленный метод валидации
        private bool ValidateInput()
        {
            StringBuilder errors = new StringBuilder();

            // Проверка обязательных полей
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
                errors.AppendLine("Фамилия обязательна для заполнения");

            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
                errors.AppendLine("Имя обязательно для заполнения");

            if (dpBirthDate.SelectedDate == null)
                errors.AppendLine("Дата рождения обязательна для заполнения");
            else if (dpBirthDate.SelectedDate > DateTime.Today.AddYears(-18))
                errors.AppendLine("Сотрудник должен быть старше 18 лет");

            if (cbGender.SelectedItem == null)
                errors.AppendLine("Укажите пол сотрудника");

            if (dpHireDate.SelectedDate == null)
                errors.AppendLine("Дата приема на работу обязательна для заполнения");
            else if (dpBirthDate.SelectedDate != null && dpHireDate.SelectedDate < dpBirthDate.SelectedDate.Value.AddYears(18))
                errors.AppendLine("Дата приема на работу не может быть раньше совершеннолетия сотрудника");

            // Проверка уникальности логина (только для нового сотрудника)
            if (_isNewEmployee && !string.IsNullOrEmpty(txtLogin.Text))
            {
                bool loginExists = _context.employees.Any(emp => emp.login == txtLogin.Text);
                if (loginExists)
                    errors.AppendLine("Этот логин уже занят");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибки ввода",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

