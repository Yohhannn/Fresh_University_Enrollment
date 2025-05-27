using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Fresh_University_Enrollment.Models;
using Npgsql;

namespace Fresh_University_Enrollment.Controllers
{
    public class CourseController : Controller
    {
        private readonly string _connectionString;

        public CourseController()
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Enrollment"].ConnectionString;
        }

        public ActionResult Course()
        {
            var courses = GetCoursesFromDatabase(); 
            return View("~/Views/Admin/Courses.cshtml",courses);
        }
        // GET: /Course/Create
        public ActionResult Create()
        {
            return View("~/Views/Admin/AddProgram.cshtml");
        }

        // GET: /Course/Edit/{id}
        public ActionResult Edit(string id)
        {
            var course = GetCourseById(id);
            if (course == null) return HttpNotFound();

            return View("~/Views/Admin/EditProgram.cshtml", course);
        }

        // GET: /Course/GetAllCourses
        // GET: /Course/GetAllCourses?progCode=XXX&ayCode=YYY
        [HttpGet]
        public JsonResult GetAllCourses(string progCode = null, string ayCode = null)
        {
            var courses = new List<dynamic>();
            try
            {
                using (var db = new NpgsqlConnection(_connectionString))
                {
                    db.Open();

                    string sql = @"
                        SELECT 
                            c.CRS_CODE, 
                            c.CRS_TITLE, 
                            COALESCE(cat.CTG_NAME, 'General') AS Category,
                            COALESCE(p.PREQ_CRS_CODE, 'None') AS Prerequisite,
                            c.CRS_UNITS, 
                            c.CRS_LEC, 
                            c.CRS_LAB
                        FROM COURSE c
                        LEFT JOIN COURSE_CATEGORY cat ON c.CTG_CODE = cat.CTG_CODE
                        LEFT JOIN PREREQUISITE p ON p.CRS_CODE = c.CRS_CODE
                        WHERE 1=1
                    ";

                    // Exclude courses already assigned if progCode and ayCode are provided
                    if (!string.IsNullOrEmpty(progCode) && !string.IsNullOrEmpty(ayCode))
                    {
                        sql += @"
                            AND c.CRS_CODE NOT IN (
                                SELECT crs_code FROM curriculum_course
                                WHERE prog_code = @progCode AND ay_code = @ayCode
                            )
                        ";
                    }

                    using (var cmd = new NpgsqlCommand(sql, db))
                    {
                        if (!string.IsNullOrEmpty(progCode) && !string.IsNullOrEmpty(ayCode))
                        {
                            cmd.Parameters.AddWithValue("@progCode", progCode);
                            cmd.Parameters.AddWithValue("@ayCode", ayCode);
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                courses.Add(new
                                {
                                    code = reader["CRS_CODE"].ToString(),
                                    title = reader["CRS_TITLE"].ToString(),
                                    category = reader["Category"].ToString(),
                                    prerequisite = reader["Prerequisite"].ToString(),
                                    units = reader["CRS_UNITS"] != DBNull.Value ? Convert.ToDecimal(reader["CRS_UNITS"]) : 0,
                                    lec = reader["CRS_LEC"] != DBNull.Value ? Convert.ToInt32(reader["CRS_LEC"]) : 0,
                                    lab = reader["CRS_LAB"] != DBNull.Value ? Convert.ToInt32(reader["CRS_LAB"]) : 0
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception or handle as needed
                Console.WriteLine($"Error fetching courses: {ex.Message}");
            }

            return Json(courses, JsonRequestBehavior.AllowGet);
        }



        private List<Course> GetCoursesFromDatabase()
        {
            var courses = new List<Course>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
            SELECT 
                c.CRS_CODE, 
                c.CRS_TITLE, 
                COALESCE(cat.CTG_NAME, 'General') AS Category,
                p.PREQ_CRS_CODE,
                c.CRS_UNITS, 
                c.CRS_LEC, 
                c.CRS_LAB
            FROM COURSE c
            LEFT JOIN COURSE_CATEGORY cat ON c.CTG_CODE = cat.CTG_CODE
            LEFT JOIN PREREQUISITE p ON p.CRS_CODE = c.CRS_CODE", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            courses.Add(new Course
                            {
                                Crs_Code = reader["CRS_CODE"]?.ToString(),
                                Crs_Title = reader["CRS_TITLE"]?.ToString(),
                                Ctg_Name = reader["Category"]?.ToString(),
                                Preq_Crs_Code = reader["PREQ_CRS_CODE"]?.ToString(),
                                Crs_Units = reader["CRS_UNITS"] != DBNull.Value ? Convert.ToDecimal(reader["CRS_UNITS"]) : 0,
                                Crs_Lec = reader["CRS_LEC"] != DBNull.Value ? Convert.ToInt32(reader["CRS_LEC"]) : 0,
                                Crs_Lab = reader["CRS_LAB"] != DBNull.Value ? Convert.ToInt32(reader["CRS_LAB"]) : 0
                            });
                        }
                    }
                }
            }

            return courses;
        }

        private object GetCourseById(string id)
        {
            // Implement logic to fetch single course by ID
            // For now, just returning null
            return null;
        }
    }
}