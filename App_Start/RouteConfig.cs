using System.Web.Mvc;
using System.Web.Routing;

namespace Fresh_University_Enrollment
{
    public class RouteConfig
    {
        
        public static void RegisterRoutes(RouteCollection routes)
        {
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "MainAdminRoute",
                url: "Admin/Dashboard",
                defaults: new { controller = "Admin", action = "MainAdmin" }
            );
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "MainSchedu;eRoute",
                url: "Home/Schedule",
                defaults: new { controller = "Main", action = "Student_Schedule" }
            );
            
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "SignUpEntry",
                url: "Auth/SignUp/Entry",
                defaults: new { controller = "SignUp", action = "Entry" }
            );
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "MainViewGradeRoute",
                url: "Home/Grades",
                defaults: new { controller = "Main", action = "Student_Grade" }
            );
            
            
            
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "MainEnrollmentRoute",
                url: "Home/Enrollment",
                defaults: new { controller = "Main", action = "Student_Enrollment" }
            );
            
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "MainProfileRoute",
                url: "Home/Profile",
                defaults: new { controller = "Main", action = "Student_Profile" }
            );
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "MainRoute",
                url: "Home",
                defaults: new { controller = "Main", action = "MainHome" }
            );
                        
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "SignUpRoute",
                url: "SignUp",
                defaults: new { controller = "Login", action = "SignUp" }
            );
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "LoginAdminRoute",
                url: "Login/Admin",
                defaults: new { controller = "Login", action = "LoginAdmin" }
            );
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "LoginHeadRoute",
                url: "Login/Head",
                defaults: new { controller = "Login", action = "LoginHead" }
            );

            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "LoginTeacherRoute",
                url: "Login/Teacher",
                defaults: new { controller = "Login", action = "LoginTeacher" }
            );
            
            
            //LOGIN
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        
            // Add this specific route before the default route
            routes.MapRoute(
                name: "LoginRoute",
                url: "Login/Student",
                defaults: new { controller = "Login", action = "Login" }
            );
            
            
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}