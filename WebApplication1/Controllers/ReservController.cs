using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
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
            /*if (HttpContext.Session.GetString("Account_name") == null)
            {
                TempData["message"] = "請登入!";
                return RedirectToAction("Login", "Account");
            }*/
            var searchH = _context.HOSPITAL_H.ToList();
            return View(searchH);
        }

        public async Task<IActionResult> searchDepartment(string hospital)
        {
            /*if (HttpContext.Session.GetString("Account_name") == null)
            {
                TempData["message"] = "請登入!";
                return RedirectToAction("Login", "Account");
            }*/

            var searchDpm = await _context.DOCTOR_H
                            .Where(result => result.Hospital_id == hospital)
                            .GroupBy(result => result.Doctor_specialization)
                            .Select(group => group.Key)
                            .ToListAsync();

            var hospitalname = await _context.HOSPITAL_H
                            .Where(p => p.Hospital_id == hospital)
                            .Select(p => p.Hospital_name)
                            .FirstOrDefaultAsync();
            ViewData["hospitalname"] = hospitalname;
            ViewBag.Hospital = hospital;
            ViewData["Department"] = searchDpm;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> reservTable(string department, string hospital)
        {
            var reserv = await _context.RESERVATION_H
                        .Join(
                            _context.DOCTOR_H,
                            reserv => reserv.Doctor_id,
                            doctor => doctor.Doctor_id,
                            (reserv, doctor) => new
                            {
                                reserv.Reserv_id,
                                doctor.Doctor_id,
                                reserv.Reserv_time,
                                reserv.Reserv_stat,
                                doctor.Doctor_name,
                                doctor.Doctor_specialization,
                                doctor.Hospital_id
                            }
                        )
                        .Where(result => result.Doctor_specialization == department && result.Hospital_id == hospital)
                        .ToListAsync();
            ViewBag.Hospital= hospital;
            ViewBag.Department = department;

            return View(reserv);
        }

        //Get顯示確認資料
        [HttpGet]
        public async Task<IActionResult> confirmReserv(string reserv, string hospital, string department)
        {
            var confirm = await _context.RESERVATION_H
                        .Join(
                            _context.DOCTOR_H,
                            reserv => reserv.Doctor_id,
                            doctor => doctor.Doctor_id,
                            (reserv, doctor) => new
                            {
                                reserv.Reserv_id,
                                doctor.Doctor_id,
                                reserv.Reserv_time,
                                reserv.Reserv_stat,
                                doctor.Doctor_name,
                                doctor.Doctor_specialization,
                                doctor.Hospital_id
                            }
                        )
                        .Where(result => result.Reserv_id == reserv)
                        .ToListAsync();
            var hospitalname=await _context.HOSPITAL_H
                            .Where(p=>p.Hospital_id==hospital)
                            .Select(p =>p.Hospital_name)
                            .FirstOrDefaultAsync();
            ViewData["hospitalname"] = hospitalname;

            var accountid = HttpContext.Session.GetString("Account_id");

            var info = await _context.PATIENT_H
                .Where((p => p.Account_id == accountid))
                .Select(info => new
                {
                    Name=info.Patient_name,
                    Nidcard=info.Patient_nidcard,
                    birth=info.Patient_birth,
                    Phone=info.Patient_phone
                })
                .FirstOrDefaultAsync();
            if(info == null)
            {
                return NotFound("無法找到對應的患者資料");
            }
            else
            {
                ViewData["Name"] = info.Name;
                ViewData["Nidcard"] = info.Nidcard;
                ViewData["birth"] = info.birth;
                ViewData["Phone"] = info.Phone;
            }

            ViewBag.Hospital = hospital;
            ViewBag.Department = department;
            ViewData["reservation_id"] = reserv;

            return View(confirm);
        }
        // POST 版：確認掛號並更新資料庫
        [HttpPost]
        public async Task<IActionResult> confirmReserv(string reserv)
        {
            if (HttpContext.Session.GetString("Account_name") == null)
            {
                TempData["message"] = "請登入!";
                return RedirectToAction("Login", "Account");
            }

            // 取得當前使用者的 Account_id 
            var accountid = HttpContext.Session.GetString("Account_id");

            // 如果找不到 Account_id，可能需要處理錯誤（例如，未登入）
            if (accountid == null)
            {
                TempData["message"] = "請登入後再確認掛號";
                return RedirectToAction("Login", "Account");
            }

            // 取得預約資料
            var reservation = await _context.RESERVATION_H
                .Where(r => r.Reserv_id == reserv)
                .FirstOrDefaultAsync();

            if (reservation == null)
            {
                return NotFound("找不到對應的預約資料");
            }

            // 更新資料：修改 Reserv_stat 和 Patient_id
            reservation.Reserv_stat = "disable";  // 設定為 "disable"
            // 儲存更新
            _context.RESERVATION_H.Update(reservation);
            var patientid = await _context.PATIENT_H
                                          .Where(p => p.Account_id == accountid)
                                          .Select(p => p.Patient_id)
                                          .FirstOrDefaultAsync();
            // 建立新的 PatientReservation 資料
            var patientReservation = new PatientReservation
            {
                PatientReserv_id =await GeneratePatientReservId(), // 呼叫方法生成唯一 ID
                Reserv_id = reserv,
                Patient_id = patientid
            };

            // 新增資料到 PatientReservation_H
            await _context.PATIENTRESERVATION_H.AddAsync(patientReservation);
            // 儲存所有變更
            await _context.SaveChangesAsync();
            
            TempData["message"] = "掛號成功!";
            return RedirectToAction("Homepage","Account");  // 或者跳轉到其他需要的頁面
        }

        //建立PatientReservation的編號
        private async Task<string> GeneratePatientReservId()
        {
            // 查詢資料庫中目前的最大 ID
            var maxId = await _context.PATIENTRESERVATION_H
                .OrderByDescending(pr => pr.PatientReserv_id)
                .Select(pr => pr.PatientReserv_id)
                .FirstOrDefaultAsync();

            // 如果沒有資料，從 PR001 開始
            if (maxId == null)
            {
                return "PR001";
            }

            // 取出數字部分，並轉為數字
            int currentMaxNumber = int.Parse(maxId.Substring(2));

            // 編號加 1 並格式化為三位數
            string nextId = "PR" + (currentMaxNumber + 1).ToString("D3");

            return nextId;
        }

    }
}
