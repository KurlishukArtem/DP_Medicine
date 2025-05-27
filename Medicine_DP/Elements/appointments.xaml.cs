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

        public Brush StatusColor
        {
            get { return (Brush)GetValue(StatusColorProperty); }
            set { SetValue(StatusColorProperty, value); }
        }

        public static readonly DependencyProperty StatusColorProperty =
            DependencyProperty.Register("StatusColor", typeof(Brush), typeof(appointments));
        public appointments(Models.appointments appointment)
        {
            InitializeComponent();
            _appointment = appointment;
            
            LoadAppointmentData();
        }

        private void LoadAppointmentData()
        {
            //try
            //{
            // Основные данные о приеме
            lbDate.Text = _appointment.appointment_date.ToString("dd.MM.yyyy");
            lbTime.Text = _appointment.start_time.ToString();
            lbStatus.Text = _appointment.status;
            tbNotes.Text = string.IsNullOrEmpty(_appointment.notes) ? "Нет заметок" : _appointment.notes;
            lbCreated.Text = _appointment.created_at.ToString("g");

            // Установка цвета статуса
            StatusColor = _appointment.status switch
            {
                "scheduled" => Brushes.Green,
                "completed" => Brushes.Blue,
                "canceled" => Brushes.Red,
                "no-show" => Brushes.Orange,
                _ => Brushes.Gray
            };

            // Загрузка связанных данных
            var patient = _context.patients
                .FirstOrDefault(p => p.patient_id == _appointment.patient_id);
            lbPatient.Text = patient != null ?
                $"{patient.last_name} {patient.first_name} {patient.middle_name}" :
                "Неизвестно";

            var doctor = _context.employees
                .FirstOrDefault(e => e.employee_id == _appointment.employee_id);
            lbDoctor.Text = doctor != null ?
                $"{doctor.last_name} {doctor.first_name} {doctor.middle_name}" :
                "Неизвестно";

            var service = _context.services
                .FirstOrDefault(s => s.service_id == _appointment.service_id);
            lbService.Text = service.service_name ?? "Неизвестно";

            var room = _context.appointments
                .FirstOrDefault(r => r.room_id == _appointment.room_id);
            lbRoom.Text = room.room_id.ToString();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
            //        MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new appointments_Edit_Window(_appointment);
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
