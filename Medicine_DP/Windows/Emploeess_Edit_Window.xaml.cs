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
            txtPassword.Text = _employee.password_hash;
            chkIsActive.IsChecked = _employee.is_active == 1;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                // Проверка на уникальность email
                string email = txtEmail.Text;
                if (_context.employees.Any(emp => emp.email == email && (_isNewEmployee || emp.employee_id != _employee.employee_id)))
                {
                    MessageBox.Show("Сотрудник с таким email уже существует", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка на уникальность login
                string login = txtLogin.Text;
                if (_context.employees.Any(emp => emp.login == login && (_isNewEmployee || emp.employee_id != _employee.employee_id)))
                {
                    MessageBox.Show("Сотрудник с таким логином уже существует", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Создаем нового сотрудника или используем существующего
                if (_isNewEmployee)
                {
                    _employee = new employees
                    {
                        password_hash = "temp123", // Временный пароль (рекомендуется хеширование)
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
                _employee.email = email;
                _employee.hire_date = dpHireDate.SelectedDate.Value;
                _employee.address = txtAddress.Text;
                _employee.login = login;
                _employee.password_hash = txtPassword.Text;
                _employee.is_active = chkIsActive.IsChecked == true ? 1 : 0;

                // Для нового сотрудника генерируем логин, если не заполнен
                if (_isNewEmployee && string.IsNullOrEmpty(_employee.login))
                {
                    _employee.login = GenerateDefaultLogin();
                }

                _context.SaveChanges();

                MessageBox.Show(_isNewEmployee ? "Новый сотрудник успешно добавлен" : "Данные сотрудника успешно обновлены",
                              "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

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
                errors.AppendLine("• Фамилия обязательна для заполнения");
            else if (txtLastName.Text.Length > 50)
                errors.AppendLine("• Фамилия не должна превышать 50 символов");

            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
                errors.AppendLine("• Имя обязательно для заполнения");
            else if (txtFirstName.Text.Length > 50)
                errors.AppendLine("• Имя не должно превышать 50 символов");

            if (!string.IsNullOrWhiteSpace(txtMiddleName.Text) && txtMiddleName.Text.Length > 50)
                errors.AppendLine("• Отчество не должно превышать 50 символов");

            if (string.IsNullOrWhiteSpace(txtPosition.Text))
                errors.AppendLine("• Должность обязательна для заполнения");
            else if (txtPosition.Text.Length > 100)
                errors.AppendLine("• Должность не должна превышать 100 символов");

            if (!string.IsNullOrWhiteSpace(txtSpecialization.Text) && txtSpecialization.Text.Length > 100)
                errors.AppendLine("• Специализация не должна превышать 100 символов");

            // Проверка даты рождения
            if (dpBirthDate.SelectedDate == null)
                errors.AppendLine("• Дата рождения обязательна для заполнения");
            else
            {
                if (dpBirthDate.SelectedDate > DateTime.Today)
                    errors.AppendLine("• Дата рождения не может быть в будущем");
                else if (dpBirthDate.SelectedDate > DateTime.Today.AddYears(-18))
                    errors.AppendLine("• Сотрудник должен быть старше 18 лет");
                else if (dpBirthDate.SelectedDate < DateTime.Today.AddYears(-100))
                    errors.AppendLine("• Проверьте дату рождения (возраст более 100 лет)");
            }

            // Проверка пола
            if (cbGender.SelectedItem == null)
                errors.AppendLine("• Укажите пол сотрудника");

            // Проверка номера телефона
            if (!string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(txtPhoneNumber.Text, @"^[\d\s\(\)\+-]{10,20}$"))
                    errors.AppendLine("• Некорректный формат номера телефона");
            }

            // Проверка email
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
                errors.AppendLine("• Email обязателен для заполнения");
            else if (txtEmail.Text.Length > 100)
                errors.AppendLine("• Email не должен превышать 100 символов");
            else if (!IsValidEmail(txtEmail.Text))
                errors.AppendLine("• Некорректный формат email");

            // Проверка даты приема на работу
            if (dpHireDate.SelectedDate == null)
                errors.AppendLine("• Дата приема на работу обязательна для заполнения");
            else
            {
                if (dpHireDate.SelectedDate > DateTime.Today)
                    errors.AppendLine("• Дата приема на работу не может быть в будущем");

                if (dpBirthDate.SelectedDate != null && dpHireDate.SelectedDate < dpBirthDate.SelectedDate.Value.AddYears(14))
                    errors.AppendLine("• Дата приема на работу не может быть раньше 14-летия сотрудника");
            }

            // Проверка адреса
            if (!string.IsNullOrWhiteSpace(txtAddress.Text) && txtAddress.Text.Length > 200)
                errors.AppendLine("• Адрес не должен превышать 200 символов");

            // Проверка логина
            if (string.IsNullOrWhiteSpace(txtLogin.Text))
                errors.AppendLine("• Логин обязателен для заполнения");
            else if (txtLogin.Text.Length > 50)
                errors.AppendLine("• Логин не должен превышать 50 символов");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(txtLogin.Text, @"^[a-zA-Z0-9_]+$"))
                errors.AppendLine("• Логин может содержать только буквы, цифры и подчеркивание");

            // Проверка пароля
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
                errors.AppendLine("• Пароль обязателен для заполнения");
            else if (txtPassword.Text.Length < 6)
                errors.AppendLine("• Пароль должен содержать минимум 6 символов");
            else if (txtPassword.Text.Length > 100)
                errors.AppendLine("• Пароль не должен превышать 100 символов");

            if (errors.Length > 0)
            {
                MessageBox.Show("Обнаружены следующие ошибки:\n\n" + errors.ToString(),
                               "Ошибки ввода",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        // Метод для проверки валидности email
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

