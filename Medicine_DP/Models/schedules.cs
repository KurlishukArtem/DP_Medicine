using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medicine_DP.Models
{
    public class schedules
    {
        [Key]
        public int schedule_id { get; set; }
        
        [Column("employee_id")] // Явно указываем имя столбца
        public int employee_id { get; set; }
        public int day_of_week { get; set; }
        [Column(TypeName = "time")] // Для MySQL/SQL Server
        public TimeSpan start_time { get; set; }
        [Column(TypeName = "time")] // Для MySQL/SQL Server
        public TimeSpan end_time { get; set; }
        public int room_id { get; set; }
        public int is_working_day { get; set; }
        [ForeignKey("employee_id")]
        public virtual employees employee { get; set; }
    }
}
