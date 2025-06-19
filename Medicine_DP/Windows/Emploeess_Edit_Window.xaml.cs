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
                string email = txtEmail.Text.Trim();
                string login = txtLogin.Text.Trim();
                string password = txtPassword.Text;

                // Проверка уникальности email (исключая текущего сотрудника при редактировании)
                if (_context.employees.Any(emp =>
                    emp.email == email &&
                    (_isNewEmployee || emp.employee_id != _employee.employee_id)))
                {
                    MessageBox.Show("Сотрудник с таким email уже существует", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка уникальности login (исключая текущего сотрудника при редактировании)
                if (_context.employees.Any(emp =>
                    emp.login == login &&
                    (_isNewEmployee || emp.employee_id != _employee.employee_id)))
                {
                    MessageBox.Show("Сотрудник с таким логином уже существует", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (_isNewEmployee)
                {
                    // Создание нового сотрудника
                    _employee = new employees
                    {
                        last_name = txtLastName.Text.Trim(),
                        first_name = txtFirstName.Text.Trim(),
                        middle_name = txtMiddleName.Text.Trim(),
                        position = txtPosition.Text.Trim(),
                        specialization = txtSpecialization.Text.Trim(),
                        birth_date = dpBirthDate.SelectedDate.Value,
                        gender = ((ComboBoxItem)cbGender.SelectedItem).Tag.ToString()[0],
                        phone_number = txtPhoneNumber.Text.Trim(),
                        email = email,
                        hire_date = dpHireDate.SelectedDate.Value,
                        address = txtAddress.Text.Trim(),
                        login = string.IsNullOrEmpty(login) ? GenerateDefaultLogin() : login,
                        password_hash = HashPassword.Hash(txtPassword.Text),
                        is_active = chkIsActive.IsChecked == true ? 1 : 0,
                        rooms = 0 // Или другое значение по умолчанию
                    };
                    _context.employees.Add(_employee);
                }
                else
                {
                    // Обновление существующего сотрудника
                    _employee.last_name = txtLastName.Text.Trim();
                    _employee.first_name = txtFirstName.Text.Trim();
                    _employee.middle_name = txtMiddleName.Text.Trim();
                    _employee.position = txtPosition.Text.Trim();
                    _employee.specialization = txtSpecialization.Text.Trim();
                    _employee.birth_date = dpBirthDate.SelectedDate.Value;
                    _employee.gender = ((ComboBoxItem)cbGender.SelectedItem).Tag.ToString()[0];
                    _employee.phone_number = txtPhoneNumber.Text.Trim();
                    _employee.email = email;
                    _employee.hire_date = dpHireDate.SelectedDate.Value;
                    _employee.address = txtAddress.Text.Trim();
                    _employee.login = login;

                    // Обновляем пароль только если поле не пустое
                    if (!string.IsNullOrEmpty(password))
                    {
                        _employee.password_hash = HashPassword.Hash(password);
                    }

                    _employee.is_active = chkIsActive.IsChecked == true ? 1 : 0;
                    

                    // Явно помечаем сущность как измененную
                    _context.Entry(_employee).State = EntityState.Modified;
                }

                _context.SaveChanges();

                MessageBox.Show(_isNewEmployee ? "Новый сотрудник успешно добавлен" : "Данные сотрудника успешно обновлены",
                              "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
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
            this.Close();
        }
    }
}

