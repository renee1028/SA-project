using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

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
        }
    }
}
