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
using Medicine_DP.Models;
using Medicine_DP.Windows;

namespace Medicine_DP.Elements
{
    /// <summary>
    /// Логика взаимодействия для Services_El.xaml
    /// </summary>
    public partial class Services_El : UserControl
    {
        services _services;
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

        }
    }
}
