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

namespace Medicine_DP.Elements
{
    /// <summary>
    /// Логика взаимодействия для Medical_Records_El.xaml
    /// </summary>
    public partial class Medical_Records_El : UserControl
    {
        private readonly medical_records _medicalRecord;
        private readonly DataContext _context = Main._main._context;
        public static string _loginUser;
        public Medical_Records_El(medical_records record, string loginUser)
        {
            InitializeComponent();
            _loginUser = loginUser;
            _medicalRecord = record;
            LoadMedicalRecordData();
            ConfigureUIForUserRole();
        }

        private void ConfigureUIForUserRole()
        {
            var currentUser = _context.employees.FirstOrDefault(e => e.login == _loginUser);
            bool isPatient = _context.patients.Any(p => p.login == _loginUser);

            if (isPatient) // Если это пациент
            {
                // Пациенты видят только свои записи
                if (_medicalRecord.patient_id != _context.patients.First(p => p.login == _loginUser).patient_id)
                {
                    this.Visibility = Visibility.Collapsed;
                    return;
                }
                btnDelete.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Collapsed;
            }
            else if (currentUser != null) // Если это сотрудник
            {
                // Администратор видит все записи
                if (currentUser.position == "Администратор")
                {
                    btnDelete.Visibility = Visibility.Visible;
                    btnEdit.Visibility = Visibility.Visible;
                }
                // Врач видит только свои записи
                else if (currentUser.position == "Врач")
                {
                    if (currentUser.employee_id != _medicalRecord.employee_id)
                    {
                        this.Visibility = Visibility.Collapsed;
                        return;
                    }
                    btnDelete.Visibility = Visibility.Visible;
                    btnEdit.Visibility = Visibility.Visible;
                }
                // Остальные сотрудники не видят ничего
                else
                {
                    this.Visibility = Visibility.Collapsed;
                    return;
                }
            }
            else
            {
                // Если пользователь не найден
                this.Visibility = Visibility.Collapsed;
                MessageBox.Show("Не удалось определить роль пользователя", "Ошибка");
            }
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

                // статус
                lbStatus.Text = _medicalRecord.status ?? "Активна";
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
                LoadMedicalRecordData();
            };
            editWindow.ShowDialog();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var confirmResult = MessageBox.Show(
                    $"Вы действительно хотите отменить эту медицинскую запись?\n\n" +
                    $"Дата: {_medicalRecord.record_date:dd.MM.yyyy}\n" +
                    $"Пациент: {lbPatient.Text}",
                    "Подтверждение отмены",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirmResult != MessageBoxResult.Yes) return;

                _medicalRecord.status = "Отменена";
                _context.SaveChanges();
                LoadMedicalRecordData();

                MessageBox.Show("Медицинская запись отменена", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отмене записи: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

