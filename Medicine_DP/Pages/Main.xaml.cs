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
            // Проверяем сначала сотрудников, затем пациентов
            bool isEmployee = _context.employees.Any(e => e.login == _loginUser);
            bool isPatient = _context.patients.Any(p => p.login == _loginUser);

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
            else if (isEmployee) // Если это сотрудник
            {
                // Показываем все кнопки
                employees.Visibility = Visibility.Visible;
                medical_tests.Visibility = Visibility.Visible;
                medications.Visibility = Visibility.Visible;
                patients.Visibility = Visibility.Visible;
                payments.Visibility = Visibility.Visible;
                createAppointment.Visibility = Visibility.Visible;
                AddPage.Visibility = Visibility.Visible;
            }
            else
            {
                // Если пользователь не найден (не должно происходить после успешного входа)
                MessageBox.Show("Не удалось определить роль пользователя", "Ошибка");
            }
        }

        public void CreateUIapps()
        {
            var Patient = _context.patients.FirstOrDefault(p => p.login == _loginUser);
            List<appointments> list;
            if (Patient != null)
            {
                list = _context.appointments.Where(x=>x.patient_id == Patient.patient_id).ToList();
            }
            else
            {
                list = _context.appointments.ToList();
            }
            parent.Children.Clear();
            foreach (var apps in list)
            {
                parent.Children.Add(new Elements.appointments(apps, _loginUser));
            }
        }

        public void CreateUIEmp()
        {

            parent.Children.Clear();
            foreach (var apps in new DataContext().employees)
            {
                parent.Children.Add(new Elements.Emploeess_El(apps));
            }
        }

        public void CreateUI<T>(IEnumerable<T> items)
        {
            
            parent.Children.Clear();

            foreach (var item in items)
            {

                if (item is Models.medical_records record)
                {
                    parent.Children.Add(new Elements.Medical_Records_El(record));
                }
                if (item is Models.medical_tests tests)
                {
                    parent.Children.Add(new Elements.Medical_Tests_El(tests));
                }
                if (item is Models.medications med)
                {
                    parent.Children.Add(new Elements.Medications_El(med));
                }
                if (item is Models.patients patients)
                {
                    parent.Children.Add(new Elements.Patients_El(patients));
                }
                if (item is Models.payments payments)
                {
                    parent.Children.Add(new Elements.Payments_El(payments));
                }
                if (item is Models.rooms room)
                {
                    parent.Children.Add(new Elements.Rooms_El(room));
                }
                if (item is Models.services shedules)
                {
                    parent.Children.Add(new Elements.Services_El(shedules));
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
