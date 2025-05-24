using System.Web.Mvc;
using System.Web.Routing;

namespace Fresh_University_Enrollment
{
    public class RouteConfig
    {
        
        public static void RegisterRoutes(RouteCollection routes)
        {
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            
            routes.MapRoute(
                name: "LoginFacultyRoute",
                url: "Auth/LoginFaculty",
                defaults: new { controller = "Auth", action = "LoginFaculty" }
            );
            
            // Add this specific route before the default route
            routes.MapRoute(
                name: "AdminCourseRoute",
                url: "Admin/Course",
                defaults: new { controller = "Admin", action = "Admin_Course" }
            );
            
            // Add this specific route before the default route
            routes.MapRoute(
                name: "AdminCurriculumRoute",
                url: "Admin/Curriculum",
                defaults: new { controller = "Admin", action = "Admin_Curriculum" }
            );
            
            routes.MapRoute(
                name: "SignUpEntry",
                url: "Auth/Entry",
                defaults: new { controller = "Auth", action = "Entry" }
            );
            
            
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "MainAdminRoute",
                url: "Admin/Dashboard",
                defaults: new { controller = "Admin", action = "MainAdmin" }
            );
            
            
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "MainSchedule;eRoute",
                url: "Home/Schedule",
                defaults: new { controller = "Main", action = "Student_Schedule" }
            );
            
            
            // Add this specific route before the default route
            routes.MapRoute(
                name: "MainViewGradeRoute",
                url: "Home/Grades",
                defaults: new { controller = "Main", action = "Student_Grade" }
            );
            
            routes.MapRoute(
                name: "LoginStudentRoute",
                url: "Auth/LoginStudent",
                defaults: new { controller = "Auth", action = "LoginStudent" }
            );
            
          
            // Add this specific route before the default route
            routes.MapRoute(
                name: "MainEnrollmentRoute",
                url: "Home/Enrollment",
                defaults: new { controller = "Main", action = "Student_Enrollment" }
            );
            
            
            
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "MainProfileRoute",
                url: "Home/Profile",
                defaults: new { controller = "Main", action = "Student_Profile" }
            );
            
           
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "MainHomeRoute",
                url: "Main/Home",
                defaults: new { controller = "Main", action = "MainHome" }
            );
                        
            
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "SignUpRoute",
                url: "SignUp",
                defaults: new { controller = "Login", action = "SignUp" }
            );
            
            
            // Add this specific route before the default route
            routes.MapRoute(
                name: "LoginAdminRoute",
                url: "Login/Admin",
                defaults: new { controller = "Login", action = "LoginAdmin" }
            );
            
            
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "LoginHeadRoute",
                url: "Login/Head",
                defaults: new { controller = "Login", action = "LoginHead" }
            );

            
           
            // Add this specific route before the default route
            routes.MapRoute(
                name: "LoginTeacherRoute",
                url: "Login/Teacher",
                defaults: new { controller = "Login", action = "LoginTeacher" }
            );
            
            
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "LoginRoute",
                url: "Login/Student",
                defaults: new { controller = "Login", action = "Login" }
            );
            
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}