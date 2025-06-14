using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
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
using Medicine_DP.Windows;
using Microsoft.EntityFrameworkCore;

namespace Medicine_DP.Pages
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        
        Models.employees employees;
        private readonly DataContext dataContext = new DataContext();
        
        public Login()
        {
            InitializeComponent();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string loginUser = UsernameTextBox.Text;
            string passwordBox = Password.Password.ToString();

            try
            {
                // Сначала проверяем сотрудников
                var empl = dataContext.employees.FirstOrDefault(x => x.login == loginUser && x.password_hash == passwordBox);
                if (empl != null)
                {
                    MainWindow.init.OpenPages(new Pages.Main(loginUser));
                    return;
                }

                // Затем проверяем пациентов
                var patient = dataContext.patients.FirstOrDefault(x => x.login == loginUser && x.password_hash == passwordBox);
                if (patient != null)
                {
                    MainWindow.init.OpenPages(new Pages.Main(loginUser));
                    return;
                }

                MessageBox.Show("Неверный логин или пароль!", "Ошибка");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при входе: {ex.Message}", "Ошибка");
            }
        }

        private void Reg_Click(object sender, RoutedEventArgs e)
        {
            Patients_Edit_Window patients_Edit_ = new Patients_Edit_Window();
            patients_Edit_.Show();
        }

        private async void Forgot_Password_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Создаем диалоговое окно для ввода email
                var inputDialog = new InputDialogWindow("Восстановление пароля",
                    "Введите email, указанный при регистрации:");

                if (inputDialog.ShowDialog() == true)
                {
                    string email = inputDialog.Answer?.Trim(); // Добавлен оператор ? для безопасного вызова

                    if (string.IsNullOrWhiteSpace(email))
                    {
                        MessageBox.Show("Email не может быть пустым.", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Ищем пользователя в базе данных (пациента или сотрудника)
                    using (var context = new DataContext())
                    {
                        // Проверяем в таблице пациентов
                        var patient = await context.patients
                            .FirstOrDefaultAsync(p => p.email == email);

                        // Если не найден пациент, проверяем в таблице сотрудников
                        var employee = patient == null
                            ? await context.employees
                                .FirstOrDefaultAsync(emp => emp.email == email)
                            : null;

                        if (patient == null && employee == null)
                        {
                            MessageBox.Show("Пользователь с указанным email не найден.", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        // Генерируем новый случайный пароль
                        string newPassword = GenerateRandomPassword(8);
                        string hashedPassword = HashPassword(newPassword);

                        // Обновляем пароль в базе данных
                        if (patient != null)
                        {
                            patient.password_hash = hashedPassword;
                        }
                        else if (employee != null)
                        {
                            employee.password_hash = hashedPassword;
                        }

                        await context.SaveChangesAsync();

                        // Отправляем email с новым паролем
                        bool emailSent = await SendPasswordResetEmail(email, newPassword);

                        if (emailSent)
                        {
                            MessageBox.Show("Новый пароль отправлен на указанный email.", "Успешно",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось отправить email. Обратитесь к администратору.", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод для проверки валидности email
        public class EmailValidator
        {
            public bool IsValidEmail(string email)
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(email);
                    return addr.Address == email;
                }
                catch
                {
                    return false;
                }
            }
        }

        // Метод для генерации случайного пароля
        public string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var random = new Random();
            var chars = new char[length];

            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(validChars.Length)];
            }

            return new string(chars);
        }

        // Метод для хеширования пароля (упрощенная версия)
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        // Метод для отправки email
        private async Task<bool> SendPasswordResetEmail(string email, string newPassword)
        {
            try
            {
                // Настройки SMTP (замените на свои)
                string smtpServer = "smtp.yandex.ru";
                int smtpPort = 587;
                string smtpUsername = "ya.erro2018@yandex.ru";
                string smtpPassword = "erkrkesksafpysrg";
                bool enableSsl = true;

                using (var client = new System.Net.Mail.SmtpClient(smtpServer, smtpPort))
                {
                    client.Credentials = new System.Net.NetworkCredential(smtpUsername, smtpPassword);
                    client.EnableSsl = enableSsl;

                    var message = new System.Net.Mail.MailMessage
                    {
                        From = new System.Net.Mail.MailAddress(smtpUsername),
                        Subject = "Восстановление пароля - Med-Clinic",
                        Body = $"Ваш новый пароль: {newPassword}\n\n" +
                               "Рекомендуем изменить его после входа в систему.\n\n" +
                               "С уважением,\nАдминистрация Med-Clinic",
                        IsBodyHtml = false
                    };

                    message.To.Add(email);

                    await client.SendMailAsync(message);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
