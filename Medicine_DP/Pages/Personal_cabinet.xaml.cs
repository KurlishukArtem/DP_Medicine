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
using Medicine_DP.Config;
using Medicine_DP.Models;
using Medicine_DP.Windows;
using Microsoft.EntityFrameworkCore;

namespace Medicine_DP.Pages
{
    /// <summary>
    /// Логика взаимодействия для Personal_cabinet.xaml
    /// </summary>
    public partial class Personal_cabinet : Page
    {
        private readonly DataContext _context = Main._main._context;
        private employees _employee;
        public patients _patient;
        string _username;
        public Personal_cabinet(string username, patients patients = null)
        {
            InitializeComponent();
            _username = username;
            _patient = patients;
            LoadUserData(username);
            LoadAppointments();
        }
        private void LoadUserData(string username)
        {
            // Проверяем, является ли пользователь сотрудником
            _employee = _context.employees
                .FirstOrDefault(e => e.login == username);

            if (_employee != null)
            {
                // Заполняем данные сотрудника
                lblFullName.Content = $"{_employee.last_name} {_employee.first_name} {_employee.middle_name}";
                lblBirthDate.Content = _employee.birth_date.ToString("dd.MM.yyyy");
                lblPhone.Content = _employee.phone_number;
                lblEmail.Content = _employee.email;
                lblAddress.Content = _employee.address;

                // Скрываем панель пациента
                patientPanel.Visibility = Visibility.Collapsed;
                //btnNewAppointment.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Проверяем, является ли пользователь пациентом
                _patient = _context.patients
                    .FirstOrDefault(p => p.login == username);

                if (_patient != null)
                {
                    // Заполняем данные пациента
                    lblFullName.Content = $"{_patient.last_name} {_patient.first_name} {_patient.middle_name}";
                    lblBirthDate.Content = _patient.birth_date.ToString("dd.MM.yyyy");
                    lblPhone.Content = _patient.phone_number;
                    lblEmail.Content = _patient.email;
                    lblAddress.Content = _patient.address;
                    lblPolicy.Content = _patient.policy_number;
                    lblSnils.Content = _patient.snils;

                    // Показываем панель пациента
                    patientPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Пользователь не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    
                }
            }
        }

        private void LoadAppointments()
        {
            //if (_employee != null)
            //{
            //    // Для сотрудника загружаем записи с информацией о пациенте и услуге
            //    dgAppointments.ItemsSource = _context.appointments
            //        .Where(a => a.employee_id == _employee.employee_id)

            //        .OrderByDescending(a => a.appointment_date)
            //        .ToList();
            //}
            //else if (_patient != null)
            //{
            //    // Для пациента загружаем записи с информацией о враче и услуге
            //    dgAppointments.ItemsSource = _context.appointments
            //        .Where(a => a.patient_id == _patient.patient_id)

            //        .OrderByDescending(a => a.appointment_date)
            //        .ToList();
            //}
        }

        private void btnNewAppointment_Click(object sender, RoutedEventArgs e)
        {

        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPages(new Pages.Main(_username));
        }

        private void zapis_Click(object sender, RoutedEventArgs e)
        {
            // Создаем экземпляр окна для добавления новой записи
            Windows.AddAppointmentWindow addAppointmentWindow = new AddAppointmentWindow();

            // Настраиваем окно

            addAppointmentWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner; // Позиционируем по центру родительского окна

            //// Подписываемся на событие закрытия окна (если нужно обновить данные после закрытия)
            //addAppointmentWindow.Closed += (s, args) =>
            //{
            //    // Здесь можно обновить данные в главном окне, если нужно
            //    // Например: RefreshAppointmentsList();
            //};
            addAppointmentWindow.Show();
        }

        private void Exit_out_Click(object sender, RoutedEventArgs e)
        {

            // Создаем диалоговое окно подтверждения
            MessageBoxResult result = MessageBox.Show(
                "Вы точно хотите выйти из аккаунта?",
                "Подтверждение выхода",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            // Если пользователь подтвердил выход
            if (result == MessageBoxResult.Yes)
            {
                // Переходим на страницу входа
                MainWindow.init.OpenPages(new Pages.Login());

            }
            // Если пользователь передумал, ничего не делаем
        }

        private void btnEditProfile_Click(object sender, RoutedEventArgs e)
        {
            if (_employee != null)
            {
                // Редактирование данных сотрудника
                var editWindow = new Employees_Edit_Window(_employee);
                editWindow.Show();

                // Обновляем данные после редактирования
                if (editWindow.DialogResult == true)
                {
                    LoadUserData(_username);
                }
            }
            else if (_patient != null)
            {
                // Редактирование данных пациента
                var editWindow = new Patients_Edit_Window(_patient);
                editWindow.Show();

                // Обновляем данные после редактирования
                if (editWindow.DialogResult == true)
                {
                    LoadUserData(_username);
                }
                
            }
            else
            {
                MessageBox.Show("Не удалось определить тип пользователя для редактирования",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //private void btnNewAppointment_Click(object sender, RoutedEventArgs e)
        //{
        //    if (_patient != null)
        //    {
        //        var appointmentWindow = new AddAppointmentWindow(_patient.patient_id);
        //        appointmentWindow.ShowDialog();
        //        LoadAppointments();

        //    }
        //}
    }
}
