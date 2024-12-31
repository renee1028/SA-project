﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApplication1.Data;

#nullable disable

namespace WebApplication1.Migrations
{
    [DbContext(typeof(CmsContext))]
    [Migration("20241230161512_InitialDB")]
    partial class InitialDB
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WebApplication1.Models.Account", b =>
                {
                    b.Property<string>("Account_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Account_name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Account_password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Account_role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Account_id");

                    b.ToTable("ACCOUNT_H");
                });

            modelBuilder.Entity("WebApplication1.Models.Doctor", b =>
                {
                    b.Property<string>("Doctor_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Account_id")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Doctor_email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Doctor_name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Doctor_phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Doctor_specialization")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Hospital_id")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Doctor_id");

                    b.ToTable("DOCTOR_H");
                });

            modelBuilder.Entity("WebApplication1.Models.Hospital", b =>
                {
                    b.Property<string>("Hospital_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Hospital_address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Hospital_name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Hospital_phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Hospital_id");

                    b.ToTable("HOSPITAL_H");
                });

            modelBuilder.Entity("WebApplication1.Models.MRecord", b =>
                {
                    b.Property<string>("MRecord_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Doctor_id")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateOnly>("MRecord_date")
                        .HasColumnType("date");

                    b.Property<string>("MRecord_diagnosis")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MRecord_prescription")
                        .HasColumnType("int");

                    b.Property<string>("MRecord_treatmentplan")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Patient_id")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MRecord_id");

                    b.ToTable("MEDICAL_RECORD_H");
                });

            modelBuilder.Entity("WebApplication1.Models.Patient", b =>
                {
                    b.Property<string>("Patient_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Account_id")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Patient_address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateOnly>("Patient_birth")
                        .HasColumnType("date");

                    b.Property<string>("Patient_email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Patient_gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Patient_name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Patient_nhicard")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Patient_nidcard")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Patient_phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Patient_id");

                    b.ToTable("PATIENT_H");
                });

            modelBuilder.Entity("WebApplication1.Models.PatientReservation", b =>
                {
                    b.Property<string>("PatientReserv_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Patient_id")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Reserv_id")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PatientReserv_id");

                    b.ToTable("PATIENTRESERVATION_H");
                });

            modelBuilder.Entity("WebApplication1.Models.Reservation", b =>
                {
                    b.Property<string>("Reserv_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Doctor_id")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Reserv_stat")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Reserv_time")
                        .HasColumnType("datetime2");

                    b.HasKey("Reserv_id");

                    b.ToTable("RESERVATION_H");
                });
#pragma warning restore 612, 618
        }
    }
}