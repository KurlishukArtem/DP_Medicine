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
using static System.Net.Mime.MediaTypeNames;

namespace Medicine_DP.Elements
{
    /// <summary>
    /// Логика взаимодействия для Medical_Tests_El.xaml
    /// </summary>
    public partial class Medical_Tests_El : UserControl
    {
        public Medical_Tests_El(medical_tests test)
        {
            InitializeComponent();
            lbTestId.Text = test.test_id.ToString();
            lbTestName.Text = test.test_name ?? "Не указано";
            lbCategory.Text = test.category ?? "Не указано";
            lbPrice.Text = $"{test.price:N2} руб.";

            tbDescription.Text = test.description ?? "Нет описания";
            tbPreparation.Text = test.preparation ?? "Не требуется";
            tbNormalValues.Text = test.normal_values ?? "Не указаны";
        }
    }
}
