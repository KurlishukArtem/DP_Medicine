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
using Medicine_DP.Config;
using Medicine_DP.Models;

namespace Medicine_DP.Windows
{
    /// <summary>
    /// Логика взаимодействия для Room_Edit_Window.xaml
    /// </summary>
    public partial class Room_Edit_Window : Window
    {
        public rooms _room;
        public DataContext _context = new DataContext();
        public Room_Edit_Window(rooms room = null)
        {
            InitializeComponent();
            _room = room;
            if (room != null)
            {
                this.txtRoomNumber.Text = room.room_number;
                this.cbRoomType.Text = room.room_type;
                this.txtFloor.Text = room.floor.ToString();
                this.txtDescription.Text = room.description;
            }
        }

        

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (_room == null)
            {
                
            }
            else
            {
                _room.room_number = txtRoomNumber.Text;
                _room.room_type = cbRoomType.Text;
                _room.floor = int.Parse(txtFloor.Text);
                _room.description = txtDescription.Text;

                _context.rooms.Update(_room);
                _context.SaveChanges();

                
                MessageBox.Show("Кабинет успешно изменён", "Изменение", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }


        }
    }
}
