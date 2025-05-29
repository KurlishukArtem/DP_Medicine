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
using System.Windows.Shapes;
using Medicine_DP.Config;
using Medicine_DP.Models;
using Medicine_DP.Pages;

namespace Medicine_DP.Windows
{
    /// <summary>
    /// Логика взаимодействия для Medical_records_Edit_Window.xaml
    /// </summary>
    public partial class Medical_records_Edit_Window : Window
    {
        private readonly DataContext _context = Main._main._context;
        private medical_records _record;
        private bool _isNewRecord;

        public string WindowTitle => _isNewRecord ? "Новая медицинская запись" : "Редактирование записи";
        public Visibility ShowRecordId => _isNewRecord ? Visibility.Collapsed : Visibility.Visible;
        public Medical_records_Edit_Window(medical_records record = null)
        {
            InitializeComponent();
            DataContext = this;

            _record = record ?? new medical_records();
            _isNewRecord = record == null;

            LoadComboBoxData();

            if (!_isNewRecord)
            {
                LoadRecordData();
            }
            else
            {
                dpRecordDate.SelectedDate = DateTime.Today;
                cbStatus.SelectedIndex = 0; // Выбираем "Активна" по умолчанию
            }
        }

        private void LoadComboBoxData()
        {
            try
            {
                // Загрузка пациентов
                var patients = _context.patients
                    .Select(p => new
                    {
                        p.patient_id,
                        FullName = p.last_name + " " + p.first_name + " " + p.middle_name
                    })
                    .ToList();
                cbPatients.ItemsSource = patients;

                // Загрузка врачей
                var doctors = _context.employees
                    //.Where(e => e.position.ToLower().Contains("врач"))
                    .Select(e => new
                    {
                        e.employee_id,
                        FullName = e.last_name + " " + e.first_name + " " + e.middle_name
                    })
                    .ToList();
                cbDoctors.ItemsSource = doctors;

                // Загрузка записей на прием
                var appointments = _context.appointments
                    .Select(a => new
                    {
                        a.appointment_id,
                        AppointmentInfo = a.appointment_date.ToString("dd.MM.yyyy") + " - " +
                                         a.start_time.ToString(@"hh\:mm") + " - " +
                                         a.patient_id
                    })
                    .ToList();
                cbAppointments.ItemsSource = appointments;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void LoadRecordData()
        {
            txtRecordId.Text = _record.record_id.ToString();
            cbPatients.SelectedValue = _record.patient_id;
            cbDoctors.SelectedValue = _record.employee_id;
            cbAppointments.SelectedValue = _record.appointment_id;
            dpRecordDate.SelectedDate = _record.record_date;
            txtDiagnosis.Text = _record.diagnosis;
            txtTreatment.Text = _record.treatment;
            txtPrescription.Text = _record.prescription;
            txtRecommendations.Text = _record.recommendations;
            foreach (ComboBoxItem item in cbStatus.Items)
            {
                if (item.Tag.ToString() == _record.status)
                {
                    cbStatus.SelectedItem = item;
                    break;
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                if (_isNewRecord)
                {
                    _record = new medical_records();
                    _context.medical_records.Add(_record);
                }

                // Обновление данных
                _record.patient_id = (int)cbPatients.SelectedValue;
                _record.employee_id = (int)cbDoctors.SelectedValue;
                _record.appointment_id = (int)cbAppointments.SelectedValue;
                _record.record_date = dpRecordDate.SelectedDate.Value;
                _record.diagnosis = txtDiagnosis.Text;
                _record.treatment = txtTreatment.Text;
                _record.prescription = txtPrescription.Text;
                _record.recommendations = txtRecommendations.Text;
                _record.status = (cbStatus.SelectedItem as ComboBoxItem)?.Tag.ToString();

                _context.Update(_record);
                _context.SaveChanges();

                MessageBox.Show("Медицинская запись успешно сохранена", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            if (cbPatients.SelectedItem == null ||
                cbDoctors.SelectedItem == null ||
                cbAppointments.SelectedItem == null ||
                dpRecordDate.SelectedDate == null ||
                cbStatus.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

