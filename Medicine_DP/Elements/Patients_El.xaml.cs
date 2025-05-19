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
    /// Логика взаимодействия для Patients_El.xaml
    /// </summary>
    public partial class Patients_El : UserControl
    {
        public Patients_El(patients patient)
        {
            InitializeComponent();
            lbFullName.Text = $"{patient.last_name} {patient.first_name} {patient.middle_name}";
            lbBirthDate.Text = patient.birth_date.ToString("dd.MM.yyyy");
            lbPhone.Text = patient.phone_number ?? "Не указано";
            lbEmail.Text = patient.email ?? "Не указано";
            lbAddress.Text = patient.address ?? "Не указано";

            lbPassport.Text = $"{patient.passport_series} {patient.passport_number}";
            lbSnils.Text = patient.snils ?? "Не указано";
            lbPolicy.Text = patient.policy_number ?? "Не указано";
            lbRegDate.Text = patient.registration_date.ToString("dd.MM.yyyy");
            tbNotes.Text = patient.notes ?? "Нет примечаний";
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
