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
        private const int TimeSlotIntervalMinutes = 30;

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
                // Устанавливаем значения по умолчанию
                txtStartTime.Text = "08:30";
                txtEndTime.Text = "18:00";
                cbIsWorkingDay.IsChecked = true;
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
                if (!ValidateInput()) return;

                int employeeId = (int)cbDoctor.SelectedValue;
                int dayOfWeek = int.Parse(txtDayOfWeek.Text);
                TimeSpan startTime = TimeSpan.Parse(txtStartTime.Text);
                TimeSpan endTime = TimeSpan.Parse(txtEndTime.Text);

                // Явное преобразование для nullable int
                int? roomId = null;
                if (!string.IsNullOrWhiteSpace(txtRoom.Text))
                {
                    roomId = int.Parse(txtRoom.Text);
                }

                bool isWorkingDay = cbIsWorkingDay.IsChecked == true;

                if (_schedule == null)
                {
                    _context.schedules.Add(new schedules
                    {
                        employee_id = employeeId,
                        day_of_week = dayOfWeek,
                        start_time = startTime,
                        end_time = endTime,
                        room_id = roomId,
                        is_working_day = isWorkingDay
                    });
                }
                else
                {
                    _schedule.employee_id = employeeId;
                    _schedule.day_of_week = dayOfWeek;
                    _schedule.start_time = startTime;
                    _schedule.end_time = endTime;
                    _schedule.room_id = roomId;
                    _schedule.is_working_day = isWorkingDay;
                    _context.schedules.Update(_schedule);
                }

                _context.SaveChanges();
                MessageBox.Show("Расписание сохранено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Shablon_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInput()) return;

                int employeeId = (int)cbDoctor.SelectedValue;
                int dayOfWeek = int.Parse(txtDayOfWeek.Text);
                TimeSpan startTime = TimeSpan.Parse(txtStartTime.Text);
                TimeSpan endTime = TimeSpan.Parse(txtEndTime.Text);

                // Исправление для C# 8.0 - замена тернарного оператора на явную проверку
                int? roomId = null;
                if (!string.IsNullOrWhiteSpace(txtRoom.Text))
                {
                    roomId = int.Parse(txtRoom.Text);
                }

                bool isWorkingDay = cbIsWorkingDay.IsChecked == true;

                // Проверка на корректность временного интервала
                if (startTime >= endTime)
                {
                    MessageBox.Show("Время окончания должно быть позже времени начала", "Ошибка",
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Подтверждение удаления старых записей
                var existingSchedules = _context.schedules
                    .Where(s => s.employee_id == employeeId && s.day_of_week == dayOfWeek)
                    .ToList();

                if (existingSchedules.Any())
                {
                    var result = MessageBox.Show(
                        "У врача уже есть расписание на этот день. Удалить существующие записи?",
                        "Подтверждение",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _context.schedules.RemoveRange(existingSchedules);
                        _context.SaveChanges();
                    }
                    else
                    {
                        return;
                    }
                }

                // Создание временных слотов с интервалом 30 минут
                List<schedules> timeSlots = new List<schedules>();
                TimeSpan currentTime = startTime;

                while (currentTime < endTime)
                {
                    TimeSpan slotEndTime = currentTime.Add(TimeSpan.FromMinutes(TimeSlotIntervalMinutes));
                    if (slotEndTime > endTime) slotEndTime = endTime;

                    timeSlots.Add(new schedules
                    {
                        employee_id = employeeId,
                        day_of_week = dayOfWeek,
                        start_time = currentTime,
                        end_time = slotEndTime,
                        room_id = roomId,
                        is_working_day = isWorkingDay
                    });

                    currentTime = slotEndTime;
                }

                _context.schedules.AddRange(timeSlots);
                _context.SaveChanges();

                MessageBox.Show($"Создано {timeSlots.Count} временных слотов", "Успех",
                               MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            if (cbDoctor.SelectedItem == null)
            {
                MessageBox.Show("Выберите врача", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(txtDayOfWeek.Text, out int day) || day < 1 || day > 7)
            {
                MessageBox.Show("Введите день недели от 1 до 7", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!TimeSpan.TryParse(txtStartTime.Text, out _))
            {
                MessageBox.Show("Введите корректное время начала (формат HH:mm)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!TimeSpan.TryParse(txtEndTime.Text, out _))
            {
                MessageBox.Show("Введите корректное время окончания (формат HH:mm)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtRoom.Text) && !int.TryParse(txtRoom.Text, out _))
            {
                MessageBox.Show("Номер кабинета должен быть числом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}

