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

namespace Medicine_DP.Elements
{
    /// <summary>
    /// Логика взаимодействия для Shedules_El.xaml
    /// </summary>
    public partial class Shedules_El : UserControl
    {
        public schedules _schedule;
        private readonly DataContext _context = Main._main._context;
        public static string _loginUser;
        public patients _patients;
        employees _employees;

        public Shedules_El(schedules schedule, string loginUser = null)
        {
            InitializeComponent();
            _schedule = schedule;
            _loginUser = loginUser;
            LoadScheduleData();
            ConfigureUIForUserRole();
        }

        private void LoadScheduleData()
        {
            // ID расписания
            lbScheduleId.Text = _schedule.schedule_id.ToString();

            // ФИО врача
            var doctor = _context.employees.FirstOrDefault(x => x.employee_id == _schedule.employee_id);
            lbDoctorName.Text = doctor != null ?
                $"{doctor.last_name} {doctor.first_name} {doctor.middle_name}" :
                "Неизвестно";

            // День недели
            lbDayOfWeek.Text = GetDayName(_schedule.day_of_week);

            // Время начала и окончания
            lbStartTime.Text = _schedule.start_time.ToString(@"hh\:mm");
            lbEndTime.Text = _schedule.end_time.ToString(@"hh\:mm");

            // Номер кабинета
            lbRoomId.Text = _schedule.room_id?.ToString() ?? "Не указан";

            // Статус — рабочий / выходной
            lbWorkingStatus.Text = _schedule.is_working_day ? "Рабочий" : "Выходной";
        }

        private string GetDayName(int dayNumber)
        {
            string[] days = {
            "Воскресенье", "Понедельник", "Вторник",
            "Среда", "Четверг", "Пятница", "Суббота"
        };

            return dayNumber >= 1 && dayNumber <= 7 ? days[dayNumber - 1] : "Неизвестно";
        }

        private void ConfigureUIForUserRole()
        {
            var currentUser = _context.employees.FirstOrDefault(e => e.login == _loginUser);
            bool isPatient = _context.patients.Any(p => p.login == _loginUser);

            if (isPatient)
            {
                // Пациенты не видят расписаний вообще
                this.Visibility = Visibility.Collapsed;
                return;
            }
            else if (currentUser != null)
            {
                // Администратор видит все расписания
                if (currentUser.position == "Администратор")
                {
                    btnEdit.Visibility = Visibility.Visible;
                    btnDelete.Visibility = Visibility.Visible;
                }
                // Врач видит только свои расписания
                else if (currentUser.position == "Врач")
                {
                    if (currentUser.employee_id != _schedule.employee_id)
                    {
                        this.Visibility = Visibility.Collapsed;
                        return;
                    }
                    btnEdit.Visibility = Visibility.Visible;
                    btnDelete.Visibility = Visibility.Visible;
                }
                // Остальные сотрудники не видят расписаний
                else
                {
                    this.Visibility = Visibility.Collapsed;
                    return;
                }
            }
            else
            {
                MessageBox.Show("Не удалось определить роль пользователя", "Ошибка");
                this.Visibility = Visibility.Collapsed;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new Windows.Shedules_Edit_Window(_schedule);
            editWindow.Closed += (s, args) =>
            {
                LoadScheduleData();
            };
            editWindow.Show();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var scheduleToDelete = context.schedules
                        .FirstOrDefault(s => s.schedule_id == _schedule.schedule_id);

                    if (scheduleToDelete == null)
                    {
                        MessageBox.Show("Запись не найдена", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    int dayOfWeekNumber = scheduleToDelete.day_of_week;

                    bool hasAppointments = context.appointments
                        .AsEnumerable()
                        .Any(a => a.employee_id == scheduleToDelete.employee_id &&
                               (int)a.appointment_date.DayOfWeek == dayOfWeekNumber);

                    if (hasAppointments)
                    {
                        MessageBox.Show("Невозможно удалить расписание — есть записи на приём в этот день.\n" +
                                       "Сначала отмените или перенесите записи.",
                                       "Ошибка удаления",
                                       MessageBoxButton.OK,
                                       MessageBoxImage.Error);
                        return;
                    }

                    context.schedules.Remove(scheduleToDelete);
                    context.SaveChanges();

                    if (Parent is Panel parentPanel)
                    {
                        parentPanel.Children.Remove(this);
                    }

                    MessageBox.Show("Расписание успешно удалено", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

