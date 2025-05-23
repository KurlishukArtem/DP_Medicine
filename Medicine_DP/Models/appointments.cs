using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medicine_DP.Models
{
    public class appointments
    {
        [Key]
        public int appointment_id { get; set; }

        [ForeignKey("patient_id")]
        
        public int patient_id { get; set; }

        [ForeignKey("employee_id")]
        
        public int employee_id { get; set; }

        [ForeignKey("service_id")]
        
        public int service_id { get; set; }

        public int? room_id { get; set; }
        public DateTime appointment_date { get; set; }
        public int start_time { get; set; }
        public string status { get; set; } = "scheduled";
        public string notes { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
    }
}
