﻿using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult searchPage()
        {
            if (HttpContext.Session.GetString("Account_name") == null)
            {
                TempData["message"] = "請登入!";
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Consultation(string specialization, string doctorName, string recordDate)
        {
            if (HttpContext.Session.GetString("Account_name") == null)
            {
                TempData["message"] = "請登入!";
                return RedirectToAction("Login", "Account");
            }
            var mRecordsQuery = _context.MEDICAL_RECORD_H
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



        [HttpGet]
        public async Task<IActionResult> MRecordList(string specialization, string doctorName, string recordDate)
        {
            if (HttpContext.Session.GetString("Account_name") == null)
            {
                TempData["message"] = "請登入!";
                return RedirectToAction("Login", "Account");
            }
            var accountid = HttpContext.Session.GetString("Account_id");
            // 使用 EF Core 查詢 Patient_id
            var patientid = await _context.PATIENT_H
                                          .Where(p => p.Account_id == accountid)
                                          .Select(p => p.Patient_id)
                                          .FirstOrDefaultAsync();

            var mRecordsQuery = _context.MEDICAL_RECORD_H
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

            if (!string.IsNullOrEmpty(specialization))
            {
                mRecordsQuery = mRecordsQuery.Where(record => record.Doctor_specialization == specialization);
            }

            // 根據醫生名稱過濾
            if (!string.IsNullOrEmpty(doctorName))
            {
                mRecordsQuery = mRecordsQuery.Where(record => record.Doctor_name == doctorName);
            }

            if (!string.IsNullOrEmpty(recordDate))
            {
                var date = DateTime.Parse(recordDate);  // 將字串轉換為 DateTime
                var dateOnly = DateOnly.FromDateTime(date); // 將 DateTime 轉換為 DateOnly
                mRecordsQuery = mRecordsQuery.Where(record => record.MRecord_date == dateOnly);  // 直接比較 DateOnly
            }

            var mRecords = await mRecordsQuery
                .OrderByDescending(record => record.MRecord_date)
                .ToListAsync();  // 獲取結果

            var recordDates = mRecords
                            .Select(record => record.MRecord_date)
                            .Distinct()
                            .ToList();

            // 將資料傳遞到視圖中
            ViewData["RecordDates"] = recordDates;

            if (!mRecords.Any())
            {
                ViewData["Message"] = "找不到資料";
            }

            var doctorSpecializations = mRecords.Select(record => record.Doctor_specialization).Distinct().ToList();
            ViewData["DoctorSpecializations"] = doctorSpecializations;

            var doctorNames = mRecords.Select(record => record.Doctor_name).Distinct().ToList();
            ViewData["DoctorNames"] = doctorNames;

            return View(mRecords);
        }
        public async Task<IActionResult> GetDoctorsBySpecialization(string specialization)
        {
            if (string.IsNullOrEmpty(specialization))
        {
            return Json(new { doctors = new List<string>() }); // 如果沒有選擇科別，返回空醫師列表
        }

        // 查詢該科別有病歷記錄的醫師
        var doctors = await _context.MEDICAL_RECORD_H
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

            var report = await _context.MEDICAL_RECORD_H.FirstOrDefaultAsync(m => m.MRecord_id.ToString() == Id);


            if (report == null)
                return NotFound();

            return View(report);
        }

    }
}
