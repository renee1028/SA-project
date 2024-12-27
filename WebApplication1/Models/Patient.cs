using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication1.Models
{
    public class Patient
    {
        [Key]
        [Display(Name = "患者編號")]
        public string Patient_id { get; set; }

        [Display(Name = "用戶編號")]
        public string Account_id { get; set; }
        [Display(Name = "姓名")]
        public string Patient_name { get; set; }
        [Display(Name = "生日")]
        public DateOnly Patient_birth { get; set; }
        [Display(Name = "性別")]
        public string Patient_gender { get; set; }
        [Display(Name = "電話")]
        public string Patient_phone { get; set; }
        [Display(Name = "電子郵箱")]
        public string Patient_email { get; set; }
        [Display(Name = "住址")]
        public string Patient_address { get; set; }
        [Display(Name = "健保卡卡號")]
        public string Patient_nhicard { get; set; }
        [Display(Name = "身份證字號")]
        public string Patient_nidcard { get; set; }
    }
}