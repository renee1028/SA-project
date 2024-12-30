using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
namespace WebApplication1.Data
{
    public class CmsContext: DbContext
    {
        public CmsContext(DbContextOptions<CmsContext> options) : base(options)
        {
        }
        
        public DbSet<Account> ACCOUNT_H { get; set; }
        public DbSet<Doctor> DOCTOR_H { get; set; }
        public DbSet<Hospital> HOSPITAL_H { get; set; }
        public DbSet<MRecord> MEDICAL_RECORD_H { get; set; }
        public DbSet<Patient> PATIENT_H { get; set; }
        public DbSet<Reservation> RESERVATION_H { get; set; }
        public DbSet<PatientReservation> PATIENTRESERVATION_H { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
