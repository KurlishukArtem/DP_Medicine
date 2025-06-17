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
using Medicine_DP.Elements;
using Medicine_DP.Models;
using Medicine_DP.Windows;
using Microsoft.EntityFrameworkCore;

namespace Medicine_DP.Pages
{
    /// <summary>
    /// Логика взаимодействия для Add_Page.xaml
    /// </summary>
    public partial class Add_Page : Page
    {
        private readonly DataContext _context = Main._main._context;
        private employees _employee;
        private patients _patient;
        string _username;
        public schedules _schedules;
        public Add_Page(string username = null)
        {
            InitializeComponent();
            _username = username;
            LoadUserData(username);
            ConfigureUIForUserRole();
        }

        private void LoadUserData(string username)
        {
            _employee = _context.employees.FirstOrDefault(e => e.login == username);
            _patient = _context.patients.FirstOrDefault(p => p.login == username);
        }

        private void ConfigureUIForUserRole()
        {
            if (_employee == null && _patient == null)
            {
                // Неавторизованный пользователь - скрываем все
                HideAllSections();
                return;
            }

            if (_employee != null)
            {
                // Настройка для сотрудников
                switch (_employee.position)
                {
                    case "Администратор":
                        // Администратор видит все разделы
                        break;

                    case "Врач":
                        // Врач - скрываем ненужные разделы
                        HideSection(AddEmloyesContainer);
                        HideSection(AddPatientContainer);
                        HideSection(AddPaymentContainer);
                        HideSection(AddServiceContainer);
                        break;

                    default:
                        // Другие сотрудники - скрываем все
                        HideAllSections();
                        break;
                }
            }
            else
            {
                // Пациент - скрываем все
                HideAllSections();
            }
        }

        private void HideAllSections()
        {
            HideSection(AddEmloyesContainer);
            HideSection(AddRecordContainer);
            HideSection(AddMedicationsContainer);
            HideSection(AddTestContainer);
            HideSection(AddPatientContainer);
            HideSection(AddPaymentContainer);
            HideSection(AddServiceContainer);
            HideSection(AddSheduleContainer);
        }

        private void HideSection(Border container)
        {
            container.Visibility = Visibility.Collapsed;
        }

        private void AddEmloyes_Click(object sender, RoutedEventArgs e)
        {
            if (_employee?.position != "Администратор") return;
            Emploeess_Edit_Window emploeess_Edit_Window = new Emploeess_Edit_Window();
            emploeess_Edit_Window.Show();
        }

        private void AddRecord_Click(object sender, RoutedEventArgs e)
        {
            if (_employee == null) return;
            Medical_records_Edit_Window medical_Records_Edit_Window = new Medical_records_Edit_Window();
            medical_Records_Edit_Window.Show();
        }

        private void AddMedications_Click(object sender, RoutedEventArgs e)
        {
            if (_employee == null) return;
            Medications_Edit_Window medications_Edit_Window = new Medications_Edit_Window();
            medications_Edit_Window.Show();
        }

        private void AddTest_Click(object sender, RoutedEventArgs e)
        {
            if (_employee == null) return;
            Medtest_Edit_Window medtest_Edit_Window = new Medtest_Edit_Window();
            medtest_Edit_Window.Show();
        }

        private void AddPatient_Click(object sender, RoutedEventArgs e)
        {
            if (_employee?.position != "Администратор") return;
            Patients_Edit_Window patients_Edit_ = new Patients_Edit_Window();
            patients_Edit_.Show();
        }

        private void AddPayment_Click(object sender, RoutedEventArgs e)
        {
            if (_employee?.position != "Администратор") return;
            Payments_Window payments_Window = new Payments_Window();
            payments_Window.Show();
        }

        private void AddService_Click(object sender, RoutedEventArgs e)
        {
            if (_employee?.position != "Администратор") return;
            Service_Window service_Window = new Service_Window();
            service_Window.Show();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPages(new Pages.Main(_username));
        }

        private void AddShedule_Click(object sender, RoutedEventArgs e)
        {
            if (_employee == null) return;

            if (_employee.position == "Врач")
            {
                Shedules_Edit_Window shedules_Edit_Window = new Shedules_Edit_Window(_schedules);
                shedules_Edit_Window.Show();
            }
            else if (_employee.position == "Администратор")
            {
                Shedules_Edit_Window shedules_Edit_Window = new Shedules_Edit_Window();
                shedules_Edit_Window.Show();
            }
        }
    }
}
