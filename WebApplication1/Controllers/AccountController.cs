using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using static Azure.Core.HttpHeader;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        readonly string connect = "Server=140.138.144.66\\SQL1422;Database=1122netdbB;User Id=1122netdbB;Password=yzuim1122netdbB;TrustServerCertificate=True;";
        private readonly CmsContext _context;
        public AccountController(CmsContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Homepage()
        {
            if (HttpContext.Session.GetString("Account_name") == null)
            {
                TempData["message"] = "請登入!";
                return RedirectToAction("Login", "Account");
            }

            var accountid = HttpContext.Session.GetString("Account_id");

            var info = await _context.PATIENT_H
                            .Where((p => p.Account_id == accountid))
                            .Select(info => new
                            {
                                info.Patient_id,
                                info.Patient_name,
                                info.Patient_gender,
                                info.Patient_nidcard,
                                info.Patient_birth,
                                info.Patient_phone
                            })
                            .FirstOrDefaultAsync();
            var patientid=info.Patient_id;
            // 取得病患所有的預約資料，並連接到 Reservation_H 表格以取得詳細資料
            var patientReservations = await _context.PATIENTRESERVATION_H
                .Where(p => p.Patient_id == patientid)
                .Join(_context.RESERVATION_H,
                    patientRes => patientRes.Reserv_id,
                    reserv => reserv.Reserv_id,
                    (patientRes, reserv) => new { patientRes, reserv })
                .Join(_context.DOCTOR_H,
                    combined => combined.reserv.Doctor_id,
                    doctor => doctor.Doctor_id,
                    (combined, doctor) => new { combined, doctor })
                .Join(_context.HOSPITAL_H,
                    combinedDoctor => combinedDoctor.doctor.Hospital_id,
                    hospital => hospital.Hospital_id,
                    (combinedDoctor, hospital) => new PatientReservView
                    {
                        PatientReserv_id = combinedDoctor.combined.patientRes.PatientReserv_id,
                        Reserv_time = combinedDoctor.combined.reserv.Reserv_time,
                        Doctor_name = combinedDoctor.doctor.Doctor_name,
                        Doctor_specialization = combinedDoctor.doctor.Doctor_specialization,
                        Hospital_name = hospital.Hospital_name
                    })
                .ToListAsync();

            ViewData["patientReserv"] = patientReservations;

            ViewData["patientInfo"] = info;

            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Account, string pw)
        {
            if (Account == null || pw == null)
            {
                TempData["message"] = "請輸入帳號及密碼!";
                return RedirectToAction("Login", "Account");
            }

            DataTable table = new DataTable();
            using (var connection = new SqlConnection(connect))
            {
                string query = $@"SELECT * FROM
                                ACCOUNT_H
                                Where Account_name = '{Account}' AND Account_password = '{pw}' ";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@account", Account);
                    command.Parameters.AddWithValue("@password", pw);

                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(table);
                }
            }

            var users = (from row in table.AsEnumerable()
                         select new Account
                         {
                             Account_name = row["Account_name"].ToString(),
                             Account_id= row["Account_id"].ToString(),
                             Account_role = row["Account_role"].ToString()
                         }).ToList();

            if (users.Count != 0)
            {
                HttpContext.Session.SetString("Account_name", users.First().Account_name);
                HttpContext.Session.SetString("Account_id", users.First().Account_id);
                if(users.First().Account_role =="患者")
                {
                    return RedirectToAction("Homepage", "Account");
                }
                else
                {
                    return RedirectToAction("DocHomepage", "Doctor");
                }             
            }
            else
            {
                TempData["message"] = "登入失敗!";
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult Logout()
        {
            if (HttpContext.Session.GetString("Account_name") == null)
            {
                TempData["message"] = "請登入!";
                return RedirectToAction("Login", "Account");
            }

            TempData["message"] = "成功登出!";
            HttpContext.Session.Remove("Account_name");
            HttpContext.Session.Remove("Account_id");
            return RedirectToAction("Login", "Account");
        }
    }
}
