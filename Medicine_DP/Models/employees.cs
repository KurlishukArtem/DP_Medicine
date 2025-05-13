using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medicine_DP.Models
{
    public class employees
    {
        [Key]
        public int employee_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string middle_name { get; set; }
        public string position { get; set; }
        public string specialization { get; set; }
        public DateTime birth_date { get; set; }
        public char gender { get; set; }
        public string phone_number { get; set; }
        public string email { get; set; }
        public DateTime hire_date { get; set; }
        public string address { get; set; }
        public string login { get; set; }
        public string password_hash { get; set; }
        public int is_active { get; set; }
    }
}
