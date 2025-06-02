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
using Medicine_DP.Pages;

namespace Medicine_DP.Windows
{
    /// <summary>
    /// Логика взаимодействия для Service_Window.xaml
    /// </summary>
    public partial class Service_Window : Window
    {
        private readonly services _service;
        private readonly DataContext _context = Main._main._context;
        
        public Service_Window(services service = null)
        {
            InitializeComponent();
            _service = service ?? new services();

            // Заполнение полей
            txtServiceId.Text = _service.service_id > 0
                ? _service.service_id.ToString()
                : "Новая услуга";

            txtServiceName.Text = _service.service_name;
            txtPrice.Text = _service.price.ToString("0.00");
            cbCategory.Text = _service.category;
            chkIsActive.IsChecked = _service.is_active;
            txtDescription.Text = _service.description;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
                // Валидация данных
                if (string.IsNullOrWhiteSpace(txtServiceName.Text))
                    throw new Exception("Название услуги обязательно для заполнения");

                if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
                    throw new Exception("Некорректная цена услуги");

                if (string.IsNullOrWhiteSpace(cbCategory.Text))
                    throw new Exception("Необходимо указать категорию");

                // Обновление модели
                _service.service_name = txtServiceName.Text;
                _service.price = price;
                _service.category = cbCategory.Text;
                _service.is_active = chkIsActive.IsChecked ?? true;
                _service.description = txtDescription.Text;

                // Сохранение в БД
                if (_service.service_id == 0)
                {
                    _context.services.Add(_service);
                }
                else
                {
                    _context.services.Update(_service);
                }

                _context.SaveChanges();

                MessageBox.Show("Данные услуги успешно сохранены", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                
                this.Close();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
            //        MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }
    }
}

