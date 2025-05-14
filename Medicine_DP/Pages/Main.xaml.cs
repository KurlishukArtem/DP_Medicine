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
using Medicine_DP.Config;
using Medicine_DP.Windows;
using Microsoft.EntityFrameworkCore;

namespace Medicine_DP.Pages
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class Main : Page
    {

        public Main()
        {
            InitializeComponent();
        }

        public void CreateUIapps()
        {
            
            parent.Children.Clear();
            foreach (var apps in new DataContext().appointments)
            {
                parent.Children.Add(new Elements.appointments(apps));
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

        private void rooms_Click(object sender, RoutedEventArgs e)
        {
            CreateUI(new DataContext().rooms);
        }

        private void schedules_Click(object sender, RoutedEventArgs e)
        {
            CreateUI(new DataContext().services);
        }

        private void createAppointment_Click(object sender, RoutedEventArgs e)
        {
            // Создаем экземпляр окна для добавления новой записи
            AddAppointmentWindow addAppointmentWindow = new AddAppointmentWindow();

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
    }
}
