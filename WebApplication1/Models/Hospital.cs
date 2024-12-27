using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Hospital
    {
        [Key]
        [Display(Name = "醫院編號")]
        public string Hospital_id { get; set; }

        [Display(Name = "醫院名字")]
        public string Hospital_name { get; set; }

        [Display(Name = "醫院地址")]
        public string Hospital_address { get; set; }
        [Display(Name = "醫院電話")]
        public string Hospital_phone { get; set; }
    }
}
