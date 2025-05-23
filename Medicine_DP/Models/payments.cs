using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Medicine_DP.Elements;

namespace Medicine_DP.Models
{
    public class payments
    {
        [Key]
        public int payment_id { get; set; }

        [ForeignKey("appointment_id")]
        public virtual Appointment Appointment { get; set; }
        public int? appointment_id { get; set; }

        [ForeignKey("patient_id")]
        public virtual Patient Patient { get; set; }
        public int patient_id { get; set; }

        public decimal amount { get; set; }
        public DateTime payment_date { get; set; } = DateTime.Now;
        public string payment_method { get; set; }
        public string status { get; set; } = "completed";
    }
}
