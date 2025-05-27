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
using static System.Net.Mime.MediaTypeNames;

namespace Medicine_DP.Elements
{
    /// <summary>
    /// Логика взаимодействия для Medical_Tests_El.xaml
    /// </summary>
    public partial class Medical_Tests_El : UserControl
    {
        medical_tests _medtests;
        public Medical_Tests_El(medical_tests test)
        {
            InitializeComponent();
            _medtests = test;
            lbTestId.Text = test.test_id.ToString();
            lbTestName.Text = test.test_name ?? "Не указано";
            lbCategory.Text = test.category ?? "Не указано";
            lbPrice.Text = $"{test.price:N2} руб.";

            tbDescription.Text = test.description ?? "Нет описания";
            tbPreparation.Text = test.preparation ?? "Не требуется";
            tbNormalValues.Text = test.normal_values ?? "Не указаны";
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Medtest_Edit_Window medtest_Edit_Window = new Medtest_Edit_Window(_medtests);
            medtest_Edit_Window.Show();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Подтверждение удаления
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
                    // Проверяем связанные записи в test_results
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

                    // Проверяем связанные записи в prescriptions (если есть связь)
                    // Если в вашей БД есть связь между тестами и рецептами, добавьте проверку здесь

                    // Находим тест для удаления
                    var testToDelete = context.medical_tests.Find(_medtests.test_id);
                    if (testToDelete != null)
                    {
                        context.medical_tests.Remove(testToDelete);
                        int affectedRows = context.SaveChanges();

                        if (affectedRows > 0)
                        {
                            MessageBox.Show("Тест успешно удален", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                           
                        }
                    }
                }
            }
            catch (DbUpdateException dbEx)
            {
                string errorMessage = "Ошибка базы данных:\n";
               
                    errorMessage += dbEx.Message;
                

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
