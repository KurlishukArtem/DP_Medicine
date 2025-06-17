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
using Medicine_DP.Pages;
using Medicine_DP.Windows;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace Medicine_DP.Elements
{
    /// <summary>
    /// Логика взаимодействия для appointments.xaml
    /// </summary>
    public partial class appointments : UserControl
    {
        private readonly Models.appointments _appointment;
        private readonly Pages.Main _mainPage = Main._main;
        private readonly DataContext _context= Main._main._context;
        public static string _loginUser;
        public Brush StatusColor
        {
            get { return (Brush)GetValue(StatusColorProperty); }
            set { SetValue(StatusColorProperty, value); }
        }

        public static readonly DependencyProperty StatusColorProperty =
            DependencyProperty.Register("StatusColor", typeof(Brush), typeof(appointments));
        public appointments(Models.appointments appointment, string loginUser = null)
        {
            InitializeComponent();
            _appointment = appointment;
            _loginUser = loginUser;
            LoadAppointmentData();
            ConfigureUIForUserRole();
        }
        private void ConfigureUIForUserRole()
        {
            var currentUser = _context.employees.FirstOrDefault(e => e.login == _loginUser);

            if (currentUser == null)
            {
                // Проверяем, может это пациент
                bool isPatient = _context.patients.Any(p => p.login == _loginUser);
                if (isPatient)
                {
                    btnDelete.Visibility = Visibility.Collapsed;
                    btnEdit.Visibility = Visibility.Collapsed;
                    return;
                }

                MessageBox.Show("Не удалось определить роль пользователя", "Ошибка");
                return;
            }

            // Если пользователь - врач
            if (currentUser.position == "Врач")
            {
                // Скрываем кнопки, если запись не принадлежит этому врачу
                bool isOwnAppointment = _appointment.employee_id == currentUser.employee_id;
                btnDelete.Visibility = isOwnAppointment ? Visibility.Visible : Visibility.Collapsed;
                btnEdit.Visibility = isOwnAppointment ? Visibility.Visible : Visibility.Collapsed;

                // Можно также скрыть/показать весь элемент управления
                this.Visibility = isOwnAppointment ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (currentUser.position == "Администратор")
            {
                // Администратор видит все записи
                btnDelete.Visibility = Visibility.Visible;
                btnEdit.Visibility = Visibility.Visible;
            }
        }

        private void LoadAppointmentData()
        {
            try
            {
                // Форматируем дату и время
                lbDate.Text = _appointment.appointment_date.ToString("dd.MM.yyyy");
                lbTime.Text = _appointment.start_time.ToString(@"hh\:mm");

                // Устанавливаем локализованный статус
                lbStatus.Text = GetLocalizedStatus(_appointment.status);

                // Устанавливаем цвет статуса
                StatusColor = GetStatusBrush(_appointment.status);

                // Заполняем остальные поля
                tbNotes.Text = string.IsNullOrEmpty(_appointment.notes) ? "Нет заметок" : _appointment.notes;
                lbCreated.Text = _appointment.created_at.ToString("g");

                // Загружаем связанные данные
                LoadRelatedData();

                // Настраиваем видимость элементов управления
                ConfigureControlsVisibility();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRelatedData()
        {
            // Загрузка данных пациента
            var patient = _context.patients
                .FirstOrDefault(p => p.patient_id == _appointment.patient_id);
            lbPatient.Text = patient != null
                ? $"{patient.last_name} {patient.first_name} {patient.middle_name}"
                : "Неизвестно";

            // Загрузка данных врача
            var doctor = _context.employees
                .FirstOrDefault(e => e.employee_id == _appointment.employee_id);
            lbDoctor.Text = doctor != null
                ? $"{doctor.last_name} {doctor.first_name} {doctor.middle_name}"
                : "Неизвестно";

            // Загрузка данных услуги
            var service = _context.services
                .FirstOrDefault(s => s.service_id == _appointment.service_id);
            lbService.Text = service?.service_name ?? "Неизвестно";
            
            // Номер кабинета (может быть null)
            lbRoom.Text = _appointment.room_id?.ToString() ?? "Не указан";
        }

        private string GetLocalizedStatus(string status)
        {
            return status switch
            {
                "scheduled" => "Запланирована",
                "completed" => "Завершена",
                "canceled" => "Отменена",
                "no-show" => "Неявка",
                _ => status
            };
        }

        private Brush GetStatusBrush(string status)
        {
            return status switch
            {
                "scheduled" => Brushes.Green,
                "completed" => Brushes.Blue,
                "canceled" => Brushes.Red,
                "no-show" => Brushes.Orange,
                _ => Brushes.Gray
            };
        }

        private void ConfigureControlsVisibility()
        {
            // Проверяем роль текущего пользователя
            var currentUser = _context.employees.FirstOrDefault(e => e.login == _loginUser);
            var currentPatient = _context.patients.FirstOrDefault(p => p.login == _loginUser);

            // Настройка видимости для пациентов
            if (currentPatient != null)
            {
                bool isPatientAppointment = _appointment.patient_id == currentPatient.patient_id;
                bool isScheduled = _appointment.status == "scheduled";

                lblCancelAppointment.Visibility = isPatientAppointment && isScheduled
                    ? Visibility.Visible
                    : Visibility.Collapsed;

                btnEdit.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
                return;
            }

            // Настройка видимости для сотрудников
            if (currentUser != null)
            {
                bool isAdmin = currentUser.position == "Администратор";
                bool isDoctorOwner = currentUser.position == "Врач" &&
                                  currentUser.employee_id == _appointment.employee_id;

                btnEdit.Visibility = isAdmin || isDoctorOwner ? Visibility.Visible : Visibility.Collapsed;
                btnDelete.Visibility = isAdmin || isDoctorOwner ? Visibility.Visible : Visibility.Collapsed;
                lblCancelAppointment.Visibility = Visibility.Collapsed;
                return;
            }

            // Для неавторизованных пользователей скрываем все элементы управления
            btnEdit.Visibility = Visibility.Collapsed;
            btnDelete.Visibility = Visibility.Collapsed;
            lblCancelAppointment.Visibility = Visibility.Collapsed;
        }

        private async void LblCancelAppointment_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var result = MessageBox.Show("Вы действительно хотите отменить эту запись?",
                                           "Подтверждение отмены",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                // Обновляем статус записи
                _appointment.status = "canceled";

                // Сохраняем изменения в БД
                using (var context = new DataContext())
                {
                    context.appointments.Update(_appointment);
                    await context.SaveChangesAsync();
                }

                // Обновляем отображение
                LoadAppointmentData();

                // Обновляем список записей
                if (_mainPage != null)
                {
                    _mainPage.CreateUIapps();
                }

                MessageBox.Show("Запись успешно отменена", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отмене записи: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new AddAppointmentWindow(_appointment);
            editWindow.Closed += (s, args) =>
            {
                
                    _mainPage.CreateUIapps();
                
            };
            editWindow.ShowDialog();
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                var confirm = MessageBox.Show("Удалить запись о приеме?", "Подтверждение", MessageBoxButton.YesNo);
                if (confirm != MessageBoxResult.Yes) return;

                using (var newContext = new DataContext())
                {
                    // Получаем запись без зависимостей
                    var appointment = new Models.appointments { appointment_id = _appointment.appointment_id };
                    newContext.appointments.Attach(appointment);
                    newContext.appointments.Remove(appointment);

                    await newContext.SaveChangesAsync();
                    MessageBox.Show("Запись удалена");
                    _mainPage.CreateUIapps();
                }
            }
            catch (DbUpdateException ex) when (ex.InnerException is MySqlException mysqlEx &&
                                             mysqlEx.Message.Contains("a foreign key constraint fails"))
            {
                MessageBox.Show("Нельзя удалить запись, так как есть связанные данные");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }

        }

       
        
    }
}
