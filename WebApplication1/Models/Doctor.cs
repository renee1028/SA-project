using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Doctor
    {

        [Key]
        [Display(Name = "醫生編號")]
        public string Doctor_id { get; set; }

        [Display(Name = "用戶編號")]
        public string Account_id { get; set; }

        [Display(Name = "醫院編號")]
        public string Hospital_id { get; set; }

        [Display(Name = "醫師")]
        public string Doctor_name { get; set; }
        [Display(Name = "科別")]
        public string Doctor_specialization { get; set; }

        [Display(Name = "聯絡電話")]
        public string Doctor_phone { get; set; }

        [Display(Name = "醫生電子郵箱")]
        public string Doctor_email { get; set; }

    }
}