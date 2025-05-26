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
using Medicine_DP.Elements;
using Medicine_DP.Models;
using Medicine_DP.Pages;

namespace Medicine_DP.Windows
{
    /// <summary>
    /// Логика взаимодействия для appointments_Edit_Window.xaml
    /// </summary>
    public partial class appointments_Edit_Window : Window
    {
        private Models.appointments _appointment;
        private bool _isNewAppointment;
        private readonly DataContext _context = Main._main._context;
       

        
        public string WindowTitle => _isNewAppointment ? "Новая запись на прием" : "Редактирование записи";
        public Visibility ShowAppointmentId => _isNewAppointment ? Visibility.Collapsed : Visibility.Visible;

        public appointments_Edit_Window(Models.appointments appointment = null)
        {
            InitializeComponent();

            _appointment = appointment;
            _isNewAppointment = _appointment == null;

            DataContext = this;
            LoadComboBoxData();
            //InitializeTimeSlots();

            if (!_isNewAppointment)
            {
                LoadAppointmentData();
            }
            else
            {
                dpAppointmentDate.SelectedDate = DateTime.Today;
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
                    //.Where(e => e.employee_id == 2) // role_id = 2 для врачей
                    .Select(e => new
                    {
                        e.employee_id,
                        FullName = e.last_name + " " + e.first_name + " " + e.middle_name
                    })
                    .ToList();
                cbDoctors.ItemsSource = doctors;

                // Загрузка услуг
                cbServices.ItemsSource = _context.services.ToList();

                dynamic selectedDoctor = cbDoctors.SelectedItem;
                int doctorId = selectedDoctor.employee_id;
                employees _empl = _context.employees.FirstOrDefault(x => x.employee_id == doctorId);


                // Загрузка кабинетов
                cbRooms.Text = _empl.rooms.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        //private void InitializeTimeSlots()
        //{
        //    var timeSlots = new List<string>();
        //    var startTime = new DateTime(2000, 1, 1, 8, 0, 0); // Начало рабочего дня в 8:00
        //    var endTime = new DateTime(2000, 1, 1, 18, 0, 0);  // Конец рабочего дня в 18:00

        //    while (startTime <= endTime)
        //    {
        //        timeSlots.Add(startTime.ToString("HH:mm"));
        //        startTime = startTime.AddMinutes(30); // Интервалы по 30 минут
        //    }

        //    cbTimeSlots.ItemsSource = timeSlots;
        //}

        private void LoadAppointmentData()
        {
            txtAppointmentId.Text = _appointment.appointment_id.ToString();
            cbPatients.SelectedValue = _appointment.patient_id;
            cbDoctors.SelectedValue = _appointment.employee_id;
            cbServices.SelectedValue = _appointment.service_id;
            cbRooms.Text = _appointment.room_id.ToString();
            dpAppointmentDate.SelectedDate = _appointment.appointment_date;

            // Преобразование времени из int (минуты) в TimeSpan
            var time = TimeSpan.FromMinutes(_appointment.start_time);
            cbTimeSlots.SelectedItem = time.ToString(@"hh\:mm");

            // Установка статуса
            foreach (ComboBoxItem item in cbStatus.Items)
            {
                if (item.Tag.ToString() == _appointment.status)
                {
                    cbStatus.SelectedItem = item;
                    break;
                }
            }

            txtNotes.Text = _appointment.notes;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                //if (_isNewAppointment)
                //{
                //    _appointment = new Models.appointments();
                //    {
                //        created_at = DateTime.Now;
                //    };
                //    _context.appointments.Add(_appointment);
                //}

                // Обновление данных
                _appointment.patient_id = (int)cbPatients.SelectedValue;
                _appointment.employee_id = (int)cbDoctors.SelectedValue;
                _appointment.service_id = (int)cbServices.SelectedValue;
                _appointment.room_id = (int.Parse(cbRooms.Text));
                _appointment.appointment_date = dpAppointmentDate.SelectedDate.Value;

                // Преобразование времени в минуты (int)
                var time = new TimeSpan(20,30,00);    /* TimeSpan.Parse(cbTimeSlots.SelectedItem.ToString());*/
                _appointment.start_time = (int)time.TotalMinutes; ; 

                _appointment.status = ((ComboBoxItem)cbStatus.SelectedItem).Tag.ToString();
                _appointment.notes = txtNotes.Text;
                _context.appointments.Update(_appointment);
                _context.SaveChanges();

                MessageBox.Show("Запись успешно сохранена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            if (cbPatients.SelectedItem == null ||
                cbDoctors.SelectedItem == null ||
                cbServices.SelectedItem == null ||
                cbRooms.Text == null ||
                dpAppointmentDate.SelectedDate == null ||
                //cbTimeSlots.SelectedItem == null ||
                cbStatus.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (dpAppointmentDate.SelectedDate < DateTime.Today)
            {
                MessageBox.Show("Дата приема не может быть в прошлом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

