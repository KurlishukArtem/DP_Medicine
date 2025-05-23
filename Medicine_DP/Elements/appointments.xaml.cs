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

namespace Medicine_DP.Elements
{
    /// <summary>
    /// Логика взаимодействия для appointments.xaml
    /// </summary>
    public partial class appointments : UserControl
    {
        private readonly Models.appointments _appointment;
        private readonly DataContext _context = new DataContext();
        private readonly Pages.Main _mainPage;

        public Brush StatusColor
        {
            get { return (Brush)GetValue(StatusColorProperty); }
            set { SetValue(StatusColorProperty, value); }
        }

        public static readonly DependencyProperty StatusColorProperty =
            DependencyProperty.Register("StatusColor", typeof(Brush), typeof(appointments));
        public appointments(Models.appointments appointment, Main mainPage = null)
        {
            InitializeComponent();
            _appointment = appointment;
            _mainPage = mainPage;
            LoadAppointmentData();
        }

        private void LoadAppointmentData()
        {
            try
            {
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
                lbService.Text = service?.service_name ?? "Неизвестно";

                var room = _context.rooms
                    .FirstOrDefault(r => r.room_id == _appointment.room_id);
                lbRoom.Text = room?.room_type ?? "Неизвестно";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new appointments_Edit_Window(_appointment);
            editWindow.Closed += (s, args) =>
            {
                if (_mainPage != null)
                {
                    _mainPage.CreateUIapps();
                }
            };
            editWindow.ShowDialog();
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var confirmResult = MessageBox.Show(
                    $"Вы действительно хотите удалить запись на прием?\n\n" +
                    $"Дата: {_appointment.appointment_date:dd.MM.yyyy}\n" +
                    $"Время: {_appointment.start_time}\n" +
                    $"Пациент: {lbPatient.Text}",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirmResult != MessageBoxResult.Yes) return;

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Удаление связанных записей
                        var records = await _context.medical_records
                            .Where(mr => mr.appointment_id == _appointment.appointment_id)
                            .ToListAsync();

                        foreach (var record in records)
                        {
                            var prescriptions = await _context.prescriptions
                                .Where(p => p.record_id == record.record_id)
                                .ToListAsync();
                            _context.prescriptions.RemoveRange(prescriptions);
                        }

                        _context.medical_records.RemoveRange(records);

                        var payments = await _context.payments
                            .Where(p => p.appointment_id == _appointment.appointment_id)
                            .ToListAsync();
                        _context.payments.RemoveRange(payments);

                        // Удаление самой записи
                        _context.appointments.Remove(_appointment);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        MessageBox.Show("Запись на прием успешно удалена", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        _mainPage?.CreateUIapps();
                    }
                    catch (DbUpdateException dbEx)
                    {
                        await transaction.RollbackAsync();
                        MessageBox.Show($"Ошибка базы данных: {dbEx.InnerException?.Message}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
