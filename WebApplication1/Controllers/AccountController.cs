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
                                info.Patient_name,
                                info.Patient_gender,
                                info.Patient_nidcard,
                                info.Patient_birth,
                                info.Patient_phone
                            })
                            .FirstOrDefaultAsync();
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
                                Where Account_name = '{Account}' AND Account_password = '{pw}' AND Account_role='患者'";

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
                             Account_id= row["Account_id"].ToString()
                         }).ToList();

            if (users.Count != 0)
            {
                HttpContext.Session.SetString("Account_name", users.First().Account_name);
                HttpContext.Session.SetString("Account_id", users.First().Account_id);
                return RedirectToAction("Homepage", "Account");
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
