using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medicine_DP.Models
{
    public class rooms
    {
        [Key]
        public int room_id { get; set; }
        public string room_number { get; set; }
        public string room_type { get; set; }
        public int floor { get; set; }
        public string description { get; set; }
        public int is_available { get; set; }
    }
}
