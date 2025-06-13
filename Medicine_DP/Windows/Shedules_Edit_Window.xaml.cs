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

                
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Shablon_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbDoctor.SelectedItem == null)
                {
                    MessageBox.Show("Выберите врача", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int employeeId = (int)cbDoctor.SelectedValue;

                // Проверяем, выбран ли день недели
                if (!int.TryParse(txtDayOfWeek.Text, out int dayOfWeek) || dayOfWeek < 1 || dayOfWeek > 7)
                {
                    MessageBox.Show("Введите корректный день недели (от 1 до 7)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                TimeSpan start = new TimeSpan(8, 30, 0); // 08:30
                TimeSpan end = new TimeSpan(18, 0, 0);  // 18:00
                TimeSpan interval = TimeSpan.FromMinutes(30);

                // Получаем номер кабинета
                int? roomId = string.IsNullOrWhiteSpace(txtRoom.Text) ? (int?)null : int.Parse(txtRoom.Text);

                bool isWorkingDay = cbIsWorkingDay.IsChecked == true;

                // Очищаем предыдущие записи (если это редактирование существующего расписания одного дня)
                var existingSchedules = _context.schedules
                    .Where(s => s.employee_id == employeeId && s.day_of_week == dayOfWeek)
                    .ToList();

                if (existingSchedules.Count > 0)
                {
                    var confirm = MessageBox.Show(
                        $"Врач уже имеет расписание на этот день. Удалить старые записи и применить шаблон?",
                        "Подтверждение",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (confirm == MessageBoxResult.Yes)
                    {
                        _context.schedules.RemoveRange(existingSchedules);
                        _context.SaveChanges();
                    }
                    else
                    {
                        return;
                    }
                }

                // Генерируем слоты с 8:30 до 18:00 каждые 30 мин
                for (TimeSpan time = start; time < end; time = time.Add(interval))
                {
                    var scheduleSlot = new schedules
                    {
                        employee_id = employeeId,
                        day_of_week = dayOfWeek,
                        start_time = time,
                        end_time = time.Add(interval),
                        room_id = roomId,
                        is_working_day = isWorkingDay
                    };

                    _context.schedules.Add(scheduleSlot);
                }

                _context.SaveChanges();
                MessageBox.Show("Шаблон расписания успешно применён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании шаблона: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

