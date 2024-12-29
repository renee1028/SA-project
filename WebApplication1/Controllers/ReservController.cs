using Microsoft.AspNetCore.Mvc;
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

        public IActionResult searchDepartment()
        {
            /*if (HttpContext.Session.GetString("Account_name") == null)
            {
                TempData["message"] = "請登入!";
                return RedirectToAction("Login", "Account");
            }*/

            var searchDpm = _context.DOCTOR_H
                .GroupBy(result => result.Doctor_specialization)
                .Select(group => group.Key)
                .ToList();
            ViewData["Department"] = searchDpm;
<<<<<<< HEAD
            return View();
        }
        public async Task<IActionResult> reservTable(string department, string hospital)
        {
            var reservations = await _context.RESERVATION_H
            .Join(
                _context.DOCTOR_H,
                reserv => reserv.Doctor_id,   // 使用病歷表中的 Doctor_id
                doctor => doctor.Doctor_id,                  // 使用醫師表中的 Doctor_id
                (reserv, doctor) => new
                {
                    reserv.Reserv_id,
                    reserv.Patient_id,
                    doctor.Doctor_id,
                    reserv.Reserv_time,
                    reserv.Reserv_stat,
                    doctor.Doctor_name,
                    doctor.Doctor_specialization,
                    doctor.Hospital_id
                }
            )
            .OrderBy(x => x.Reserv_time)
            .ToListAsync();
            
            ViewData["Department"] = department;
            ViewData["Reservations"] = reservations;
            return View();
=======
            return View(searchDpm);
>>>>>>> origin/master
        }
    }
}
