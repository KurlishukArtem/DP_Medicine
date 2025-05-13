using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Windows.Ink;

namespace Medicine_DP.Models
{
    public class medical_records
    {
        [Key]
        public int record_id { get; set; }
        public int patient_id  { get; set; }
        public int employee_id { get; set; }
        public int appointment_id { get; set; }
        public DateTime record_date { get; set; }
        public string diagnosis { get; set; }
        public string treatment { get; set; }
        public string prescription { get; set; }
        public string recommendations { get; set; }
    }
}
