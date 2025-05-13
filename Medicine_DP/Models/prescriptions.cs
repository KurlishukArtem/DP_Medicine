using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime;
using System.Text;

namespace Medicine_DP.Models
{
    public class prescriptions
    {
        [Key]
        public int prescription_id { get; set; }
        public int record_id { get; set; }
        public int medication_id { get; set; }
        public string dosage { get; set; }
        public string frequency { get; set; }
        public string duration { get; set; }
        public string instructions { get; set; }
    }
}
