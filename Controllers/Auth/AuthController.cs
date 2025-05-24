using System;
using System.Configuration;
using System.Web.Mvc;
using Fresh_University_Enrollment.Models;
using Fresh_University_Enrollment.Utilities;
using Npgsql;

namespace Fresh_University_Enrollment.Controllers.Auth
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
        
        [HttpGet]
        [Route("Entry")]
        public ActionResult Entry()
        {
            return View("~/Views/Auth/SignUp.cshtml");
        }
        
        [HttpPost]
        [Route("Entry")]
        public ActionResult Entry(Student student)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(student.Stud_Code) ||
                    string.IsNullOrEmpty(student.Stud_Lname) ||
                    string.IsNullOrEmpty(student.Stud_Fname) ||
                    student.Stud_Dob == DateTime.MinValue ||
                    string.IsNullOrEmpty(student.Stud_Address) ||
                    string.IsNullOrEmpty(student.Stud_Contact) ||
                    string.IsNullOrEmpty(student.Stud_Email) ||
                    string.IsNullOrEmpty(student.Stud_Password))
                {
                    return Json(new { mess = 0, error = "All required fields must be filled." }, JsonRequestBehavior.AllowGet);
                }

                using (var db = new NpgsqlConnection(_connectionString))
                {
                    db.Open();

                    // Check if student code already exists
                    using (var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM STUDENT WHERE STUD_CODE = @studCode", db))
                    {
                        checkCmd.Parameters.AddWithValue("@studCode", student.Stud_Code);
                        int studCodeExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (studCodeExists > 0)
                        {
                            return Json(new { mess = 2, error = "Student code already exists.", field = "Stud_Code" }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    // Check if email already exists
                    using (var checkEmailCmd = new NpgsqlCommand("SELECT COUNT(*) FROM STUDENT WHERE STUD_EMAIL = @studEmail", db))
                    {
                        checkEmailCmd.Parameters.AddWithValue("@studEmail", student.Stud_Email);
                        int emailExists = Convert.ToInt32(checkEmailCmd.ExecuteScalar());

                        if (emailExists > 0)
                        {
                            return Json(new { mess = 3, error = "Email address is already in use.", field = "Stud_Email" }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    // Hash the password
                    var hashedPassword = PasswordUtil.HashPassword(student.Stud_Password);

                    // Insert student record
                    using (var cmd = new NpgsqlCommand(@"
                        INSERT INTO STUDENT 
                        (STUD_CODE, STUD_LNAME, STUD_FNAME, STUD_MNAME, STUD_BOD, STUD_CONTACT, STUD_EMAIL, STUD_ADDRESS, STUD_PASSWORD)
                        VALUES (@studCode, @lastName, @firstName, @middleName, @birthDate, @contactNo, @emailAddress, @address, @password)", db))
                    {
                        cmd.Parameters.AddWithValue("@studCode", student.Stud_Code);
                        cmd.Parameters.AddWithValue("@lastName", student.Stud_Lname);
                        cmd.Parameters.AddWithValue("@firstName", student.Stud_Fname);
                        cmd.Parameters.AddWithValue("@middleName", string.IsNullOrEmpty(student.Stud_Mname) ? DBNull.Value : (object)student.Stud_Mname);
                        cmd.Parameters.AddWithValue("@birthDate", student.Stud_Dob);
                        cmd.Parameters.AddWithValue("@contactNo", student.Stud_Contact);
                        cmd.Parameters.AddWithValue("@emailAddress", student.Stud_Email);
                        cmd.Parameters.AddWithValue("@address", student.Stud_Address);
                        cmd.Parameters.AddWithValue("@password", hashedPassword);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Json(new
                            {
                                mess = 1,
                                message = "Student record created successfully.",
                                redirectUrl = Url.Action("Login", "Login", new { message = "Student record created successfully." })
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { mess = 0, error = "Failed to create student record." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { mess = 0, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
[HttpPost]
public ActionResult LoginFaculty(Faculty faculty)
{
    try
    {
        // Trim inputs to avoid whitespace issues
        faculty.Username = faculty.Username?.Trim();
        faculty.Password = faculty.Password?.Trim();

        using var db = new NpgsqlConnection(_connectionString);
        db.Open();
        using var cmd = db.CreateCommand();
        // Use ILIKE for case-insensitive comparison
        cmd.CommandText = "SELECT id, username, password, isadmin, isprogramhead FROM faculty WHERE username ILIKE @username";
        cmd.Parameters.AddWithValue("@username", faculty.Username);

        using var reader = cmd.ExecuteReader();
        if (!reader.HasRows)
        {
            return Json(new { 
                success = false, 
                message = "Invalid username or password" 
            }, JsonRequestBehavior.AllowGet);
        }

        reader.Read();

        var hashedPassword = reader["password"].ToString();
                    
        if (!PasswordUtil.VerifyPassword(faculty.Password, hashedPassword))
        {
            return Json(new { 
                success = false, 
                message = "Invalid username or password" 
            }, JsonRequestBehavior.AllowGet);
        }

        // Use the correct column names from your database
        var isAdmin = Convert.ToBoolean(reader["isadmin"]);
        var isProgHead = Convert.ToBoolean(reader["isprogramhead"]);

        var role = isAdmin ? "Admin" : 
            isProgHead ? "Program Head" : "Professor";

        var redirectUrl = role switch
        {
            "Admin" => Url.Action("Dashboard", "Admin"),
            "Program Head" => Url.Action("Dashboard", "ProgramHead"),
            "Professor" => Url.Action("Dashboard", "Teacher"),
            _ => Url.Action("Index", "Home")
        };

        return Json(new
        {
            success = true,
            message = $"Login successful as {role}",
            redirectUrl = redirectUrl,
            data = new { 
                Username = reader["username"].ToString(), 
                Role = role 
            }
        }, JsonRequestBehavior.AllowGet);
    }
    catch (Exception ex)
    {
        return Json(new { 
            success = false, 
            message = "Login failed. Please try again later.",
            error = ex.Message 
        });
    }
}
        
        [HttpGet]
        public ActionResult LoginFaculty()
        {
            // Return the login view for faculty
            return View("~/Views/Auth/LoginFaculty.cshtml");
        }

        [HttpPost]
        public ActionResult LoginStudent()
        {
            try
            {
                string studCode = Request.Form["Stud_Code"];
                string password = Request.Form["Stud_Password"];

                using (var db = new NpgsqlConnection(_connectionString))
                {
                    db.Open();
                    using (var cmd = db.CreateCommand())
                    {
                        cmd.CommandText = "SELECT STUD_CODE, STUD_FNAME, STUD_LNAME, STUD_EMAIL, STUD_PASSWORD FROM STUDENT WHERE STUD_CODE = @studCode";
                        cmd.Parameters.AddWithValue("@studCode", studCode);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                return Json(new { success = false, message = "Invalid student code or password." }, JsonRequestBehavior.AllowGet);
                            }

                            reader.Read();

                            var hashedPassword = reader["STUD_PASSWORD"].ToString();

                            if (!PasswordUtil.VerifyPassword(password, hashedPassword))
                            {
                                return Json(new { success = false, message = "Invalid student code or password." }, JsonRequestBehavior.AllowGet);
                            }

                            var studentData = new
                            {
                                Stud_Code = reader["STUD_CODE"].ToString(),
                                Stud_Fname = reader["STUD_FNAME"].ToString(),
                                Stud_Lname = reader["STUD_LNAME"].ToString(),
                                Stud_Email = reader["STUD_EMAIL"].ToString()
                            };

                            return Json(new
                            {
                                success = true,
                                message = "Login successful!",
                                data = studentData
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message }, JsonRequestBehavior.DenyGet);
            }
        }
    }
}