using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class DoctorController : Controller
    {
        private readonly CmsContext _context;
        public DoctorController(CmsContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult InputId()
        {
            return View();
        }

        public IActionResult MRecordPage()
        {
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ValidatePatient(string idNumber, string nhcNumber)
        {
            var patient = await _context.PATIENT_H
                                        .FirstOrDefaultAsync(p => p.Patient_nidcard == idNumber && p.Patient_nhicard == nhcNumber);
            
            if (patient != null)
            {
                // 找到患者，返回病历数据
                var patientRecords = await _context.MEDICAL_RECORD_H
                    .Where(r => r.Patient_id == patient.Patient_id)
                    .Join(_context.DOCTOR_H, record => record.Doctor_id, doctor => doctor.Doctor_id,
                        (record, doctor) => new
                        {
                            record.MRecord_id,
                            record.MRecord_date,
                            record.MRecord_diagnosis,
                            record.MRecord_treatmentplan,
                            doctor.Doctor_name,
                            doctor.Doctor_specialization
                        })
                    .ToListAsync();

                return Json(new { isValid = true, patientRecords});
            }

            // 未找到患者
            return Json(new { isValid = false });
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Age")] MRecord mrecord)
        {
            //用ModelState.IsValid判斷資料是否通過驗證
            if (ModelState.IsValid)
            {
                //將entity加入DbSet
                _context.MEDICAL_RECORD_H.Add(mrecord);
                //將資料異動儲存到資料庫
                await _context.SaveChangesAsync();
                //導向至Index動作方法
                return RedirectToAction(nameof(Index));
            }

            return View(mrecord);
        }
    }
}
