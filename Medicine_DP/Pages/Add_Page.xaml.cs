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
using Medicine_DP.Windows;

namespace Medicine_DP.Pages
{
    /// <summary>
    /// Логика взаимодействия для Add_Page.xaml
    /// </summary>
    public partial class Add_Page : Page
    {
        public Add_Page()
        {
            InitializeComponent();
        }

        private void AddEmloyes_Click(object sender, RoutedEventArgs e)
        {
            Emploeess_Edit_Window emploeess_Edit_Window = new Emploeess_Edit_Window();
            emploeess_Edit_Window.Show();
        }
    }
}
