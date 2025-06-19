using Medicine_DP.Config;
using Medicine_DP.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace Medicine_DP.Windows
{
    /// <summary>
    /// Логика взаимодействия для Patients_Edit_Window.xaml
    /// </summary>
    public partial class Patients_Edit_Window : Window
    {
        public patients _patient;
        private readonly DataContext _context = new DataContext();
        public Patients_Edit_Window(patients patient = null)
        {
            InitializeComponent();

            _patient = patient;

            if (patient != null)
            {
                // Заполнение полей для редактирования
                txtPatientId.Text = patient.patient_id.ToString();
                txtLastName.Text = patient.last_name;
                txtFirstName.Text = patient.first_name;
                txtMiddleName.Text = patient.middle_name;
                dpBirthDate.SelectedDate = patient.birth_date;

                if (patient.gender == Char.Parse("M"))
                    rbMale.IsChecked = true;
                else
                    rbFemale.IsChecked = true;

                txtPhone.Text = patient.phone_number;
                txtEmail.Text = patient.email;
                txtAddress.Text = patient.address;
                txtPassportSeries.Text = patient.passport_series;
                txtPassportNumber.Text = patient.passport_number;
                txtSnils.Text = patient.snils;
                txtPolicyNumber.Text = patient.policy_number;
                dpRegistrationDate.SelectedDate = patient.registration_date;
                txtNotes.Text = patient.notes;
                txtLogin.Text = patient.login;
            }
            else
            {
                // Установка значений по умолчанию для нового пациента
                txtPatientId.Text = "Новый пациент";
                dpRegistrationDate.SelectedDate = DateTime.Now;
                rbMale.IsChecked = true;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ValidatePatientData()
        {
            var errors = new List<string>();

            // Проверка обязательных текстовых полей
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
                errors.Add("Фамилия обязательна для заполнения");
            else if (txtLastName.Text.Length > 50)
                errors.Add("Фамилия не должна превышать 50 символов");

            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
                errors.Add("Имя обязательно для заполнения");
            else if (txtFirstName.Text.Length > 50)
                errors.Add("Имя не должно превышать 50 символов");

            if (string.IsNullOrWhiteSpace(txtMiddleName.Text))
                errors.Add("Отчество обязательно для заполнения");
            else if (txtMiddleName.Text.Length > 50)
                errors.Add("Отчество не должно превышать 50 символов");

            // Проверка даты рождения
            if (dpBirthDate.SelectedDate == null)
                errors.Add("Дата рождения обязательна для заполнения");
            else
            {
                if (dpBirthDate.SelectedDate > DateTime.Today)
                    errors.Add("Дата рождения не может быть в будущем");
                else if (dpBirthDate.SelectedDate < DateTime.Today.AddYears(-120))
                    errors.Add("Проверьте дату рождения (возраст более 120 лет)");
            }

            // Проверка телефона
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
                errors.Add("Телефон обязателен для заполнения");
            else if (!Regex.IsMatch(txtPhone.Text, @"^[\d\s\(\)\+-]{10,15}$"))
                errors.Add("Некорректный формат телефона");

            // Проверка email
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                if (txtEmail.Text.Length > 100)
                    errors.Add("Email не должен превышать 100 символов");
                else if (!IsValidEmail(txtEmail.Text))
                    errors.Add("Некорректный формат email");
            }

            // Проверка адреса
            if (string.IsNullOrWhiteSpace(txtAddress.Text))
                errors.Add("Адрес обязателен для заполнения");
            else if (txtAddress.Text.Length > 200)
                errors.Add("Адрес не должен превышать 200 символов");

            // Проверка паспортных данных
            if (string.IsNullOrWhiteSpace(txtPassportSeries.Text))
                errors.Add("Серия паспорта обязательна для заполнения");
            else if (!Regex.IsMatch(txtPassportSeries.Text, @"^\d{4}$"))
                errors.Add("Серия паспорта должна содержать 4 цифры");

            if (string.IsNullOrWhiteSpace(txtPassportNumber.Text))
                errors.Add("Номер паспорта обязателен для заполнения");
            else if (!Regex.IsMatch(txtPassportNumber.Text, @"^\d{6}$"))
                errors.Add("Номер паспорта должен содержать 6 цифр");

            // Проверка СНИЛС
            if (string.IsNullOrWhiteSpace(txtSnils.Text))
                errors.Add("СНИЛС обязателен для заполнения");
            else if (!Regex.IsMatch(txtSnils.Text, @"^\d{3}-\d{3}-\d{3} \d{2}$"))
                errors.Add("СНИЛС должен быть в формате: XXX-XXX-XXX XX");

            // Проверка полиса
            if (string.IsNullOrWhiteSpace(txtPolicyNumber.Text))
                errors.Add("Номер полиса обязателен для заполнения");
            else if (txtPolicyNumber.Text.Length != 16 || !txtPolicyNumber.Text.All(char.IsDigit))
                errors.Add("Номер полиса должен содержать 16 цифр");

            // Проверка даты регистрации
            if (dpRegistrationDate.SelectedDate == null)
                errors.Add("Дата регистрации обязательна для заполнения");
            else if (dpRegistrationDate.SelectedDate > DateTime.Today)
                errors.Add("Дата регистрации не может быть в будущем");

            // Проверка логина и пароля (если требуется)
            if (!string.IsNullOrWhiteSpace(txtLogin.Text) && txtLogin.Text.Length > 50)
                errors.Add("Логин не должен превышать 50 символов");

            if (!string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                if (txtPassword.Password.Length < 6)
                    errors.Add("Пароль должен содержать минимум 6 символов");
                else if (txtPassword.Password.Length > 100)
                    errors.Add("Пароль не должен превышать 100 символов");
            }

            if (errors.Count > 0)
            {
                throw new Exception("Обнаружены следующие ошибки:\n\n" + string.Join("\n", errors));
            }
        }

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
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка согласия
                if (chkConsent.IsChecked != true)
                {
                    MessageBox.Show("Вы должны дать согласие на обработку персональных данных.",
                                  "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Валидация всех полей
                ValidatePatientData();

                // Проверка уникальности email
                if (_context.patients.Any(p => p.email == txtEmail.Text &&
                                            (_patient == null || p.patient_id != _patient.patient_id)))
                {
                    throw new Exception("Пациент с таким email уже существует");
                }

                // Проверка уникальности СНИЛС
                if (_context.patients.Any(p => p.snils == txtSnils.Text &&
                                            (_patient == null || p.patient_id != _patient.patient_id)))
                {
                    throw new Exception("Пациент с таким СНИЛС уже существует");
                }

                // Проверка уникальности паспорта
                if (_context.patients.Any(p => p.passport_series == txtPassportSeries.Text &&
                                            p.passport_number == txtPassportNumber.Text &&
                                            (_patient == null || p.patient_id != _patient.patient_id)))
                {
                    throw new Exception("Пациент с такими паспортными данными уже существует");
                }

            // Логика сохранения (остается без изменений)
            if (_patient == null)
            {
                // Создание нового пациента
                _patient = new patients
                {
                    last_name = txtLastName.Text,
                    first_name = txtFirstName.Text,
                    middle_name = txtMiddleName.Text,
                    birth_date = dpBirthDate.SelectedDate.Value,
                    gender = rbMale.IsChecked == true ? 'М' : 'Ж',
                    phone_number = txtPhone.Text,
                    email = txtEmail.Text,
                    address = txtAddress.Text,
                    passport_series = txtPassportSeries.Text,
                    passport_number = txtPassportNumber.Text,
                    snils = txtSnils.Text,
                    policy_number = txtPolicyNumber.Text,
                    registration_date = dpRegistrationDate.SelectedDate ?? DateTime.Now,
                    notes = txtNotes.Text,
                    login = txtLogin.Text,
                    password_hash = HashPassword.Hash(txtPassword.Password),
                };
                _context.patients.Add(_patient); // Только Add для нового пациента
            }
            else
            {
                // Обновление существующего пациента
                _patient.last_name = txtLastName.Text;
                _patient.first_name = txtFirstName.Text;
                _patient.middle_name = txtMiddleName.Text;
                _patient.birth_date = dpBirthDate.SelectedDate.Value;
                _patient.gender = rbMale.IsChecked == true ? 'М' : 'Ж';
                _patient.phone_number = txtPhone.Text;
                _patient.email = txtEmail.Text;
                _patient.address = txtAddress.Text;
                _patient.passport_series = txtPassportSeries.Text;
                _patient.passport_number = txtPassportNumber.Text;
                _patient.snils = txtSnils.Text;
                _patient.policy_number = txtPolicyNumber.Text;
                _patient.notes = txtNotes.Text;
                _patient.login = txtLogin.Text;
                    _patient.password_hash = HashPassword.Hash(txtPassword.Password);

                // Для существующего пациента помечаем как измененный
                _context.Entry(_patient).State = EntityState.Modified;
            }
            _context.SaveChanges();
                MessageBox.Show("Данные пациента успешно сохранены", "Успех",
                               MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка валидации",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    
    }
}
