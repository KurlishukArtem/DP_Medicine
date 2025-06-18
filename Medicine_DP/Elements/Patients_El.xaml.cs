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
using QuestPDF.Infrastructure;

namespace Medicine_DP.Elements
{
    /// <summary>
    /// Логика взаимодействия для Patients_El.xaml
    /// </summary>
    public partial class Patients_El : UserControl
    {
        patients _patients;
        bool _visitsExpanded = false;
        StackPanel _visitsPanel;

        public Patients_El(patients patient)
        {
            InitializeComponent();
            this._patients = patient;
            LoadPatientData();
            InitializeVisitsPanel();

            // Добавляем обработчик клика по всему элементу пациента
            this.MouseDown += PatientElement_Click;
        }

        private void LoadPatientData()
        {
            lbFullName.Text = $"{_patients.last_name} {_patients.first_name} {_patients.middle_name}";
            lbBirthDate.Text = _patients.birth_date.ToString("dd.MM.yyyy");
            lbPhone.Text = _patients.phone_number ?? "Не указано";
            lbEmail.Text = _patients.email ?? "Не указано";
            lbAddress.Text = _patients.address ?? "Не указано";
            lbPassport.Text = $"{_patients.passport_series} {_patients.passport_number}";
            lbSnils.Text = _patients.snils ?? "Не указано";
            lbPolicy.Text = _patients.policy_number ?? "Не указано";
            tbNotes.Text = _patients.notes ?? "Нет примечаний";
            lbGender.Text = _patients.gender.ToString();
        }

        private void InitializeVisitsPanel()
        {
            _visitsPanel = new StackPanel { Visibility = Visibility.Collapsed, Margin = new Thickness(0, 10, 0, 0) };

            // Добавляем панель посещений в основной Grid
            if (this.Content is Border border && border.Child is Grid mainGrid)
            {
                mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                Grid.SetRow(_visitsPanel, 1);
                Grid.SetColumnSpan(_visitsPanel, 3);
                mainGrid.Children.Add(_visitsPanel);
            }
        }

        private void PatientElement_Click(object sender, MouseButtonEventArgs e)
        {
            // Игнорируем клики по кнопкам редактирования/удаления
            if (e.OriginalSource is Button)
                return;

            _visitsExpanded = !_visitsExpanded;

            if (_visitsExpanded)
            {
                LoadVisitsHistory();
                _visitsPanel.Visibility = Visibility.Visible;
            }
            else
            {
                _visitsPanel.Visibility = Visibility.Collapsed;
                _visitsPanel.Children.Clear();
            }
        }

        private void LoadVisitsHistory()
        {
            _visitsPanel.Children.Clear();

            try
            {
                using (var db = new DataContext())
                {
                    var visits = db.medical_records
                        .Where(r => r.patient_id == _patients.patient_id)
                        .OrderByDescending(r => r.record_date)
                        .ToList();

                    if (!visits.Any())
                    {
                        _visitsPanel.Children.Add(new TextBlock
                        {
                            Text = "История посещений пуста",
                            FontStyle = FontStyles.Italic,
                            Margin = new Thickness(10),
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Center
                        });
                        return;
                    }

                    var header = new TextBlock
                    {
                        Text = "История посещений",
                        FontSize = 16,
                        FontWeight = FontWeights.Bold,  // Используем FontWeights.Bold вместо FontWeight.Bold
                        Margin = new Thickness(0, 0, 0, 10),
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center
                    };
                    _visitsPanel.Children.Add(header);

                    foreach (var visit in visits)
                    {
                        var visitCard = new Border
                        {
                            Background = Brushes.White,
                            BorderBrush = Brushes.LightGray,
                            BorderThickness = new Thickness(1),
                            CornerRadius = new CornerRadius(5),
                            Margin = new Thickness(0, 0, 0, 10),
                            Padding = new Thickness(10)
                        };

                        var grid = new Grid();
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });

                        // Дата и статус
                        var dateStatusPanel = new StackPanel();
                        dateStatusPanel.Children.Add(new TextBlock
                        {
                            Text = visit.record_date.ToString("dd.MM.yyyy HH:mm"),
                            FontWeight = FontWeights.Bold
                        });
                        dateStatusPanel.Children.Add(new TextBlock
                        {
                            Text = $"Статус: {visit.status}",
                            Foreground = GetStatusColor(visit.status)
                        });

                        // Диагноз и лечение
                        var diagnosisPanel = new StackPanel();
                        diagnosisPanel.Children.Add(new TextBlock
                        {
                            Text = $"Диагноз: {visit.diagnosis ?? "Не указан"}",
                            TextWrapping = TextWrapping.Wrap
                        });
                        diagnosisPanel.Children.Add(new TextBlock
                        {
                            Text = $"Лечение: {visit.treatment ?? "Не указано"}",
                            TextWrapping = TextWrapping.Wrap,
                            Margin = new Thickness(0, 5, 0, 0)
                        });

                        Grid.SetColumn(dateStatusPanel, 0);
                        Grid.SetColumn(diagnosisPanel, 1);

                        grid.Children.Add(dateStatusPanel);
                        grid.Children.Add(diagnosisPanel);

                        visitCard.Child = grid;
                        _visitsPanel.Children.Add(visitCard);
                    }
                }
            }
            catch (Exception ex)
            {
                _visitsPanel.Children.Add(new TextBlock
                {
                    Text = $"Ошибка загрузки истории: {ex.Message}",
                    Foreground = Brushes.Red,
                    Margin = new Thickness(10)
                });
            }
        }

        private Brush GetStatusColor(string status)
        {
            switch (status?.ToLower())
            {
                case "активна":
                    return Brushes.Green;
                case "completed":
                    return Brushes.Blue;
                case "cancelled":
                    return Brushes.Red;
                case "archived":
                    return Brushes.Gray;
                default:
                    return Brushes.Black;
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Patients_Edit_Window patients_window = new Patients_Edit_Window(_patients);
            patients_window.Show();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                $"Вы уверены, что хотите удалить пациента {_patients.last_name} {_patients.first_name} {_patients.middle_name}?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new DataContext())
                    {
                        var patientToDelete = db.patients.FirstOrDefault(p => p.patient_id == _patients.patient_id);

                        if (patientToDelete != null)
                        {
                            db.patients.Remove(patientToDelete);
                            db.SaveChanges();

                            if (Parent is Panel parentPanel)
                            {
                                parentPanel.Children.Remove(this);
                            }

                            MessageBox.Show("Пациент успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении пациента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
