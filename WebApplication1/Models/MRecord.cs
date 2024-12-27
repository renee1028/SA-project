using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class MRecord
    {
        [Key]
        [Display(Name = "病歷編號")]
        public string MRecord_id { get; set; }

        [Display(Name = "患者")]
        public string Patient_id { get; set; }

        [Display(Name = "醫師")]
        public string Doctor_id { get; set; }
        [Display(Name = "病歷診斷")]
        public string MRecord_diagnosis { get; set; }

        [Display(Name = "治療方案")]
        public string MRecord_treatmentplan { get; set; }

        [Display(Name = "看診日期")]
        public DateOnly MRecord_date { get; set; }

        [Display(Name = "藥單數量")]
        public int MRecord_prescription { get; set; }


    }
}
