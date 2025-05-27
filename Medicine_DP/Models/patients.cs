using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Windows.Controls;

namespace Medicine_DP.Models
{
    public class patients
    {
        [Key]
        public int patient_id { get; set; }

        public string first_name { get; set; }
        public string last_name { get; set; }
        public string middle_name { get; set; }
        public DateTime birth_date { get; set; }
        public char gender { get; set; }
        public string phone_number { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string passport_series { get; set; }
        public string passport_number { get; set; }
        public string snils { get; set; }
        public string policy_number { get; set; }
        public DateTime registration_date { get; set; } = DateTime.Now;
        public string notes { get; set; }
        public string login { get; set; }
        public string password_hash { get; set; }

        
    }
}
