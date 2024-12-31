namespace WebApplication1.Models
{
    public class PatientReservView
    {
        public string PatientReserv_id { get; set; }
        public DateTime Reserv_time { get; set; }
        public string Doctor_name { get; set; }
        public string Doctor_specialization { get; set; }
        public string Hospital_name { get; set; }
    }
}
