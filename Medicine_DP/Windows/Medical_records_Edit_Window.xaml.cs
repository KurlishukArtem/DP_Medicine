using Medicine_DP.Config;
using Medicine_DP.Models;
using Medicine_DP.Pages;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;

namespace Medicine_DP.Windows
{
    /// <summary>
    /// Логика взаимодействия для Medical_records_Edit_Window.xaml
    /// </summary>
    public partial class Medical_records_Edit_Window : Window
    {
        private readonly DataContext _context = Main._main._context;
        private medical_records _record;
        private bool _isNewRecord;

        public string WindowTitle => _isNewRecord ? "Новая медицинская запись" : "Редактирование записи";
        public Visibility ShowRecordId => _isNewRecord ? Visibility.Collapsed : Visibility.Visible;
        public Medical_records_Edit_Window(medical_records record = null)
        {
            InitializeComponent();
            DataContext = this;

            _record = record ?? new medical_records();
            _isNewRecord = record == null;

            LoadComboBoxData();

            if (!_isNewRecord)
            {
                LoadRecordData();
            }
            else
            {
                dpRecordDate.SelectedDate = DateTime.Today;
                cbStatus.SelectedIndex = 0; // Выбираем "Активна" по умолчанию
            }
        }

        private void LoadComboBoxData()
        {
            try
            {
                // Загрузка пациентов
                var patients = _context.patients
                    .Select(p => new
                    {
                        p.patient_id,
                        FullName = p.last_name + " " + p.first_name + " " + p.middle_name
                    })
                    .ToList();
                cbPatients.ItemsSource = patients;

                // Загрузка врачей
                var doctors = _context.employees
                    //.Where(e => e.position.ToLower().Contains("врач"))
                    .Select(e => new
                    {
                        e.employee_id,
                        FullName = e.last_name + " " + e.first_name + " " + e.middle_name
                    })
                    .ToList();
                cbDoctors.ItemsSource = doctors;

                // Загрузка записей на прием
                var appointments = _context.appointments
                    .Select(a => new
                    {
                        a.appointment_id,
                        AppointmentInfo = a.appointment_date.ToString("dd.MM.yyyy") + " - " +
                                         a.start_time.ToString(@"hh\:mm") + " - " +
                                         a.patient_id
                    })
                    .ToList();
                cbAppointments.ItemsSource = appointments;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void LoadRecordData()
        {
            txtRecordId.Text = _record.record_id.ToString();
            cbPatients.SelectedValue = _record.patient_id;
            cbDoctors.SelectedValue = _record.employee_id;
            cbAppointments.SelectedValue = _record.appointment_id;
            dpRecordDate.SelectedDate = _record.record_date;
            txtDiagnosis.Text = _record.diagnosis;
            txtTreatment.Text = _record.treatment;
            txtPrescription.Text = _record.prescription;
            txtRecommendations.Text = _record.recommendations;
            foreach (ComboBoxItem item in cbStatus.Items)
            {
                if (item.Tag.ToString() == _record.status)
                {
                    cbStatus.SelectedItem = item;
                    break;
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            //try
            //{
            medical_records medical_Records = new medical_records();
            if (medical_Records == null)
            {

                medical_Records.patient_id = int.Parse(cbPatients.Text);
                medical_Records.employee_id = int.Parse(cbDoctors.Text);
                medical_Records.appointment_id = int.Parse(cbAppointments.Text);
                medical_Records.record_date = DateTime.Parse(dpRecordDate.Text);
                medical_Records.diagnosis = txtDiagnosis.Text;
                medical_Records.treatment = txtTreatment.Text;
                medical_Records.prescription = txtPrescription.Text;
                medical_Records.recommendations = txtRecommendations.Text;
                medical_Records.status = cbAppointments.Text;

                _context.medical_records.Add(_record);
            }
            else
            {
                // Обновление данных
                _record.patient_id = (int)cbPatients.SelectedValue;
                _record.employee_id = (int)cbDoctors.SelectedValue;
                _record.appointment_id = (int)cbAppointments.SelectedValue;
                _record.record_date = dpRecordDate.SelectedDate.Value;
                _record.diagnosis = txtDiagnosis.Text;
                _record.treatment = txtTreatment.Text;
                _record.prescription = txtPrescription.Text;
                _record.recommendations = txtRecommendations.Text;
                _record.status = (cbStatus.SelectedItem as ComboBoxItem)?.Tag.ToString();

                _context.Update(_record);
            }
            _context.SaveChanges();

            MessageBox.Show("Медицинская запись успешно сохранена", "Успех",
                          MessageBoxButton.OK, MessageBoxImage.Information);

            var patient = _context.patients.FirstOrDefault(p => p.patient_id == _record.patient_id);
            if (patient != null && !string.IsNullOrWhiteSpace(patient.email))
            {
                string fullName = $"{patient.last_name} {patient.first_name}";
                var pdfBytes = MedicalReportGenerator.GenerateMedicalReport(
                                fullName,
                                _record.diagnosis,
                                _record.treatment,
                                _record.prescription,
                                _record.recommendations
                            );

                // Временный файл для отправки
                string tempFilePath = Path.Combine(Path.GetTempPath(), $"diagnosis_{Guid.NewGuid()}.pdf");
                File.WriteAllBytes(tempFilePath, pdfBytes);

                SendDiagnosisPdfByEmail(patient.email, fullName, tempFilePath);
            }
            Close();

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
            //                  MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }

        private void SendDiagnosisPdfByEmail(string toEmail, string patientName, string pdfPath)
        {
            try
            {
                var fromAddress = new MailAddress("xeniaao@yandex.ru", "Медицинская система");
                var toAddress = new MailAddress(toEmail, patientName);
                const string fromPassword = "nlfxehuparchzfqs";
                string subject = "Ваш диагноз";
                string body = $"Здравствуйте, {patientName}!\n\nВо вложении — ваш медицинский диагноз в формате PDF.";

                var smtp = new SmtpClient
                {
                    Host = "smtp.yandex.ru",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                };

                message.Attachments.Add(new Attachment(pdfPath));
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке PDF: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Удаление временного файла
                if (File.Exists(pdfPath))
                    File.Delete(pdfPath);
            }
        }


        public static class MedicalReportGenerator
        {
            public static byte[] GenerateMedicalReport(string patientName, string diagnosis, string treatment, string prescription, string recommendations)
            {
                QuestPDF.Settings.License = LicenseType.Community;
                var pdfBytes = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(40);
                        page.DefaultTextStyle(x => x.FontSize(14));

                        page.Header().Text("Медицинское заключение").SemiBold().FontSize(20).AlignCenter();

                        page.Content().Column(col =>
                        {
                            col.Spacing(10);

                            col.Item().Text($"Имя пациента: {patientName}");
                            col.Item().Text($"Диагноз: {diagnosis}");
                            col.Item().Text($"Лечение: {treatment}");
                            col.Item().Text($"Назначения: {prescription}");
                            col.Item().Text($"Рекомендации: {recommendations}");
                        });

                        page.Footer().AlignCenter().Text(txt =>
                        {
                            txt.Span("Отчёт создан автоматически");
                        });
                    });
                }).GeneratePdf();

                return pdfBytes;
            }
        }

        private bool ValidateInput()
        {
            if (cbPatients.SelectedItem == null ||
                cbDoctors.SelectedItem == null ||
                cbAppointments.SelectedItem == null ||
                dpRecordDate.SelectedDate == null ||
                cbStatus.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }
    }
}

