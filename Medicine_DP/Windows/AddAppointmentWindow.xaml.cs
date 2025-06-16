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
using Microsoft.EntityFrameworkCore;

namespace Medicine_DP.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddAppointmentWindow.xaml
    /// </summary>
    public partial class AddAppointmentWindow : Window
    {
        public ComboBox DoctorsComboBox => cbDoctors;
        public DatePicker DatePicker => dpDate;
        public ComboBox TimeComboBox => cbTime;
        public TextBox RoomTextBox => tbRoom;
        public DataContext _context = Main._main._context;
        public List<TimeSpan> _availableTimes = new List<TimeSpan>();
        public Models.appointments _appointment;

        private bool _isEditMode = false;

        public AddAppointmentWindow(Models.appointments appointment = null)
        {
            InitializeComponent();
            _context = new DataContext();
            _appointment = appointment;
            _isEditMode = _appointment != null;
            LoadData();

            if (_isEditMode)
            {
                LoadAppointmentData();
            }
        }

        private void LoadData()
        {
            try
            {
                // Загрузка пациентов
                cbPatients.ItemsSource = _context.patients
                    .Select(p => new
                    {
                        p.patient_id,
                        FullName = $"{p.last_name} {p.first_name} {p.middle_name}"
                    })
                    .ToList();

                // Загрузка врачей
                cbDoctors.ItemsSource = _context.employees
                    .Where(e => e.is_active == 1)
                    .Select(e => new
                    {
                        e.employee_id,
                        FullName = $"{e.last_name} {e.first_name} {e.middle_name}",
                        e.specialization
                    })
                    .ToList();

                // Загрузка услуг
                cbServices.ItemsSource = _context.services
                    .Where(s => s.is_active)
                    .Select(s => new
                    {
                        s.service_id,
                        Service = $"{s.service_name} ({s.price} руб.)"
                    })
                    .ToList();

                dpDate.SelectedDate = DateTime.Today;
                dpDate.DisplayDateStart = DateTime.Today;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void LoadAppointmentData()
        {
            // Установка пациента
            var patient = _context.patients.FirstOrDefault(p => p.patient_id == _appointment.patient_id);
            if (patient != null)
            {
                cbPatients.SelectedItem = cbPatients.Items
                    .Cast<dynamic>()
                    .FirstOrDefault(i => i.patient_id == patient.patient_id);
            }

            // Установка врача
            var doctor = _context.employees.FirstOrDefault(d => d.employee_id == _appointment.employee_id);
            if (doctor != null)
            {
                cbDoctors.SelectedItem = cbDoctors.Items
                    .Cast<dynamic>()
                    .FirstOrDefault(i => i.employee_id == doctor.employee_id);

                tbRoom.Text = doctor.rooms.ToString();
            }

            // Установка даты
            dpDate.SelectedDate = _appointment.appointment_date;

            // Установка услуги
            var service = _context.services.FirstOrDefault(s => s.service_id == _appointment.service_id);
            if (service != null)
            {
                cbServices.SelectedItem = cbServices.Items
                    .Cast<dynamic>()
                    .FirstOrDefault(i => i.service_id == service.service_id);
            }

            // Установка заметок
            tbNotes.Text = _appointment.notes;

            // После загрузки данных — обновляем доступное время
            UpdateAvailableTimes();

            // Устанавливаем текущее время после формирования списка
            if (_availableTimes.Count > 0)
            {
                var selectedTime = _appointment.start_time;
                int index = _availableTimes.FindIndex(t => t == selectedTime);
                if (index >= 0)
                {
                    cbTime.SelectedIndex = index;
                }
            }
        }

        private void cbDoctors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbDoctors.SelectedItem == null || dpDate.SelectedDate == null)
                return;

            UpdateAvailableTimes();
        }

        private void dpDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbDoctors.SelectedItem == null || dpDate.SelectedDate == null)
                return;

            UpdateAvailableTimes();
        }

        public void UpdateAvailableTimes()
        {
            try
            {
                dynamic selectedDoctor = cbDoctors.SelectedItem;
                int doctorId = selectedDoctor.employee_id;
                DateTime selectedDate = dpDate.SelectedDate.Value;

                employees _empl = _context.employees.FirstOrDefault(x => x.employee_id == doctorId);

                int dayOfWeek = (int)selectedDate.DayOfWeek;
                if (dayOfWeek == 0) dayOfWeek = 7;

                var schedule = _context.schedules
                    .AsNoTracking()
                    .Where(s => s.employee_id == doctorId &&
                                s.day_of_week == dayOfWeek &&
                                s.is_working_day);

                if (!schedule.Any())
                {
                    MessageBox.Show("Врач не работает в выбранный день");
                    cbTime.ItemsSource = null;
                    tbRoom.Text = "";
                    return;
                }

                var bookedTimes = _context.appointments
                    .Where(a => a.employee_id == doctorId &&
                                a.appointment_date == selectedDate &&
                                a.status != "cancelled")
                    .Select(a => a.start_time)
                    .ToList();

                _availableTimes.Clear();

                foreach (var item in schedule)
                {
                    _availableTimes.Add(item.start_time);
                }

                // Удаляем уже занятое время
                _availableTimes = _availableTimes.Except(bookedTimes).ToList();

                cbTime.ItemsSource = _availableTimes
                    .Select(t => t.ToString(@"hh\:mm"))
                    .ToList();

                tbRoom.Text = _empl.rooms.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении времени: {ex.Message}");
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInput()) return;

                int patientId = ((dynamic)cbPatients.SelectedItem).patient_id;
                int employeeId = ((dynamic)cbDoctors.SelectedItem).employee_id;
                int serviceId = ((dynamic)cbServices.SelectedItem).service_id;
                int? roomId = int.TryParse(tbRoom.Text, out int room) ? room : (int?)null;
                DateTime date = dpDate.SelectedDate.Value;
                TimeSpan selectedTime = _availableTimes[cbTime.SelectedIndex];
                string notes = tbNotes.Text.Trim();

                if (_isEditMode)
                {
                    // Редактирование существующей записи
                    _appointment.patient_id = patientId;
                    _appointment.employee_id = employeeId;
                    _appointment.service_id = serviceId;
                    _appointment.room_id = roomId;
                    _appointment.appointment_date = date;
                    _appointment.start_time = selectedTime;
                    _appointment.notes = notes;

                    _context.appointments.Update(_appointment);
                }
                else
                {
                    // Создание новой записи
                    var newAppointment = new appointments
                    {
                        patient_id = patientId,
                        employee_id = employeeId,
                        service_id = serviceId,
                        room_id = roomId,
                        appointment_date = date,
                        start_time = selectedTime,
                        notes = notes,
                        status = "scheduled",
                        created_at = DateTime.Now
                    };

                    _context.appointments.Add(newAppointment);
                }

                _context.SaveChanges();
                MessageBox.Show("Запись успешно сохранена");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private bool ValidateInput()
        {
            if (cbPatients.SelectedItem == null)
            {
                MessageBox.Show("Выберите пациента");
                return false;
            }

            if (cbDoctors.SelectedItem == null)
            {
                MessageBox.Show("Выберите врача");
                return false;
            }

            if (dpDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату");
                return false;
            }

            if (cbTime.SelectedItem == null)
            {
                MessageBox.Show("Выберите время");
                return false;
            }

            if (cbServices.SelectedItem == null)
            {
                MessageBox.Show("Выберите услугу");
                return false;
            }

            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _context?.Dispose();
        }
    }
}