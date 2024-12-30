using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class InitialDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ACCOUNT_H",
                columns: table => new
                {
                    Account_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Account_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Account_password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Account_role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ACCOUNT_H", x => x.Account_id);
                });

            migrationBuilder.CreateTable(
                name: "DOCTOR_H",
                columns: table => new
                {
                    Doctor_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Account_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hospital_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Doctor_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Doctor_specialization = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Doctor_phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Doctor_email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOCTOR_H", x => x.Doctor_id);
                });

            migrationBuilder.CreateTable(
                name: "HOSPITAL_H",
                columns: table => new
                {
                    Hospital_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Hospital_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hospital_address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hospital_phone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HOSPITAL_H", x => x.Hospital_id);
                });

            migrationBuilder.CreateTable(
                name: "MEDICAL_RECORD_H",
                columns: table => new
                {
                    MRecord_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Patient_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Doctor_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MRecord_diagnosis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MRecord_treatmentplan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MRecord_date = table.Column<DateOnly>(type: "date", nullable: false),
                    MRecord_prescription = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MEDICAL_RECORD_H", x => x.MRecord_id);
                });

            migrationBuilder.CreateTable(
                name: "PATIENT_H",
                columns: table => new
                {
                    Patient_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Account_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Patient_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Patient_birth = table.Column<DateOnly>(type: "date", nullable: false),
                    Patient_gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Patient_phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Patient_email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Patient_address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Patient_nhicard = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Patient_nidcard = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PATIENT_H", x => x.Patient_id);
                });

            migrationBuilder.CreateTable(
                name: "PATIENTRESERVATION_H",
                columns: table => new
                {
                    PatientReserv_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Reserv_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Patient_id = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PATIENTRESERVATION_H", x => x.PatientReserv_id);
                });

            migrationBuilder.CreateTable(
                name: "RESERVATION_H",
                columns: table => new
                {
                    Reserv_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Doctor_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reserv_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reserv_stat = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RESERVATION_H", x => x.Reserv_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ACCOUNT_H");

            migrationBuilder.DropTable(
                name: "DOCTOR_H");

            migrationBuilder.DropTable(
                name: "HOSPITAL_H");

            migrationBuilder.DropTable(
                name: "MEDICAL_RECORD_H");

            migrationBuilder.DropTable(
                name: "PATIENT_H");

            migrationBuilder.DropTable(
                name: "PATIENTRESERVATION_H");

            migrationBuilder.DropTable(
                name: "RESERVATION_H");
        }
    }
}
