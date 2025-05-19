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
using System.Windows.Shapes;
using Medicine_DP.Config;
using Medicine_DP.Models;

namespace Medicine_DP.Windows
{
    /// <summary>
    /// Логика взаимодействия для Payments_Window.xaml
    /// </summary>
    public partial class Payments_Window : Window
    {
        public payments _payment;
        public DataContext _context = new DataContext();

        public Payments_Window(payments payment = null)
        {
            InitializeComponent();

            _payment = payment;

            // Загрузка списка записей на прием
            cbAppointment.ItemsSource = _context.appointments.ToList();

            if (payment != null)
            {
                // Заполнение полей для редактирования
                txtPaymentId.Text = payment.payment_id.ToString();
                cbAppointment.SelectedValue = payment.appointment_id;
                txtAmount.Text = payment.amount.ToString("0.00");
                dpPaymentDate.SelectedDate = payment.payment_date;
                cbPaymentMethod.Text = payment.payment_method;
                cbStatus.Text = payment.status;

                // Загрузка информации о пациенте
                var patient = _context.patients.Find(payment.patient_id);
                if (patient != null)
                    txtPatient.Text = $"{patient.last_name} {patient.first_name} {patient.middle_name}";
            }
            else
            {
                // Установка значений по умолчанию для нового платежа
                txtPaymentId.Text = "Новый платеж";
                dpPaymentDate.SelectedDate = DateTime.Now;
                cbPaymentMethod.SelectedIndex = 0;
                cbStatus.SelectedIndex = 0;
            }

            // Обработка изменения выбранной записи на прием
            cbAppointment.SelectionChanged += (s, e) =>
            {
                if (cbAppointment.SelectedItem is appointments appointment)
                {
                    var patient = _context.patients.Find(appointment.patient_id);
                    if (patient != null)
                        txtPatient.Text = $"{patient.last_name} {patient.first_name} {patient.middle_name}";
                }
            };
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация
                if (cbAppointment.SelectedItem == null)
                    throw new Exception("Необходимо выбрать запись на прием");
                //if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
                //    throw new Exception("Некорректная сумма платежа");
                if (dpPaymentDate.SelectedDate == null)
                    throw new Exception("Необходимо указать дату платежа");

                var appointment = (appointments)cbAppointment.SelectedItem;

                if (_payment == null)
                {
                    // Создание нового платежа
                    _payment = new payments
                    {
                        appointment_id = appointment.appointment_id,
                        patient_id = appointment.patient_id,
                        amount = _payment.amount,
                        payment_date = dpPaymentDate.SelectedDate.Value,
                        payment_method = cbPaymentMethod.Text,
                        status = ((ComboBoxItem)cbStatus.SelectedItem).Tag.ToString()
                    };
                    _context.payments.Add(_payment);
                }
                else
                {
                    // Обновление существующего платежа
                    _payment.appointment_id = appointment.appointment_id;
                    _payment.patient_id = appointment.patient_id;
                    _payment.amount = _payment.amount;
                    _payment.payment_date = dpPaymentDate.SelectedDate.Value;
                    _payment.payment_method = cbPaymentMethod.Text;
                    _payment.status = ((ComboBoxItem)cbStatus.SelectedItem).Tag.ToString();

                    _context.payments.Update(_payment);
                }

                _context.SaveChanges();
                MessageBox.Show("Данные платежа успешно сохранены", "Успех",
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
