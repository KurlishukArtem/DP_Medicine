using System;
using System.Collections.Generic;
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
using Medicine_DP.Models;

namespace Medicine_DP.Elements
{
    /// <summary>
    /// Логика взаимодействия для appointments.xaml
    /// </summary>
    public partial class appointments : UserControl
    {
        public appointments(Models.appointments _appointments)
        {
            InitializeComponent();
            lb_appointments.Text = _appointments.appointment_id.ToString();
            lb_patient_id.Text = _appointments.patient_id.ToString();
            lb_employeess.Text = _appointments.employee_id.ToString();
            lb_service_id.Text = _appointments.service_id.ToString();
            lb_room_id.Text = _appointments.room_id.ToString();
            lb_appointment_date.Text = _appointments.appointment_date.ToString();
            lb_start_time.Text = _appointments.start_time.ToString();
            lb_status.Text = _appointments.status;
            lb_notes.Text = _appointments.notes;
            lb_created_at.Text = _appointments.created_at.ToString();
            
        }
    }
}
