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

            ViewData["Department"] = searchDpm;
            return View();
        }
    }
}
