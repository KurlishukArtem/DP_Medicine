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
    /// Логика взаимодействия для Rooms_El.xaml
    /// </summary>
    public partial class Rooms_El : UserControl
    {
        public Rooms_El(rooms room)
        {
            InitializeComponent();
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
    }
}
