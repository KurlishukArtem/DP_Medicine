using Medicine_DP.Config;
using Medicine_DP.Models;
using Microsoft.EntityFrameworkCore;
using System;
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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (chkConsent.IsChecked != true)
            {
                MessageBox.Show("Вы должны дать согласие на обработку персональных данных.",
                                "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //try
            //{
            // Валидация обязательных полей
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
                throw new Exception("Фамилия обязательна для заполнения");
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
                throw new Exception("Имя обязательно для заполнения");
            if (dpBirthDate.SelectedDate == null)
                throw new Exception("Дата рождения обязательна для заполнения");

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
                    password_hash = txtPassword.Password
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
                _patient.password_hash = txtPassword.Password;

                // Для существующего пациента помечаем как измененный
                _context.Entry(_patient).State = EntityState.Modified;
            }

            _context.SaveChanges();
            MessageBox.Show("Данные пациента успешно сохранены", "Успех",
                           MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
            //                    MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }
    }
}
