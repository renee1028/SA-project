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
            return View(searchDpm);
        }
    }
}
