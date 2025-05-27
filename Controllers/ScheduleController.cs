using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Fresh_University_Enrollment.Models;
using Npgsql;

namespace Fresh_University_Enrollment.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["Enrollment"].ConnectionString;

        public ActionResult Index()
        {
            // ✅ Fetch course data from database
            var courseList = GetCourseFromDatabase();

            // ✅ Pass course data to ViewBag
            ViewBag.Course = courseList;

            // ✅ Return view after setting ViewBag
            return View("~/Views/Program-Head/SetSchedules.cshtml");
        }

        private List<Course> GetCourseFromDatabase()
        {
            var courseList = new List<Course>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT \"crs_code\", \"crs_title\" FROM \"course\"", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            courseList.Add(new Course
                            {
                                Crs_Code = reader.GetString(0),
                                Crs_Title = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return courseList;
        }
    }
}