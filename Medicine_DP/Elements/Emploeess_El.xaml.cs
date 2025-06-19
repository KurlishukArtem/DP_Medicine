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
using Medicine_DP.Pages;
using Medicine_DP.Windows;
using Microsoft.EntityFrameworkCore;

namespace Medicine_DP.Elements
{
    /// <summary>
    /// Логика взаимодействия для Emploeess_El.xaml
    /// </summary>
    public partial class Emploeess_El : UserControl
    {
        private readonly employees _employee;
        private readonly DataContext _context = Main._main._context;
        public static string _loginUser;
        public Brush IsActiveColor
        {
            get { return (Brush)GetValue(IsActiveColorProperty); }
            set { SetValue(IsActiveColorProperty, value); }
        }

        public static readonly DependencyProperty IsActiveColorProperty =
          DependencyProperty.Register("IsActiveColor", typeof(Brush), typeof(Emploeess_El));
        public Emploeess_El(Models.employees employee, string loginUser)
        {

            InitializeComponent();
            _employee = employee;
            _loginUser = loginUser;
            LoadEmployeeData();
            ConfigureUIForUserRole();
        }
        private void ConfigureUIForUserRole()
        {
            // Проверяем сначала сотрудников, затем пациентов
            bool isEmployee = _context.employees.Any(e => e.login == _loginUser);
            bool isPatient = _context.patients.Any(p => p.login == _loginUser);

            if (isPatient) // Если это пациент
            {
                btnDelete.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Collapsed;
            }
            else if (isEmployee) // Если это сотрудник
            {
                btnDelete.Visibility = Visibility.Visible;
                btnEdit.Visibility = Visibility.Visible;
            }
            else
            {
                // Если пользователь не найден (не должно происходить после успешного входа)
                MessageBox.Show("Не удалось определить роль пользователя", "Ошибка");
            }
        }
        private void LoadEmployeeData()
        {
            try
            {
                // Основная информация
                lbFullName.Text = $"{_employee.last_name} {_employee.first_name} {_employee.middle_name}";
                lbPosition.Text = _employee.position ?? "Не указано";
                lbSpecialization.Text = _employee.specialization ?? "Не указано";
                lbBirthDate.Text = _employee.birth_date.ToString("dd.MM.yyyy");
                lbGender.Text = GetGenderDisplay(_employee.gender);

                // Контактная информация
                lbPhone.Text = _employee.phone_number ?? "Не указано";
                lbEmail.Text = _employee.email ?? "Не указано";
                lbHireDate.Text = _employee.hire_date.ToString("dd.MM.yyyy");
                lbAddress.Text = _employee.address ?? "Не указано";
                lbLogin.Text = _employee.login ?? "Не указано";

                // Статус активности
                lbIsActive.Text = _employee.is_active == 1 ? "Активен" : "Неактивен";
                IsActiveColor = _employee.is_active == 1 ? Brushes.Green : Brushes.Red;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetGenderDisplay(char gender)
        {
            return gender switch
            {
                'M' => "Мужской",
                'F' => "Женский",
                _ => "Не указан"
            };
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
                var confirmResult = MessageBox.Show(
                    $"Вы действительно хотите удалить сотрудника?\n\n" +
                    $"{_employee.last_name} {_employee.first_name} {_employee.middle_name}\n" +
                    $"Должность: {_employee.position}",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirmResult != MessageBoxResult.Yes) return;

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    //try
                    //{
                        //// Проверка связанных записей
                        //bool hasAppointments = await _context.appointments
                        //    .AnyAsync(a => a.employee_id == _employee.employee_id);

                        //bool hasMedicalRecords = await _context.medical_records
                        //    .AnyAsync(m => m.employee_id == _employee.employee_id);

                        //bool hasSchedules = await _context.schedules
                        //    .AnyAsync(s => s.employee_id == _employee.employee_id);

                        //if (hasAppointments || hasMedicalRecords || hasSchedules)
                        //{
                        //    MessageBox.Show("Нельзя удалить сотрудника, так как есть связанные записи",
                        //        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        //    return;
                        //}

                        _context.employees.Remove(_employee);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        MessageBox.Show("Сотрудник успешно удален", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    //}
                    //catch (DbUpdateException dbEx)
                    //{
                    //    await transaction.RollbackAsync();
                    //    MessageBox.Show($"Ошибка базы данных: {dbEx.InnerException?.Message}",
                    //        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    //}
                    //catch (Exception ex)
                    //{
                    //    await transaction.RollbackAsync();
                    //    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    //        MessageBoxButton.OK, MessageBoxImage.Error);
                    //}
                }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Непредвиденная ошибка: {ex.Message}", "Ошибка",
            //        MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new Emploeess_Edit_Window(_employee);
            editWindow.Closed += (s, args) =>
            {
                // Обновляем данные после редактирования
                LoadEmployeeData();
            };
            editWindow.ShowDialog();
        }

        
    }
}
