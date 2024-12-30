using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class PatientReservation
    {
        [Key]
        [Display(Name = "患者預約編號")]
        public string PatientReserv_id { get; set; }
        [Display(Name = "預約編號")]
        public string Reserv_id { get; set; }
        [Display(Name = "患者編號")]
        public string Patient_id { get; set; }
    }
}
