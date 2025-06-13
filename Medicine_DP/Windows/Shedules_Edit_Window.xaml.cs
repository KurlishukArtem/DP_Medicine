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
    /// Логика взаимодействия для Shedules_Edit_Window.xaml
    /// </summary>
    public partial class Shedules_Edit_Window : Window
    {
        public schedules _schedule;
        private readonly DataContext _context = Main._main._context;

        public Shedules_Edit_Window(schedules schedule = null)
        {
            InitializeComponent();
            _schedule = schedule;

            LoadDoctors();

            if (_schedule != null)
            {
                // Заполняем поля из существующей записи
                cbDoctor.SelectedValue = _schedule.employee_id;
                txtDayOfWeek.Text = _schedule.day_of_week.ToString();
                txtStartTime.Text = _schedule.start_time.ToString(@"hh\:mm");
                txtEndTime.Text = _schedule.end_time.ToString(@"hh\:mm");
                txtRoom.Text = _schedule.room_id?.ToString();
                cbIsWorkingDay.IsChecked = _schedule.is_working_day;
            }
            else
            {
                cbIsWorkingDay.IsChecked = true; // По умолчанию — рабочий день
            }
        }

        private void LoadDoctors()
        {
            // Загружаем список врачей в ComboBox
            var doctors = _context.employees
                .Where(e => e.is_active == 1)
                .Select(e => new
                {
                    e.employee_id,
                    FullName = $"{e.last_name} {e.first_name}"
                })
                .ToList();

            cbDoctor.ItemsSource = doctors;
            cbDoctor.DisplayMemberPath = "FullName";
            cbDoctor.SelectedValuePath = "employee_id";
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbDoctor.SelectedItem == null)
                {
                    MessageBox.Show("Выберите врача", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int employeeId = (int)cbDoctor.SelectedValue;
                int dayOfWeek = int.Parse(txtDayOfWeek.Text);
                TimeSpan startTime = TimeSpan.Parse(txtStartTime.Text);
                TimeSpan endTime = TimeSpan.Parse(txtEndTime.Text);
                int? roomId = string.IsNullOrWhiteSpace(txtRoom.Text) ? (int?)null : int.Parse(txtRoom.Text);
                bool isWorkingDay = cbIsWorkingDay.IsChecked == true;

                if (_schedule == null)
                {
                    // Создание новой записи
                    var newSchedule = new schedules
                    {
                        employee_id = employeeId,
                        day_of_week = dayOfWeek,
                        start_time = startTime,
                        end_time = endTime,
                        room_id = roomId,
                        is_working_day = isWorkingDay
                    };

                    _context.schedules.Add(newSchedule);
                    _context.SaveChanges();
                    MessageBox.Show("Расписание успешно добавлено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Обновление существующей записи
                    _schedule.employee_id = employeeId;
                    _schedule.day_of_week = dayOfWeek;
                    _schedule.start_time = startTime;
                    _schedule.end_time = endTime;
                    _schedule.room_id = roomId;
                    _schedule.is_working_day = isWorkingDay;

                    _context.schedules.Update(_schedule);
                    _context.SaveChanges();
                    MessageBox.Show("Расписание успешно изменено", "Изменение", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

