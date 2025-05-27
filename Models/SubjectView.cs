namespace Fresh_University_Enrollment.Models;

public class SubjectViewModel
{
    public string CourseCode { get; set; }
    public string Title { get; set; }
    public string Time { get; set; }
    public string Days { get; set; }
    public string Room { get; set; } = "N/A";
    public int Units { get; set; }
}