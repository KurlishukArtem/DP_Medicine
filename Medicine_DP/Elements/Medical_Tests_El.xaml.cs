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
using static System.Net.Mime.MediaTypeNames;

namespace Medicine_DP.Elements
{
    /// <summary>
    /// Логика взаимодействия для Medical_Tests_El.xaml
    /// </summary>
    public partial class Medical_Tests_El : UserControl
    {
        private readonly medical_tests _medtests;
        private readonly string _loginUser;
        private readonly DataContext _context = Main._main._context;

        public Medical_Tests_El(medical_tests test, string loginUser)
        {
            InitializeComponent();
            _medtests = test;
            _loginUser = loginUser;

            // Загрузка данных теста
            lbTestId.Text = test.test_id.ToString();
            lbTestName.Text = test.test_name ?? "Не указано";
            lbCategory.Text = test.category ?? "Не указано";
            lbPrice.Text = $"{test.price:N2} руб.";
            tbDescription.Text = test.description ?? "Нет описания";
            tbPreparation.Text = test.preparation ?? "Не требуется";
            tbNormalValues.Text = test.normal_values ?? "Не указаны";

            ConfigureUIForUserRole();
        }

        private void ConfigureUIForUserRole()
        {
            var currentEmployee = _context.employees.FirstOrDefault(e => e.login == _loginUser);
            var isPatient = _context.patients.Any(p => p.login == _loginUser);

            if (isPatient)
            {
                // Настройки для пациентов
                btnDelete.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Collapsed;
            }
            else if (currentEmployee != null)
            {
                // Настройки для сотрудников в зависимости от должности
                switch (currentEmployee.position)
                {
                    case "Врач":
                        // Врачи могут только просматривать тесты
                        btnDelete.Visibility = Visibility.Collapsed;
                        btnEdit.Visibility = Visibility.Collapsed;
                        break;

                    case "Администратор":
                        // Администраторы имеют полный доступ
                        btnDelete.Visibility = Visibility.Visible;
                        btnEdit.Visibility = Visibility.Visible;
                        break;


                    default:
                        // Остальные сотрудники - только просмотр
                        btnDelete.Visibility = Visibility.Collapsed;
                        btnEdit.Visibility = Visibility.Collapsed;
                        break;
                }
            }
            else
            {
                MessageBox.Show("Не удалось определить роль пользователя", "Ошибка");
                btnDelete.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Collapsed;
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var currentEmployee = _context.employees.FirstOrDefault(e => e.login == _loginUser);
            if (currentEmployee?.position == "Лаборант" || currentEmployee?.position == "Администратор")
            {
                Medtest_Edit_Window medtest_Edit_Window = new Medtest_Edit_Window(_medtests);
                medtest_Edit_Window.Closed += (s, args) =>
                {
                    // Обновление данных после редактирования
                    lbTestName.Text = _medtests.test_name ?? "Не указано";
                    lbCategory.Text = _medtests.category ?? "Не указано";
                    lbPrice.Text = $"{_medtests.price:N2} руб.";
                    tbDescription.Text = _medtests.description ?? "Нет описания";
                    tbPreparation.Text = _medtests.preparation ?? "Не требуется";
                    tbNormalValues.Text = _medtests.normal_values ?? "Не указаны";
                };
                medtest_Edit_Window.Show();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var currentEmployee = _context.employees.FirstOrDefault(e => e.login == _loginUser);
            if (currentEmployee?.position != "Администратор")
            {
                MessageBox.Show("У вас нет прав на удаление тестов", "Ошибка прав доступа");
                return;
            }

            try
            {
                var confirmResult = MessageBox.Show(
                    $"Вы действительно хотите удалить этот тест?\n\n" +
                    $"Название: {_medtests.test_name}\n" +
                    $"Категория: {_medtests.category}\n" +
                    $"Цена: {_medtests.price:N2} руб.",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirmResult != MessageBoxResult.Yes)
                    return;

                using (var context = new DataContext())
                {
                    bool hasTestResults = context.test_results
                        .Any(tr => tr.test_id == _medtests.test_id);

                    if (hasTestResults)
                    {
                        MessageBox.Show("Невозможно удалить тест, так как существуют связанные результаты анализов.\n" +
                                      "Сначала удалите все связанные результаты.",
                            "Ошибка удаления",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }

                    var testToDelete = context.medical_tests.Find(_medtests.test_id);
                    if (testToDelete != null)
                    {
                        context.medical_tests.Remove(testToDelete);
                        int affectedRows = context.SaveChanges();

                        if (affectedRows > 0)
                        {
                            MessageBox.Show("Тест успешно удален", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            // Можно добавить событие для обновления UI
                        }
                    }
                }
            }
            catch (DbUpdateException dbEx)
            {
                string errorMessage = "Ошибка базы данных:\n" + dbEx.Message;
                MessageBox.Show(errorMessage, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
