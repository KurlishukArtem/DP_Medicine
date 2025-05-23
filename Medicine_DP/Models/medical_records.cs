using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Windows.Ink;
using Medicine_DP.Elements;

namespace Medicine_DP.Models
{
    public class medical_records
    {
        [Key]
        public int record_id { get; set; }

        [ForeignKey("patient_id")]
        
        public int patient_id { get; set; }

        [ForeignKey("employee_id")]
        
        public int employee_id { get; set; }

        [ForeignKey("appointment_id")]
        
        public int? appointment_id { get; set; }

        public DateTime record_date { get; set; } = DateTime.Now;
        public string diagnosis { get; set; }
        public string treatment { get; set; }
        public string prescription { get; set; }
        public string recommendations { get; set; }


    }
}
