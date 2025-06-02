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

namespace Medicine_DP.Elements
{
    /// <summary>
    /// Логика взаимодействия для Medications_El.xaml
    /// </summary>
    public partial class Medications_El : UserControl
    {
        medications _medications;
        private readonly DataContext _context = Main._main._context;
        public static string _loginUser;
        public Medications_El(medications med, string loginUser)
        {
            InitializeComponent();
            _medications = med;
            _loginUser = loginUser;
            lbMedicationId.Text = med.medication_id.ToString();
            lbName.Text = med.name ?? "Не указано";
            lbManufacturer.Text = med.manufacturer ?? "Не указано";
            lbDosageForm.Text = med.dosage_form ?? "Не указано";
            lbDosage.Text = med.dosage ?? "Не указано";

            lbQuantity.Text = $"{med.quantity_in_stock} шт.";
            lbMinStock.Text = med.minimum_stock_level.ToString();
            lbPrice.Text = $"{med.price:N2} руб.";
            tbDescription.Text = med.description ?? "Нет описания";
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
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Medications_Edit_Window medications_Edit_Window = new Medications_Edit_Window(_medications);
            medications_Edit_Window.ShowDialog();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Запрос подтверждения перед удалением
            MessageBoxResult result = MessageBox.Show(
                $"Вы уверены, что хотите удалить лекарство {_medications.name}?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new DataContext()) // Замените YourDbContext на ваш класс контекста
                    {
                        // Находим medication для удаления по ID
                        var medicationToDelete = db.medications.FirstOrDefault(m => m.medication_id == _medications.medication_id);

                        if (medicationToDelete != null)
                        {
                            // Удаляем medication из базы данных
                            db.medications.Remove(medicationToDelete);
                            db.SaveChanges();

                            // Удаляем UserControl из интерфейса
                            var parent = Parent as Panel; // Предполагаем, что родитель - это Panel (StackPanel, WrapPanel и т.д.)
                            parent?.Children.Remove(this);

                            MessageBox.Show("Лекарство успешно удалено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении лекарства: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
