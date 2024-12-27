using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.Models
{
    public class Account
    {
        [Key]
        [Display(Name = "用戶編號")]
        public string Account_id { get; set; }

        [Display(Name = "帳號")]
        public string Account_name { get; set; }
        [Display(Name = "密碼")]
        public string Account_password { get; set; }
        [Display(Name = "身份")]
        public string Account_role { get; set; }
    }
}