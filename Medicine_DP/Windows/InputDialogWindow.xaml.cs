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

namespace Medicine_DP.Windows
{
    /// <summary>
    /// Логика взаимодействия для InputDialogWindow.xaml
    /// </summary>
    public partial class InputDialogWindow : Window
    {
        // Изменяем свойство Answer с автоматической реализацией на полноценное свойство с полем
        private string _answer = string.Empty;
        public string Answer
        {
            get { return _answer; }
            set { _answer = value; }
        }

        public InputDialogWindow(string title, string question)
        {
            InitializeComponent();
            this.Title = title;
            lblQuestion.Content = question;

            // Устанавливаем DataContext для привязки
            this.DataContext = this;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtAnswer.SelectAll();
            txtAnswer.Focus();
        }
    }
}
