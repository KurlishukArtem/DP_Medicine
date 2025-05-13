using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medicine_DP.Models
{
    public class services
    {
        [Key]
        public int service_id { get; set; }
        public string service_name { get; set; }
        public string description { get; set; }
        public double price { get; set; }
        public string category { get; set; }
        public int is_active { get; set; }
    }
}
