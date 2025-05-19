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
    /// Логика взаимодействия для Medications_Edit_Window.xaml
    /// </summary>
    public partial class Medications_Edit_Window : Window
    {
        public medications _medication;
        public DataContext _context = new DataContext();
        public Medications_Edit_Window(medications medication = null)
        {
            InitializeComponent();
            _medication = medication;

            if (medication != null)
            {
                // Заполнение полей для редактирования
                txtMedicationId.Text = medication.medication_id.ToString();
                txtName.Text = medication.name;
                txtManufacturer.Text = medication.manufacturer;
                cbDosageForm.Text = medication.dosage_form;
                txtDosage.Text = medication.dosage;
                txtQuantity.Text = medication.quantity_in_stock.ToString();
                txtMinStock.Text = medication.minimum_stock_level.ToString();
                txtPrice.Text = medication.price.ToString("0.00");
                txtDescription.Text = medication.description;
            }
            else
            {
                // Установка значений по умолчанию для нового препарата
                txtMedicationId.Text = "Новый препарат";
                txtQuantity.Text = "0";
                txtMinStock.Text = "5";
                cbDosageForm.SelectedIndex = 0;
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
                if (_medication == null)
                {
                    // Создание нового препарата
                    _medication = new medications
                    {
                        name = txtName.Text,
                        manufacturer = txtManufacturer.Text,
                        dosage_form = cbDosageForm.Text,
                        dosage = txtDosage.Text,
                        quantity_in_stock = int.Parse(txtQuantity.Text),
                        minimum_stock_level = int.Parse(txtMinStock.Text),
                        price = double.Parse(txtPrice.Text),
                        description = txtDescription.Text
                    };
                    _context.Add(_medication);
                }
                else
                {
                    // Обновление существующего препарата
                    _medication.name = txtName.Text;
                    _medication.manufacturer = txtManufacturer.Text;
                    _medication.dosage_form = cbDosageForm.Text;
                    _medication.dosage = txtDosage.Text;
                    _medication.quantity_in_stock = int.Parse(txtQuantity.Text);
                    _medication.minimum_stock_level = int.Parse(txtMinStock.Text);
                    _medication.price = double.Parse(txtPrice.Text);
                    _medication.description = txtDescription.Text;

                    _context.Update(_medication);
                }

                _context.SaveChanges();
                MessageBox.Show("Данные препарата успешно сохранены", "Успех",
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
