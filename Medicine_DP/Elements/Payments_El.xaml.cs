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
    /// Логика взаимодействия для Payments_El.xaml
    /// </summary>
    public partial class Payments_El : UserControl
    {
        payments _payments;
        public static string _loginUser;
        private readonly DataContext _context = Main._main._context;

        public Payments_El(payments payment, string loginUser)
        {
            InitializeComponent();
            _payments = payment;
            _loginUser = loginUser;
            lbPaymentId.Text = payment.payment_id.ToString();
            lbAppointmentId.Text = payment.appointment_id.ToString();
            lbPatientId.Text = payment.patient_id.ToString();
            lbAmount.Text = $"{payment.amount:N2} руб.";

            lbPaymentDate.Text = payment.payment_date.ToString("dd.MM.yyyy HH:mm");
            lbPaymentMethod.Text = GetPaymentMethodDisplay(payment.payment_method);
            lbStatus.Text = GetStatusDisplay(payment.status);
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
        private string GetPaymentMethodDisplay(string method)
        {
            return method switch
            {
                "cash" => "Наличные",
                "card" => "Карта",
                "transfer" => "Перевод",
                _ => method
            };
        }

        private string GetStatusDisplay(string status)
        {
            return status switch
            {
                "completed" => "Завершен",
                "pending" => "Ожидание",
                "refunded" => "Возврат",
                _ => status
            };
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Payments_Window payments_Window = new Payments_Window(_payments);    
            payments_Window.Show();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            //var result = MessageBox.Show(...);

            //if (result == MessageBoxResult.Yes)
            //{
              
                    using (var db = new DataContext())
                    {
                        var paymentToDelete =  db.payments
                            .FirstOrDefault(p => p.payment_id == _payments.payment_id);

                        if (paymentToDelete != null)
                        {
                            db.payments.Remove(paymentToDelete);
                            db.SaveChangesAsync();

                            if (Parent is Panel parentPanel)
                            {
                                parentPanel.Children.Remove(this);
                            }

                        MessageBox.Show("Пациент успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    }
                
        }
    }
}
