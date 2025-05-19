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
    /// Логика взаимодействия для Service_Window.xaml
    /// </summary>
    public partial class Service_Window : Window
    {
        public services _service;
        public DataContext _context = new DataContext();
        public Service_Window(services service = null)
        {
            InitializeComponent();
            _service = service;

            if (service != null)
            {
                // Заполнение полей для редактирования
                txtServiceId.Text = service.service_id.ToString();
                txtServiceName.Text = service.service_name;
                txtPrice.Text = service.price.ToString("0.00");
                cbCategory.Text = service.category;
                chkIsActive.IsChecked = service.is_active == 1;
                txtDescription.Text = service.description;
            }
            else
            {
                // Установка значений по умолчанию для новой услуги
                txtServiceId.Text = "Новая услуга";
                txtPrice.Text = "0.00";
                chkIsActive.IsChecked = true;
                cbCategory.SelectedIndex = 0;
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация данных
                if (string.IsNullOrWhiteSpace(txtServiceName.Text))
                    throw new Exception("Название услуги обязательно для заполнения");
                if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
                    throw new Exception("Некорректная цена услуги");
                if (string.IsNullOrWhiteSpace(cbCategory.Text))
                    throw new Exception("Необходимо указать категорию");

                if (_service == null)
                {
                    // Создание новой услуги
                    _service = new services
                    {
                        service_name = txtServiceName.Text,
                        price = (double)price,
                        category = cbCategory.Text,
                        is_active = chkIsActive.IsChecked == true ? 1 : 0,
                        description = txtDescription.Text
                    };
                    _context.services.Add(_service);
                }
                else
                {
                    // Обновление существующей услуги
                    _service.service_name = txtServiceName.Text;
                    _service.price = (double)price;
                    _service.category = cbCategory.Text;
                    _service.is_active = chkIsActive.IsChecked == true ? 1 : 0;
                    _service.description = txtDescription.Text;

                    _context.services.Update(_service);
                }

                _context.SaveChanges();
                MessageBox.Show("Данные услуги успешно сохранены", "Успех",
                               MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
