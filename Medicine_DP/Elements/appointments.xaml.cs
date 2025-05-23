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
        Models.patients _patients;
        Models.appointments appointments_mod;
        DataContext _context = new DataContext();
        Pages.Main _main;
        public appointments(Models.appointments _appointments, Main main = null)
        {
            InitializeComponent();
            _main = main;
            appointments_mod = _appointments;
            lb_appointments.Text = _appointments.appointment_id.ToString();
            lb_patient_id.Text = new DataContext().patients.FirstOrDefault(x => x.patient_id == _appointments.patient_id).last_name;
            lb_employeess.Text = new DataContext().employees.FirstOrDefault(x => x.employee_id == _appointments.employee_id).first_name;
            lb_service_id.Text = new DataContext().services.FirstOrDefault(x => x.service_id == _appointments.service_id).service_name;
            lb_room_id.Text = new DataContext().rooms.FirstOrDefault(x => x.room_id == _appointments.room_id).room_type;
            lb_appointment_date.Text = _appointments.appointment_date.ToString();
            lb_start_time.Text = _appointments.start_time.ToString();
            lb_status.Text = _appointments.status;
            lb_notes.Text = _appointments.notes;
            lb_created_at.Text = _appointments.created_at.ToString();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            appointments_Edit_Window appointments_Edit_Window = new appointments_Edit_Window(appointments_mod);
            appointments_Edit_Window.Show();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Подтверждение удаления
                var confirmMessage = $"Вы уверены, что хотите удалить запись на прием?\n\n" +
                                   $"ID: {appointments_mod.appointment_id}\n" +
                                   $"Дата: {appointments_mod.appointment_date:dd.MM.yyyy}\n" +
                                   $"Пациент: {lb_patient_id.Text}";

                if (MessageBox.Show(confirmMessage, "Подтверждение удаления",
                                  MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                {
                    return;
                }

                // 2. Обработка всех зависимых таблиц
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        // a) Удаление связанных prescription записей
                        var medicalRecords = _context.medical_records
                            .Where(mr => mr.appointment_id == appointments_mod.appointment_id)
                            .ToList();

                        foreach (var record in medicalRecords)
                        {
                            var prescriptions = _context.prescriptions
                                .Where(p => p.record_id == record.record_id)
                                .ToList();

                            _context.prescriptions.RemoveRange(prescriptions);
                        }

                        // b) Удаление medical_records
                        _context.medical_records.RemoveRange(medicalRecords);

                        // c) Удаление payments
                        var payments = _context.payments
                            .Where(p => p.appointment_id == appointments_mod.appointment_id)
                            .ToList();

                        _context.payments.RemoveRange(payments);

                        // d) Удаление основной записи
                        _context.appointments.Remove(appointments_mod);

                        // Сохранение всех изменений
                        _context.SaveChanges();
                        transaction.Commit();

                        MessageBox.Show("Запись на прием и все связанные данные успешно удалены",
                                      "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        _main.CreateUIapps();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}\n\n" +
                                      $"Подробности: {ex.InnerException?.Message}",
                                      "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
