using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medicine_DP.Models
{
    public class medical_tests
    {
        [Key]
        public int test_id { get; set; }

        public string test_name { get; set; }
        public string description { get; set; }
        public string preparation { get; set; }
        public string normal_values { get; set; }
        public decimal price { get; set; }
        public string category { get; set; }
    }
}
