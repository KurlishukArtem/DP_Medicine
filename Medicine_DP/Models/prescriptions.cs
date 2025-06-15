using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime;
using System.Text;

namespace Medicine_DP.Models
{
    public class prescriptions
    {
        [Key]
        public int prescription_id { get; set; }

        [ForeignKey("record_id")]

        public int record_id { get; set; }

        [ForeignKey("medication_id")]
        
        public int medication_id { get; set; }

        public string dosage { get; set; }
        public string frequency { get; set; }
        public string duration { get; set; }
        public string instructions { get; set; }
    }
}
