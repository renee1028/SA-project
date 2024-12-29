using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    public class ReservController : Controller
    {
        private readonly CmsContext _context;
        public ReservController(CmsContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult searchHospital()
        {
            if (HttpContext.Session.GetString("Account_name") == null)
            {
                TempData["message"] = "請登入!";
                return RedirectToAction("Login", "Account");
            }
            var searchH = _context.HOSPITAL_H.ToList();
            return View(searchH);
        }

        public async Task<IActionResult> searchDepartment(string hospital)
        {
            if (HttpContext.Session.GetString("Account_name") == null)
            {
                TempData["message"] = "請登入!";
                return RedirectToAction("Login", "Account");
            }

            var searchDpm = await _context.DOCTOR_H
                            .Where(result => result.Hospital_id == hospital)
                            .GroupBy(result => result.Doctor_specialization)
                            .Select(group => group.Key)
                            .ToListAsync();
            ViewBag.Hospital = hospital;
            ViewData["Department"] = searchDpm;
            return View();
        }
        public async Task<IActionResult> reservTable(string department, string hospital)
        {
            var reservations = await (from reserv in _context.RESERVATION_H
                                      join doctor in _context.DOCTOR_H on reserv.Doctor_id equals doctor.Doctor_id
                                      where doctor.Doctor_specialization == department && doctor.Hospital_id == hospital
                                      orderby reserv.Reserv_time
                                      select new
                                      {
                                          reserv.Reserv_id,
                                          reserv.Patient_id,
                                          DoctorName = doctor.Doctor_name,
                                          reserv.Reserv_time,
                                          reserv.Reserv_stat
                                      }).ToListAsync();

            ViewData["Department"] = department;
            ViewData["Reservations"] = reservations;
            return View();
        }
    }
}
