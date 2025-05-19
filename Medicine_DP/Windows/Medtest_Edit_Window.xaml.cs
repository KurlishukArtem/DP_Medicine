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
using System.Windows.Shapes;
using Medicine_DP.Config;
using Medicine_DP.Models;

namespace Medicine_DP.Windows
{
    /// <summary>
    /// Логика взаимодействия для Medtest_Edit_Window.xaml
    /// </summary>
    public partial class Medtest_Edit_Window : Window
    {
        public medical_tests _test;
        public DataContext _context = new DataContext();
        public Medtest_Edit_Window(medical_tests test = null)
        {
            InitializeComponent();
            _test = test;

            if (test != null)
            {
                txtTestId.Text = test.test_id.ToString();
                txtTestName.Text = test.test_name;
                txtCategory.Text = test.category;
                txtPrice.Text = test.price.ToString();
                txtPreparation.Text = test.preparation;
                txtNormalValues.Text = test.normal_values;
                txtDescription.Text = test.description;
            }
            else
            {
                txtTestId.Text = "Новый тест";
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (_test == null)
            {
                // Создание нового теста
                medical_tests newTest = new medical_tests
                {
                    test_name = txtTestName.Text,
                    category = txtCategory.Text,
                    price = double.Parse(txtPrice.Text),
                    preparation = txtPreparation.Text,
                    normal_values = txtNormalValues.Text,
                    description = txtDescription.Text
                };

                _context.medical_tests.Add(newTest);
            }
            else
            {
                // Обновление существующего теста
                _test.test_name = txtTestName.Text;
                _test.category = txtCategory.Text;
                _test.price = double.Parse(txtPrice.Text);
                _test.preparation = txtPreparation.Text;
                _test.normal_values = txtNormalValues.Text;
                _test.description = txtDescription.Text;

                _context.medical_tests.Update(_test);
            }

            try
            {
                _context.SaveChanges();
                MessageBox.Show("Данные теста успешно сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
