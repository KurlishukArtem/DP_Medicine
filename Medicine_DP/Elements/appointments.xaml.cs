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
using Medicine_DP.Windows;

namespace Medicine_DP.Elements
{
    /// <summary>
    /// Логика взаимодействия для appointments.xaml
    /// </summary>
    public partial class appointments : UserControl
    {
        Models.patients _patients;
        Models.appointments _appointments;
        public appointments(Models.appointments _appointments)
        {
            InitializeComponent();
            lb_appointments.Text = _appointments.appointment_id.ToString();
            lb_patient_id.Text = new DataContext().patients.FirstOrDefault(x => x.patient_id == _appointments.patient_id).last_name;
            lb_employeess.Text = new DataContext().employees.FirstOrDefault(x => x.employee_id == _appointments.employee_id).first_name;
            lb_service_id.Text = new DataContext().services.FirstOrDefault(x => x.service_id == _appointments.service_id).service_name;
            lb_room_id.Text = new DataContext().rooms.FirstOrDefault(x => x.room_id == _appointments.room_id).room_type;
            lb_appointment_date.Text = _appointments.appointment_date.ToString();
            lb_start_time.Text = _appointments.start_time.ToString();
            lb_status.Text = _appointments.status;
            lb_notes.Text = _appointments.notes;
            lb_created_at.Text = _appointments.created_at.ToString();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            appointments_Edit_Window appointments_Edit_Window = new appointments_Edit_Window();
            appointments_Edit_Window.Show();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
