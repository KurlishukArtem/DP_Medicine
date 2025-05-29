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
using Medicine_DP.Elements;
using Medicine_DP.Windows;

namespace Medicine_DP.Pages
{
    /// <summary>
    /// Логика взаимодействия для Add_Page.xaml
    /// </summary>
    public partial class Add_Page : Page
    {
        public Add_Page()
        {
            InitializeComponent();
        }

        private void AddEmloyes_Click(object sender, RoutedEventArgs e)
        {
            Emploeess_Edit_Window emploeess_Edit_Window = new Emploeess_Edit_Window();
            emploeess_Edit_Window.Show();
        }

        private void AddRecord_Click(object sender, RoutedEventArgs e)
        {
            Medical_records_Edit_Window medical_Records_Edit_Window = new Medical_records_Edit_Window();
            medical_Records_Edit_Window.Show();
        }

        private void AddMedications_Click(object sender, RoutedEventArgs e)
        {
            Medications_Edit_Window medications_Edit_Window = new Medications_Edit_Window();
            medications_Edit_Window.Show();
        }

        private void AddTest_Click(object sender, RoutedEventArgs e)
        {
            Medtest_Edit_Window medtest_Edit_Window = new Medtest_Edit_Window();
            medtest_Edit_Window.Show();
        }

        private void AddPatient_Click(object sender, RoutedEventArgs e)
        {
            Patients_Edit_Window patients_Edit_ = new Patients_Edit_Window();
            patients_Edit_.Show();
        }

        private void AddPayment_Click(object sender, RoutedEventArgs e)
        {
            Payments_Window payments_Window = new Payments_Window();
            payments_Window.Show();
        }

        private void AddService_Click(object sender, RoutedEventArgs e)
        {
            Service_Window service_Window = new Service_Window();
            service_Window.Show();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPages(new Pages.Main());
        }
    }
}
