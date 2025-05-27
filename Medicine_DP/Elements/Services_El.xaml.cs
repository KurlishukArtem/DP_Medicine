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
    /// Логика взаимодействия для Services_El.xaml
    /// </summary>
    public partial class Services_El : UserControl
    {
        services _services;
        private readonly DataContext _context = Main._main._context;
        public Services_El(services service)
        {
            InitializeComponent();
            _services = service;
            lbServiceId.Text = service.service_id.ToString();
            lbServiceName.Text = service.service_name;
            lbCategory.Text = service.category ?? "Не указана";
            lbPrice.Text = $"{service.price:N2} руб.";

            //lbActiveStatus.Text = service.is_active == 1 ? "Активна" : "Неактивна";
            tbDescription.Text = service.description ?? "Описание отсутствует";
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Service_Window serviceWindow = new Service_Window(_services);
            serviceWindow.Show();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Запрос подтверждения перед удалением
            MessageBoxResult result = MessageBox.Show(
                $"Вы уверены, что хотите удалить услугу \"{_services.service_name}\"?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Используем существующий контекст из Main
                    using (_context)
                    {
                        // Находим услугу для удаления по ID
                        var serviceToDelete = _context.services.FirstOrDefault(s => s.service_id == _services.service_id);

                        if (serviceToDelete != null)
                        {
                            // Проверка на связанные записи (например, назначения этой услуги)
                            bool hasAppointments = _context.appointments.Any(a => a.service_id == _services.service_id);
                            if (hasAppointments)
                            {
                                MessageBox.Show("Невозможно удалить услугу, так как она связана с записями на прием.\n" +
                                              "Сначала удалите связанные записи или деактивируйте услугу.",
                                              "Ошибка удаления",
                                              MessageBoxButton.OK,
                                              MessageBoxImage.Error);
                                return;
                            }

                            // Удаляем услугу из базы данных
                            _context.services.Remove(serviceToDelete);
                            _context.SaveChanges();

                            // Удаляем UserControl из интерфейса
                            if (Parent is Panel parentPanel)
                            {
                                parentPanel.Children.Remove(this);
                            }

                            MessageBox.Show("Услуга успешно удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (DbUpdateException dbEx)
                {
                    MessageBox.Show($"Ошибка при удалении услуги из базы данных: {dbEx.InnerException?.Message ?? dbEx.Message}",
                                  "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении услуги: {ex.Message}",
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
