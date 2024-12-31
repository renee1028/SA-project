using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class MRecordController : Controller
    {
        private readonly CmsContext _context;
        public MRecordController(CmsContext context)
        {
            _context = context;
        }


        public IActionResult searchPage()
        {
            if (HttpContext.Session.GetString("Account_name") == null)
            {
                TempData["message"] = "請登入!";
                return RedirectToAction("Login", "Account");
            }
            //選擇病例或是看診報告
            return View();
        }

        //看診紀錄
        [HttpGet]
        public async Task<IActionResult> Consultation(string specialization, string doctorName, string recordDate)
        {
            if (HttpContext.Session.GetString("Account_name") == null)
            {
                TempData["message"] = "請登入!";
                return RedirectToAction("Login", "Account");
            }
            var accountid = HttpContext.Session.GetString("Account_id");

            var patientid = await _context.PATIENT_H
                                          .Where(p => p.Account_id == accountid)
                                          .Select(p => p.Patient_id)
                                          .FirstOrDefaultAsync();
            var mRecordsQuery = _context.MEDICAL_RECORD_H
                .Where((p => p.Patient_id == patientid))
                .Join(
                    _context.DOCTOR_H,                          // 連接 DOCTOR_H 表
                    medicalRecord => medicalRecord.Doctor_id,   // MEDICAL_RECORD_H 的 Doctor_id
                    doctor => doctor.Doctor_id,                 // DOCTOR_H 的 Doctor_id
                    (medicalRecord, doctor) => new              // 投影結果
                    {
                        medicalRecord.MRecord_date,
                        doctor.Doctor_specialization,
                        doctor.Doctor_name
                    }
                );
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
                        Reserv_id = combinedDoctor.combined.reserv.Reserv_id,
                        Reserv_time = combinedDoctor.combined.reserv.Reserv_time,
                        Doctor_name = combinedDoctor.doctor.Doctor_name,
                        Doctor_specialization = combinedDoctor.doctor.Doctor_specialization,
                        Hospital_name = hospital.Hospital_name
                    })
                .ToListAsync();

            ViewData["patientReserv"] = patientReservations;

            /*
            // 根據科別過濾
            if (!string.IsNullOrEmpty(specialization))
            {
                mRecordsQuery = mRecordsQuery.Where(record => record.Doctor_specialization == specialization);
            }

            // 根據醫生名稱過濾
            if (!string.IsNullOrEmpty(doctorName))
            {
                mRecordsQuery = mRecordsQuery.Where(record => record.Doctor_name == doctorName);
            }

            // 根據日期過濾
            if (!string.IsNullOrEmpty(recordDate))
            {
                var date = DateTime.Parse(recordDate);  // 將字串轉換為 DateTime
                var dateOnly = DateOnly.FromDateTime(date); // 將 DateTime 轉換為 DateOnly
                mRecordsQuery = mRecordsQuery.Where(record => record.MRecord_date == dateOnly);  // 直接比較 DateOnly
            }
            */

            // 獲取所有結果，並按日期倒序排序
            var mRecords = await mRecordsQuery
                .OrderByDescending(record => record.MRecord_date)
                .ToListAsync();

            // 根據科別分組
            var groupedBySpecialization = mRecords
                .GroupBy(record => record.Doctor_specialization)
                .ToList();

            // 獲取所有科別名稱
            var doctorSpecializations = mRecords.Select(record => record.Doctor_specialization).Distinct().ToList();
            ViewData["DoctorSpecializations"] = doctorSpecializations;

            // 獲取所有醫生名稱
            var doctorNames = mRecords.Select(record => record.Doctor_name).Distinct().ToList();
            ViewData["DoctorNames"] = doctorNames;

            // 獲取所有日期
            var recordDates = mRecords
                .Select(record => record.MRecord_date)
                .Distinct()
                .ToList();
            ViewData["RecordDates"] = recordDates;

            // 檢查是否有資料
            if (!mRecords.Any())
            {
                ViewData["Message"] = "找不到資料";
            }

            // 傳遞分組資料到視圖
            return View(groupedBySpecialization);
        }

        [HttpPost]
        public async Task<IActionResult> Consultation(string reservationId)
        {
            try
            {
                // 查詢預約記錄
                var PatientReserv = await _context.PATIENTRESERVATION_H
                                                .FirstOrDefaultAsync(r => r.Reserv_id == reservationId);
                if (PatientReserv != null)
                {
                    // 刪除 PatientReservation 資料
                    _context.PATIENTRESERVATION_H.Remove(PatientReserv);

                    // 更新 Reservation_stat 狀態
                    var reserv = await _context.RESERVATION_H
                                                     .FirstOrDefaultAsync(r => r.Reserv_id == reservationId);
                    if (reserv != null)
                    {
                        reserv.Reserv_stat = "available";
                    }

                    await _context.SaveChangesAsync(); // 使用非同步儲存變更

                    TempData["Message"] = "預約已成功取消";
                }
                else
                {
                    TempData["Message"] = "找不到預約記錄，無法取消。";
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"取消失敗：{ex.Message}";
            }

            return RedirectToAction("Consultation","MRecord");
        }



        [HttpGet]
        public async Task<IActionResult> MRecordList(string specialization, string doctorName, string recordDate)
        {
            if (HttpContext.Session.GetString("Account_name") == null)
            {
                TempData["message"] = "請登入!";
                return RedirectToAction("Login", "Account");
            }
            var accountid = HttpContext.Session.GetString("Account_id");

            var patientid = await _context.PATIENT_H
                                          .Where(p => p.Account_id == accountid)
                                          .Select(p => p.Patient_id)
                                          .FirstOrDefaultAsync();

            var allRecordsQuery = _context.MEDICAL_RECORD_H
            .Where((p => p.Patient_id == patientid))
            .Join(
                _context.DOCTOR_H,                          // 從 DOCTOR_H 表中查詢
                medicalRecord => medicalRecord.Doctor_id,   // MEDICAL_RECORD_H 的 Doctor_id
                doctor => doctor.Doctor_id,                 // DOCTOR_H 的 Doctor_id
                (medicalRecord, doctor) => new              // 投影結果
                {
                    medicalRecord.MRecord_id,
                    medicalRecord.Patient_id,
                    medicalRecord.MRecord_diagnosis,
                    medicalRecord.MRecord_treatmentplan,
                    medicalRecord.MRecord_date,
                    doctor.Doctor_name,
                    doctor.Doctor_specialization
                }
            );

            // 基於完整病例內容生成選項
            var allRecords = await allRecordsQuery.ToListAsync();
            var doctorSpecializations = allRecords
                .Select(record => record.Doctor_specialization)
                .Distinct()
                .ToList();
            var doctorNames = allRecords
                .Select(record => record.Doctor_name)
                .Distinct()
                .ToList();
            var recordDates = allRecords
                .Select(record => record.MRecord_date)
                .Distinct()
                .ToList();
            // 傳遞完整選項資料到 View
            ViewData["DoctorSpecializations"] = doctorSpecializations;
            ViewData["DoctorNames"] = doctorNames;
            ViewData["RecordDates"] = recordDates;

            // 根據過濾條件篩選資料
            var mRecordsQuery = allRecordsQuery;

            // 根據科別過濾
            if (!string.IsNullOrEmpty(specialization))
            {
                mRecordsQuery = mRecordsQuery.Where(record => record.Doctor_specialization == specialization);
            }

            // 根據醫生名稱過濾
            if (!string.IsNullOrEmpty(doctorName))
            {
                mRecordsQuery = mRecordsQuery.Where(record => record.Doctor_name == doctorName);
            }
            //病例Date過濾
            if (!string.IsNullOrEmpty(recordDate))
            {
                var date = DateTime.Parse(recordDate);  // 將字串轉換為 DateTime
                var dateOnly = DateOnly.FromDateTime(date); // 將 DateTime 轉換為 DateOnly
                mRecordsQuery = mRecordsQuery.Where(record => record.MRecord_date == dateOnly);  // 直接比較 DateOnly
            }

            var mRecords = await mRecordsQuery
                .OrderByDescending(record => record.MRecord_date)
                .ToListAsync();  // 獲取結果

            if (!mRecords.Any())
            {
                ViewData["Message"] = "找不到資料";
            }

            return View(mRecords);
        }
        //獲取該科別的醫師
        public async Task<IActionResult> GetDoctorsBySpecialization(string specialization)
        {
            if (string.IsNullOrEmpty(specialization))
            {
                return Json(new { doctors = new List<string>() }); // 如果沒有選擇科別，返回空醫師列表
            }

            var accountid = HttpContext.Session.GetString("Account_id");

            var patientid = await _context.PATIENT_H
                                          .Where(p => p.Account_id == accountid)
                                          .Select(p => p.Patient_id)
                                          .FirstOrDefaultAsync();
            // 查詢該科別有病歷記錄的醫師
            var doctors = await _context.MEDICAL_RECORD_H
                .Where((p => p.Patient_id == patientid))
                .Join(
                    _context.DOCTOR_H,
                    medicalRecord => medicalRecord.Doctor_id,   // 使用病歷表中的 Doctor_id
                    doctor => doctor.Doctor_id,                  // 使用醫師表中的 Doctor_id
                    (medicalRecord, doctor) => new
                    {
                        doctor.Doctor_name,
                        doctor.Doctor_specialization
                    }
                )
                .Where(result => result.Doctor_specialization == specialization)  // 根據科別過濾
                .GroupBy(result => result.Doctor_name)  // 根據醫師名稱分組，確保每個醫師只有一個選項
                .Select(group => group.Key)  // 只選擇醫師名稱
                .ToListAsync();

            return Json(new { doctors });
        }

        public async Task<IActionResult> Details(string? Id)
        {
            if (Id == null || _context.MEDICAL_RECORD_H == null)
            {
                var msgObject = new
                {
                    statuscode = StatusCodes.Status400BadRequest,
                    error = "無效的請求，必須提供Id編號"
                };

                return new BadRequestObjectResult(msgObject);
            }

            var report = await _context.MEDICAL_RECORD_H
                .Where(m => m.MRecord_id.ToString() == Id)
                .Join(
                    _context.PATIENT_H,
                    medicalRecord => medicalRecord.Patient_id,
                    patient => patient.Patient_id,
                    (medicalRecord, patient) => new
                    {
                        medicalRecord,
                        PatientName=patient.Patient_name
                    }
                )
                .Join(
                    _context.DOCTOR_H,
                    combined => combined.medicalRecord.Doctor_id,   // 使用病歷表中的 Doctor_id
                    doctor => doctor.Doctor_id,                  // 使用醫師表中的 Doctor_id
                    (combined, doctor) => new
                    {
                        combined.medicalRecord,
                        combined.PatientName,    // 患者姓名
                        DoctorName =doctor.Doctor_name,
                    }
                )
                .FirstOrDefaultAsync();

            // 將患者姓名和醫生姓名設置為 ViewData
            ViewData["PatientName"] = report.PatientName;
            ViewData["DoctorName"] = report.DoctorName;

            if (report == null)
                return NotFound();

            return View(report.medicalRecord);
        }

    }
}
