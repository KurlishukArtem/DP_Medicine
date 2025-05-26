using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medicine_DP.Models
{
    public class test_results
    {
        [Key]
        public int result_id { get; set; }

        public int test_id { get; set; }
        public int patient_id { get; set; }
        public int employee_id { get; set; }
        public int? appointment_id { get; set; }
        public DateTime result_date { get; set; } = DateTime.Now;
        public string result_value { get; set; }
        public string comments { get; set; }

        // Навигационные свойства
        public virtual medical_tests MedicalTest { get; set; }
        public virtual patients Patient { get; set; }
        public virtual employees Employee { get; set; }
        public virtual appointments Appointment { get; set; }
    }
}
