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
                var confirmResult = MessageBox.Show(
                    $"Вы действительно хотите удалить запись на прием?\n\n" +
                    $"Дата: {appointments_mod.appointment_date:dd.MM.yyyy}\n" +
                    $"Пациент: {lb_patient_id.Text}",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirmResult != MessageBoxResult.Yes)
                    return;

                using (var context = new DataContext())
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        int appointmentId = appointments_mod.appointment_id;

                        // 1. Удаляем связанные prescriptions
                        context.Database.ExecuteSqlRaw(
                            @"DELETE p FROM prescriptions p 
                      INNER JOIN medical_records mr ON p.record_id = mr.record_id 
                      WHERE mr.appointment_id = {0}",
                            appointmentId);

                        // 2. Удаляем медицинские записи
                        context.Database.ExecuteSqlRaw(
                            "DELETE FROM medical_records WHERE appointment_id = {0}",
                            appointmentId);

                        // 3. Удаляем платежи
                        context.Database.ExecuteSqlRaw(
                            "DELETE FROM payments WHERE appointment_id = {0}",
                            appointmentId);

                        // 4. Удаляем саму запись на прием
                        context.Database.ExecuteSqlRaw(
                            "DELETE FROM appointments WHERE appointment_id = {0}",
                            appointmentId);

                        transaction.Commit();

                        MessageBox.Show("Запись на прием успешно удалена", "Успех",
                                      MessageBoxButton.OK, MessageBoxImage.Information);

                        _main.CreateUIapps();
                        
                    }
                    catch (DbUpdateException dbEx)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка базы данных: {dbEx.InnerException?.Message}",
                                      "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
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
