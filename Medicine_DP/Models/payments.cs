using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medicine_DP.Models
{
    public class payments
    {
        [Key]
        public int payment_id { get; set; }
        public int appointment_id { get; set; }
        public int patient_id { get; set; }
        public double amount { get; set; }
        public DateTime payment_date { get; set; }
        public string payment_method { get; set; }
        public string status { get; set; }
    }
}
