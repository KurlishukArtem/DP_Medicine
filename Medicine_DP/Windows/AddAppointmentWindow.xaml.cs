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
using Microsoft.EntityFrameworkCore;

namespace Medicine_DP.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddAppointmentWindow.xaml
    /// </summary>
    public partial class AddAppointmentWindow : Window
    {
        private DataContext _context;
        private List<TimeSpan> _availableTimes = new List<TimeSpan>();
        public AddAppointmentWindow()
        {
            InitializeComponent();
            _context = new DataContext();
            LoadData();
            dpDate.SelectedDate = DateTime.Today;
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
                        FullName = p.last_name + " " + p.first_name + " " + p.middle_name
                    })
                    .ToList();

                // Загрузка врачей
                cbDoctors.ItemsSource = _context.employees
                    //.Where(e => e.position == "Врач")
                    .Select(e => new
                    {
                        e.employee_id,
                        FullName = e.last_name + " " + e.first_name + " " + e.middle_name
                    })
                    .ToList();

                // Загрузка услуг
                cbServices.ItemsSource = _context.services
                    .Select(e => new 
                    {
                        e.service_id,
                        Service = e.service_name
                    }).ToList();
                

                //services _serveces = (services)cbServices.SelectedItem;
                //int serviceId = _serveces.service_id;
                //services selectedService = _context.services.Find(serviceId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void cbDoctors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbDoctors.SelectedItem != null && dpDate.SelectedDate.HasValue)
            {
                LoadAvailableTimes();
            }
        }

        private void dpDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbDoctors.SelectedItem != null && dpDate.SelectedDate.HasValue)
            {
                LoadAvailableTimes();
            }
        }

        private void LoadAvailableTimes()
{
    _availableTimes.Clear();
    cbTime.ItemsSource = null;

    try
    {
        dynamic selectedDoctor = cbDoctors.SelectedItem;
        int doctorId = selectedDoctor.employee_id;
        DateTime selectedDate = dpDate.SelectedDate.Value;
        int dayOfWeek = (int)selectedDate.DayOfWeek;
        if (dayOfWeek == 0) dayOfWeek = 7;

        // Упрощенный запрос без использования навигационных свойств
        var schedule = _context.schedules
            .AsNoTracking()
            .FirstOrDefault(s => s.employee_id == doctorId 
                             && s.day_of_week == dayOfWeek 
                             && s.is_working_day == 1);

        if (schedule != null)
        {
                    TimeSpan startTime = schedule.start_time;
                    TimeSpan endTime = schedule.end_time;
                    TimeSpan slotDuration = TimeSpan.FromMinutes(30);

            var bookedTimes = _context.appointments
                .AsNoTracking()
                .Where(a => a.employee_id == doctorId 
                         && a.appointment_date == selectedDate)
                .Select(a => TimeSpan.FromMinutes(a.start_time))
                .ToList();

            for (TimeSpan time = startTime; time < endTime; time += slotDuration)
            {
                if (!bookedTimes.Contains(time))
                {
                    _availableTimes.Add(time);
                }
            }

            cbTime.ItemsSource = _availableTimes.Select(t => t.ToString(@"hh\:mm")).ToList();
            tbRoom.Text = schedule.room_id.ToString() ?? "Не указан";
        }
        else
        {
            MessageBox.Show("Врач не работает в выбранный день", "Информация", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка загрузки расписания: {ex.Message}", "Ошибка", 
            MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
                // Получаем ID выбранной услуги
                dynamic selectedService = cbServices.SelectedItem;
                int serviceId = selectedService.service_id;

                // Получаем полный объект услуги из БД
                services service = _context.services.Find(serviceId);

                // Получаем пациента
                dynamic selectedPatient = cbPatients.SelectedItem;
                int patientId = selectedPatient.patient_id;

                // Получаем врача
                dynamic selectedDoctor = cbDoctors.SelectedItem;
                int doctorId = selectedDoctor.employee_id;

                // Создаем новую запись
                var appointment = new appointments
                {
                    patient_id = patientId,
                    employee_id = doctorId,
                    service_id = serviceId,
                    appointment_date = dpDate.SelectedDate.Value,
                    start_time = (int)_availableTimes[cbTime.SelectedIndex].TotalMinutes,
                    status = "scheduled",
                    notes = tbNotes.Text,
                    created_at = DateTime.Now
                };

                _context.appointments.Add(appointment);
                _context.SaveChanges();

                MessageBox.Show("Запись успешно сохранена!");
                this.Close();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            //}
        }

        private bool ValidateInput()
        {
            if (cbPatients.SelectedItem == null)
            {
                MessageBox.Show("Выберите пациента", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cbDoctors.SelectedItem == null)
            {
                MessageBox.Show("Выберите врача", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!dpDate.SelectedDate.HasValue || dpDate.SelectedDate < DateTime.Today)
            {
                MessageBox.Show("Выберите корректную дату", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cbTime.SelectedItem == null || _availableTimes.Count == 0)
            {
                MessageBox.Show("Выберите время приема", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cbServices.SelectedItem == null)
            {
                MessageBox.Show("Выберите услугу", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(tbRoom.Text))
            {
                MessageBox.Show("Не указан кабинет", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
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
