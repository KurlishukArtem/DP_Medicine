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
using Microsoft.EntityFrameworkCore;

namespace Medicine_DP.Pages
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class Main : Page
    {
        public static string _loginUser;
        public static Main _main;
        public DataContext _context = new DataContext();
        Models.employees employeess;
        public Main(string loginUser = null)
        {
            InitializeComponent();
            _main = this;
            _loginUser = loginUser;
            ConfigureUIForUserRole();
        }

        private void ConfigureUIForUserRole()
        {
            // Находим текущего пользователя (если это сотрудник)
            var currentEmployee = _context.employees.FirstOrDefault(e => e.login == _loginUser);
            var isPatient = _context.patients.Any(p => p.login == _loginUser);

            if (isPatient) // Если это пациент
            {
                // Скрываем кнопки, которые не должны быть доступны пациенту
                employees.Visibility = Visibility.Collapsed;
                medical_tests.Visibility = Visibility.Collapsed;
                medications.Visibility = Visibility.Collapsed;
                patients.Visibility = Visibility.Collapsed;
                payments.Visibility = Visibility.Collapsed;
                createAppointment.Visibility = Visibility.Collapsed;
                AddPage.Visibility = Visibility.Collapsed;

                // Оставляем только нужные пункты
                appointments.Visibility = Visibility.Visible;
                medical_records.Visibility = Visibility.Visible;
                cabinet_page.Visibility = Visibility.Visible;
            }
            else if (currentEmployee != null) // Если это сотрудник
            {
                // Базовые настройки для всех сотрудников
                patients.Visibility = Visibility.Visible;
                medical_records.Visibility = Visibility.Visible;
                cabinet_page.Visibility = Visibility.Visible;
                createAppointment.Visibility = Visibility.Visible;

                // Настройки для разных должностей
                switch (currentEmployee.position)
                {
                    case "Врач":
                        // Врач видит только связанные с приемами элементы
                        employees.Visibility = Visibility.Collapsed;
                        medical_tests.Visibility = Visibility.Visible;
                        medications.Visibility = Visibility.Visible;
                        payments.Visibility = Visibility.Collapsed;
                        AddPage.Visibility = Visibility.Visible;
                        
                        break;

                    case "Администратор":
                        // Администратор видит все
                        employees.Visibility = Visibility.Visible;
                        medical_tests.Visibility = Visibility.Visible;
                        medications.Visibility = Visibility.Visible;
                        payments.Visibility = Visibility.Visible;
                        AddPage.Visibility = Visibility.Visible;
                        break;

                    default:
                        // Для других должностей - минимальные права
                        employees.Visibility = Visibility.Collapsed;
                        medical_tests.Visibility = Visibility.Collapsed;
                        medications.Visibility = Visibility.Collapsed;
                        payments.Visibility = Visibility.Collapsed;
                        AddPage.Visibility = Visibility.Collapsed;
                        break;
                }
            }
            else
            {
                // Если пользователь не найден (не должно происходить после успешного входа)
                MessageBox.Show("Не удалось определить роль пользователя", "Ошибка");

                // Скрываем все элементы на случай ошибки
                employees.Visibility = Visibility.Collapsed;
                medical_tests.Visibility = Visibility.Collapsed;
                medications.Visibility = Visibility.Collapsed;
                patients.Visibility = Visibility.Collapsed;
                payments.Visibility = Visibility.Collapsed;
                createAppointment.Visibility = Visibility.Collapsed;
                AddPage.Visibility = Visibility.Collapsed;
                appointments.Visibility = Visibility.Collapsed;
                medical_records.Visibility = Visibility.Collapsed;
                cabinet_page.Visibility = Visibility.Collapsed;
            }
        }

        public void CreateUIapps()
        {
            var patient = _context.patients.FirstOrDefault(p => p.login == _loginUser);
            IQueryable<appointments> query = _context.appointments;

            if (patient != null)
            {
                query = query.Where(x => x.patient_id == patient.patient_id);
            }

            // Сортируем по дате и времени
            var appointmentsList = query
                .OrderBy(a => a.appointment_date)
                .ThenBy(a => a.start_time)
                .ToList();

            parent.Children.Clear();

            foreach (var app in appointmentsList)
            {
                parent.Children.Add(new Elements.appointments(app, _loginUser));
            }
        }

        public void CreateUIEmp()
        {

            parent.Children.Clear();
            foreach (var apps in new DataContext().employees)
            {
                parent.Children.Add(new Elements.Emploeess_El(apps, _loginUser));
            }
        }

        public void CreateUI<T>(IEnumerable<T> items)
        {
            
            parent.Children.Clear();

            foreach (var item in items)
            {

                if (item is Models.medical_records record)
                {
                    parent.Children.Add(new Elements.Medical_Records_El(record, _loginUser));
                }
                if (item is Models.medical_tests tests)
                {
                    parent.Children.Add(new Elements.Medical_Tests_El(tests, _loginUser));
                }
                if (item is Models.medications med)
                {
                    parent.Children.Add(new Elements.Medications_El(med, _loginUser));
                }
                if (item is Models.patients patients)
                {
                    parent.Children.Add(new Elements.Patients_El(patients));
                }
                if (item is Models.payments payments)
                {
                    parent.Children.Add(new Elements.Payments_El(payments, _loginUser));
                }
                if (item is Models.rooms room)
                {
                    parent.Children.Add(new Elements.Rooms_El(room));
                }
                if (item is Models.schedules shedules)
                {
                    parent.Children.Add(new Elements.Shedules_El(shedules, _loginUser));
                }
                if (item is Models.services services)
                {
                    parent.Children.Add(new Elements.Services_El(services, _loginUser));
                }
            }
        }

        private void appointments_Click(object sender, RoutedEventArgs e)
        {
            CreateUIapps();
        }

      

        private void employees_Click(object sender, RoutedEventArgs e)
        {
            CreateUIEmp();
        }

        private void medical_records_Click(object sender, RoutedEventArgs e)
        {
            CreateUI(new DataContext().medical_records);
        }

        private void medtical_tests_Click(object sender, RoutedEventArgs e)
        {
            CreateUI(new DataContext().medical_tests);

        }

        private void medications_Click(object sender, RoutedEventArgs e)
        {
            CreateUI(new DataContext().medications);
        }

        private void patients_Click(object sender, RoutedEventArgs e)
        {
            CreateUI(new DataContext().patients);
        }

        private void payments_Click(object sender, RoutedEventArgs e)
        {
            CreateUI(new DataContext().payments);
        }

        

        private void schedules_Click(object sender, RoutedEventArgs e)
        {
            CreateUI(new DataContext().services);
        }

        private void EditShedules_Click(object sender, RoutedEventArgs e)
        {
            CreateUI(new DataContext().schedules);
        }
        private void createAppointment_Click(object sender, RoutedEventArgs e)
        {
            // Создаем экземпляр окна для добавления новой записи
            Windows.AddAppointmentWindow addAppointmentWindow = new AddAppointmentWindow();

            // Настраиваем окно
            
            addAppointmentWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner; // Позиционируем по центру родительского окна

            //// Подписываемся на событие закрытия окна (если нужно обновить данные после закрытия)
            //addAppointmentWindow.Closed += (s, args) =>
            //{
            //    // Здесь можно обновить данные в главном окне, если нужно
            //    // Например: RefreshAppointmentsList();
            //};
            addAppointmentWindow.Show(); 
        }
        
        private void cabinet_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPages(new Pages.Personal_cabinet(_loginUser));
        }

        private void AddPage_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPages(new Pages.Add_Page(_loginUser));
        }

        
    }
}
