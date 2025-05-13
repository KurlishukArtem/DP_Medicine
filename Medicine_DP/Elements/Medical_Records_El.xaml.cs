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
    /// Логика взаимодействия для Medical_Records_El.xaml
    /// </summary>
    public partial class Medical_Records_El : UserControl
    {
        public Medical_Records_El(medical_records record)
        {
            InitializeComponent();
            lbRecordId.Text = record.record_id.ToString();
            lbPatientId.Text = record.patient_id.ToString();
            lbEmployeeId.Text = record.employee_id.ToString();
            lbAppointmentId.Text = record.appointment_id.ToString();
            lbRecordDate.Text = record.record_date.ToString("dd.MM.yyyy HH:mm");

            tbDiagnosis.Text = record.diagnosis ?? "Не указано";
            tbTreatment.Text = record.treatment ?? "Не указано";
            tbPrescription.Text = record.prescription ?? "Не указано";
            tbRecommendations.Text = record.recommendations ?? "Не указано";
        }
    }
}
