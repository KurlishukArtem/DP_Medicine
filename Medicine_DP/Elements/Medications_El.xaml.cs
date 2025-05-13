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
    /// Логика взаимодействия для Medications_El.xaml
    /// </summary>
    public partial class Medications_El : UserControl
    {
        public Medications_El(medications med)
        {
            InitializeComponent();
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
    }
}
