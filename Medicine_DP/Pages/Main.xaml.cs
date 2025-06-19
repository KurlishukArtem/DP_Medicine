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
        public static patients CurrentPatient { get; set; }
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
                        medications.Visibility = Visibility.Collapsed;
                        payments.Visibility = Visibility.Collapsed;
                        AddPage.Visibility = Visibility.Visible;
                        
                        break;

                    case "Администратор":
                        // Администратор видит все
                        employees.Visibility = Visibility.Visible;
                        medical_tests.Visibility = Visibility.Visible;
                        medications.Visibility = Visibility.Collapsed;
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

        private DatePicker _datePicker;
        private Button _applyFilterButton;

        public void CreateUIapps()
        {
            var patient = _context.patients.FirstOrDefault(p => p.login == _loginUser);
            IQueryable<appointments> query = _context.appointments;

            if (patient != null)
            {
                query = query.Where(x => x.patient_id == patient.patient_id);
            }
            else
            {
                // Для врача добавляем фильтр по employee_id и дате
                var currentEmployee = _context.employees.FirstOrDefault(e => e.login == _loginUser);
                if (currentEmployee?.position == "Врач")
                {
                    query = query.Where(x => x.employee_id == currentEmployee.employee_id);

                    // Применяем фильтр по дате, если он установлен
                    if (_datePicker != null && _datePicker.SelectedDate.HasValue)
                    {
                        query = query.Where(x => x.appointment_date.Date == _datePicker.SelectedDate.Value.Date);
                    }
                }
            }

            // Сортируем по дате и времени
            var appointmentsList = query
                .OrderBy(a => a.appointment_date)
                .ThenBy(a => a.start_time)
                .ToList();

            parent.Children.Clear();

            // Добавляем элементы управления фильтром для врача/администратора
            if (_context.employees.Any(e => e.login == _loginUser && (e.position == "Врач" || e.position == "Администратор")))
            {
                AddFilterControls();
            }

            // Добавляем поле поиска пациентов для администратора
            if (_context.employees.Any(e => e.login == _loginUser && e.position == "Администратор"))
            {
                AddPatientSearchControls();
            }
            if (_context.employees.Any(e => e.login == _loginUser && e.position == "Врач"))
            {
                AddPatientSearchControls();
            }


            foreach (var app in appointmentsList)
            {
                parent.Children.Add(new Elements.appointments(app, _loginUser));
            }
        }

        private void AddPatientSearchControls()
        {
            var searchPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var searchTextBox = new TextBox
            {
                Width = 250,
                Margin = new Thickness(0, 0, 10, 0),
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(5),
                Tag = "Введите ФИО пациента"
            };

            // Обработчик для подсказки
            searchTextBox.GotFocus += (s, e) =>
            {
                if (searchTextBox.Text == (string)searchTextBox.Tag)
                    searchTextBox.Text = "";
            };

            searchTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(searchTextBox.Text))
                    searchTextBox.Text = (string)searchTextBox.Tag;
            };

            var searchButton = new Button
            {
                Content = "Поиск пациента",
                Margin = new Thickness(0, 0, 10, 0),
                Padding = new Thickness(10, 5, 10, 5),
                Background = Brushes.LightGreen,
                Cursor = Cursors.Hand
            };
            searchButton.Click += (s, e) => SearchPatientAppointments(searchTextBox.Text);

            var resetButton = new Button
            {
                Content = "Сбросить",
                Margin = new Thickness(0, 0, 10, 0),
                Padding = new Thickness(10, 5, 10, 5),
                Background = Brushes.LightGray,
                Cursor = Cursors.Hand
            };
            resetButton.Click += (s, e) =>
            {
                searchTextBox.Text = "";
                CreateUIapps();
            };

            searchPanel.Children.Add(new TextBlock
            {
                Text = "Поиск пациента:",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0)
            });
            searchPanel.Children.Add(searchTextBox);
            searchPanel.Children.Add(searchButton);
            searchPanel.Children.Add(resetButton);

            parent.Children.Add(searchPanel);
        }

        private void SearchPatientAppointments(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText) || searchText == "Введите ФИО пациента")
            {
                MessageBox.Show("Введите ФИО пациента для поиска", "Поиск",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                // Разбиваем поисковую строку на части
                var searchParts = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Ищем пациентов, у которых ФИО содержит все части поисковой строки
                var foundPatients = _context.patients.AsEnumerable()
     .Where(p => searchParts.All(part =>
         p.last_name.Contains(part, StringComparison.OrdinalIgnoreCase) ||
         p.first_name.Contains(part, StringComparison.OrdinalIgnoreCase) ||
         (p.middle_name != null && p.middle_name.Contains(part, StringComparison.OrdinalIgnoreCase))))
     .Select(p => p.patient_id)
     .ToList();

                if (!foundPatients.Any())
                {
                    MessageBox.Show("Пациенты не найдены", "Результаты поиска",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Фильтруем записи по найденным пациентам
                var filteredAppointments = _context.appointments
                
                    .Where(a => foundPatients.Contains(a.patient_id))
                    .OrderBy(a => a.appointment_date)
                    .ThenBy(a => a.start_time)
                    .ToList();
                var filteredPatients = _context.patients
                    .Where(a => foundPatients.Contains(a.patient_id))
                    .ToList();

                parent.Children.Clear();

                // Добавляем элементы управления фильтром (если нужно)
                if (_context.employees.Any(e => e.login == _loginUser && (e.position == "Врач" || e.position == "Администратор")))
                {
                    AddFilterControls();
                    AddPatientSearchControls();
                }

                foreach (var app in filteredAppointments)
                {
                    parent.Children.Add(new Elements.appointments(app, _loginUser));
                }
                foreach (var app in filteredPatients)
                {
                    parent.Children.Add(new Elements.Patients_El(app));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AddFilterControls()
        {
            var filterPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 10)
            };

            _datePicker = new DatePicker
            {
                SelectedDate = DateTime.Today,
                Margin = new Thickness(0, 0, 10, 0),
                Width = 150
            };

            _applyFilterButton = new Button
            {
                Content = "Применить фильтр",
                Margin = new Thickness(0, 0, 10, 0),
                Padding = new Thickness(10, 5, 10, 5),
                Background = Brushes.LightBlue
            };
            _applyFilterButton.Click += ApplyFilterButton_Click;

            filterPanel.Children.Add(new TextBlock
            {
                Text = "Дата приема:",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0)
            });
            filterPanel.Children.Add(_datePicker);
            filterPanel.Children.Add(_applyFilterButton);

            // Кнопка сброса фильтра
            var resetButton = new Button
            {
                Content = "Сбросить",
                Margin = new Thickness(0, 0, 10, 0),
                Padding = new Thickness(10, 5, 10, 5),
                Background = Brushes.LightGray
            };
            resetButton.Click += (s, e) =>
            {
                _datePicker.SelectedDate = DateTime.Today;
                CreateUIapps();
            };
            filterPanel.Children.Add(resetButton);

            parent.Children.Add(filterPanel);
        }
        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            CreateUIapps();
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
