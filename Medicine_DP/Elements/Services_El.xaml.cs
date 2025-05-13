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

namespace Medicine_DP.Elements
{
    /// <summary>
    /// Логика взаимодействия для Services_El.xaml
    /// </summary>
    public partial class Services_El : UserControl
    {
        public Services_El(services service)
        {
            InitializeComponent();
            lbServiceId.Text = service.service_id.ToString();
            lbServiceName.Text = service.service_name;
            lbCategory.Text = service.category ?? "Не указана";
            lbPrice.Text = $"{service.price:N2} руб.";

            lbActiveStatus.Text = service.is_active == 1 ? "Активна" : "Неактивна";
            tbDescription.Text = service.description ?? "Описание отсутствует";
        }
    }
}
