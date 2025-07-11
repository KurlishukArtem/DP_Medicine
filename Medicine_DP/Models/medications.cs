﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medicine_DP.Models
{
    public class medications
    {
        [Key]
        public int medication_id { get; set; }

        public string name { get; set; }
        public string description { get; set; }
        public string manufacturer { get; set; }
        public string dosage_form { get; set; }
        public string dosage { get; set; }
        public int quantity_in_stock { get; set; } = 0;
        public int minimum_stock_level { get; set; } = 5;
        public double price { get; set; }
    }
}
