using System;
using System.Web.Mvc;
using Fresh_University_Enrollment.Models;
using Npgsql;
using System.Configuration;
using Fresh_University_Enrollment.Utilities;

namespace Fresh_University_Enrollment.Controllers.Auth
{
    public class SignUpController : Controller
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

        // POST: /Auth/SignUp/Entry
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
                                redirectUrl = Url.Action("Login", "Auth", new { message = "Student record created successfully." })
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
    }
}
