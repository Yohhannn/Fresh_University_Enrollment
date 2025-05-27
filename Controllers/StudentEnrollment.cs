using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Fresh_University_Enrollment.Models;
using Npgsql;

namespace Fresh_University_Enrollment.Controllers
{
    public class StudentEnrollmentController : Controller
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["Enrollment"].ConnectionString;

        public ActionResult Student_Enrollment()
        {
            var sessionStudCode = Session["Stud_Code"];

            if (sessionStudCode == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int studCode;
            if (!int.TryParse(sessionStudCode.ToString(), out studCode))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Load student data
                var student = GetStudentById(studCode);
                if (student == null)
                {
                    ViewBag.ErrorMessage = "Student not found.";
                    return View("~/Views/Shared/Error.cshtml");
                }

                // Load programs for dropdown
                var programs = GetProgramsFromDatabase();
                ViewBag.Programs = programs;

                // Return the view with student model and programs in ViewBag
                return View("~/Views/Main/StudentEnroll.cshtml", student);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error loading student data: {ex.Message}";
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        private Student GetStudentById(int studCode)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM STUDENT WHERE STUD_CODE = @studCode";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@studCode", studCode);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Student
                            {
                                Stud_Id = reader.GetInt32(reader.GetOrdinal("stud_id")),
                                Stud_Lname = reader["stud_lname"]?.ToString(),
                                Stud_Fname = reader["stud_fname"]?.ToString(),
                                Stud_Mname = reader["stud_mname"]?.ToString(),
                                Stud_Dob = Convert.ToDateTime(reader["stud_dob"]),
                                Stud_Contact = reader["stud_contact"]?.ToString(),
                                Stud_Email = reader["stud_email"]?.ToString(),
                                Stud_Address = reader["stud_address"]?.ToString(),
                                Stud_Code = Convert.ToInt32(reader["stud_code"])
                            };
                        }
                    }
                }
            }

            return null;
        }

        private List<Program> GetProgramsFromDatabase()
        {
            var programs = new List<Program>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT \"prog_code\", \"prog_title\" FROM \"program\"", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            programs.Add(new Program
                            {
                                ProgCode = reader.GetString(0),
                                ProgTitle = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            return programs;
        }
    }
}