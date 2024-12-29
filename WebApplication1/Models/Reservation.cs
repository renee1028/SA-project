using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Reservation
    {
        [Key]
        [Display(Name = "預約編號")]
        public string Reserv_id { get; set; }

        [Display(Name = "患者")]
        public string? Patient_id { get; set; }

        [Display(Name = "醫生")]
        public string Doctor_id { get; set; }

        [Display(Name = "預約時間")]
        public DateTime Reserv_time { get; set; }

        [Display(Name = "預約狀態")]
        public string Reserv_stat { get; set; }

    }
}
