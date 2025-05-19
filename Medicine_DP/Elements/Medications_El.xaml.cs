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
    /// Логика взаимодействия для Medications_El.xaml
    /// </summary>
    public partial class Medications_El : UserControl
    {
        medications _medications;
        public Medications_El(medications med)
        {
            InitializeComponent();
            _medications = med;
            lbMedicationId.Text = med.medication_id.ToString();
            lbName.Text = med.name ?? "Не указано";
            lbManufacturer.Text = med.manufacturer ?? "Не указано";
            lbDosageForm.Text = med.dosage_form ?? "Не указано";
            lbDosage.Text = med.dosage ?? "Не указано";

            lbQuantity.Text = $"{med.quantity_in_stock} шт.";
            lbMinStock.Text = med.minimum_stock_level.ToString();
            lbPrice.Text = $"{med.price:N2} руб.";
            tbDescription.Text = med.description ?? "Нет описания";
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Medications_Edit_Window medications_Edit_Window = new Medications_Edit_Window(_medications);
            medications_Edit_Window.ShowDialog();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
