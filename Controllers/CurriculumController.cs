using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Fresh_University_Enrollment.Models;
using Npgsql;

namespace Fresh_University_Enrollment.Controllers
{
    public class CurriculumController : Controller
    {
        private readonly string _connectionString;

        public CurriculumController()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["Enrollment"].ConnectionString;
        }
        
        [HttpGet]
        public JsonResult GetAllAcademicYears()
        {
            var academicYears = new List<dynamic>();
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string sql = @"
                        SELECT ay_code, ay_start_year, ay_end_year
                        FROM academic_year
                        ORDER BY ay_start_year DESC
                    ";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            academicYears.Add(new
                            {
                                AyCode = reader["ay_code"].ToString(),
                                Display = $"{reader["ay_start_year"]} - {reader["ay_end_year"]}"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Optionally log ex.Message
            }

            return Json(academicYears, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAssignedAcademicYears(string progCode)
        {
            var assignedAcademicYears = new List<dynamic>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    string sql = @"
                SELECT ay.ay_code, ay.ay_start_year, ay.ay_end_year
                FROM academic_year ay
                INNER JOIN curriculum c ON c.ay_code = ay.ay_code
                WHERE c.prog_code = @progCode
                ORDER BY ay.ay_start_year DESC
            ";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("progCode", progCode);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                assignedAcademicYears.Add(new
                                {
                                    AyCode = reader["ay_code"].ToString(),
                                    Display = $"{reader["ay_start_year"]} - {reader["ay_end_year"]}"
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Optionally log error
            }

            return Json(assignedAcademicYears, JsonRequestBehavior.AllowGet);
        }


        // GET: /Curriculum/
        public ActionResult Index()
        {
            var curriculums = GetCurriculumsFromDatabase();
            return View("~/Views/Admin/Curriculums.cshtml", curriculums);
        }

        // POST: /Curriculum/Add
        [HttpPost]
public JsonResult Add(string ProgCode, string AyCode)
{
    try
    {
        if (string.IsNullOrEmpty(ProgCode) || string.IsNullOrEmpty(AyCode))
        {
            return Json(new { success = false, message = "Program code and Academic Year code are required." });
        }

        using (var conn = new NpgsqlConnection(_connectionString))
        {
            conn.Open();

            // Get ay_start_year for the given AyCode
            string getStartYearSql = @"
                SELECT ay_start_year
                FROM academic_year
                WHERE ay_code = @ayCode
                LIMIT 1
            ";

            string ayStartYear = null;

            using (var cmd = new NpgsqlCommand(getStartYearSql, conn))
            {
                cmd.Parameters.AddWithValue("ayCode", AyCode);
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    ayStartYear = result.ToString();
                }
                else
                {
                    return Json(new { success = false, message = "Academic Year not found." });
                }
            }

            // Generate cur_code with AY start year
            string generatedCurCode = $"CURR-{ProgCode}-{ayStartYear}";

            // Check if curriculum with same prog_code and ay_code exists
            string checkSql = @"
                SELECT COUNT(*) FROM curriculum
                WHERE prog_code = @progCode AND ay_code = @ayCode
            ";

            using (var checkCmd = new NpgsqlCommand(checkSql, conn))
            {
                checkCmd.Parameters.AddWithValue("progCode", ProgCode);
                checkCmd.Parameters.AddWithValue("ayCode", AyCode);

                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count > 0)
                {
                    return Json(new { success = false, message = "Curriculum with this Program and Academic Year already exists." });
                }
            }

            // Insert new curriculum with generated cur_code
            string insertSql = @"
                INSERT INTO curriculum (cur_code, prog_code, ay_code)
                VALUES (@curCode, @progCode, @ayCode);
            ";

            using (var insertCmd = new NpgsqlCommand(insertSql, conn))
            {
                insertCmd.Parameters.AddWithValue("curCode", generatedCurCode);
                insertCmd.Parameters.AddWithValue("progCode", ProgCode);
                insertCmd.Parameters.AddWithValue("ayCode", AyCode);

                int rowsAffected = insertCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to add curriculum." });
                }
            }
        }
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}


        private List<Curriculum> GetCurriculumsFromDatabase()
        {
            var curriculums = new List<Curriculum>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT cur_code, prog_code, ay_code
                    FROM curriculum
                ";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        curriculums.Add(new Curriculum
                        {
                            CurCode = reader["cur_code"].ToString(),
                            ProgCode = reader["prog_code"].ToString(),
                            AyCode = reader["ay_code"].ToString()
                        });
                    }
                }
            }

            return curriculums;
        }
    }
}
