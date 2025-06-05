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
        private readonly DataContext _context = Main._main._context;
        private List<TimeSpan> _availableTimes = new List<TimeSpan>();
        public AddAppointmentWindow(Models.appointments appointment = null)
        {
            InitializeComponent();
            _context = new DataContext();
            LoadData();
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

        private void UpdateAvailableTimes()
        {
            try
            {
                dynamic selectedDoctor = cbDoctors.SelectedItem;
            int doctorId = selectedDoctor.employee_id;
            DateTime selectedDate = dpDate.SelectedDate.Value;
            
            employees _empl = _context.employees.FirstOrDefault(x=>x.employee_id==doctorId);


            int dayOfWeek = (int)selectedDate.DayOfWeek;
            if (dayOfWeek == 0) dayOfWeek = 7;

            // Исправленный запрос
            var schedule = _context.schedules
                .AsNoTracking()
                .Where(s => s.employee_id == doctorId &&
                                   s.day_of_week == dayOfWeek &&
                                   s.is_working_day);                                                   

            if (schedule == null)
                {
                    MessageBox.Show("Врач не работает в выбранный день");
                    cbTime.ItemsSource = null;
                    tbRoom.Text = "";
                    return;
                }

                // Получаем занятые времена
                var bookedTimes = _context.appointments
                    .Where(a => a.employee_id == doctorId &&
                               a.appointment_date == selectedDate &&
                               a.status != "cancelled")
                    .Select(a => TimeSpan.FromMinutes(a.start_time))
                    .ToList();

                // Генерируем доступные временные слоты
                _availableTimes.Clear();
               

                

                foreach ( var item in schedule )
                {
                _availableTimes.Add(item.start_time);
                }

                // Отображаем доступное время
                cbTime.ItemsSource = _availableTimes
                    .Select(t => t.ToString(@"hh\:mm"))
                    .ToList();

            // Показываем кабинет
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

                var appointment = new appointments
                {
                    patient_id = ((dynamic)cbPatients.SelectedItem).patient_id,
                    employee_id = ((dynamic)cbDoctors.SelectedItem).employee_id,
                    service_id = ((dynamic)cbServices.SelectedItem).service_id,
                    room_id = int.TryParse(tbRoom.Text, out int room) ? room : (int?)null,
                    appointment_date = dpDate.SelectedDate.Value,
                    start_time = (int)_availableTimes[cbTime.SelectedIndex].TotalMinutes,
                    notes = tbNotes.Text,
                    status = "scheduled"
                };

                _context.appointments.Add(appointment);
                _context.SaveChanges();

                MessageBox.Show("Запись успешно сохранена");
                
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
