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
    /// Логика взаимодействия для Rooms_El.xaml
    /// </summary>
    public partial class Rooms_El : UserControl
    {
        public rooms _rooms;
        public Rooms_El(rooms room)
        {
            InitializeComponent();
            _rooms = room;
            lbRoomId.Text = room.room_id.ToString();
            lbRoomNumber.Text = room.room_number;
            lbRoomType.Text = GetRoomTypeDisplay(room.room_type);
            lbFloor.Text = room.floor.ToString();

            //lbAvailability.Text = room.is_available == 1 ? "Доступен" : "Занят";
            tbDescription.Text = room.description ?? "Нет описания";


        }
        private string GetRoomTypeDisplay(string type)
        {
            return type switch
            {
                "consultation" => "Консультационный",
                "procedure" => "Процедурный",
                "operation" => "Операционная",
                "diagnostic" => "Диагностический",
                _ => type
            };
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Room_Edit_Window room_Edit_Window = new Room_Edit_Window(_rooms);
            room_Edit_Window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            room_Edit_Window.Show();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
