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

namespace Medicine_DP.Elements
{
    /// <summary>
    /// Логика взаимодействия для Medical_Records_El.xaml
    /// </summary>
    public partial class Medical_Records_El : UserControl
    {
        private readonly medical_records _medicalRecord;
        private readonly DataContext _context = new DataContext();
        public Medical_Records_El(medical_records record)
        {
            InitializeComponent();
            _medicalRecord = record;
            LoadMedicalRecordData();
        }

        private void LoadMedicalRecordData()
        {
            try
            {
                // Основная информация о записи
                lbRecordDate.Text = _medicalRecord.record_date.ToString("dd.MM.yyyy HH:mm");

                // Загрузка связанных данных
                var patient = _context.patients
                    .FirstOrDefault(p => p.patient_id == _medicalRecord.patient_id);
                lbPatient.Text = patient != null ?
                    $"{patient.last_name} {patient.first_name} {patient.middle_name}" :
                    _medicalRecord.patient_id.ToString();

                var doctor = _context.employees
                    .FirstOrDefault(e => e.employee_id == _medicalRecord.employee_id);
                lbDoctor.Text = doctor != null ?
                    $"{doctor.last_name} {doctor.first_name} {doctor.middle_name}" :
                    _medicalRecord.employee_id.ToString();

                if (_medicalRecord.appointment_id.HasValue)
                {
                    var appointment = _context.appointments
                        .FirstOrDefault(a => a.appointment_id == _medicalRecord.appointment_id);
                    lbAppointment.Text = appointment != null ?
                        appointment.appointment_date.ToString("dd.MM.yyyy") :
                        _medicalRecord.appointment_id.ToString();
                }
                else
                {
                    lbAppointment.Text = "Не привязано к приёму";
                }

                // Медицинские данные
                tbDiagnosis.Text = _medicalRecord.diagnosis ?? "Не указано";
                tbTreatment.Text = _medicalRecord.treatment ?? "Не указано";
                tbRecommendations.Text = _medicalRecord.recommendations ?? "Не указано";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new Medical_records_Edit_Window(_medicalRecord);
            editWindow.Closed += (s, args) =>
            {
                // Обновляем данные после редактирования
                LoadMedicalRecordData();
            };
            editWindow.ShowDialog();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var confirmResult = MessageBox.Show(
                    $"Вы действительно хотите удалить эту медицинскую запись?\n\n" +
                    $"Дата: {_medicalRecord.record_date:dd.MM.yyyy}\n" +
                    $"Пациент: {lbPatient.Text}",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirmResult != MessageBoxResult.Yes) return;

                // Проверка наличия связанных рецептов
                bool hasPrescriptions = _context.prescriptions
                    .Any(p => p.record_id == _medicalRecord.record_id);

                if (hasPrescriptions)
                {
                    MessageBox.Show("Нельзя удалить запись, так как к ней привязаны рецепты",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _context.medical_records.Remove(_medicalRecord);
                _context.SaveChanges();

                MessageBox.Show("Медицинская запись успешно удалена", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

